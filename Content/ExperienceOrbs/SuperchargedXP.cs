using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace BSWLmod.Content.ExperienceOrbs
{
	public class SuperchargedXP : XPOrb
    {
        public override int XP => BSWLmod.orb5XP;
        public override bool Withdrawl => false;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }

        public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Purple;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<HugeDormant>()
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}