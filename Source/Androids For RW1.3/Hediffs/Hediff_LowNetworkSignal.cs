using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class Hediff_LowNetworkSignal : HediffWithComps
    {
        public override bool ShouldRemove
        {
            get
            {
                return false;
            }
        }

        public override bool Visible
        {
            get
            {
                return CurrentlyLowNetworkSignal() == 1;
            }
        }

        public override float Severity {
            get
            {
                if (CurrentlyLowNetworkSignal() == 1)
                    return 0.1f;
                else
                    return 0.0f;
            }

        }

        public override HediffStage CurStage
        {
            get
            {
                return this.def.stages[CurrentlyLowNetworkSignal()];
            }
        }

        public override int CurStageIndex
        {
            get
            {
                return CurrentlyLowNetworkSignal();
            }
        }

        private int CurrentlyLowNetworkSignal()
        {
            int curGT = Find.TickManager.TicksGame;
            if ( curGT >= executorGT)
            {
                int newVal = lastCurrentlyLowNetworkSignal;
                //If low network perf disabled by settings or the pawn have an RX chip
                if (Settings.disableLowNetworkMalus || (Settings.disableLowNetworkMalusInCaravans && pawn.Map == null) || pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_HediffRXChip) != null)
                    newVal = 0;
                else
                {
                    //Caching CAS comp
                    if (cas == null)
                        cas = Utils.getCachedCAS(pawn);

                    if (cas != null)
                    {
                        if (cas.isSurrogate && cas.surrogateController != null)
                        {
                            //Other factions surrogate always have the lowSkymind network debuff (except if generated with a RX chip)
                            if(pawn.Faction != Faction.OfPlayer)
                            {
                                newVal = 1;
                            }
                            else{
                                if (!Utils.GCATPP.isThereSkyMindAntennaOrRelayInMap(pawn.Map.GetUniqueLoadID()))
                                    newVal = 1;
                                else
                                    newVal = 0;
                            }
                        }
                        else
                            newVal = 0;
                    }
                    else
                        newVal = 0;

                    //Log.Message("H=> " + pawn.LabelCap + " controlled = " +(cas.surrogateController != null)+" ANtenna = "+ Utils.GCATPP.isThereSkyMindAntennaOrRelayInMap(pawn.Map)+" res = "+ lastCurrentlyLowNetworkSignal);
                }

                if (newVal != lastCurrentlyLowNetworkSignal)
                    pawn.health.capacities.Notify_CapacityLevelsDirty();

                lastCurrentlyLowNetworkSignal = newVal;
                executorGT = curGT + Rand.Range(160, 420);
            }
            return lastCurrentlyLowNetworkSignal;
        }

        private int executorGT = 0;
        private int lastCurrentlyLowNetworkSignal = 0;
        private CompAndroidState cas;
    }
}
