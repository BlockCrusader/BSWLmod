using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using System.IO;
using System;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Humanizer;

namespace BSWLmod.Common
{
	internal class ItemLeveling : GlobalItem
	{
		public int experience;
		public int baseXP = -1;
		public int level;

        // Used to dynamically update base XP from Mechanical Experience Runes, since the value can be changed via. config at any time
        public bool customBaseCheck = false;

		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			if (entity.damage > 0 && entity.consumable == false && entity.ammo == AmmoID.None && entity.accessory == false)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void LoadData(Item item, TagCompound tag)
		{
			if (tag.ContainsKey("baseXP"))
			{
				baseXP = tag.Get<int>("baseXP"); 
			}

			experience = 0;
            // Load experience tag. XP is allowed to be loaded regardless of Base XP
            // This is done to prevent weapons' XP from being wiped on accident when baseXP is reset (Ex: via. a Blank Rune)
            if (tag.ContainsKey("experience"))
            {
				GainExperience(item, tag.Get<int>("experience"));
			}
			level = 0;

			if (baseXP > 0) // levels can only be granted if the weapon has an established base XP
			{
				CheckLevel(item, tag.Get<int>("experience"), null, true); 
			}
		}

		public override void SaveData(Item item, TagCompound tag)
		{
			if(experience > 0)
            {
				tag["experience"] = experience; 
			}
			if(baseXP > 0)
            {
				tag["baseXP"] = baseXP; 
			}
		}

		public override void NetSend(Item item, BinaryWriter writer)
		{
			writer.Write(baseXP);
			writer.Write(experience);
		}

		public override void NetReceive(Item item, BinaryReader reader)
		{
			baseXP = reader.ReadInt32();
			experience = 0;
			int netXP = reader.ReadInt32();
			GainExperience(item, netXP);
			level = 0;
			if(baseXP > 0)
            {
				CheckLevel(item, netXP, null, true);
			}
		}

        /// <summary>
        /// Sets the item's Base XP.
        /// </summary>
		/// <param name="item">The item to recieve the base XP value. Used for checking rarity.</param>
		/// <param name="newBaseXP">The base XP value to set.</param>
		/// <param name="customBase">Indicates use of the Mechanical Experience Rune. Overrides newBaseXP with a pre-defined value set in config.</param>
		/// <param name="rarityBased">Indicates use of the Prism Experience Rune. Overrides newBaseXP with a value determined by rarity.</param>
		/// <param name="calcDPS">Indicates use of the Unstable Experience Rune. Overrides newBaseXP with a calculated value based on stats/DPS. Highly innaccurate.</param>
        public void SetBaseXP(Item item, int newBaseXP, bool customBase, bool rarityBased, bool calcDPS)
		{
			if (baseXP <= 0)
			{
				customBaseCheck = false;

				if(newBaseXP > 0)
                {
					baseXP = newBaseXP;
				}	

				if (customBase) // Used by Mechanical Experience Runes. Set the base XP as custom-set in the config.
				{
					customBaseCheck = true; 
					baseXP = ModContent.GetInstance<Config>().customBaseXP;
				}

				if (rarityBased) // Used by Prism Experience Runes. Sets the base XP based on rarity.
				{
					int rarity = CheckRarity(item);
                    switch (rarity)
					{
						case -1:
						case 0:
                            baseXP = BSWLmod.rarity0XP;
                            break;
                        case 1:
                            baseXP = BSWLmod.rarity1XP;
                            break;
                        case 2:
                            baseXP = BSWLmod.rarity2XP;
                            break;
                        case 3:
                            baseXP = BSWLmod.rarity3XP;
                            break;
                        case 4:
                            baseXP = BSWLmod.rarity4XP;
                            break;
                        case 5:
                            baseXP = BSWLmod.rarity5XP;
                            break;
                        case 6:
                            baseXP = BSWLmod.rarity6XP;
                            break;
                        case 7:
                            baseXP = BSWLmod.rarity7XP;
                            break;
                        case 8:
                            baseXP = BSWLmod.rarity8XP;
                            break;
                        case 9:
                            baseXP = BSWLmod.rarity9XP;
                            break;
                        case 10:
                            baseXP = BSWLmod.rarity10XP;
                            break;
                        default:
                            baseXP = BSWLmod.rarity11XP;
                            break;
					}
				}

				if (calcDPS) // Used by Unstable Experience Runes. Attempts (and fails) to calculate the weapon's base DPS, and set Base XP using that calculation.
				{
					float itemDmg = item.damage;
					float itemCrit = 1.04f + (0.01f * item.crit);
					float itemSpeed = 60f / (item.useTime + item.reuseDelay);

					baseXP = (int)(itemDmg * itemCrit * itemSpeed * 20f); 

					if (baseXP < 100) // Set a lower cap for XP here, just in case
					{
						baseXP = 100;
					}
				}
			}

			// Base XP can't exceed this level, to avoid lvl 100 requiring more than the max allowed XP (2 billion)
			if (baseXP > BSWLmod.maxBaseXP)
			{
				baseXP = BSWLmod.maxBaseXP;
			}
		}

        /// <summary>
        /// Returns the rarity value of the item, while accounting for some hardcoded exceptions.
        /// </summary>
		/// <param name="item">The item to check.</param>
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

        /// <summary>
        /// Sets the item's Base XP back to the default of -1, which disables leveling. Used by Blank Rune.
        /// </summary>
        public void ResetBaseXP()
        {
			baseXP = -1;
        }

        /// <summary>
        /// Used by the Mechanical Experience Rune. Updates items using that rune to match their Base XP to the set value in config.
        /// </summary>
        private void RecheckBaseXP()
		{
			if(customBaseCheck == true)
            {
				baseXP = ModContent.GetInstance<Config>().customBaseXP;
			}

			if (baseXP > BSWLmod.maxBaseXP)
			{
				baseXP = BSWLmod.maxBaseXP;
			}
		}

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
			// Limits what kinds of enemies can yield XP to prevent cheesing (Mainly statue or target dummy farming)
            if (!target.immortal && target.chaseable && target.lifeMax > 5 && !target.dontTakeDamage && !target.friendly && !target.SpawnedFromStatue)
			{
                int dmgOverride = -1;
                // Patches killing blows not properly granting XP by setting it to be the target's last HP count before death

                if (target.life <= 0 && target.realLife == -1 && target.TryGetGlobalNPC(out KillingBlowPatch globalTarget))
                {
                    if (globalTarget.oldHP > 0)
                    {
                        dmgOverride = globalTarget.oldHP;
                        CombatText.NewText(player.Hitbox, Color.Cyan, globalTarget.oldHP, true);
                    }
                }
				OnHitXP(player, target, hit, dmgOverride, item);
			}
		}

        /// <summary>
        /// Returns whether or not a boss or miniboss is alive somewhere in the world.
        /// </summary>
        private static bool FindActiveBoss()
        {
			bool activeBoss = false;

			for (int i = 0; i < 200; i++)
			{
				NPC target = Main.npc[i];
				if (target.active && GeneralBossCheck(target))
				{
					activeBoss = true;
					break;
				}
			}
			
			return activeBoss;
		}

        /// <summary>
        /// Returns whether or not the given NPC counts as a boss. This includes NPC.boss being true, but also a check for a minimap head and some hardcoded exceptions.
        /// </summary>
		/// <param name="target">The NPC to check.</param>
        private static bool GeneralBossCheck(NPC target)
        {
			// Check for bosses in 3 different ways
			bool variableCheck = target.boss; // If target.boss is true
			bool headCheck = false; // If the target has a head icon for the minimap. This also catches minibosses that the first check doesn't
			bool manualCheck = false; // Hardcoded check for certain NPC ids, made in order to cover boss parts (Ex: Skeletron/Prime/Golem's limbs)

			if (target.GetBossHeadTextureIndex() > 0)
            {
				headCheck = true;
            }

			if (
				target.type == NPCID.EaterofWorldsBody || target.type == NPCID.EaterofWorldsHead || target.type == NPCID.EaterofWorldsTail
				|| target.type == NPCID.PrimeCannon || target.type == NPCID.PrimeSaw || target.type == NPCID.PrimeVice || target.type == NPCID.PrimeLaser
				|| target.type == NPCID.SkeletronHand
				|| target.type == NPCID.WallofFleshEye 
				|| target.type == NPCID.GolemFistLeft || target.type == NPCID.GolemFistRight || target.type == NPCID.GolemHead 
				|| target.type == NPCID.Creeper
				|| target.type == NPCID.PlanterasTentacle
			   )
			{
				manualCheck = true;
			}

			return (variableCheck || headCheck || manualCheck);
        }

        /// <summary>
        /// Tells if an event (Invasion) is active in the world. Includes moon events (Blood, Frost, Eclipse, etc.), Lunar Events, and Slime Rain. Returns an integer that has different meanings, as opposed to a bool
        /// </summary>
		/// <returns>0 if no event is detected, 1 for most events, 2 for Old One's Army, and 3 for Lunar Events.</returns>
        private static int CheckEvents()
        {
            if (DD2Event.Ongoing)
            {
				return 2;
            }
			bool bMoon = Main.bloodMoon;
			bool eclipse = Main.eclipse;
			bool fMoon = Main.snowMoon;
			bool pMoon = Main.pumpkinMoon;
			bool invasion = Main.invasionType != 0;
			bool slime = Main.slimeRain;
            if (bMoon || eclipse || fMoon || pMoon || invasion || slime)
            {
				return 1;
            }
			if (NPC.LunarApocalypseIsUp)
			{
				return 3;
			}
			return 0;
        }

        /// <summary>
        /// Extension of OnHitNPC hooks that takes hit data and uses it to derive and issue XP gain.
        /// </summary>
		/// <param name="player">The player that inflicted the hit. If the XP granted from the hit causes a level up, this player recieves the awareness feedback.</param>
		/// <param name="target">The NPC struck. NPCs with certain properties (Ex: worms) apply multipliers on XP gain. Note that there are also other multipliers for other conditions, like active events.</param>
		/// <param name="hit">The hit's data. The Damage from the HitInfo is used to determine XP gain.</param>
		/// <param name="dmgOverride">If the hit calling this function kills the target, the target's HP before the blow should be filled in here. This patches a bug where killing blows don't grant XP.</param>
		/// <param name="item">The item/weapon that inflicted the hit. If the XP granted from the hit causes a level up, the item's name will be used in level up feedback.</param>
		/// <param name="itemNameOverride">An alternative to providing the 'item' paramenter, used by projectile hits. Conveys the responsible item/weapon's name for feedback purposes.</param>
        public void OnHitXP(Player player, NPC target, NPC.HitInfo hit, int dmgOverride = -1, Item item = null, string itemNameOverride = "")
		{
            if (!ModContent.GetInstance<Config>().xpGain || baseXP <= 0)
            {
				return;
            }

            int xp = hit.Damage;

			if(dmgOverride > 0)
            {
				xp = dmgOverride;
            }

			if(player.GetModPlayer<PlayerLeveling>().valueForEffort) // Effort Rune bonus XP
			{
				float bonusMult = 1f + (Main.rand.NextFloat(0.1f, 0.25f) * ModContent.GetInstance<Config>().effortMultiplier);
				xp = (int)(xp * bonusMult);
			}

            // Worm AI enemies have an XP reduction since they generally have high HP 
            if (target.aiStyle == NPCAIStyleID.Worm || target.aiStyle == NPCAIStyleID.TheDestroyer)
			{
				xp = (int)(xp * ModContent.GetInstance<Config>().wormXP);
			}

			// Bosses and events reduce XP gains when active - given the large health pools - to combat power-leveling and/or minion farming
			if (FindActiveBoss() && CheckEvents() > 0)
			{
				float eventXP = ModContent.GetInstance<Config>().eventXP;
				if(CheckEvents() == 2)
                {
					eventXP *= 0.5f;
				}
				else if(CheckEvents() == 3)
                {
					eventXP = 0.25f + (0.75f * ModContent.GetInstance<Config>().eventXP);
				}
				float mult = (ModContent.GetInstance<Config>().bossXP + eventXP) / 3f;
				xp = (int)(xp * mult);
			}
			else if (FindActiveBoss() && CheckEvents() <= 0)
			{
				xp = (int)(xp * ModContent.GetInstance<Config>().bossXP);
			}
			else if (CheckEvents() > 0 && !FindActiveBoss())
			{
				float eventXP = ModContent.GetInstance<Config>().eventXP;
				if (CheckEvents() == 2)
				{
					eventXP *= 0.5f;
				}
				else if (CheckEvents() == 3)
				{
					eventXP = 0.25f + (0.75f * ModContent.GetInstance<Config>().eventXP);
				}

				xp = (int)(xp * eventXP);
			}

            GainExperience(item, xp, false, player, itemNameOverride);
        }

        /// <summary>
        /// Adds XP to the item. Accounts for caps and levels.
        /// </summary>
		/// <param name="item">The item/weapon gaining XP. If it levels up, the item's name will be used in level up feedback.</param>
		/// <param name="xp">The XP to gain.</param>
		/// <param name="allowNegativeXP">Normally negative xp values are defaulted to 0. Setting this to true allows negative xp values, which deduct from the total experience count. Used by Withdrawl Orbs.</param>
		/// <param name="owner">The owner/holder of the item, if applicable. This player recieves feedback from a level up.</param>
		/// <param name="itemNameOverride">An alternative to providing the 'item' paramenter, used by projectile hits. Conveys the responsible item/weapon's name for feedback purposes.</param>
        public void GainExperience(Item item, int xp, bool allowNegativeXP = false, Player owner = null, string itemNameOverride = "")
		{
			xp = (int)(xp * ModContent.GetInstance<Config>().xpMultiplier);

			// Items using the Mechanical Experience Rune constantly update their Base XP in case the config. value changes
			RecheckBaseXP();

			if (xp < 0 && !allowNegativeXP)
			{
				xp = 0;
			}

			if (experience < ModContent.GetInstance<Config>().xpCap)
			{
                // A 'true' cap of 2 Billion is in place so as to steer well clear of the 32 bit integer limit. XP may never exceed this cap
                if (experience >= BSWLmod.maxXP - xp)
                {
                    experience = BSWLmod.maxXP;
                }
				else
                {
                    experience += xp;
                }
			}
            // Prevent final XP values from being negative
            if (experience < 0)
			{
				experience = 0;
			}

			if (baseXP > 0) 
			{
				CheckLevel(item, experience, owner, itemNameOverride: itemNameOverride); 
			}
		}

        /// <summary>
        /// Updates level based on XP.
        /// </summary>
		/// <param name="item">The item/weapon gaining XP. If it levels up, the item's name will be used in level up feedback.</param>
		/// <param name="xp">The NPC to check.</param>
		/// <param name="owner">The owner/holder of the item, if applicable. This player recieves feedback from a level up.</param>
		/// <param name="loading">Set to true if the game is loading. Bugs are caused if level up feedback occurs during loading, so this prevents it when true.</param>
		/// <param name="itemNameOverride">An alternative to providing the 'item' paramenter, used by projectile hits. Conveys the responsible item/weapon's name for feedback purposes.</param>
        private void CheckLevel(Item item, int xp, Player owner = null, bool loading = false, string itemNameOverride = "")
		{
			int usedBase = baseXP;
			if (baseXP <= 0)
			{
				return;
			}
			int oldLevel = level; // Save the current level before resetting it, for later purposes
			level = 0; // First, set level to 0, we'll work our way up next
			for (int i = 0; i <= 100; i++) // For loop going up to 100, representing weapon level
			{
				level = i; // Set level to i

				// Account for previous levels' XP
				int nonCumulativeXP = xp;
				if (i > 0) 
				{
					int previousXP = 0;
					for (int j = i; j > 0; j--) 
					{
						previousXP += (int)(usedBase * Math.Pow(1.03, j - 1));
					}
					nonCumulativeXP -= previousXP; 

					if (nonCumulativeXP < 0) 
					{
						nonCumulativeXP = 0;
					}
				}

				if (nonCumulativeXP >= (int)(usedBase * Math.Pow(1.03, i)) && level < 100) // Check if there is enough XP to level up
				{
					continue;
				}
				else // If there isn't enough XP, break the loop, cutting level off at the current i value
				{
					break;
				}
			}

			if (level > ModContent.GetInstance<Config>().levelCap) 
			{
				level = ModContent.GetInstance<Config>().levelCap;
			}

			if (level > oldLevel && !loading && owner != null)
			{
                LevelUpFeedback(item, owner, oldLevel, itemNameOverride);
            }
		}

        /// <summary>
        /// Sets an item's level. Used by Mystic and Volatile Experience Orbs.
        /// </summary>
		/// <param name="item">The item/weapon gaining XP. If it levels up, the item's name will be used in level up feedback.</param>
		/// <param name="newLevel">The level to set to.</param>
		/// <param name="owner">The owner/holder of the item, if applicable. This player recieves feedback from a level up.</param>
		/// <param name="itemNameOverride">An alternative to providing the 'item' paramenter, used by projectile hits. Conveys the responsible item/weapon's name for feedback purposes.</param>
        public void ForceLevel(Item item, int newLevel, Player owner = null, string itemNameOverride = "")
		{
            if (baseXP <= 0)
            {
                return;
            }

            int usedBase = baseXP;

			if(newLevel < 0)
            {
				newLevel = 0;
            }
			if(newLevel > ModContent.GetInstance<Config>().levelCap)
            {
				newLevel = ModContent.GetInstance<Config>().levelCap;
            }

            int oldLevel = level;

			// Set XP to the proper amount for the desired level
            int setXP = 0;
            for (int j = newLevel; j > 0; j--)
            {
                setXP += (int)(usedBase * Math.Pow(1.03, j - 1));
            }
            experience = setXP;
            CheckLevel(item, experience, null, true); // Level up checks are done manually, so we return true to 'loading' to prevent them in CheckLevel

            if (level > oldLevel && owner != null)
            {
                LevelUpFeedback(item, owner, oldLevel, itemNameOverride);
            }
        }

        /// <summary>
        /// Generates a sound effect and text popups to notify the player of a level up.
        /// </summary>
		/// <param name = "owner">The player who will recieve the feedback.</param>
		/// <param name = "item">The item that leveled up. Its name will be used in text feedback.</param>
        /// <param name="itemNameOverride">An alternative to providing the 'item' paramenter, used by projectile hits. Conveys the responsible item/weapon's name for text feedback.</param>
        /// <param name="oldLevel">The level before the level up. Not neccessarily one level down, if a large amount XP was gained at once.</param>
        private void LevelUpFeedback(Item item, Player owner, int oldLevel, string itemNameOverride = "")
        {
			if (ModContent.GetInstance<Config>().lvlUpSound)
			{
				if (owner != null)
				{
					SoundEngine.PlaySound(SoundID.ResearchComplete, owner.position);
				}
			}
			string itemName = "";
			if(itemNameOverride != "")
            {
				itemName = itemNameOverride;
			}
            else if(item != null)
            {
				itemName = item.HoverName;
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
                    if (itemName != "")
                    {
                        Main.NewText(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.WeaponMaxOut").FormatWith(oldLevel, level, itemName)
                            , textColor);
                    }
                    else
                    {
                        Main.NewText(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.WeaponMaxOutDef").FormatWith(oldLevel, level)
                            , textColor);
                    }
                    CombatText.NewText(owner.Hitbox, textColor,
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.WeaponMaxOutPopup")
                        , true);
                }
                else
                {
                    if (itemName != "")
                    {
                        Main.NewText(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.WeaponLevelUp").FormatWith(oldLevel, level, itemName)
                            , textColor);
                    }
                    else
                    {
                        Main.NewText(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.WeaponLevelUpDef").FormatWith(oldLevel, level)
                            , textColor);
                    }
                    CombatText.NewText(owner.Hitbox, textColor,
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.WeaponLevelUpPopup"));
                }
            }
		}

        /// <summary>
        /// Returns how much XP is needed to level up, based on a given current XP and level.
        /// </summary>
        /// <param name="currentLevel">The current level; XP needed for the proceeding level is calculated.</param>
		/// <param name="xp">The current, cumulative XP.</param>
        private int GetNeededXP(int xp, int currentLevel)
		{
			int usedBase = baseXP;

			int nonCumulativeXP = xp;
			if (level > 0) 
			{
				int previousXP = 0;
				for (int j = level; j > 0; j--) 
				{
					previousXP += (int)(usedBase * Math.Pow(1.03, j - 1));
				}
				nonCumulativeXP -= previousXP; 

				if (nonCumulativeXP < 0) 
				{
					nonCumulativeXP = 0;
				}
			}

			return ((int)(usedBase * (Math.Pow(1.03, currentLevel)))) - nonCumulativeXP;
		}
		
		public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
		{
			if (ModContent.GetInstance<Config>().dmgMultiplier > 0)
			{
				damage *= GetDamageBoost();
			}
		}

        /// <summary>
        /// Gets a multiplier for damage based on level.
        /// </summary>
        private float GetDamageBoost()
		{
			float damageBoost = (float)Math.Pow(1.01105, level);

            if (BSWLmod.AugmentsCrossMod.wepAugsEnabled && ModContent.GetInstance<Config>().augmentCrossmod)
            {
				// Lower power as trade-off for Augment slots. Base max damage is x2.25 with this formula
				damageBoost = (float)Math.Pow(1.00815, level);
			}

			if (damageBoost > ModContent.GetInstance<Config>().dmgCap) 
			{
				damageBoost = ModContent.GetInstance<Config>().dmgCap;
			}
            // Adjust extra damage based on config
            damageBoost--;
			damageBoost = damageBoost * ModContent.GetInstance<Config>().dmgMultiplier;
			damageBoost++;
			return damageBoost;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			if (experience >= 0 && baseXP > 0) // These tooltips are only displayed if a base XP is established
			{
				Color lvlDisplay = Color.White;
				Color xpDisplay = Color.White;
				Color dmgDisplay = Color.White;
				Color baseXPDisplay = Color.White;
				
				if (ModContent.GetInstance<Config>().coloredDisplays == true) 
				{
					lvlDisplay = ColoredLevelText(level);
					xpDisplay = ColoredXPText(experience, level);
					dmgDisplay = ColoredDamageText(level);
					baseXPDisplay = Color.LightSteelBlue;
				}

				int usedBase = baseXP;

				if (level < 100) 
				{
					tooltips.Add(new TooltipLine(Mod, "level",
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.LvlStat").FormatWith(level))
                    { OverrideColor = lvlDisplay });
				}
				else 
				{
					if (ModContent.GetInstance<Config>().showLvl100 == false)
					{
						tooltips.Add(new TooltipLine(Mod, "level",
                            Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.LvlStat").FormatWith(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.EX")))
                        { OverrideColor = lvlDisplay });
					}
					else
					{
						tooltips.Add(new TooltipLine(Mod, "level",
                            Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.LvlStat").FormatWith(100))
                        { OverrideColor = lvlDisplay });
					}
				}

				if (level < ModContent.GetInstance<Config>().levelCap) 
				{
					int toNextLvl = GetNeededXP(experience, level);
					tooltips.Add(new TooltipLine(Mod, "experience",
                         Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.XPStat").FormatWith(experience, toNextLvl))
                    { OverrideColor = xpDisplay });
				}
				else 
				{
					if (experience < BSWLmod.maxXP)
					{
						tooltips.Add(new TooltipLine(Mod, "experience",
                            Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.XPStat100").FormatWith(experience))
                        { OverrideColor = xpDisplay });
					}
					else 
					{
						if (ModContent.GetInstance<Config>().showMaxXP == false)
						{
							tooltips.Add(new TooltipLine(Mod, "experience",
                                Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.XPStat100").FormatWith(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.Maxed")))
                            { OverrideColor = xpDisplay });
						}
						else
						{
							tooltips.Add(new TooltipLine(Mod, "experience",
                                Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.XPStat100").FormatWith(experience))
                            { OverrideColor = xpDisplay });
						}
					}
				}

				if (ModContent.GetInstance<Config>().showDMG == true && ModContent.GetInstance<Config>().dmgMultiplier > 0)
				{
					if (GetDamageBoost() >= (((3f - 1f) * ModContent.GetInstance<Config>().dmgMultiplier) + 1) && ModContent.GetInstance<Config>().showMaxDMG == false)
					{
						tooltips.Add(new TooltipLine(Mod, "damage",
                            Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.DmgStat").FormatWith(Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.Maxed")))
                        { OverrideColor = dmgDisplay });
					}
					else
					{
						string dmgString = $"x{Math.Round(GetDamageBoost(), 2)}";
						tooltips.Add(new TooltipLine(Mod, "damage",
                            Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.DmgStat").FormatWith(dmgString))
                        { OverrideColor = dmgDisplay });
					}
				}

				if (ModContent.GetInstance<Config>().showBaseXP == true)
				{
					tooltips.Add(new TooltipLine(Mod, "baseXP",
                        Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.BaseXPStat").FormatWith(usedBase)) 
					{ OverrideColor = baseXPDisplay });
				}
			}

			// If the weapon still needs a base XP established, add a tooltip explaining the weapon can't level up, and hint at how to enable the feature
			else if (ModContent.GetInstance<Config>().showDisabledLeveling == true) 
			{
				Color runeRequirementDisplay;
				tooltips.Add(new TooltipLine(Mod, "levelingDisabled",
                    Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.NoLeveling")) 
				{ OverrideColor = Color.DarkGray });
				if (ModContent.GetInstance<Config>().detailedDisabledLeveling == true)
				{
					int rarity = CheckRarity(item);
					// Get and return the needed rune type, and set a corresponding text color at the same time
					string runeString;

                    switch (rarity)
                    {
                        case -1:
                        case 0:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.BasicRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityNormal;
                            break;
                        case 1:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.EnhancedRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityBlue;
                            break;
                        case 2:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.TaintedRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityGreen;
                            break;
                        case 3:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.InfernalRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityOrange;
                            break;
                        case 4:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.TwilightRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityRed;
                            break;
                        case 5:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.HolyRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityPink;
                            break;
                        case 6:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.InfusedRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityPurple;
                            break;
                        case 7:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.VerdantRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityLime;
                            break;
                        case 8:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.EerieRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityYellow;
                            break;
                        case 9:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.XenoRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityCyan;
                            break;
                        case 10:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.MoonRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityDarkRed;
                            break;
                        default:
                            runeString = Language.GetTextValue("Mods.BSWLmod.Items.PhantasmalRune.DisplayName");
                            runeRequirementDisplay = Colors.RarityDarkPurple;
                            break;
                    }

					// Reset the color if colored displays are off
					if(ModContent.GetInstance<Config>().coloredDisplays == false)
                    {
						runeRequirementDisplay = Color.DarkGray;
					}
                    if (Main.keyState.PressingShift())
                    {
						tooltips.Add(new TooltipLine(Mod, "runeRequirements",
                            Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.ViewRunes").FormatWith(runeString))
                        { OverrideColor = runeRequirementDisplay });
					}
                    else
                    {
						tooltips.Add(new TooltipLine(Mod, "viewRunes",
                            Language.GetTextValue("Mods.BSWLmod.CommonItemtooltip.ShiftView")) 
						{ OverrideColor = Color.DarkGray });
					}
				}
			}
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

        /// <summary>
        /// Returns a color - for text display - dynamically based on progress to a level-up.
        /// </summary>
		/// <param name="xp">The current, cumulative XP.</param>
		/// <param name="currentLevel">The level used to determine the next level to be reached.</param>
        private Color ColoredXPText(int xp, int currentLevel)
		{
			Color barColorStart = new(0.29f, 0.36f, 0.6f);
			Color barColorHalf = new(0.34f, 0.6f, 0.7f);
			Color barColorMax = new(0.38f, 0.8f, 0.71f);
			if (currentLevel < ModContent.GetInstance<Config>().levelCap) 
			{
				int usedBase = baseXP;

				int xpToGo = GetNeededXP(xp, currentLevel);
				int nextLvlXP = (int)(usedBase * Math.Pow(1.03, currentLevel));
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
				return ColorSwap(new(97, 204, 181), new(153, 242, 223), 6f);
			}
		}

        /// <summary>
        /// Returns a color - for text display - dynamically based on damage boost from levels.
        /// </summary>
		/// <param name="currentLevel">The level used to determine the damage boost the color should be based on.</param>
        private static Color ColoredDamageText(int currentLevel)
		{
			Color barColorStart = new(0.85f, 0.24f, 0f);
			Color barColorHalf = new(0.87f, 0.05f, 0.05f);
			Color barColorMax = new(0.89f, 0.12f, 0.6f);
			float dmgProgress = currentLevel / 100f; 
			if (dmgProgress >= 1f) 
			{
				return ColorSwap(new(227, 31, 152), new(238, 21, 103), 6f);
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
 