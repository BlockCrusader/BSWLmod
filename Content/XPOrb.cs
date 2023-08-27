using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using BSWLmod.Common;
using Microsoft.Xna.Framework;

namespace BSWLmod.Content
{
    // All non-dormant Experience Orbs inherit from this base
	public abstract class XPOrb : ModItem
	{
        /// <summary>
        /// The base amount of XP granted/deducted by the Orb. Scales with the target item's Base XP.
        /// </summary>
		public abstract int XP { get; }

        /// <summary>
        /// Whether or not the Orb is a Withdrawl Orb. Withdrawl Orbs deduct XP rather than grant it.
        /// </summary>
        public abstract bool Withdrawl { get; }
        
        public override void SetDefaults()
        {
            Item.value = Item.buyPrice(0, 0, 0, 0);
            Item.maxStack = 9999;
        }

        
        public sealed override bool CanRightClick()
		{
            
            Item wep = Main.LocalPlayer.HeldItem;
            if (wep != null)
            {
                if (wep.TryGetGlobalItem(out ItemLeveling enchantGlobalItem))
                {
                    if (enchantGlobalItem.baseXP > 0)
                    {
                        if ((enchantGlobalItem.experience < ModContent.GetInstance<Config>().xpCap && !Withdrawl)
                            || (enchantGlobalItem.experience > 0 && Withdrawl)) 
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
                float xp = (float)XP * (enchantGlobalItem.baseXP / BSWLmod.rarity0XP);
                if (Withdrawl)
                {
                    xp *= -1f;
                }
                xp = (int)(xp * ModContent.GetInstance<Config>().orbMultiplier);
                enchantGlobalItem.GainExperience(wep, (int)xp, Withdrawl, player);

                SoundEngine.PlaySound(Withdrawl ? SoundID.Item113 : SoundID.ResearchComplete, player.position);
            }
            
        }

        
        public override Color? GetAlpha(Color lightColor)
        {
            return Withdrawl ? new(255, 255, 255, 240) : new(255, 255, 255, 230);
        }
    }
}