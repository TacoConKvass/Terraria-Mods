using Terraria.Localization;
using TML = Terraria.ModLoader;
using Xna = Microsoft.Xna.Framework;

namespace CSM_Mod.Content;

public class PocheetosItem : TML.ModItem
{
	public override string Texture => "CSM_Mod/Assets/PocheetosItem";

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Common.CSM_Transformation.Keybinds.Transformation);

	public override void SetDefaults() {
		Item.Size = new Xna.Vector2();
		Item.accessory = true;
	}

	public override void UpdateEquip(Terraria.Player player) {
		player.GetModPlayer<Common.CSM_Transformation>().Available = true;
	}
}

public class Pocheetos : TML.ModNPC
{
	public override string Texture => "CSM_Mod/Assets/Pocheetos";
	static int spawnCount = 0;

	public override void SetStaticDefaults() {
		Terraria.Main.npcFrameCount[Type] = 4;
	}

	public override void SetDefaults() {
		NPC.lifeMax = 200;
		NPC.Size = new Xna.Vector2(45, 30);
		NPC.lavaImmune = true;

		NPC.aiStyle = Terraria.ID.NPCAIStyleID.Passive;
		AIType = Terraria.ID.NPCID.Bunny;

		NPC.catchItem = TML.ModContent.ItemType<PocheetosItem>();
	}

	public override void PostAI() {
		NPC.frameCounter += (NPC.velocity.X > 0 ? 1 : 0);
		NPC.spriteDirection = -NPC.direction;
	}

	public override void FindFrame(int frameHeight) {
		NPC.frame.Y = frameHeight * ((int)NPC.frameCounter / 10);
		NPC.frameCounter %= 30;
	}

	public override float SpawnChance(TML.NPCSpawnInfo spawnInfo) {
		return spawnCount < 1 && spawnInfo.SpawnTileY < Terraria.Main.maxTilesY * Terraria.Main.UnderworldLayer && Terraria.Main.hardMode ? 0.1f : 0;
	}

	public override bool? CanBeCaughtBy(Terraria.Item item, Terraria.Player player) => true;
	
	public override int SpawnNPC(int tileX, int tileY) {
		spawnCount++;
		return base.SpawnNPC(tileX, tileY);
	}

	public override void OnCaughtBy(Terraria.Player player, Terraria.Item item, bool failed) { spawnCount--; }

	public override void OnKill() { spawnCount--; }
}
