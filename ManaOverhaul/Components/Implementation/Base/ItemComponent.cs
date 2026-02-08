using Terraria.ModLoader;

namespace ManaOverhaul.Components;

public class ItemComponent : GlobalItem {
	public override bool InstancePerEntity => true;

	/// <summary>
	/// Determines whether this component is enabled
	/// </summary>
	public bool Enabled { get; set; } = false;
}