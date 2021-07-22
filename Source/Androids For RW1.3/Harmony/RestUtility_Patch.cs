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
            public static void Listener(ref bool __result, Thing bedThing, Pawn sleeper, Pawn traveler, bool checkSocialProperness, bool allowMedBedEvenIfSetToNoCare, bool ignoreOtherReservations, GuestStatus? guestStatus = null)
            {
                try
                {
                    bool bedIsAndroidM7Pod = Utils.ExceptionSurrogateM7Pod.Contains(bedThing.def.defName);
                    bool bedIsAndroidPod = Utils.ExceptionSurrogatePod.Contains(bedThing.def.defName);
                    //bool sleeperIsNotControlledSurrogate = sleeper.IsSurrogateAndroid(false, true);
                    bool sleeperIsSurrogate = sleeper.IsSurrogateAndroid();
                    bool sleeperIsRegularAndroid = Utils.ExceptionRegularAndroidList.Contains(sleeper.def.defName);
                    bool isSurrogateM7 = (sleeper.def == ThingDefOfAT.M7Mech && sleeperIsSurrogate);
                    //bool isSleepingSpot = bedThing.def.defName == "SleepingSpot" || bedThing.def.defName == "DoubleSleepingSpot";

                    //Intediction aux non android l'usage des PODS
                    //PodM7
                    if (bedIsAndroidM7Pod)
                    {
                        //SI pas un surrogate M7 alors pas d'utilisation possible
                        if (! isSurrogateM7)
                        {
                            __result = false;
                        }
                    }
                    else if (bedIsAndroidPod)
                    {
                        //Si pas un surrogate standard alors utilisation pas possible
                        if (!(sleeperIsRegularAndroid && sleeper.def.defName != Utils.M7))
                            __result = false;
                    }

                    //Interdiction aux szurrogates de se servir des autres lits
                    /*if(!bedIsAndroidPod && !bedIsAndroidM7Pod && !isSleepingSpot)
                    {
                        //Si M7 et surrogate controlé ou non ==>interdiction OU si surrogate android non controllé ==>Interdiction
                        if (isSurrogateM7 || sleeperIsRegularAndroid )
                            __result = false;
                    }*/

                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] RestUtility.IsValidBedFor : " + e.Message + " - " + e.StackTrace);
                }
            }
        }
    }
}