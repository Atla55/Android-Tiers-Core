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
    internal class TaleUtility_Patch

    {
        /*
         * Equivalent au patching en prefixe de la methode KILL, car aussinon il fallait un PREFIX avec les incompatibilitées que sa peut entrainer
         */
        [HarmonyPatch(typeof(TaleUtility), "Notify_PawnDied")]
        public class Notify_PawnDied_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn victim, DamageInfo? dinfo)
            {
                try
                {
                    if (victim.IsSurrogateAndroid())
                        Utils.insideKillFuncSurrogate = true;
                    //Deconnecte le tué
                    Utils.GCATPP.disconnectUser(victim);
                    //Log.Message("YOU KILLED "+__instance.LabelCap);
                    //Is surrogate android used ?
                    if (victim.IsSurrogateAndroid(true))
                    {
                        //Obtention controlleur
                        CompAndroidState cas = victim.TryGetComp<CompAndroidState>();
                        if (cas == null)
                            return;

                        //Arret du mode de control chez le controller
                        CompSurrogateOwner cso = cas.surrogateController.TryGetComp<CompSurrogateOwner>();
                        cso.stopControlledSurrogate(victim);
                    }
                    //Log.Message("YOU KILLED END");
                    Utils.insideKillFuncSurrogate = false;
                }
                catch (Exception e)
                {
                    Log.Message("[ATPP] TaleUtility.Notify_PawnDied(Error) : " + e.Message + " - " + e.StackTrace);

                    if (victim.IsSurrogateAndroid())
                        Utils.insideKillFuncSurrogate = false;
                }
            }
        }
    }
}