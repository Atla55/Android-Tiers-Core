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
                bool bedIsAndroidM7Pod = Utils.ExceptionSurrogateM7Pod.Contains(bedThing.def.defName);
                bool bedIsAndroidPod = Utils.ExceptionSurrogatePod.Contains(bedThing.def.defName);
                //bool sleeperIsNotControlledSurrogate = sleeper.IsSurrogateAndroid(false, true);
                bool sleeperIsRegularAndroid = Utils.ExceptionRegularAndroidList.Contains(sleeper.def.defName);
                bool isSurrogateM7 = (sleeper.def == ThingDefOfAT.M7Mech && sleeper.IsSurrogateAndroid());
                bool isM8 = sleeper.def == ThingDefOfAT.M8Mech;
                bool isHospitalBed = bedThing.def == ThingDefOfAT.HospitalBed;
                //bool isSleepingSpot = bedThing.def.defName == "SleepingSpot" || bedThing.def.defName == "DoubleSleepingSpot";

                //Intediction aux non android l'usage des PODS
                //PodM7
                if (bedIsAndroidM7Pod)
                {
                    //SI pas un surrogate M7 ou M8 alors pas d'utilisation possible
                    if (!(isSurrogateM7 || isM8))
                    {
                        __result = false;
                    }
                }
                else if (bedIsAndroidPod)
                {
                    //Si pas un surrogate standard alors utilisation pas possible
                    if (!(sleeperIsRegularAndroid && (sleeper.def.defName != Utils.M7 && sleeper.def.defName != Utils.M8)))
                        __result = false;
                }
                else if(!Settings.allowAndroidToUseHospitalBed && isHospitalBed && sleeperIsRegularAndroid)
                {
                    __result = false;
                }
            }
        }
    }
}