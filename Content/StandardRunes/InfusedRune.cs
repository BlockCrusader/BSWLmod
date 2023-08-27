using Terraria;
using Terraria.ID;
using BSWLmod.Content.OtherRunes;
using Terraria.Localization;
using Terraria.GameContent.Creative;

namespace BSWLmod.Content.StandardRunes
{
	public class InfusedRune : XPRune
    {
        public override int Tier => 6;
        public override int BaseXP => BSWLmod.rarity6XP;
        public override bool Ethereal => false;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BaseXP);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }


        public override void SetDefaults()
        {
            Item.value = Item.buyPrice(0, 8, 10, 0);
        }

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddIngredient(ItemID.SoulofMight)
				.AddIngredient(ItemID.SoulofSight)
				.AddTile(TileID.AdamantiteForge)
				.Register();

			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddIngredient(ItemID.SoulofSight)
				.AddIngredient(ItemID.SoulofFright)
				.AddTile(TileID.AdamantiteForge)
				.Register();

			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddIngredient(ItemID.SoulofFright)
				.AddIngredient(ItemID.SoulofMight)
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}