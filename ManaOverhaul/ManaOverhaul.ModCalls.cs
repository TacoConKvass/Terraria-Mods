using ManaOverhaul.Utils;
using System;
using Terraria.ModLoader;

namespace ManaOverhaul;

public partial class ManaOverhaul {
	public override object Call(params object[] args) {
		// Invalid calls
		if (args is null) throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
		if (args.Length == 0) throw new ArgumentException("Arguments cannot be empty");
		if (args[0] is not string callName) throw new ArgumentException("First argument must be the name of the call");

		switch (callName) {
			case "LoadPrefabs": return LoadPrefabs(args[1..]);
			default: break;
		}

		return false;
	}

	internal bool LoadPrefabs(object[] args) {
		if (args[0] is not Mod mod) throw new ArgumentException("Second argument of call 'LoadPrefabs' must be your a mod object");
		
		PrefabLoader.LoadPrefabsFromMod(mod);

		return true;
	}	
}