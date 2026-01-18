using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MilesVanity.Content;

[AutoloadEquip(EquipType.Head)]
public class DarkConquerersMask : ModItem
{
	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(3);

	public override void SetDefaults() {
		Item.width = 22;
		Item.height = 28;

		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(silver: 75);
		Item.maxStack = 1;

		Item.defense = 6;
	}

	public override void UpdateEquip(Player player) {
		player.GetDamage(DamageClass.Generic) += .03f;
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddIngredient(ItemID.GoldBar, 15)
			.AddIngredient(ItemID.Silk, 10)
			.AddTile(TileID.Anvils)
			.Register();
	}
}