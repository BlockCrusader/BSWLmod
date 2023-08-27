using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using BSWLmod.Common;
using Terraria.Audio;

namespace BSWLmod.Content.OtherRunes
{
	public class BlankRune : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemNoGravity[Item.type] = true; 
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.value = Item.buyPrice(0, 0, 0, 0);
			Item.rare = -1;
			Item.maxStack = 9999;
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
						return true;
					}
				}
			}
			return false;
		}

		public override void RightClick(Player player)
		{
			Item wep = Main.LocalPlayer.HeldItem; 
			if(wep.TryGetGlobalItem(out ItemLeveling enchantGlobalItem))
			{
                enchantGlobalItem.ResetBaseXP();
                SoundEngine.PlaySound(SoundID.Item113, player.position);
            }
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<DormantRune>()
				.AddCondition(Condition.NearWater)
				.Register();
		}
	}
}