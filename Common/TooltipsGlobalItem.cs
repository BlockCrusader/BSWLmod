using BSWLmod.Content.ExperienceOrbs;
using BSWLmod.Content.OtherRunes;
using BSWLmod.Content.StandardRunes;
using BSWLmod.Content.EtherealRunes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Humanizer;

namespace BSWLmod.Common
{
	public class TooltipsGlobalItem : GlobalItem
	{
        private ItemLeveling levelingGlobalItem;
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ItemType<BasicRune>() ||
                entity.type == ModContent.ItemType<EnhancedRune>() ||
                entity.type == ModContent.ItemType<TaintedRune>() ||
                entity.type == ModContent.ItemType<InfernalRune>() ||
                entity.type == ModContent.ItemType<TwilightRune>() ||
                entity.type == ModContent.ItemType<HolyRune>() ||
                entity.type == ModContent.ItemType<InfusedRune>() ||
                entity.type == ModContent.ItemType<VerdantRune>() ||
                entity.type == ModContent.ItemType<EerieRune>() ||
                entity.type == ModContent.ItemType<XenoRune>() ||
                entity.type == ModContent.ItemType<MoonRune>() ||
                entity.type == ModContent.ItemType<PhantasmalRune>() ||
                entity.type == ModContent.ItemType<OverclockedRune>() ||
                entity.type == ModContent.ItemType<PrismRune>() ||
                entity.type == ModContent.ItemType<UnstableRune>() ||
                entity.type == ModContent.ItemType<MechanicalRune>() ||
                entity.type == ModContent.ItemType<BlankRune>() ||
                entity.type == ModContent.ItemType<WhiteRune>() ||
                entity.type == ModContent.ItemType<BlueRune>() ||
                entity.type == ModContent.ItemType<GreenRune>() ||
                entity.type == ModContent.ItemType<OrangeRune>() ||
                entity.type == ModContent.ItemType<LightRedRune>() ||
                entity.type == ModContent.ItemType<PinkRune>() ||
                entity.type == ModContent.ItemType<LightPurpleRune>() ||
                entity.type == ModContent.ItemType<LimeRune>() ||
                entity.type == ModContent.ItemType<YellowRune>() ||
                entity.type == ModContent.ItemType<CyanRune>() ||
                entity.type == ModContent.ItemType<RedRune>() ||
                entity.type == ModContent.ItemType<PurpleRune>() ||
                entity.type == ModContent.ItemType<DullXP>() ||
                entity.type == ModContent.ItemType<WeakXP>() ||
                entity.type == ModContent.ItemType<Xp>() ||
                entity.type == ModContent.ItemType<PowerfulXP>() ||
                entity.type == ModContent.ItemType<SuperchargedXP>() ||
                entity.type == ModContent.ItemType<MysticXP>() ||
                entity.type == ModContent.ItemType<ShallowWithdrawl>() ||
                entity.type == ModContent.ItemType<RestrainedWithdrawl>() ||
                entity.type == ModContent.ItemType<WithdrawlOrb>() ||
                entity.type == ModContent.ItemType<AbsorbingWithdrawl>() ||
                entity.type == ModContent.ItemType<AbyssalWithdrawl>() ||
                entity.type == ModContent.ItemType<VolatileWithdrawl>();
            
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Color textColor = Color.White;
            Player player = Main.LocalPlayer;
            if (player != null)
            {
                Item heldItem = player.HeldItem;
                if (heldItem != null)
                {
                    if (ModContent.GetInstance<Config>().coloredDisplays)
                    {
                        textColor = GetRarityColor(heldItem);
                    }
                    if (heldItem.damage > 0 && heldItem.consumable == false && heldItem.ammo == AmmoID.None && heldItem.accessory == false)
                    {
                        if (!heldItem.TryGetGlobalItem(out levelingGlobalItem))
                        {
                            return;
                        }

                        // XP orbs; does the weapon have a base XP?
                        if ((item.type == ModContent.ItemType<BlankRune>() ||
                             item.type == ModContent.ItemType<DullXP>() ||
                             item.type == ModContent.ItemType<WeakXP>() ||
                             item.type == ModContent.ItemType<Xp>() ||
                             item.type == ModContent.ItemType<PowerfulXP>() ||
                             item.type == ModContent.ItemType<SuperchargedXP>() ||
                             item.type == ModContent.ItemType<MysticXP>() ||
                             item.type == ModContent.ItemType<ShallowWithdrawl>() ||
                             item.type == ModContent.ItemType<RestrainedWithdrawl>() ||
                             item.type == ModContent.ItemType<WithdrawlOrb>() ||
                             item.type == ModContent.ItemType<AbsorbingWithdrawl>() ||
                             item.type == ModContent.ItemType<AbyssalWithdrawl>() ||
                             item.type == ModContent.ItemType<VolatileWithdrawl>())
                            && levelingGlobalItem.baseXP > 0)
                        {
                            tooltips.Add(new TooltipLine(Mod, "xpTarget",
                                Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.TargetGear").FormatWith(heldItem.HoverName))
                            { OverrideColor = textColor });
                            return;
                        }

                        // Runes; does the weapon lack a base XP?
                        else if (!(item.type == ModContent.ItemType<BlankRune>() ||
                             item.type == ModContent.ItemType<DullXP>() ||
                             item.type == ModContent.ItemType<WeakXP>() ||
                             item.type == ModContent.ItemType<Xp>() ||
                             item.type == ModContent.ItemType<PowerfulXP>() ||
                             item.type == ModContent.ItemType<SuperchargedXP>() ||
                             item.type == ModContent.ItemType<MysticXP>() ||
                             item.type == ModContent.ItemType<ShallowWithdrawl>() ||
                             item.type == ModContent.ItemType<RestrainedWithdrawl>() ||
                             item.type == ModContent.ItemType<WithdrawlOrb>() ||
                             item.type == ModContent.ItemType<AbsorbingWithdrawl>() ||
                             item.type == ModContent.ItemType<AbyssalWithdrawl>() ||
                             item.type == ModContent.ItemType<VolatileWithdrawl>())
                            && levelingGlobalItem.baseXP <= 0)
                        {
                            tooltips.Add(new TooltipLine(Mod, "xpTarget",
                                Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.TargetGear").FormatWith(heldItem.HoverName))
                            { OverrideColor = textColor });
                            return;
                        }
                    }
                }
            }
            
        }

        public static int CheckRarity(Item item)
        {
            int returnValue = item.OriginalRarity;
            if (returnValue > 11)
            {
                returnValue = 11;
            }

            // Note the following hard-coded exceptions:
            // Slime Staff is treated as Green (2) rarity instead of Light Red (4)
            // Waffle's Iron is treated as Light Purple (6) rarity instead of Pink (5)
            // Terraprisma is treated as Cyan (9) rarity instead of Pink (5)
            // Zenith is treated as Purple (11) rarity instead of Red (10)
            if (item.type == ItemID.SlimeStaff)
            {
                returnValue = 2;
            }
            if (item.type == ItemID.WaffleIron)
            {
                returnValue = 6;
            }
            if (item.type == ItemID.EmpressBlade)
            {
                returnValue = 9;
            }
            if (item.type == ItemID.Zenith)
            {
                returnValue = 11;
            }

            return returnValue;
        }

        public Color GetRarityColor(Item item)
        {
            int rarity = CheckRarity(item);
            switch (rarity)
            {
                case -1:
                case 0:
                    return Colors.RarityNormal;
                case 1:
                    return Colors.RarityBlue;
                case 2:
                    return Colors.RarityGreen;
                case 3:
                    return Colors.RarityOrange;
                case 4:
                    return Colors.RarityRed;
                case 5:
                    return Colors.RarityPink;
                case 6:
                    return Colors.RarityPurple;
                case 7:
                    return Colors.RarityLime;
                case 8:
                    return Colors.RarityYellow;
                case 9:
                    return Colors.RarityCyan;
                case 10:
                    return Colors.RarityDarkRed;
                default:
                    return Colors.RarityDarkPurple;
            }
        }
    }
}