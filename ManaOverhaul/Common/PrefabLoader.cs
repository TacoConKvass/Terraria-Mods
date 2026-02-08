using Newtonsoft.Json.Linq;
using ManaOverhaul.Components;
using System.IO;
using System.Linq;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria;

namespace ManaOverhaul.Utils;

public class PrefabLoader : ModSystem {
	public override void PostSetupContent() {
		LoadPrefabsFromMod(Mod);
	}

	public static void LoadPrefabsFromMod(Mod mod) {
		var assets = mod.GetFileNames().Where(t => t.EndsWith(".mana-overhaul"));
		mod.Logger.Debug("Started loading Mana Overhaul prefabs");
		mod.Logger.Debug("Prefab files found: " + assets.Count().ToString());

		foreach (string fullFilePath in assets) {
			using Stream stream = mod.GetFileStream(fullFilePath);
			using StreamReader streamReader = new StreamReader(stream);
			string hjsonText = streamReader.ReadToEnd();
			string jsonText = Hjson.HjsonValue.Parse(hjsonText).ToString(Hjson.Stringify.Plain);
			JToken json = JToken.Parse(jsonText);

			if (json["Items"] is JContainer items) {
				foreach (JToken itemToken in items) {
					if (itemToken is not JProperty { Name: string itemName, Value: JObject components }) {
						continue;
					}

					if (components["ChangeScaleWithMana"] is JObject changeScaleWithManaData) 
						ChangeScaleWithManaData.DeserializeFor<Item>(ItemID.Search.GetId(itemName), changeScaleWithManaData);

					if (components["AppliesManaDrain"] is JObject appliesManaDrainData) 
						ManaDrainData.DeserializeFor<Item>(ItemID.Search.GetId(itemName), appliesManaDrainData);
				}
			}

			if (json["Projectiles"] is JContainer projectiles) {
				foreach (JToken projectileToken in projectiles) {
					if (projectileToken is not JProperty { Name: string projectileName, Value: JObject components }) {
						continue;
					}

					if (components["ChangeScaleWithMana"] is JObject changeScaleWithManaData) 
						ChangeScaleWithManaData.DeserializeFor<Projectile>(ProjectileID.Search.GetId(projectileName), changeScaleWithManaData);

					if (components["AppliesManaDrain"] is JObject appliesManaDrainData) 
						ManaDrainData.DeserializeFor<Projectile>(ProjectileID.Search.GetId(projectileName), appliesManaDrainData);
				}
			}

			if (json["NPCs"] is JContainer npcs) {
				foreach (JToken npcToken in npcs) {
					if (npcToken is not JProperty { Name: string npcName, Value: JObject components }) {
						continue;
					}

					if (components["AppliesManaDrain"] is JObject appliesManaDrainData) 
						ManaDrainData.DeserializeFor<NPC>(NPCID.Search.GetId(npcName), appliesManaDrainData);

					if (components["Resists"] is JObject resistances)
						Resistance.DeserializeFor<NPC>(NPCID.Search.GetId(npcName), resistances);
				}
			}
		}
	}
}