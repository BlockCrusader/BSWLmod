using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BSWLmod.Common
{
	// Establishes a connection between projectiles and their parent weapon (Or rather, the respecitve ItemLeveling instance)
	// This allows projectile hits to yield XP to the respective weapon
	internal class ProjectileSync : GlobalProjectile
	{
		private ItemLeveling sourceGlobalItem;
		private string sourceItemName = "";
		public override bool InstancePerEntity => true;
		public override void OnSpawn(Projectile projectile, IEntitySource source)
		{
			if (source is IEntitySource_WithStatsFromItem itemSource)
			{
				itemSource.Item.TryGetGlobalItem(out sourceGlobalItem);
				sourceItemName = itemSource.Item.HoverName;
			}
			else if (source is EntitySource_Parent parent && parent.Entity is Projectile proj) // Adds support for secondary (As well as tertiary and so on) projectiles
			{
				ProjectileSync parentProj = proj.GetGlobalProjectile<ProjectileSync>();
				if (parentProj.sourceGlobalItem != null)
				{
					sourceGlobalItem = parentProj.sourceGlobalItem;
					sourceItemName = parentProj.sourceItemName;
				}
			}
		}

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
		{
            // Limits what kinds of enemies can yield XP to prevent cheesing (Mainly statue or target dummy farming)
            if (!target.immortal && target.chaseable && target.lifeMax > 5 && !target.dontTakeDamage && !target.friendly && !target.SpawnedFromStatue)
            {
                if (sourceGlobalItem == null)
				{
					return;
				}

				int owner = projectile.owner;
				if (owner < 0 || owner >= Main.player.Length)
				{
					return;
				}

				Player player = Main.player[owner];
				int dmgOverride = -1;
                // Patches killing blows not properly granting XP by setting it to be the target's last HP count before death
				if (target.life <= 0 && target.realLife == -1 && target.TryGetGlobalNPC(out KillingBlowPatch globalTarget))
                {
                    if (globalTarget.oldHP > 0)
                    {
                        dmgOverride = globalTarget.oldHP;
                    }
                }
                sourceGlobalItem.OnHitXP(player, target, hit, dmgOverride, itemNameOverride: sourceItemName);
			}
		}
	}
}