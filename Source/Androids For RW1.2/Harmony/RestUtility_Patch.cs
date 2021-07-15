using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace MOARANDROIDS
{
    internal class RestUtility_Patch

    {
        /*
         * PostFix évitant d'attribuer de need comfort et outdoor aux T1 et T2 et l'hygiene a l'ensemble des robots
         */
        [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
        public class IsValidBedFor_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Thing bedThing, Pawn sleeper, Pawn traveler, bool sleeperWillBePrisoner, bool checkSocialProperness, bool allowMedBedEvenIfSetToNoCare, bool ignoreOtherReservations, ref bool __result)
            {
                try
                {
                    bool bedIsSurrogateM7Pod = Utils.ExceptionSurrogateM7Pod.Contains(bedThing.def.defName);
                    bool bedIsSurrogatePod = Utils.ExceptionSurrogatePod.Contains(bedThing.def.defName);
                    //bool sleeperIsNotControlledSurrogate = sleeper.IsSurrogateAndroid(false, true);
                    bool sleeperIsSurrogate = sleeper.IsSurrogateAndroid();
                    bool sleeperIsRegularAndroid = Utils.ExceptionRegularAndroidList.Contains(sleeper.def.defName);
                    bool isSurrogateM7 = (sleeper.def.defName == Utils.M7 && sleeperIsSurrogate);
                    bool isSleepingSpot = bedThing.def.defName == "SleepingSpot" || bedThing.def.defName == "DoubleSleepingSpot";

                    //Intediction aux non surrogates l'usage des PODS
                    //PodM7
                    if (bedIsSurrogateM7Pod)
                    {
                        //SI pas un surrogate M7 alors pas d'utilisation possible
                        if (! isSurrogateM7)
                        {
                            __result = false;
                        }
                    }
                    else if (bedIsSurrogatePod)
                    {
                        //Si pas un surrogate standard alors utilisation pas possible
                        if (!(sleeperIsRegularAndroid && sleeper.def.defName != Utils.M7))
                            __result = false;
                    }

                    //Interdiction aux szurrogates de se servir des autres lits
                    if(!bedIsSurrogatePod && !bedIsSurrogateM7Pod && !isSleepingSpot)
                    {
                        //Si M7 et surrogate controlé ou non ==>interdiction OU si surrogate android non controllé ==>Interdiction
                        if (isSurrogateM7 || sleeperIsRegularAndroid )
                            __result = false;
                    }

                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] RestUtility.IsValidBedFor : " + e.Message + " - " + e.StackTrace);
                }
            }
        }
    }
}