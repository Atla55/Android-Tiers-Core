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
            CompPowerTrader cpt = Utils.getCachedCPT(this);
            if (!cpt.PowerOn)
            {
                return new FloatMenuOption("CannotUseNoPower".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            if ( !myPawn.IsAndroidTier())
            {
                return new FloatMenuOption("ATPP_CanOnlyBeUsedByAndroid".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }

            CompAndroidState ca = Utils.getCachedCAS(myPawn);
            if (ca == null || !ca.UseBattery)
                return new FloatMenuOption("ATPP_CannotUseBecauseNotInBatteryMode".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);


            CompReloadStation rs = Utils.getCachedReloadStation(this);
            int nb = rs.getNbAndroidReloading(true);

            if(nb >= 8)
            {
                return new FloatMenuOption("ATPP_CannotUseEveryPlaceUsed".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }

            return null;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
        {
            FloatMenuOption failureReason = this.GetFailureReason(myPawn);
            if (failureReason != null)
            {
                yield return failureReason;
            }
            else
            {

                yield return new FloatMenuOption("ATPP_ForceReload".Translate(), delegate () {
                    CompReloadStation rs = Utils.getCachedReloadStation(this);

                    Job job = new Job(JobDefOfAT.ATPP_GoReloadBattery, new LocalTargetInfo(rs.getFreeReloadPlacePos(myPawn)), new LocalTargetInfo(this));
                    myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);

                }, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
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
                     CompReloadStation rs = Utils.getCachedReloadStation(this);
                     foreach(var cp in this.tmpPawnsCanReach) {
                         IntVec3 reloadPlacePos = rs.getFreeReloadPlacePos(cp);
                         Job job = new Job(JobDefOfAT.ATPP_GoReloadBattery, new LocalTargetInfo(reloadPlacePos), new LocalTargetInfo(this));
                         cp.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                     }

                 }, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
        }

    }
}
