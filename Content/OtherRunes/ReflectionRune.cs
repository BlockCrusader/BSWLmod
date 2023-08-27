using BSWLmod.Common;
using Humanizer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BSWLmod.Content.OtherRunes
{
	public class ReflectionRune : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 26;
			Item.value = Item.buyPrice(0, 0, 0, 0);
			Item.rare = ItemRarityID.Purple;
		}
		
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
			TooltipLine excessLine = tooltips.FirstOrDefault(line => line.Mod == "Terraria" && line.Name == "Tooltip3");
			int excessIndex = tooltips.IndexOf(excessLine);

			Color lvlDisplay = Color.White;
			Color xpDisplay = Color.White;
			Color dmgDisplay = Color.White;

			Player player = Main.LocalPlayer;
			PlayerLeveling globalPlayer = player.GetModPlayer<PlayerLeveling>();

			int level = globalPlayer.level;
			float experience = globalPlayer.experience;

			if (ModContent.GetInstance<Config>().coloredDisplays) 
			{
				lvlDisplay = ColoredLevelText(level);
				xpDisplay = ColoredXPText(globalPlayer, experience, level);
				dmgDisplay = ColoredDamageText(level);
			}

			if (ModContent.GetInstance<Config>().showDMG == true && ModContent.GetInstance<Config>().playerDamage > 0)
			{
				if (globalPlayer.GetDamageBoost() >= (((1.4f - 1f) * ModContent.GetInstance<Config>().playerDamage) + 1) && ModContent.GetInstance<Config>().showMaxDMG == false)
				{
					tooltips.Insert(excessIndex, new TooltipLine(Mod, "damage",
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.DmgStat").FormatWith(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.Maxed")))
					{ OverrideColor = dmgDisplay });
				}
				else
				{
					string dmgString = $"x{Math.Round(globalPlayer.GetDamageBoost(), 2)}";
					tooltips.Insert(excessIndex, new TooltipLine(Mod, "damage",
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.DmgStat").FormatWith(dmgString))
                    { OverrideColor = dmgDisplay });
				}
			}

			float currentXP = (float)Math.Round(experience, 0);
			if (level < ModContent.GetInstance<Config>().levelCap) 
			{
				float xpToGo = (float)Math.Round(globalPlayer.GetNeededXP(experience, level), 0);
                tooltips.Insert(excessIndex, new TooltipLine(Mod, "experience",
                    Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerXPStat").FormatWith(currentXP, xpToGo))
                { OverrideColor = xpDisplay });
			}
			else 
			{
				if (experience < float.MaxValue * 0.95f)
				{
					tooltips.Insert(excessIndex, new TooltipLine(Mod, "experience",
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerXPStat100").FormatWith(currentXP))
                    { OverrideColor = xpDisplay });
				}
				else
				{
					if (ModContent.GetInstance<Config>().showMaxXP == false)
					{
						tooltips.Insert(excessIndex, new TooltipLine(Mod, "experience",
                            Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerXPStat100").FormatWith(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.Maxed")))
                        { OverrideColor = xpDisplay });
					}
					else
					{
						tooltips.Insert(excessIndex, new TooltipLine(Mod, "experience",
                            Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerXPStat100").FormatWith(currentXP))
                        { OverrideColor = xpDisplay });
					}
				}
			}

			if (level < 100) 
			{
				tooltips.Insert(excessIndex, new TooltipLine(Mod, "level",
                    Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerLvlStat").FormatWith(level))
                { OverrideColor = lvlDisplay });
			}
			else 
			{
				if (ModContent.GetInstance<Config>().showLvl100 == false)
				{
					tooltips.Insert(excessIndex, new TooltipLine(Mod, "level",
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerLvlStat").FormatWith(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.EX")))
                    { OverrideColor = lvlDisplay });
				}
				else
				{
					tooltips.Insert(excessIndex, new TooltipLine(Mod, "level",
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerLvlStat").FormatWith(100))
                    { OverrideColor = lvlDisplay });
				}
			}

			tooltips.Remove(excessLine);
		}

		// The below methods are more or less copies of those used in ItemLeveling, see those for details
		private static Color ColoredLevelText(int currentLevel)
		{
			Color barColorStart = new(0.74f, 0.74f, 0.71f);
			Color barColorHalf = new(0.8f, 0.8f, 0.57f);
			Color barColorMax = new(0.86f, 0.86f, 0.43f);
			float lvlProgress = currentLevel / 100f; 
			if (lvlProgress >= 1f) 
			{
				return ColorSwap(new Color(219, 219, 109), new Color(246, 246, 146), 6f);
			}
			else if (lvlProgress > 0.5f)
			{
				return LerpColour(barColorHalf, barColorMax, (lvlProgress - 0.5f) * 2);
			}
			else
			{
				return LerpColour(barColorStart, barColorHalf, lvlProgress * 2);
			}
		}
		private static Color ColoredXPText(PlayerLeveling globalPlayer, float xp, int currentLevel)
		{
			Color barColorStart = new(0.29f, 0.36f, 0.6f);
			Color barColorHalf = new(0.34f, 0.6f, 0.7f);
			Color barColorMax = new(0.38f, 0.8f, 0.71f);
			if (currentLevel < 100)
			{
				float xpToGo = globalPlayer.GetNeededXP(xp, currentLevel);
				float nextLvlXP = (int)(8000f * Math.Pow(1.0125f, Math.Pow(currentLevel, 1.5f)));
				float xpProgress = 1f - ((float)xpToGo / (float)nextLvlXP);

				if (xpProgress > 0.5f)
				{
					return LerpColour(barColorHalf, barColorMax, (xpProgress - 0.5f) * 2);
				}
				else
				{
					return LerpColour(barColorStart, barColorHalf, xpProgress * 2);
				}
			}
			else 
			{
				return ColorSwap(new Color(97, 204, 181), new Color(153, 242, 223), 6f);
			}
		}
		private static Color ColoredDamageText(int currentLevel)
		{
			Color barColorStart = new(0.85f, 0.24f, 0f);
			Color barColorHalf = new(0.87f, 0.05f, 0.05f);
			Color barColorMax = new(0.89f, 0.12f, 0.6f);
			float dmgProgress = currentLevel / 100f; 
			if (dmgProgress >= 1f) 
			{
				return ColorSwap(new Color(227, 31, 152), new Color(238, 21, 103), 6f);
			}
			else if (dmgProgress > 0.5f)
			{
				return LerpColour(barColorHalf, barColorMax, (dmgProgress - 0.5f) * 2);
			}
			else
			{
				return LerpColour(barColorStart, barColorHalf, dmgProgress * 2);
			}
		}
		private static Color LerpColour(Color c1, Color c2, float amount)
		{
			return new Color(
				MathHelper.Lerp(c1.R / 255f, c2.R / 255f, amount),
				MathHelper.Lerp(c1.G / 255f, c2.G / 255f, amount),
				MathHelper.Lerp(c1.B / 255f, c2.B / 255f, amount));
		}
		private static Color ColorSwap(Color firstColor, Color secondColor, float seconds)
		{
			float timer = (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly * ((float)Math.PI * 2f) * (1f / seconds)));
			return Color.Lerp(firstColor, secondColor, timer);
		}
		
	}
}