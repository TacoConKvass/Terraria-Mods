using CalamityMod.CalPlayer;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;
using System.Reflection;

namespace LinearCalamityDR;

public class LinearCalamityDR : Mod {
	public override void Load() {
		Cal_DisableDiminishingDR.Apply();
	}
	public override void Unload() {
		Cal_DisableDiminishingDR.Undo();
	}

	public static ILHook Cal_DisableDiminishingDR = new ILHook( 
		typeof(CalamityPlayer).GetMethod("Limits", BindingFlags.Instance | BindingFlags.NonPublic)!,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("endurance"), i => i.MatchLdcR4(0))) {
				cursor.EmitDelegate((float zero) => float.PositiveInfinity);
			}
		},
		false
	);
}