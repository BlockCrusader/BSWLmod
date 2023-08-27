using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using BSWLmod.Common;

namespace BSWLmod.Content
{
    // Most Experience Runes inherit from this base
    public abstract class XPRune : ModItem
    {
        public abstract int Tier { get; }

        public abstract int BaseXP { get; }

        public abstract bool Ethereal { get; }
        
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.rare = Tier;
            Item.maxStack = 9999;
        }

        public sealed override bool CanRightClick()
        {

            Item wep = Main.LocalPlayer.HeldItem;
            if (wep != null)
            {
                if (wep.TryGetGlobalItem(out ItemLeveling enchantGlobalItem))
                {
                    if (enchantGlobalItem.baseXP <= 0)
                    {

                        int customRarity = ItemLeveling.CheckRarity(wep);
                        if (wep.OriginalRarity <= Tier || customRarity <= Tier || Tier >= 11 || Ethereal)
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
                enchantGlobalItem.SetBaseXP(wep, BaseXP, false, false, false);
                SoundEngine.PlaySound(SoundID.Item4, player.position);
            }

        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new(255, 255, 255, 215);
        }
        
    }
}