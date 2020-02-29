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
    internal class StartingPawnUtility_Patch

    {
        [HarmonyPatch(typeof(StartingPawnUtility), "NewGeneratedStartingPawn")]
        public class NewGeneratedStartingPawn_Patch
        {
            [HarmonyAfter(new string[] { "rimworld.rwmods.androidtiers" })]
            [HarmonyPostfix]
            public static void Listener(ref Pawn __result)
            {
                if (__result == null)
                {
                    return;
                }
                if (Faction.OfPlayer.def.basicMemberKind.defName != "AndroidT2ColonistGeneral")
                {
                    return;
                }
                else
                {
                    Random rnd = new Random();
                    PawnGenerationRequest request;
                    string pkd = "";
                    if (!Utils.TXSERIE_LOADED)
                    {
                        switch (rnd.Next(1, 3))
                        {
                            case 1:
                                pkd = "AndroidT2ColonistGeneral";
                                break;
                            case 2:
                                pkd = "AndroidT1ColonistGeneral";
                                break;
                            default:
                                pkd = Faction.OfPlayer.def.basicMemberKind.defName;
                                break;
                        }
                    }
                    else
                    {
                        switch (rnd.Next(1, 5))
                        {
                            case 1:
                                pkd = "AndroidT2ColonistGeneral";
                                break;
                            case 2:
                                pkd = "AndroidT1ColonistGeneral";
                                break;
                            case 3:
                                pkd = "ATPP_Android2ITXKind";
                                break;
                            case 4:
                                pkd = "ATPP_Android2TXKind";
                                break;
                            default:
                                pkd = Faction.OfPlayer.def.basicMemberKind.defName;
                                break;
                        }
                    }
                    request = new PawnGenerationRequest(DefDatabase<PawnKindDef>.GetNamed(pkd, false), Faction.OfPlayer, PawnGenerationContext.PlayerStarter, -1, true, false, false, false, true, TutorSystem.TutorialMode, 20f, false, true, true, false, false, false, false);
                    __result = null;
                    try
                    {
                        __result = PawnGenerator.GeneratePawn(request);
                    }
                    catch (Exception e)
                    {
                        Log.Error("[ATPP] StartingPawnUtility.NewGeneratedStartingPawn " + e.Message+" "+e.StackTrace, false);
                        __result = PawnGenerator.GeneratePawn(request);
                    }
                    __result.relations.everSeenByPlayer = true;
                    PawnComponentsUtility.AddComponentsForSpawn(__result);
                }
            }
        }
    }
}