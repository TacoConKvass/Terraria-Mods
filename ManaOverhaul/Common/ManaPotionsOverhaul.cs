using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ManaOverhaul;

public class ManaPotionsOverhaul : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		=> lateInstantiation && entity.healMana > 0 && entity.consumable;

	public override void GetHealMana(Item item, Player player, bool quickHeal, ref int healValue) {
		healValue = 0;
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
		TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealMana");

		if (line != null) {
			line.Text = Language.GetTextValue("Mods.ManaOverhaul.ManaPotionOverhaul");
		}
	}
}