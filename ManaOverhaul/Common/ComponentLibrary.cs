using ManaOverhaul.Components;
using System.Collections.Generic;

namespace ManaOverhaul.Common;

public static class ComponentLibrary {
	public static class Item {
		public static Dictionary<int, ChangeScaleWithManaData> ChangeScaleWithMana = [];
		public static Dictionary<int, ManaDrainData> AppliesManaDrain = [];
	}

	public static class Projectile {
		public static Dictionary<int, ChangeScaleWithManaData> ChangeScaleWithMana = [];
		public static Dictionary<int, ManaDrainData> AppliesManaDrain = [];
	}
	public static class NPC
	{
		public static Dictionary<int, Resistance> Resistance = [];
		public static Dictionary<int, ManaDrainData> AppliesManaDrain = [];
	}

	public static void Unload() {
		Item.ChangeScaleWithMana = null;
		Projectile.ChangeScaleWithMana = null;
	}
}