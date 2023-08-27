using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BSWLmod.Content.OtherRunes
{
	public class DormantEtherealRune : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
			ItemID.Sets.ItemIconPulse[Item.type] = true; 
			ItemID.Sets.ItemNoGravity[Item.type] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.value = Item.buyPrice(0, 0, 0, 0);
			Item.rare = -1;
			Item.maxStack = 9999;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 215);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddIngredient(ItemID.Glass, 2)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}