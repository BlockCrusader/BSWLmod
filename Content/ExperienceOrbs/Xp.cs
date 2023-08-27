using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace BSWLmod.Content.ExperienceOrbs
{
	public class Xp : XPOrb
    {
        public override int XP => BSWLmod.orb5XP;
        public override bool Withdrawl => false;

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
				.AddIngredient<Dormant>()
				.AddTile(TileID.Furnaces)
				.Register();

			CreateRecipe(3)
				.AddIngredient<PowerfulXP>()
				.AddTile(TileID.Anvils)
				.DisableDecraft()
				.Register();
		}
	}
}