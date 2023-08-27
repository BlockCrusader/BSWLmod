using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace BSWLmod.Content.ExperienceOrbs
{
	public class ShallowWithdrawl : XPOrb
    {
        public override int XP => BSWLmod.orb1XP;
        public override bool Withdrawl => true;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
		{
			Item.width = 10;
			Item.height = 10;
			Item.rare = ItemRarityID.Blue;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<DullXP>()
				.AddRecipeGroup("DSWLmod:EvilDust")
				.AddTile(TileID.Anvils)
				.Register();

			CreateRecipe(5)
				.AddIngredient<RestrainedWithdrawl>()
				.AddTile(TileID.Anvils)
				.DisableDecraft()
				.Register();
		}
	}
}