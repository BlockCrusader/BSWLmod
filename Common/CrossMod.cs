using Terraria;
using Terraria.ModLoader;

namespace BSWLmod.Common
{
	public class CrossModHelper : GlobalItem
	{
		public override bool AppliesToEntity(Item item, bool lateInstatiation)
		{
			return item != null;
		}

		public static int GetLevel(Item item)
        {
			if(!item.TryGetGlobalItem(out ItemLeveling levelItem) || !ModContent.GetInstance<Config>().augmentCrossmod)
            {
				return 0;
            }
			return levelItem.level;
        }
	}
}