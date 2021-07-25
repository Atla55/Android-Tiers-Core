using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class ThoughtWorker_SharedBed_Patch

    {
        [HarmonyPatch(typeof(ThoughtWorker_SharedBed), "CurrentStateInternal")]
        public class CurrentStateInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn p, ref ThoughtState __result)
            {
                if ( p.IsBasicAndroidTier() 
                    || p.IsSurrogateAndroid() 
                    || Utils.pawnCurrentlyControlRemoteSurrogate(p)
                    || realyDislikedPartnerAfterSurrogateResolving(p))
                {
                    __result = ThoughtState.Inactive;
                }
            }

            /*
             * We redo the job done by GetMostDislikedNonPartnerBedOwner, because there can be a surrogate which is infact the spouse/lover of the current 'p' pawn
             */
            private static bool realyDislikedPartnerAfterSurrogateResolving(Pawn p)
            {
                Building_Bed ownedBed = p.ownership.OwnedBed;
                if (ownedBed == null)
                {
                    return false;
                }
                for (int i = 0; i < ownedBed.OwnersForReading.Count; i++)
                {
                    if (ownedBed.OwnersForReading[i] != p)
                    {
                        Pawn cp = ownedBed.OwnersForReading[i];
                        //Try get pawn behind the surrogate
                        CompAndroidState cas = Utils.getCachedCAS(cp);
                        if(cas != null)
                        {
                            if (cas.isSurrogate && cas.surrogateController != null)
                                cp = cas.surrogateController;
                        }
                        if (!LovePartnerRelationUtility.LovePartnerRelationExists(p, cp))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}