using Terraria;
using Terraria.ID;
using BSWLmod.Content.OtherRunes;
using Terraria.Localization;
using Terraria.GameContent.Creative;

namespace BSWLmod.Content.StandardRunes
{
	public class EerieRune : XPRune
    {
        public override int Tier => 8;
        public override int BaseXP => BSWLmod.rarity8XP;
        public override bool Ethereal => false;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BaseXP); 
        
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.value = Item.buyPrice(0, 2, 60, 0);
        }

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddIngredient(ItemID.SpookyWood)
				.AddIngredient(ItemID.Ectoplasm)
				.AddTile(TileID.AdamantiteForge)
				.Register();
		}
	}
}