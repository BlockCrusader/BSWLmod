using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BSWLmod.Content.ExperienceOrbs;
using System.IO;
using BSWLmod.Common;

namespace BSWLmod
{
	public class BSWLmod : Mod
	{
		internal static BSWLmod Instance;

		public override void Unload()
		{
			Instance = null;
		}

		public static int rarity0XP = 1000;			
		public static int rarity1XP = 2000;			
		public static int rarity2XP = 4000;			
		public static int rarity3XP = 6500;			
		public static int rarity4XP = 10000;		
		public static int rarity5XP = 17500;		
		public static int rarity6XP = 26000;		
		public static int rarity7XP = 40000;		
		public static int rarity8XP = 55000;		
		public static int rarity9XP = 75000;		
		public static int rarity10XP = 100000;		
		public static int rarity11XP = 300000;		
		public static int overclockXP = 3000000;
        public static int maxBaseXP = 3250000;

        public static int maxXP = 2000000000;

        public static int orb1XP = 100;				
		public static int orb2XP = 300;				
		public static int orb3XP = 1000;			
		public static int orb4XP = 3000;			
		public static int orb5XP = 10000;

		// Netcoding for Player XP, based on Example Mod's netcode
		internal enum MessageType : byte
		{
			SyncPlayerXP
		}
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType)
			{
				case MessageType.SyncPlayerXP:
					byte playernumber = reader.ReadByte();
					PlayerLeveling xpPlayer = Main.player[playernumber].GetModPlayer<PlayerLeveling>();
					xpPlayer.ReceivePlayerSync(reader);

					if (Main.netMode == NetmodeID.Server)
					{
						// Forward the changes to the other clients
						xpPlayer.SyncPlayer(-1, whoAmI, false);
					}
					break;
				default:
					Logger.WarnFormat("BSWLmod: Unknown Message type: {0}", msgType);
					break;
			}
		}

		// Used for cross-mod content
		public class AugmentsCrossMod : ModSystem
		{
			public static bool wepAugsEnabled = false;

			public override void PostSetupContent()
			{
				wepAugsEnabled = ModLoader.HasMod("WeaponAugs");
			}
		}

		public class BSWLRecipesSystems : ModSystem
		{
			public override void AddRecipeGroups() 
			{
				RecipeGroup groupGems = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Gem", new int[] 
				{
				ItemID.Diamond,
				ItemID.Amber,
				ItemID.Ruby,
				ItemID.Emerald,
				ItemID.Sapphire,
				ItemID.Topaz,
				ItemID.Amethyst

				});
				RecipeGroup.RegisterGroup("DSWLmod:Gems", groupGems); 

				RecipeGroup groupFrags = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Lunar Fragment", new int[] 
				{
				ItemID.FragmentStardust,
				ItemID.FragmentVortex,
				ItemID.FragmentSolar,
				ItemID.FragmentNebula

				});
				RecipeGroup.RegisterGroup("DSWLmod:LunarFrags", groupFrags);

				RecipeGroup groupBars1 = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Copper/Tin Bar", new int[]
				{
				ItemID.CopperBar,
				ItemID.TinBar

				});
				RecipeGroup.RegisterGroup("DSWLmod:Bars1", groupBars1);

				RecipeGroup groupBars2 = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Iron/Lead Bar", new int[] // TODO: Replace with vanilla group
				{
				ItemID.IronBar,
				ItemID.LeadBar

				});
				RecipeGroup.RegisterGroup("DSWLmod:Bars2", groupBars2);

				RecipeGroup groupBars3 = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Silver/Tungsten Bar", new int[] 
				{
				ItemID.SilverBar,
				ItemID.TungstenBar

				});
				RecipeGroup.RegisterGroup("DSWLmod:Bars3", groupBars3);

				RecipeGroup groupBars4 = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Gold/Platinum Bar", new int[] 
				{
				ItemID.GoldBar,
				ItemID.PlatinumBar

				});
				RecipeGroup.RegisterGroup("DSWLmod:Bars4", groupBars4);
				 
				RecipeGroup groupEvilStuff = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Shadow Scale/Tissue Sample", new int[] 
				{
				ItemID.ShadowScale,
				ItemID.TissueSample

				});
				RecipeGroup.RegisterGroup("DSWLmod:EvilStuff", groupEvilStuff);

				RecipeGroup groupEvilDust = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Vile/Vicious Powder", new int[] 
				{
				ItemID.VilePowder,
				ItemID.ViciousPowder

				});
				RecipeGroup.RegisterGroup("DSWLmod:EvilDust", groupEvilDust);
			}
		}

		public class BSWLloot : GlobalNPC
		{
			public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
			{
				LeadingConditionRule noCondition = new LeadingConditionRule(new NoCondition());
				LeadingConditionRule bossDowned = new LeadingConditionRule(new BossDowned());
				LeadingConditionRule hardmodeActive = new LeadingConditionRule(new HardmodeActive());
				
				// Global drops for XP orbs
				if (npc.CanBeChasedBy() && npc.SpawnedFromStatue == false)
                {
					if(npc.boss == false) 
                    {
                        npcLoot.Add(noCondition);
                        npcLoot.Add(bossDowned);
                        npcLoot.Add(hardmodeActive);

                        noCondition.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TinyDormant>(), 500));
						noCondition.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SmallDormant>(), 1000));
						
						bossDowned.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Dormant>(), 2000));
						
						hardmodeActive.OnSuccess(ItemDropRule.Common(ModContent.ItemType<BigDormant>(), 4000));
						hardmodeActive.OnSuccess(ItemDropRule.Common(ModContent.ItemType<HugeDormant>(), 8000));

						noCondition.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<ShinyDormant>(), 10000, 5000));
					}
					else // Bosses get x5 rolls with better rates which are even better in Expert or above
                    {
                        npcLoot.Add(noCondition);
                        npcLoot.Add(noCondition);
                        npcLoot.Add(noCondition);
                        npcLoot.Add(noCondition);
                        npcLoot.Add(noCondition);
                        npcLoot.Add(bossDowned);
                        npcLoot.Add(bossDowned);
                        npcLoot.Add(bossDowned);
                        npcLoot.Add(bossDowned);
                        npcLoot.Add(bossDowned);
                        npcLoot.Add(hardmodeActive);
                        npcLoot.Add(hardmodeActive);
                        npcLoot.Add(hardmodeActive);
                        npcLoot.Add(hardmodeActive);
                        npcLoot.Add(hardmodeActive);

                        noCondition.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<TinyDormant>(), 375, 250));
						noCondition.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<SmallDormant>(), 750, 500));

						bossDowned.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<Dormant>(), 1500, 1000));

						hardmodeActive.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<BigDormant>(), 3000, 2000));
						hardmodeActive.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<HugeDormant>(), 6000, 4000));

						noCondition.OnSuccess(ItemDropRule.NormalvsExpert(ModContent.ItemType<ShinyDormant>(), 7000, 3000));
					}
				}
				
			}
		}

		// This one always returns true, and is used to make the item drops not appear in the bestiary (Preventing clutter) and also check the dropOrbs config
		public class NoCondition : IItemDropRuleCondition, IProvideItemConditionDescription
		{
			public bool CanDrop(DropAttemptInfo info) => ModContent.GetInstance<Config>().dropOrbs;
			public bool CanShowItemDropInUI() => false;
			public string GetConditionDescription() => null;
		}

		// This checks for the defeat of a pre-hardmode boss
		public class BossDowned : IItemDropRuleCondition, IProvideItemConditionDescription
		{
			public bool CanDrop(DropAttemptInfo info) => ModContent.GetInstance<Config>().dropOrbs &&
				(NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedDeerclops || NPC.downedQueenBee || NPC.downedSlimeKing || Main.hardMode); 
			public bool CanShowItemDropInUI() => false;
			public string GetConditionDescription() => null;
		}

		// A check for hardmode already exsists, but this is made to prevent the drops appearing in the bestiary and also check the dropOrbs config
		public class HardmodeActive : IItemDropRuleCondition, IProvideItemConditionDescription
		{
			public bool CanDrop(DropAttemptInfo info) => ModContent.GetInstance<Config>().dropOrbs && Main.hardMode;
			public bool CanShowItemDropInUI() => false;
			public string GetConditionDescription() => null;
		}
	}
}