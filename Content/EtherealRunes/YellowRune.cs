using BSWLmod.Content.OtherRunes;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;

namespace BSWLmod.Content.EtherealRunes
{
    public class YellowRune : XPRune
    {
        public override int Tier => 8;
        public override int BaseXP => BSWLmod.rarity8XP;
        public override bool Ethereal => true;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BaseXP);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<DormantEtherealRune>()
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}