using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class Hediff_SolarFlare : HediffWithComps
    {
        public override bool ShouldRemove
        {
            get
            {
                return false;
            }
        }

        public override HediffStage CurStage
        {
            get
            {
                return this.def.stages[CurStageIndex];
            }
        }

        public override float Severity
        {
            get
            {
                if (CurStageIndex == 1)
                    return 0.1f;
                else
                    return 0f;
            }

        }

        public override int CurStageIndex
        {
            get
            {
                if (!init)
                    initCache();

                if (Settings.disableSolarFlareEffect)
                {
                    if(lastStage != 0)
                    {
                        lastStage = 0;
                        pawn.health.capacities.Notify_CapacityLevelsDirty();
                    }
                    return 0;
                }

                int curGT = Find.TickManager.TicksGame;
                if (curGT >= executorGT)
                {
                    int newVal = lastStage;
                    executorGT = curGT + Rand.Range(360, 900);

                    if (Find.World.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare)
                    && ((isAndroid && !isTXWithSKin) || (isOrganic && withVXChip)))
                    {
                        newVal = 1;
                    }
                    else
                    {
                        newVal = 0;
                    }

                    if (newVal != lastStage)
                        pawn.health.capacities.Notify_CapacityLevelsDirty();

                    lastStage = newVal;
                }

                return lastStage;
            }
        }


        public override bool Visible
        {
            get
            {
                return CurStageIndex == 1;
            }
        }

        private void initCache()
        {
            if (pawn.IsAndroidOrAnimalTier())
                isAndroid = true;
            if (!isAndroid)
                isOrganic = true;
            if (pawn.VXAndVX0ChipPresent())
                withVXChip = true;
            if (pawn.def.defName == Utils.TX2 || pawn.def.defName == Utils.TX3 || pawn.def.defName == Utils.TX4)
                isTXWithSKin = true;

            init = true;
        }

        private bool init = false;
        private int lastStage = 0;
        private int executorGT = -1;
        public bool withVXChip = false;
        private bool isOrganic = false;
        private bool isTXWithSKin = false;
        private bool isAndroid = false;
    }
}
