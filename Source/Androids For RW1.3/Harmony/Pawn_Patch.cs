using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using System.Collections.Generic;
using RimWorld.Planet;

namespace MOARANDROIDS
{

    internal class Pawn_Patch
    {
        [HarmonyPatch(typeof(Pawn), "Kill")]
        public class Kill
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
            {
                try
                {
                    if (__instance.IsSurrogateAndroid())
                    {
                        Utils.insideKillFuncSurrogate = true;

                        //Si c'est un surrogate controllé temporaire alors on le restitue a sa faction
                        CompSkyMind csm = Utils.getCachedCSM(__instance);
                        if(csm != null)
                        {
                            //Log.Message("Restitution surrogate a sa faction");
                            csm.tempHackingEnding();
                        }
                    }
                    //disconnect killed user and dont broadcast the message
                    Utils.GCATPP.disconnectUser(__instance, false, false);
                    //Log.Message("YOU KILLED "+__instance.LabelCap);
                    //Is surrogate android used ?
                    if (__instance.IsSurrogateAndroid(true))
                    {
                        //Obtention controlleur
                        CompAndroidState cas = Utils.getCachedCAS(__instance);
                        if (cas == null)
                            return true;

                        //Arret du mode de control chez le controller
                        CompSurrogateOwner cso = Utils.getCachedCSO(cas.surrogateController);
                        cso.stopControlledSurrogate(__instance,false, false, true);

                        //On reset les données pour une potentiel futur resurection
                        cas.resetInternalState();

                    }

                    
                    //Log.Message("YOU KILLED END");
                    Utils.insideKillFuncSurrogate = false;
                    return true;
                }
                catch (Exception e)
                {
                    Log.Message("[ATPP] Pawn.Kill(Error) : " + e.Message + " - " + e.StackTrace);

                    if (__instance.IsSurrogateAndroid())
                        Utils.insideKillFuncSurrogate = false;
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(Pawn), "PreKidnapped")]
        public class PreKidnapped_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn __instance, Pawn kidnapper)
            {
                try
                {
                    if(__instance.def == ThingDefOfAT.M8Mech)
                    {
                        bool activeConn = false;
                        CompSkyCloudCore csc = Utils.getCachedCSC(__instance);
                        csc.isKidnapped = true;

                        if (csc != null) {
                            if (csc.controlledTurrets.Count > 0)
                                activeConn = true;
                            if (!activeConn)
                            {
                                //Check if there are active minds/surrogate connections 
                                foreach (var m in csc.storedMinds)
                                {
                                    CompSurrogateOwner cso = Utils.getCachedCSO(m);
                                    if (cso != null && cso.isThereSX())
                                    {
                                        activeConn = true;
                                        break;
                                    }
                                }
                            }

                            //Active connections then program a random disconnection
                            if (activeConn)
                            {
                                csc.KidnappedPendingDisconnectionGT = Find.TickManager.TicksGame + Rand.Range(Settings.nbMinHoursBeforeKidnappedM8Disconnected*2500, Settings.nbMaxHoursBeforeKidnappedM8Disconnected*2500);
                            }
                        }
                    }
                    else if ((__instance.IsAndroidTier() || __instance.VXChipPresent() || __instance.IsSurrogateAndroid()))
                    {
                        //On deconnecte l'user de force le cas echeant
                        Utils.GCATPP.disconnectUser(__instance);
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] Pawn.PreKidnapped(Error) : " + e.Message + " - " + e.StackTrace);
                }
            }
        }

        [HarmonyPatch(typeof(Pawn), "ButcherProducts")]
        public class ButcherProducts_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn butcher, float efficiency, Pawn __instance)
            {
                if (__instance.IsAndroidTier())
                    Utils.lastButcheredPawnIsAndroid = true;
                else
                    Utils.lastButcheredPawnIsAndroid = false;
            }
        }

        // Patch used to deregister from the mapPawns surrogates (only if the related setting is enabled) And register surrogate in the listerSurrogates
        [HarmonyPatch(typeof(Pawn), "SpawnSetup")]
        public class SpawnSetup_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Map map, bool respawningAfterLoad, Pawn __instance)
            {
                if (__instance.IsSurrogateAndroid())
                {
                    CompAndroidState cas = Utils.getCachedCAS(__instance);
                    if (cas != null)
                    {
                        if(__instance.Downed)
                            Utils.addDownedSurrogateToLister(__instance);

                        if (Settings.hideInactiveSurrogates)
                        {
                            //Remove surrogate from main lists only if inactive surrogate
                            if (cas.surrogateController == null)
                            {
                                //hide only surrogate on player's map
                                if (map != null && map.IsPlayerHome)
                                    map.mapPawns.DeRegisterPawn(__instance);
                            }
                        }
                    }
                }
                //Set a fake rest need to prevent errors
                if (__instance.IsAndroidTier() && __instance.needs != null)
                {
                    __instance.needs.rest = new Need_Rest_Fake(null);
                }
            }
        }

        
        static private Pawn Pawn_GetGizmosPrevPawn;
        static private CompSkyMind Pawn_GetGizmosPrevCSM;
        static private bool Pawn_GetGizmosPrevIsPoweredAnimalAndroids;

        [HarmonyPatch(typeof(Pawn), "GetGizmos")]
        public class GetGizmos_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn __instance, ref IEnumerable<Gizmo> __result)
            {
                //Caching to increase performance on selected pawns
                if(__instance != Pawn_GetGizmosPrevPawn)
                {
                    Pawn_GetGizmosPrevPawn = __instance;
                    Pawn_GetGizmosPrevCSM = Utils.getCachedCSM(__instance);
                    Pawn_GetGizmosPrevIsPoweredAnimalAndroids = __instance.IsPoweredAnimalAndroids();
                }

                //Si prisonnier et possede une VX2 on va obtenir les GIZMOS associés OU virusé
                if ((__instance.IsPrisoner || __instance.IsSlave) || (Pawn_GetGizmosPrevCSM != null && Pawn_GetGizmosPrevCSM.Hacked == 1))
                {
                    IEnumerable<Gizmo> tmp;
                    //Si posseseur d'une VX2

                    if (__instance.VXChipPresent())
                    {
                        CompSurrogateOwner cso = Utils.getCachedCSO(__instance);
                        if (cso != null)
                        {
                            tmp = cso.CompGetGizmosExtra();
                            if (tmp != null)
                                __result = __result.Concat(tmp);
                        }
                    }

                    //Si android prisonier ou virusé
                    if (__instance.IsAndroidTier())
                    {
                        CompAndroidState cas = Utils.getCachedCAS(__instance);

                        if (cas != null)
                        {
                            tmp = cas.CompGetGizmosExtra();
                            if (tmp != null)
                                __result = __result.Concat(tmp);
                        }
                    }

                    if (Pawn_GetGizmosPrevCSM != null && Pawn_GetGizmosPrevCSM.Hacked == -1)
                    {
                        tmp = Pawn_GetGizmosPrevCSM.CompGetGizmosExtra();
                        if (tmp != null)
                            __result = __result.Concat(tmp);
                    }
                }

                //Si animal posséder par player
                if (Pawn_GetGizmosPrevIsPoweredAnimalAndroids)
                {
                    CompAndroidState cas = null;
                    cas = Utils.getCachedCAS(__instance);
                    if (cas != null)
                    {
                        IEnumerable<Gizmo> tmp = cas.CompGetGizmosExtra();
                        if (tmp != null)
                            __result = __result.Concat(tmp);
                    }
                }
            }
        }
    }
}