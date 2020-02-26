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
    [HarmonyPatch("DrawHediffRow")]

    [StaticConstructorOnStartup]
    static public class HealthCardUtility_DrawHediffRow
    {
        static Texture2D leakingIcon;
        

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo bleedingIconField = AccessTools.Field(typeof(HealthCardUtility), "BleedingIcon");
            MethodInfo labelColorGetter = AccessTools.Property(typeof(Hediff), nameof(Hediff.LabelColor)).GetGetMethod();
            MethodInfo iconHelper = AccessTools.Method(typeof(HealthCardUtility_DrawHediffRow)
                                        , nameof(HealthCardUtility_DrawHediffRow.TransformIconColorBlueIfFemale));
            MethodInfo labelHelper = AccessTools.Method(typeof(HealthCardUtility_DrawHediffRow)
                                        , nameof(HealthCardUtility_DrawHediffRow.TransformLabelColorRedToBlueIfFemale));
                                        
            leakingIcon = ContentFinder<Texture2D>.Get("UI/Icons/Medical/Leaking", true);                                        

            foreach(var code in instructions) {
                yield return code;
                if(code.opcode == OpCodes.Ldsfld && code.operand == bleedingIconField) {
                    Log.Message("Patching");
                    yield return new CodeInstruction(OpCodes.Ldarg_1);  //TextureAndColor, Pawn on stack
                    yield return new CodeInstruction(OpCodes.Call, iconHelper); //Consume 2, leave TextureAndColor
                }   
                //Removed at Atlas' request
            /*	if(code.opcode == OpCodes.Callvirt && code.operand == labelColorGetter) {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);  //Color, Pawn on stack
                    yield return new CodeInstruction(OpCodes.Call, labelHelper); //Consume 2, leave Color
                }   */
            }   
        }

        static public Texture2D TransformIconColorBlueIfFemale(Texture2D original, Pawn pawn)
        {
            if(pawn.IsAndroid())
                return leakingIcon; 
            return original;
        }

        static public Color TransformLabelColorRedToBlueIfFemale(Color original, Pawn pawn)
        {
            if(pawn.IsAndroid())
                return Color.cyan;
            return original;
        }
    }
}
