using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MOARANDROIDS;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace BlueLeakTest
{
    [StaticConstructorOnStartup]
    static public class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Utils.harmonyInstance = new Harmony("rimworld.rwmods.androidtiers");
            Utils.harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            /*DefDatabase<ThingDef_AlienRace>.AllDefsListForReading.ForEach(delegate (ThingDef_AlienRace ar)
            {
                if (ar.race.FleshType == DefDatabase<FleshTypeDef>.GetNamed("Android") || ar.race.FleshType == DefDatabase<FleshTypeDef>.GetNamed("MechanisedInfantry"))
                {
                    AlienDefOf.alienCorpseCategory.childThingDefs.Remove(item: ar.race.corpseDef);
                    ar.race.corpseDef.thingCategories = new List<ThingCategoryDef> { MOARANDROIDS.ThingCategoryDefOf.androidCorpseCategory };
                    MOARANDROIDS.ThingCategoryDefOf.androidCorpseCategory.childThingDefs.Add(item: ar.race.corpseDef);
                    DefDatabase<RecipeDef>.GetNamed("ButcherCorpseAndroidAT").fixedIngredientFilter.SetAllow(ar.race.corpseDef, true);
                }
            });

            DefDatabase<RecipeDef>.GetNamed("ButcherCorpseAndroidAT").fixedIngredientFilter.SetAllow(MOARANDROIDS.ThingCategoryDefOf.androidCorpseCategory, true, null, null);
            DefDatabase<RecipeDef>.GetNamed("ButcherCorpseFlesh").fixedIngredientFilter.SetAllow(MOARANDROIDS.ThingCategoryDefOf.androidCorpseCategory, false);*/

        }

    }
    [HarmonyPatch(typeof(SickPawnVisitUtility), "CanVisit")]
    static class Harmony_FindRandomSickPawn
    {
        static bool Prefix(Pawn pawn, Pawn sick, JoyCategory maxPatientJoy, ref bool __result)
        {
            if (sick.needs.rest != null)
            {
                return true;
            }

            __result = sick.IsColonist && !sick.Dead && pawn != sick && sick.InBed()
            && sick.Awake() && !sick.IsForbidden(pawn) && sick.needs.joy != null
            && sick.needs.joy.CurCategory <= maxPatientJoy
            && InteractionUtility.CanReceiveInteraction(sick) && !sick.needs.food.Starving
            && pawn.CanReserveAndReach(sick, PathEndMode.InteractionCell, Danger.None, 1, -1, null, false);
            return false;
        }
    }

    /*[HarmonyPatch(typeof(RimWorld.HealthCardUtility))]
    [HarmonyPatch("DrawOverviewTab")]
    static public class AndroidLabelOverwrite
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = instructions.ToList();

            MethodInfo labelFor = AccessTools.Method(typeof(PawnCapacityDef), nameof(PawnCapacityDef.GetLabelFor), new[] { typeof(bool), typeof(bool) });

            int index = codeInstructions.FindIndex(ci => ci.operand == labelFor);

            codeInstructions[index].operand = AccessTools.Method(typeof(PawnCapacityDef), nameof(PawnCapacityDef.GetLabelFor), new[] { typeof(Pawn) });
            codeInstructions.RemoveRange(index - 6, 6);

            return codeInstructions;
        }
    }*/
        [HarmonyPatch(typeof(JobDriver_Vomit))]
    [HarmonyPatch("MakeNewToils")]
    internal static class DeclineVomitJob
    {
        public static bool Prefix(ref JobDriver_Vomit __instance, ref IEnumerable<Toil> __result)
        {
            Pawn pawn = __instance.pawn;
            bool result;
            if (pawn.IsAndroidGen())
            {
                JobDriver_Vomit instance = __instance;
                __result = new List<Toil>
                {
                    new Toil
                    {
                        initAction = delegate()
                        {
                            instance.pawn.jobs.StopAll(false);
                        }
                    }
                };
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
    }

    [HarmonyPatch(typeof(HediffSet))]
    [HarmonyPatch("CalculatePain")]
    internal static class EstimatePainGivenOverride
    {
        public static bool Prefix(ref HediffSet __instance, ref float __result)
        {
            bool result;
            if (__instance.pawn.IsAndroid())
            {
                __result = 0f;
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
    }

    [HarmonyPatch(new Type[] { typeof(Pawn) })]
    [HarmonyPatch(typeof(PawnCapacityDef))]
    [HarmonyPatch("GetLabelFor")]
    internal static class GetLabelForAndroidCapacity
    {
        public static bool Prefix(ref string __result, PawnCapacityDef __instance, Pawn pawn)
        {
            if (pawn != null && pawn.RaceProps != null && pawn.RaceProps.FleshType == DefDatabase<FleshTypeDef>.GetNamed("Android"))
            {
                if (__instance.GetModExtension<AndroidCapacityLabel>() != null)
                {
                    __result = __instance.GetModExtension<AndroidCapacityLabel>().androidNewLabel;
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("NotifyPlayerOfKilled")]
    internal static class DeadPawnMessageRemoval
    {
        private static bool Prefix(Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff, Caravan caravan)
        {
            Pawn value = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool flag = value.kindDef == MOARANDROIDS.PawnKindDefOf.MicroScyther;
            return !flag;
        }
    }



    [HarmonyPatch(typeof(FactionDialogMaker))]
    [HarmonyPatch("FactionDialogFor")]
    internal static class AndroidFactionDialogOverride
    {
        private static void Postfix(ref DiaNode __result, Pawn negotiator, Faction faction)
        {
            if (__result == null)
            {
                return;
            }
            if (faction.def.leaderTitle != "grand leader")
            {
                return;
            }

            Pawn pawn = faction.leader;
            string value = faction.leader.Name.ToStringFull;

            Random rnd = new Random();
            string key;
            if (faction.PlayerRelationKind == FactionRelationKind.Hostile)
            {
                switch (rnd.Next(1, 4))
                {
                    case 1:
                        key = "AndroidFactionGreetingHostileI";
                        break;
                    case 2:
                        key = "AndroidFactionGreetingHostileII";
                        break;
                    case 3:
                        key = "AndroidFactionGreetingHostileIII";
                        break;

                    default:
                        key = "AndroidFactionGreetingHostileI";
                        break;
                }
                
                __result.text = (key.Translate(value).AdjustedFor(pawn, "PAWN"));
            }
            else if (faction.PlayerRelationKind == FactionRelationKind.Neutral)
            {
                switch (rnd.Next(1, 4))
                {
                    case 1:
                        key = "AndroidFactionGreetingWaryI";
                        break;
                    case 2:
                        key = "AndroidFactionGreetingWaryII";
                        break;
                    case 3:
                        key = "AndroidFactionGreetingWaryIII";
                        break;

                    default:
                        key = "AndroidFactionGreetingWaryI";
                        break;
                }
                __result.text = (key.Translate(value, negotiator.LabelShort, negotiator.Named("NEGOTIATOR"), pawn.Named("LEADER")).AdjustedFor(pawn, "PAWN"));
            }
            else
            {
                switch (rnd.Next(1, 4))
                {
                    case 1:
                        key = "AndroidFactionGreetingWarmI";
                        break;
                    case 2:
                        key = "AndroidFactionGreetingWarmII";
                        break;
                    case 3:
                        key = "AndroidFactionGreetingWarmIII";
                        break;

                    default:
                        key = "AndroidFactionGreetingWarmI";
                        break;
                }
                __result.text = (key.Translate(value, negotiator.LabelShort, negotiator.Named("NEGOTIATOR"), pawn.Named("LEADER")).AdjustedFor(pawn, "PAWN"));
            }
    
        }
    }

    [HarmonyPatch(typeof(StartingPawnUtility))]
    [HarmonyPatch("NewGeneratedStartingPawn")]
    internal static class MultiplePawnRacesAtStart
    {
        private static void Postfix(ref Pawn __result)
        {
            if (__result == null)
            {
                return;
            }
            if (Faction.OfPlayer.def.basicMemberKind != MOARANDROIDS.PawnKindDefOf.AndroidT2ColonistGeneral)
            {
                return;
            }
            else
            {
                Random rnd = new Random();
                PawnGenerationRequest request;
                switch (rnd.Next(1, 3))
                {
                    case 1:
                        request = new PawnGenerationRequest(MOARANDROIDS.PawnKindDefOf.AndroidT2ColonistGeneral, Faction.OfPlayer, PawnGenerationContext.PlayerStarter, -1, true, false, false, false, true, TutorSystem.TutorialMode, 20f, false, true, true, false, false, false, false);
                        break;
                    case 2:
                        request = new PawnGenerationRequest(MOARANDROIDS.PawnKindDefOf.AndroidT1ColonistGeneral, Faction.OfPlayer, PawnGenerationContext.PlayerStarter, -1, true, false, false, false, true, TutorSystem.TutorialMode, 20f, false, true, true, false, false, false, false);
                        break;
                    default:
                        request = new PawnGenerationRequest(Faction.OfPlayer.def.basicMemberKind, Faction.OfPlayer, PawnGenerationContext.PlayerStarter, -1, true, false, false, false, true, TutorSystem.TutorialMode, 20f, false, true, true, false, false, false, false);
                        break;
                }
                __result = null;
                try
                {
                    __result = PawnGenerator.GeneratePawn(request);
                }
                catch (Exception arg)
                {
                    Log.Error("There was an exception thrown by the PawnGenerator during generating a starting pawn. Trying one more time...\nException: " + arg, false);
                    __result = PawnGenerator.GeneratePawn(request);
                }
                __result.relations.everSeenByPlayer = true;
                PawnComponentsUtility.AddComponentsForSpawn(__result);
            }

        }
    }

        [HarmonyPatch(typeof(CompFoodPoisonable))]
    [HarmonyPatch("PostIngested")]
    internal static class AndroidsFoodPoisonOverride
    {
        private static bool Prefix(CompFoodPoisonable __instance, Pawn ingester)
        {
            Pawn value = Traverse.Create(__instance).Field("ingester").GetValue<Pawn>();
            if (ingester.IsAndroid() == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

        [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("MakeDowned")]
    internal static class DiesUponDowned
    {
        private static bool Prefix(Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff, Pawn ___pawn)
        {
            Pawn value = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if(value.kindDef == MOARANDROIDS.PawnKindDefOf.MicroScyther || value.kindDef == MOARANDROIDS.PawnKindDefOf.AbominationAtlas || (value.kindDef == MOARANDROIDS.PawnKindDefOf.M7MechPawn && ___pawn.TryGetComp<CompAndroidState>() != null && !___pawn.TryGetComp<CompAndroidState>().isSurrogate))
            {
                value.Kill(null);
                return false;
            } else
            {
                return true;
            }
        }
    }
    
}
