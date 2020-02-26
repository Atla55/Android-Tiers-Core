using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;
 
namespace BlueLeakTest
{
    [HarmonyPatch(typeof(RimWorld.HealthCardUtility))]
    [HarmonyPatch("GetTooltip")]
    static public class HealthCardUtility_GetTooltip
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo tipStringExtraGetter = AccessTools.Property(typeof(Hediff), nameof(Hediff.TipStringExtra)).GetGetMethod();
            MethodInfo labelHelper = AccessTools.Method(typeof(HealthCardUtility_GetTooltip)
                                        , nameof(HealthCardUtility_GetTooltip.TransformBleedingToLeakingIfFemale));

            foreach(var code in instructions) {
                yield return code;
                if(code.opcode == OpCodes.Callvirt && code.operand == tipStringExtraGetter) {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);  //string, Pawn on stack
                    yield return new CodeInstruction(OpCodes.Call, labelHelper); //Consume 2, leave string
                }
            }   
        }

        static public string TransformBleedingToLeakingIfFemale(string original, Pawn pawn)
        {
            if(pawn.IsAndroid())
                return original.Replace("BleedingRate".Translate(), "AT_Leaking".Translate());
            return original;
        }
    }
}
