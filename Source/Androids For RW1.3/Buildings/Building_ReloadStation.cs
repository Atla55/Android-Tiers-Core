using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MOARANDROIDS
{
    public class Building_ReloadStation : Building
    {
        private List<Pawn> tmpPawnsCanReach = new List<Pawn>();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

        }

        private FloatMenuOption GetFailureReason(Pawn myPawn)
        {
            if (!myPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Some, false, false, TraverseMode.ByPawn))
            {
                return new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            if (base.Spawned && base.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare))
            {
                return new FloatMenuOption("CannotUseSolarFlare".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            if (!this.TryGetComp<CompPowerTrader>().PowerOn)
            {
                return new FloatMenuOption("CannotUseNoPower".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            if ( !Utils.ExceptionAndroidList.Contains(myPawn.def.defName))
            {
                return new FloatMenuOption("ATPP_CanOnlyBeUsedByAndroid".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }

            CompAndroidState ca = myPawn.TryGetComp<CompAndroidState>();
            if (ca == null || !ca.UseBattery)
                return new FloatMenuOption("ATPP_CannotUseBecauseNotInBatteryMode".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);


            CompReloadStation rs = this.TryGetComp<CompReloadStation>();
            int nb = rs.getNbAndroidReloading(true);

            if(nb >= 8)
            {
                return new FloatMenuOption("ATPP_CannotUseEveryPlaceUsed".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }

            return null;
        }

        public override IEnumerable<FloatMenuOption> GetMultiSelectFloatMenuOptions(List<Pawn> selPawns)
        {
            FloatMenuOption failureReason = null;
            this.tmpPawnsCanReach.Clear();
            foreach(var cp in selPawns)
            {
                failureReason = this.GetFailureReason(cp);
                if (failureReason == null)
                {
                    this.tmpPawnsCanReach.Add(cp);
                }
            }
            
            if (this.tmpPawnsCanReach.NullOrEmpty<Pawn>())
            {
                if (failureReason != null)
                    yield return failureReason;
                else
                    yield break;
            }
            else
            {
               
                 yield return new FloatMenuOption("ATPP_ForceReload".Translate(), delegate(){
                     CompReloadStation rs = this.TryGetComp<CompReloadStation>();
                     foreach(var cp in this.tmpPawnsCanReach) {
                         IntVec3 reloadPlacePos = rs.getFreeReloadPlacePos(cp);
                         Job job = new Job(DefDatabase<JobDef>.GetNamed("ATPP_GoReloadBattery"), new LocalTargetInfo(reloadPlacePos), new LocalTargetInfo(this));
                         cp.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                     }

                 }, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
        }

    }
}
