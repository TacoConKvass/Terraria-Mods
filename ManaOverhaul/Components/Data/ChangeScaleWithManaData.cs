using ManaOverhaul.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Terraria;

namespace ManaOverhaul.Components;

/// <summary>
/// Data for changing scale on mana thresholds
/// </summary
public class ChangeScaleWithManaData() {
	/// <summary>
	/// Thresholds for changing item scale
	/// </summary>
	public float[] Thresholds { get; set; }

	/// <summary>
	/// Items scale boosts activated at thresholds at thier index
	/// </summary>
	/// <remarks>
	/// The boosts are additive with eachother
	/// </remarks>
	public float[] ScaleBoosts { get; set; }

	public static ChangeScaleWithManaData Default = new ChangeScaleWithManaData() { 
		Thresholds = [.5f], 
		ScaleBoosts = [.3f]
	};

	public static void DeserializeFor<T>(int ID, JObject data) {
		Dictionary<int, ChangeScaleWithManaData> dictionary = [];

		if (typeof(T) == typeof(Item)) dictionary = ComponentLibrary.Item.ChangeScaleWithMana;
		if (typeof(T) == typeof(Projectile)) dictionary = ComponentLibrary.Projectile.ChangeScaleWithMana;

		ChangeScaleWithManaData value = data.ToObject<ChangeScaleWithManaData>();
		dictionary[ID] = value;
	}
}