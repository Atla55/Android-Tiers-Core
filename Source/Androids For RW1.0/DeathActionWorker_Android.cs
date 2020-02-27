using System;
using Androids.Integration;
using RimWorld;
using Verse;

namespace Androids
{
	// Token: 0x02000037 RID: 55
	public class DeathActionWorker_Android : DeathActionWorker
	{
		// Token: 0x060000EB RID: 235 RVA: 0x0000969C File Offset: 0x0000789C
		public override void PawnDied(Corpse corpse)
		{
			bool flag = !AndroidsModSettings.Instance.androidExplodesOnDeath;
			if (!flag)
			{
				Pawn innerPawn = corpse.InnerPawn;
				EnergyTrackerComp energyTrackerComp = corpse.InnerPawn.TryGetComp<EnergyTrackerComp>();
				bool flag2 = energyTrackerComp != null;
				if (flag2)
				{
					bool flag3 = innerPawn.health.hediffSet.hediffs.Any((Hediff hediff) => hediff.CauseDeathNow());
					Hediff firstHediffOfDef = innerPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ChjOverheating, false);
					bool flag4 = firstHediffOfDef != null && firstHediffOfDef.Severity >= 1f;
					bool flag5 = flag4 || !flag3;
					if (flag5)
					{
						float num = AndroidsModSettings.Instance.androidExplosionRadius * energyTrackerComp.energy;
						bool flag6 = flag4;
						if (flag6)
						{
							num *= 2f;
						}
						bool flag7 = num >= 1f;
						if (flag7)
						{
							GenExplosion.DoExplosion(corpse.Position, corpse.Map, num, DamageDefOf.Bomb, corpse.InnerPawn, -1, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
						}
					}
				}
				else
				{
					Log.Warning("Androids.DeathActionWorker_Android: EnergyTrackerComp is null at " + corpse.ThingID);
				}
			}
		}
	}
}
