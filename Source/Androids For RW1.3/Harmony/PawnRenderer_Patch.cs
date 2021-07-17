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
    internal class ATPP_PawnRenderer_Patch
    {
        [HarmonyPatch(typeof(PawnRenderer), "LayingFacing")]
        public class LayingFacing_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, ref Rot4 __result)
            {
                if (___pawn.def.defName == Utils.M7)
                    __result = Rot4.South;
            }
        }


        [HarmonyPatch(typeof(PawnRenderer), "DrawHeadHair")]
        [HarmonyPatch(new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(PawnRenderFlags) })]
        public class ATPP_RenderPawnInternal_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref PawnRenderer __instance, Vector3 rootLoc, Vector3 headOffset, float angle, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, PawnRenderFlags flags, Pawn ___pawn)
            {
                try
                {
                    bool flagPortrait = flags.FlagSet(PawnRenderFlags.Portrait);
                    bool state = false;
                    CompAndroidState cas = null;
                    if (___pawn != null)
                        cas = ___pawn.TryGetComp<CompAndroidState>();

                    if (___pawn != null
                        && (___pawn.def.defName == Utils.TX2
                        || ___pawn.def.defName == Utils.TX2I
                        || ___pawn.def.defName == Utils.TX2KI
                        || ___pawn.def.defName == Utils.TX3I
                        || ___pawn.def.defName == Utils.TX4I
                        || (___pawn.def.defName == Utils.TX2K && (cas.TXHurtedHeadSet || cas.TXHurtedHeadSet2))
                        || (___pawn.def.defName == Utils.TX3 && (cas.TXHurtedHeadSet || cas.TXHurtedHeadSet2)) 
                        || (___pawn.def.defName == Utils.TX4 && (cas.TXHurtedHeadSet || cas.TXHurtedHeadSet2)))
                        && !___pawn.Dead && !flags.FlagSet(PawnRenderFlags.HeadStump))
                    {
                        if (!flagPortrait)
                        {
                            if (___pawn != null)
                            {
                                Pawn_JobTracker jobs = ___pawn.jobs;
                                if ((((jobs != null) ? jobs.curDriver : null) != null))
                                {
                                    state = !___pawn.jobs.curDriver.asleep;
                                }
                            }
                        }
                        else
                            state = flagPortrait;

                        state = true;
                    }

                    if (state)
                    {
                        /*if (flagPortrait)
                            Log.Message("Refresh hair for " + ___pawn.LabelCap + " (PORTRAIT)");
                        else
                            Log.Message("Refresh hair for " + ___pawn.LabelCap + " (NO_PORTRAIT)");*/

                        Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
                        Vector3 a = rootLoc;
                        if (bodyFacing != Rot4.North)
                        {
                            a.y += 0.0281250011f;
                            rootLoc.y += 0.0234375f;
                        }
                        else
                        {
                            a.y += 0.0234375f;
                            rootLoc.y += 0.0281250011f;
                        }
                        Vector3 b = quaternion * __instance.BaseHeadOffsetAt(headFacing);
                        Vector3 loc = a + b + new Vector3(0f, 0.01f, 0f);
                        if (headFacing != Rot4.North)
                        {
                            Mesh mesh = MeshPool.humanlikeHeadSet.MeshAt(headFacing);
                            bool isHorizontal = headFacing.IsHorizontal;
                            int type = 1;
                            if (cas.TXHurtedHeadSet)
                                type = 2;
                            else if(cas.TXHurtedHeadSet2 || ___pawn.def.defName == Utils.TX2I || ___pawn.def.defName == Utils.TX2KI || ___pawn.def.defName == Utils.TX3I || ___pawn.def.defName == Utils.TX4I)
                                type = 3;

                            //Couleur yeux standard TX2
                            Color color = new Color(0.9450f, 0.76862f, 0.05882f);


                            //Couleur yeux TX3/TX4 standard (bleu cyan)
                            if(___pawn.def.defName == Utils.TX3 || ___pawn.def.defName == Utils.TX4 || ___pawn.def.defName == Utils.TX3I || ___pawn.def.defName == Utils.TX4I)
                            {
                                color = new Color(0f, 0.972549f, 0.972549f);
                            }

                            // yeux rouges si drafté OU ennemis OU TX2K
                            if (___pawn.Drafted || (___pawn.Faction != null && ___pawn.Faction.HostileTo(Faction.OfPlayer)) || ___pawn.def.defName == Utils.TX2K || ___pawn.def.defName == Utils.TX2KI)
                                color = new Color(0.75f, 0f, 0f, 1f);

                            string gender = "M";
                            if (___pawn.gender == Gender.Female)
                                gender = "F";

                            if (isHorizontal)
                            {
                                GenDraw.DrawMeshNowOrLater(mesh, loc, quaternion, Tex.getEyeGlowEffect(color, gender, type, 0).MatSingle, true);
                            }
                            else
                            {
                                GenDraw.DrawMeshNowOrLater(mesh, loc, quaternion, Tex.getEyeGlowEffect(color, gender, type, 1).MatSingle, true);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] PawnRenderer.RenderPawnInternal " + e.Message + " " + e.StackTrace);
                }
            }
        }
    }
}