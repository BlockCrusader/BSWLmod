using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace BSWLmod.Content.OtherRunes
{
	public class DormantRune : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.ShimmerTransformToItem[Item.type] = ModContent.ItemType<ReflectionRune>();
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
			ItemID.Sets.ItemNoGravity[Item.type] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.value = Item.buyPrice(0, 0, 9, 50);
			Item.rare = -1;
			Item.maxStack = 9999;
		}

        public override void AddRecipes()
		{
			CreateRecipe(2)
				.AddIngredient(ItemID.StoneBlock, 5)
				.AddRecipeGroup("DSWLmod:Gems")
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}