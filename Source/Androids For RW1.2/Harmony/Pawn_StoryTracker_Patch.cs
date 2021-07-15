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
    internal class Pawn_StoryTracker_Patch
    {
        [HarmonyPatch(typeof(Pawn_StoryTracker), "get_SkinColor")]
        public class SkinColor_Patch
        {
            [HarmonyPostfix]
            public static void Listener( ref Color __result, Pawn ___pawn)
            {
                if ( Utils.ExceptionAndroidWithoutSkinList.Contains(___pawn.def.defName)) {
                    if ((___pawn.IsSurrogateAndroid()))
                    {
                        //Log.Message(___pawn.LabelCap);
                        __result = Utils.SXColor;
                    }

                    CompAndroidState cas = ___pawn.TryGetComp<CompAndroidState>();

                    if(cas != null )
                    {
                        AndroidPaintColor pc = (AndroidPaintColor)cas.customColor;
                        if (Settings.androidsCanRust && cas.paintingIsRusted)
                        {
                            __result = Utils.androidCustomColorRust;
                        }
                        else if(pc != AndroidPaintColor.None && pc != AndroidPaintColor.Default)
                        {
                            if (pc == AndroidPaintColor.Green)
                                __result = Utils.androidCustomColorGreen;
                            else if(pc == AndroidPaintColor.Black)
                                __result = Utils.androidCustomColorBlack;
                            else if (pc == AndroidPaintColor.Gray)
                                __result = Utils.androidCustomColorGray;
                            else if (pc == AndroidPaintColor.White)
                                __result = Utils.androidCustomColorWhite;
                            else if (pc == AndroidPaintColor.Blue)
                                __result = Utils.androidCustomColorBlue;
                            else if (pc == AndroidPaintColor.Cyan)
                                __result = Utils.androidCustomColorCyan;
                            else if (pc == AndroidPaintColor.Red)
                                __result = Utils.androidCustomColorRed;
                            else if (pc == AndroidPaintColor.Orange)
                                __result = Utils.androidCustomColorOrange;
                            else if (pc == AndroidPaintColor.Yellow)
                                __result = Utils.androidCustomColorYellow;
                            else if (pc == AndroidPaintColor.Purple)
                                __result = Utils.androidCustomColorPurple;
                            else if (pc == AndroidPaintColor.Pink)
                                __result = Utils.androidCustomColorPink;
                            else if (pc == AndroidPaintColor.Khaki)
                                __result = Utils.androidCustomColorKhaki;
                        }
                    }
                }
            }
        }
    }
}