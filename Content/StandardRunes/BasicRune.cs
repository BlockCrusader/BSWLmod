using Terraria;
using Terraria.ID;
using BSWLmod.Content.OtherRunes;
using Terraria.Localization;
using Terraria.GameContent.Creative;

namespace BSWLmod.Content.StandardRunes
{
	public class BasicRune : XPRune
    {
        public override int Tier => 0;
        public override int BaseXP => BSWLmod.rarity0XP;
        public override bool Ethereal => false;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BaseXP);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
		{
			Item.value = Item.buyPrice(0, 0, 20, 50);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddRecipeGroup("DSWLmod:Bars1")
				.AddTile(TileID.Furnaces)
				.Register();

			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddRecipeGroup("DSWLmod:Bars2")
				.AddTile(TileID.Furnaces)
				.Register();

			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddRecipeGroup("DSWLmod:Bars3")
				.AddTile(TileID.Furnaces)
				.Register();

			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddRecipeGroup("DSWLmod:Bars4")
				.AddTile(TileID.Furnaces)
				.Register();
		}
	}
}