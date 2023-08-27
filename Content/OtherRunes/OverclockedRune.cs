using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using BSWLmod.Common;
using Terraria.Audio;
using Terraria.Localization;

namespace BSWLmod.Content.OtherRunes
{
	public class OverclockedRune : ModItem
	{
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(BSWLmod.overclockXP);

        public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
			ItemID.Sets.ItemNoGravity[Item.type] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.value = Item.buyPrice(0, 0, 0, 0);
			Item.rare = ItemRarityID.Purple;
			Item.maxStack = 9999;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 215);
		}

		public override bool CanRightClick()
		{
            Item wep = Main.LocalPlayer.HeldItem;
            if (wep != null)
            {
                if (wep.TryGetGlobalItem(out ItemLeveling enchantGlobalItem))
                {
                    if (enchantGlobalItem.baseXP <= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

		public override void RightClick(Player player)
		{
            Item wep = Main.LocalPlayer.HeldItem;
			if (wep.TryGetGlobalItem(out ItemLeveling enchantGlobalItem))
			{
				enchantGlobalItem.SetBaseXP(wep, BSWLmod.overclockXP, false, false, false);
				SoundEngine.PlaySound(SoundID.Item4, player.position);
			}
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