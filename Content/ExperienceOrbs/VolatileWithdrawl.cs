using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using BSWLmod.Common;
using Terraria.Audio;

namespace BSWLmod.Content.ExperienceOrbs
{
	public class VolatileWithdrawl : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
			ItemID.Sets.ItemNoGravity[Item.type] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 24;
			Item.value = Item.buyPrice(0, 0, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.maxStack = 9999;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(255, 255, 255, 225);
		}

		public override bool CanRightClick()
		{
            Item wep = Main.LocalPlayer.HeldItem;
            if (wep != null)
            {
                if (wep.TryGetGlobalItem(out ItemLeveling enchantGlobalItem))
                {
                    if (enchantGlobalItem.baseXP > 0)
                    {
                        if (enchantGlobalItem.level > 0)
						{
							return true;
						}
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
                enchantGlobalItem.ForceLevel(wep, enchantGlobalItem.level - 1, player);

                SoundEngine.PlaySound(SoundID.Item113, player.position);
            }
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<MysticXP>()
				.AddRecipeGroup("DSWLmod:EvilDust")
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}