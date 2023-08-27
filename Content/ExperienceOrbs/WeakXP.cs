using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace BSWLmod.Content.ExperienceOrbs
{
	public class WeakXP : XPOrb
    {
        public override int XP => BSWLmod.orb2XP;
        public override bool Withdrawl => false;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 14;
			Item.rare = ItemRarityID.Green;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<SmallDormant>()
				.AddTile(TileID.Furnaces)
				.Register();

			CreateRecipe(3)
				.AddIngredient<Xp>()
				.AddTile(TileID.Anvils)
				.DisableDecraft()
				.Register();
		}
	}
}