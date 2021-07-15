/*using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class ApparelGraphicRecordGetter_Patch

    {
         //
         //Prefix permettant de correler les bodyType hurted de TX avec les bodyType de base Male et female pour eviter que les vetements deconnes
         //
        [HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
        public class TryGetGraphicApparel_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(ref Apparel apparel, ref BodyTypeDef bodyType, out ApparelGraphicRecord rec, ref bool __result)
            {
                rec = new ApparelGraphicRecord(null, null);
                if (bodyType != null)
                {
                    bool ok = false;
                    //Si body Type Male android with skin
                    if (Utils.ExceptionBodyTypeDefnameAndroidWithSkinMale.Contains(bodyType.defName))
                    {
                        bodyType = BodyTypeDefOf.Male;
                        ok = true;
                    }
                    else if (Utils.ExceptionBodyTypeDefnameAndroidWithSkinFemale.Contains(bodyType.defName))
                    {
                        bodyType = BodyTypeDefOf.Female;
                        ok = true;
                    }
                   
                }
                return true;
            }
        }
    }
}*/