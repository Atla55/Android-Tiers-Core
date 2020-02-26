using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{

    // Token: 0x02000200 RID: 512
    public class ThoughtWorker_UncomfortableClothing : ThoughtWorker
    {
        // Token: 0x06000975 RID: 2421 RVA: 0x00048EA0 File Offset: 0x000472A0
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            string text = null;
            int num = 0;
            List<Apparel> wornApparel = p.apparel.WornApparel;
            for (int i = 0; i < wornApparel.Count; i++)
            {
                if (wornApparel[i].Stuff == ThingDefOf.SteelWool)
                {
                    if (text == null)
                    {
                        text = wornApparel[i].def.label;
                    }
                    num++;
                }
            }
            if (num == 0)
            {
                return ThoughtState.Inactive;
            }
            if (p.IsAndroid() == false)
            {
                if (num >= 5)
                {
                    return ThoughtState.ActiveAtStage(4, text);
                }
                return ThoughtState.ActiveAtStage(num - 1, text);
            }
            else
            {
                return ThoughtState.Inactive;
            }

        }
    }

}