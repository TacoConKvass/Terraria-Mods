using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MilesVanity.Content;

[AutoloadEquip(EquipType.Wings)]
public class DarkConquerersPresence : ModItem
{
	public override void SetStaticDefaults() {
		ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(flyTime: 60, flySpeedOverride: 6f, accelerationMultiplier: 2f);
	}

	public override void SetDefaults() {
		Item.width = 28;
		Item.height = 80;

		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 75);

		Item.accessory = true;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.GoldBar, 15)
			.AddIngredient(ItemID.Silk, 10)
			.AddTile(TileID.Anvils)
			.Register();
	}
}