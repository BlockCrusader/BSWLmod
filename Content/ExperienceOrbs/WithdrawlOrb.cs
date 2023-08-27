using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace BSWLmod.Content.ExperienceOrbs
{
	public class WithdrawlOrb : XPOrb
    {
        public override int XP => BSWLmod.orb3XP;
        public override bool Withdrawl => true;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.rare = ItemRarityID.Orange;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Xp>()
				.AddRecipeGroup("DSWLmod:EvilDust")
				.AddTile(TileID.Anvils)
				.Register();

			CreateRecipe(3)
				.AddIngredient<AbsorbingWithdrawl>()
				.AddTile(TileID.Anvils)
				.DisableDecraft()
				.Register();
		}
	}
}