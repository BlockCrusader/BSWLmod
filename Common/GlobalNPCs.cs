using Terraria;
using Terraria.ModLoader;

namespace BSWLmod.Common
{
    // Helps patch a bug where killing blows don't grant XP
    // This GlobalNPC is called upon in order to default the xp value to whatever HP the target had remaining before the blow
	public class KillingBlowPatch : GlobalNPC
	{
        public override bool InstancePerEntity => true;

        public int oldHP;

        public override bool PreAI(NPC npc)
        {
            if (npc.life > 0)
            {
                oldHP = npc.life;
            }
            return true;
        }
    }
}