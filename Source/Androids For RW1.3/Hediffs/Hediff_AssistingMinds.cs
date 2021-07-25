using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class Hediff_AssistingMinds : HediffWithComps
    { 
        public override bool ShouldRemove
        {
            get
            {
                if (!Utils.GCATPP.isConnectedToSkyMind(pawn) || Utils.GCATPP.getNbAssistingMinds() < 10)
                    return true;
                else
                    return false;
            }
        }

        public override int CurStageIndex
        {
            get
            {
                int curStage = (Utils.GCATPP.getNbAssistingMinds() - 10) / 5;
                if (curStage > 9)
                    curStage = 9;
                if (curStage < 0)
                    curStage = 0;

                return curStage;
            }
        }


        public override bool Visible
        {
            get
            {
                return true;
            }
        }
    }
}
