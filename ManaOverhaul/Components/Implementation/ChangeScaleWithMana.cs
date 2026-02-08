using Humanizer;
using ManaOverhaul.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ManaOverhaul.Components;

public class ChangeScaleWithMana_Item : ItemComponent {
	/// <inheritdoc cref="ChangeScaleWithManaData"/>
	public ChangeScaleWithManaData Data = default;

	public override void SetDefaults(Item entity) {
		if (ComponentLibrary.Item.ChangeScaleWithMana.TryGetValue(entity.type, out var value)) Data = value;
		else if (entity.CountsAsClass(DamageClass.Melee) && entity.pick == 0 && entity.ModItem == null) Data = ChangeScaleWithManaData.Default;

		Enabled = Data != null;
	}

	public override void ModifyItemScale(Item item, Player player, ref float scale) {
		if (!Enabled) {
			return;
		}

		for (int index = 0; index < Data.Thresholds.Length; index++) {
			if (player.statMana < player.statManaMax2 * Data.Thresholds[index]) {
				scale += Data.ScaleBoosts[index];
			}
		}
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
		if (!Enabled) {
			return;
		}

		for (int index = 0; index < Data.Thresholds.Length; index++) {
			tooltips.Add(new TooltipLine(
				Mod,
				"ChangeScaleWithMana",
				Language.GetTextValue("Mods.ManaOverhaul.ChangeScaleWithMana").FormatWith(
					(int)(Data.ScaleBoosts[index] * 100),
					(int)(Data.Thresholds[index] * 100)
				)
			));
		}
	}
}