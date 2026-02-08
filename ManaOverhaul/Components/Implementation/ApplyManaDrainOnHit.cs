using Humanizer;
using ManaOverhaul.Common;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ManaOverhaul.Components;

public class ApplyManaDrainOnHit_Item : ItemComponent {
	public ManaDrainData Data = default;
	
	public override void SetDefaults(Item entity) {
		if (ComponentLibrary.Item.AppliesManaDrain.TryGetValue(entity.type, out var value)) Data = value;
		else if (entity.CountsAsClass(DamageClass.Melee) && entity.pick == 0 && entity.ModItem == null && !entity.noMelee) Data = ManaDrainData.Default(entity);

		Enabled = Data != null;
	}

	public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone) {
		if (!Enabled) return;
		
		target.GetGlobalNPC<ManaDrained_NPC>().Inflict(player.whoAmI, Data.Clone());
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
		if (!Enabled) return;

		int manaPerInterval = Data.ManaPerInterval;
		float seconds = MathF.Round(Data.Interval * Data.Ticks/ 60f, 3);
		int ticks = Data.Ticks;

		tooltips.Add(new TooltipLine(
			Mod, "AppliesManaDrain",
			Language.GetTextValue("Mods.ManaOverhaul.AppliesManaDrain").FormatWith(manaPerInterval * ticks, $"{seconds} second{(seconds > 1 ? "s" : "")}")
		));
	}
}

public class ApplyManaDrainOnHit_Projectile : ProjectileComponent {
	public ManaDrainData Data = default;

	public override void SetDefaults(Projectile entity) {
		if (ComponentLibrary.Projectile.AppliesManaDrain.TryGetValue(entity.type, out var value)) Data = value;
		Enabled = Data != null;
	}

	public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
		if (!Enabled) return;

		target.GetGlobalNPC<ManaDrained_NPC>().Inflict(projectile.owner, Data.Clone());
	}

	public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info) {
		if (!Enabled || info.PvP) return;

		target.GetModPlayer<ManaDrained_Player>().Data = Data.Clone();
	}
}

public class ApplyManaDrainOnHit_NPC : NPCComponent {
	public ManaDrainData Data = default;

	public override void SetDefaults(NPC entity) {
		if (ComponentLibrary.NPC.AppliesManaDrain.TryGetValue(entity.type, out var value)) Data = value;
		Enabled = Data != null;
	}

	public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
		if (!Enabled) return;

		target.GetModPlayer<ManaDrained_Player>().Inflict(Data.Clone());
	}
}