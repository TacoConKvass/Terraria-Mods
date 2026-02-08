using ManaOverhaul.Common;
using System;
using System.Collections.Generic;
using Terraria;

namespace ManaOverhaul.Components;

public class ManaDrained_NPC : NPCComponent {
	public Dictionary<int, ManaDrainData> ManaDrainPerPlayer = [];
	public Dictionary<int, int> Timers = [];
	public float Resistance = 0f;

	public override void SetDefaults(NPC entity) {
		if (ComponentLibrary.NPC.Resistance.TryGetValue(entity.type, out var resistances))
			if (resistances.TryGetValue("ManaDrain", out var value)) Resistance = value;

		Enabled = Resistance < 1f;
		if (!Enabled) return;
	}

	public override bool PreAI(NPC npc) {
		if (!Enabled) return true;

		List<int> toRemove = [];
		foreach (int playerId in ManaDrainPerPlayer.Keys) {
			ManaDrainData data = ManaDrainPerPlayer[playerId];
			Player player = Main.player[playerId];

			if (!player.active || player.dead || data.Ticks == 0) {
				toRemove.Add(playerId);
				continue;
			}

			if (Timers[playerId] > data.Interval) {
				Timers[playerId] = 0;
				data.Ticks--;
				int manaGain = (int)(data.ManaPerInterval - data.ManaPerInterval * Resistance);
				player.statMana = Math.Min(player.statMana + manaGain, player.statManaMax2);
				CombatText.NewText(player.Hitbox, Microsoft.Xna.Framework.Color.Aqua, manaGain, true, true);
			}

			Timers[playerId]++;
		}

		foreach (int playerId in toRemove) { 
			ManaDrainPerPlayer.Remove(playerId);
			Timers.Remove(playerId);
		}

		return true;
	}

	public void Inflict(int playerId, ManaDrainData data) {
		ManaDrainPerPlayer[playerId] = data;
		Timers[playerId] = 0;
	}
}

public class ManaDrained_Player : PlayerComponent {
	public ManaDrainData Data = default;
	public int Timer = 0;

	public override void PostUpdate() {
		if (Data == null) return;

		if (Data.Ticks <= 0 || !Player.active || Player.dead) {
			Data = null;
			return;
		}

		if (Timer > Data.Interval) {
			Timer = 0;
			Data.Ticks--;
			Player.statMana = Math.Max(Player.statMana - Data.ManaPerInterval, 0);
			CombatText.NewText(Player.Hitbox, Microsoft.Xna.Framework.Color.MediumAquamarine, -Data.ManaPerInterval, true);
		}

		Timer++;
	}

	public void Inflict(ManaDrainData data) {
		Data = data;
		Timer = 0;
	}
}