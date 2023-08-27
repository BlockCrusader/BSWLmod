using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace BSWLmod.Content.ExperienceOrbs
{
	public class PowerfulXP : XPOrb
    {
        public override int XP => BSWLmod.orb4XP;
        public override bool Withdrawl => false;

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
				.AddIngredient<BigDormant>()
				.AddTile(TileID.AdamantiteForge)
				.Register();

			CreateRecipe(3)
				.AddIngredient<SuperchargedXP>()
				.AddTile(TileID.Anvils)
				.DisableDecraft()
				.Register();
		}
	}
}