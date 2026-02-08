using ManaOverhaul.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Terraria;

namespace ManaOverhaul.Components;

public class Resistance : Dictionary<string, float> {
	public static void DeserializeFor<T>(int ID, JContainer data) {
		Dictionary<int, Resistance> dictionary = [];

		if (typeof(T) == typeof(NPC)) dictionary = ComponentLibrary.NPC.Resistance;

		Resistance value = data.ToObject<Resistance>();

		if (dictionary.TryAdd(ID, value)) return;
		foreach (string key in value.Keys) {
			dictionary[ID][key] = value[key];
		}
	}
}