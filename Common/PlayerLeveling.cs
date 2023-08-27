using BSWLmod.Content.OtherRunes;
using Humanizer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BSWLmod.Common
{
	public class PlayerLeveling : ModPlayer
	{
		public float experience;
		private const float baseXP = 8000;
		private const float scalarExponent1 = 1.0125f;
		private const float scalarExponent2 = 1.5f;
		private const float dmgScalar = 1.00375f;
		private const float xpCap = float.MaxValue * 0.95f;
		public int level;
		public bool valueForEffort; // Effort Rune

        public override void ResetEffects()
        {
			valueForEffort = false;
        }

        public override void SaveData(TagCompound tag)
		{
			if(experience > 0)
            {
				tag["playerXP"] = experience;
				
			}
		}
		public override void LoadData(TagCompound tag)
		{
			experience = 0;
			if (tag.ContainsKey("playerXP"))
			{
				GainExperience(tag.GetFloat("playerXP"), true);
			}
		}

		// Netcoding based on Example Mod
		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)BSWLmod.MessageType.SyncPlayerXP);
			packet.Write((byte)Player.whoAmI);
			packet.Write(experience);
			packet.Send(toWho, fromWho);
		}
		public void ReceivePlayerSync(BinaryReader reader)
		{
			experience = reader.ReadSingle();
			CheckLevel(experience, true);
		}
		public override void CopyClientState(ModPlayer targetCopy)
		{
			PlayerLeveling clone = (PlayerLeveling)targetCopy;
			clone.experience = experience;
			clone.CheckLevel(clone.experience, true);
		}
		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			PlayerLeveling clone = (PlayerLeveling)clientPlayer;
			if (experience != clone.experience)
				SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
		}

		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
		{
			if (!mediumCoreDeath)
			{
				return new[] {
				new Item(ModContent.ItemType<ReflectionRune>()),
				};
			}

			return Enumerable.Empty<Item>();
		}

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
			if (valueForEffort)
			{
				modifiers.FinalDamage *= 1.25f;
			}
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (valueForEffort)
            {
                modifiers.FinalDamage *= 1.25f;
            }
        }

        public override void PostUpdateRunSpeeds()
        {
			if (valueForEffort)
			{
                Player.runAcceleration *= 0.75f;
                Player.maxRunSpeed *= 0.75f;
                Player.accRunSpeed *= 0.75f;
                Player.runSlowdown *= 1.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!ModContent.GetInstance<Config>().playerXP)
            {
				return;
            }
			// xp gained is equal to damage dealt. Unlike weapons, there are absolutley no modifiers on player XP gains
			// This also allows it to second as a lifetime damage stat
			int xp = hit.Damage;
			// Patches killing blows not properly granting XP by setting it to be the target's last HP count before death
			if (target.life <= 0 && target.realLife == -1 && target.TryGetGlobalNPC(out KillingBlowPatch globalTarget))
			{
				if (globalTarget.oldHP > 0)
				{
					xp = globalTarget.oldHP;
				}
			}
			GainExperience(xp);
		}

        /// <summary>
        /// Adds XP to the item. Accounts for caps and levels.
        /// </summary>
        /// <param name="xp">XP to grant.</param>
        /// <param name="loading">Set to true if the game is loading. Bugs are caused if level up feedback occurs during loading, so this prevents it when true.</param>
        private void GainExperience(float xp, bool loading = false)
		{
            // Enforce a cap to prevent issues with overflow
            if (experience >= xpCap - xp)
            {
                experience = xpCap;
            }
            else
            {
                experience += xp;
            }
			// Prevent final XP values from being negative
			if (experience < 0)
			{
				experience = 0;
			}
			CheckLevel(experience, loading);
		}

        /// <summary>
        /// Updates level based on XP and Base XP.
        /// </summary>
		/// <param name="xp">The current, cumulative XP.</param>
		/// <param name="loading">Set to true if the game is loading. Bugs are caused if level up feedback occurs during loading, so this prevents it when true.</param>
        private void CheckLevel(float xp, bool loading = false)
		{
			int oldLevel = level;
			level = 0;
			for (int i = 0; i <= 100; i++) 
			{
				level = i;
				float nonCumulativeXP = xp;
				if (i > 0) 
				{
					float previousXP = 0;
					for (int j = i; j > 0; j--) 
					{
						previousXP += (float)(baseXP * Math.Pow(scalarExponent1, Math.Pow(j - 1, scalarExponent2)));
					}
					nonCumulativeXP -= previousXP; 

					if (nonCumulativeXP < 0)
					{
						nonCumulativeXP = 0;
					}
				}
				if (nonCumulativeXP >= (float)(baseXP * Math.Pow(scalarExponent1, Math.Pow(i, scalarExponent2))) && level < 100)
				{
					continue;
				}
				else 
				{
					break;
				}
			}

			if (level > 100)
			{
				level = 100;
			}

			if (level > oldLevel) 
			{
				LevelUpFeedback(oldLevel);
			}
		}

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
		{
			if (ModContent.GetInstance<Config>().playerDamage > 0)
			{
				damage *= GetDamageBoost();
			}
		}

        /// <summary>
        /// Gets a multiplier for damage based on level.
        /// </summary>
        public float GetDamageBoost()
		{
			if(level <= 10)
            {
				return 1f;
            }
			float damageBoost = (float)Math.Pow(dmgScalar, level - 10);
			if (damageBoost > 1.4f)
			{
				damageBoost = 1.4f;
			}
			// Adjust extra damage based on config
			damageBoost--;
			damageBoost = damageBoost * ModContent.GetInstance<Config>().playerDamage;
			damageBoost++;
			return damageBoost;
		}

        /// <summary>
        /// Generates a sound effect and text popups to notify the player of a level up.
        /// </summary>
		/// <param name="oldLevel">The level before the level up. Not neccessarily one level down, if a large amount XP was gained at once.</param>
		private void LevelUpFeedback(int oldLevel)
		{
			if(Player != Main.LocalPlayer)
            {
				return;
            }
			if (ModContent.GetInstance<Config>().lvlUpSound)
			{
				SoundEngine.PlaySound(SoundID.ResearchComplete, Player.position);
			}
			if (ModContent.GetInstance<Config>().lvlUpText)
			{
                Color textColor = Color.White;
                if (ModContent.GetInstance<Config>().coloredDisplays)
				{
                    textColor = ColoredLevelText(level);
                }
                if (level >= ModContent.GetInstance<Config>().levelCap)
                {
                    Main.NewText(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerMaxOut").FormatWith(oldLevel, level)
                        , textColor);

                    CombatText.NewText(Player.Hitbox, textColor,
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerMaxOutPopup")
                        , true);
                }
                else
                {
                    Main.NewText(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerLevelUp").FormatWith(oldLevel, level)
                        , textColor);

                    CombatText.NewText(Player.Hitbox, textColor,
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.PlayerLevelUpPopup"));
                }
            }
		}

        /// <summary>
        /// Returns how much XP is needed to level up, based on a given current XP and level.
        /// </summary>
        /// <param name="currentLevel">The current level; XP needed for the proceeding level is calculated.</param>
		/// <param name="xp">The current, cumulative XP.</param>
        public float GetNeededXP(float xp, int currentLevel)
		{
			float nonCumulativeXP = xp;
			if (level > 0) 
			{
				float previousXP = 0;
				for (int j = level; j > 0; j--) 
				{
					previousXP += (float)(baseXP * Math.Pow(scalarExponent1, Math.Pow(j - 1, scalarExponent2)));
				}
				nonCumulativeXP -= previousXP; 
				if (nonCumulativeXP < 0) 
				{
					nonCumulativeXP = 0;
				}
			}

			return baseXP * (float)Math.Pow(scalarExponent1, Math.Pow(currentLevel, scalarExponent2)) - nonCumulativeXP;
		}

        /// <summary>
        /// Returns a color - for text displays - dynamically based on a given level.
        /// </summary>
        /// <param name="currentLevel">The level the color should be based on.</param>
        private static Color ColoredLevelText(int currentLevel)
		{
			Color barColorStart = new(0.74f, 0.74f, 0.71f);
			Color barColorHalf = new(0.8f, 0.8f, 0.57f);
			Color barColorMax = new(0.86f, 0.86f, 0.43f);
			float lvlProgress = currentLevel / 100f; 
			if (lvlProgress >= 1f) 
			{
				return ColorSwap(new(219, 219, 109), new(246, 246, 146), 6f);
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
	