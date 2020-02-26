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
    internal class KidnapAIUtility_Patch

    {
        [HarmonyPatch(typeof(KidnapAIUtility), "TryFindGoodKidnapVictim")]
        public class TryFindGoodKidnapVictim_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref bool __result, Pawn kidnapper, float maxDist, ref Pawn victim, List<Thing> disallowed = null)
            {
                if(__result && victim.IsSurrogateAndroid())
                {
                    CompSkyMind csm = victim.TryGetComp<CompSkyMind>();
                    //On previent le fait que les attaquant kidnappes leurs propres surrogates hackés temporairement
                    if(csm != null && csm.hacked == 3 && csm.hackOrigFaction == kidnapper.Faction)
                    {
                        if (!kidnapper.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || !kidnapper.Map.reachability.CanReachMapEdge(kidnapper.Position, TraverseParms.For(kidnapper, Danger.Some, TraverseMode.ByPawn, false)))
                        {
                            victim = null;
                            __result = false;
                        }
                        Predicate<Thing> validator = delegate (Thing t)
                        {
                            Pawn pawn = t as Pawn;
                            return pawn.RaceProps.Humanlike && pawn.Downed && pawn.Faction == Faction.OfPlayer && !(pawn.IsSurrogateAndroid() && pawn.TryGetComp<CompAndroidState>() != null && pawn.TryGetComp<CompSkyMind>().hacked == 3 && pawn.TryGetComp<CompSkyMind>().hackOrigFaction == kidnapper.Faction) && pawn.Faction.HostileTo(kidnapper.Faction) && kidnapper.CanReserve(pawn, 1, -1, null, false) && (disallowed == null || !disallowed.Contains(pawn));
                        };
                        victim = (Pawn)GenClosest.ClosestThingReachable(kidnapper.Position, kidnapper.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Some, false), maxDist, validator, null, 0, -1, false, RegionType.Set_Passable, false);
                        __result = victim != null;
                    }
                }
            }
        }
    }
}