using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace BSWLmod.Content.ExperienceOrbs
{
	public class AbsorbingWithdrawl : XPOrb
    {
        public override int XP => BSWLmod.orb4XP;
        public override bool Withdrawl => true;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.rare = ItemRarityID.Cyan;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<PowerfulXP>()
				.AddRecipeGroup("DSWLmod:EvilDust")
				.AddTile(TileID.Anvils)
				.Register();

			CreateRecipe(3)
				.AddIngredient<AbyssalWithdrawl>()
				.AddTile(TileID.Anvils)
				.DisableDecraft()
				.Register();
		}
	}
}