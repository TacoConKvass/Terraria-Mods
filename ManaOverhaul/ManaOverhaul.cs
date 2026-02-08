using ManaOverhaul.Common;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ManaOverhaul;

public partial class ManaOverhaul : Mod {
	public void OnLoad() {
		Console.WriteLine("Guh");
		UpdateManaRegenHook.Apply();
		SpawnManaStarHook.Apply();
	}

	public void OnUnload() {
		UpdateManaRegenHook.Undo();
		SpawnManaStarHook.Undo();
		ComponentLibrary.Unload();
	}

	public Hook UpdateManaRegenHook = new Hook(
		typeof(Player).GetMethod("UpdateManaRegen", BindingFlags.Public | BindingFlags.Instance),
		(Action<Player> orig, Player self) => { }
	);

	public Hook SpawnManaStarHook = new Hook(
		typeof(Item).GetMethod("NewItem_Inner", BindingFlags.NonPublic | BindingFlags.Static),
		(Func<IEntitySource, int, int, int, int, Item, int, int, bool, int, bool, bool, int> orig, IEntitySource source, int X, int Y, int Width, int Height, Item itemToClone, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup) => {
			if (Type == 184) {
				return 0;
			}

			return orig(source, X, Y, Width, Height, itemToClone, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
		}
	);
}