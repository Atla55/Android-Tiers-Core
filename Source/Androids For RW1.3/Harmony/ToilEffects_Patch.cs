﻿using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    internal class ToilEffects_Patch

    {
        /*
         * Patch used to replace the sound emitted for tending androids patients
         */
        [HarmonyPatch(new Type[] { typeof(Toil), typeof(SoundDef), typeof(float)})]
        [HarmonyPatch(typeof(ToilEffects), "PlaySustainerOrSound")]
        public class PlaySustainerOrSound_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Toil toil, SoundDef soundDef, float pitchFactor, ref Toil __result)
            {
                try
                {
                    
                    if (soundDef == SoundDefOf.Interact_Tend )
                    {
                        __result = toil.PlaySustainerOrSound(delegate() {
                            try
                            {
                                Pawn actor = toil.GetActor();
                                if (actor != null && actor.CurJob != null && actor.CurJob.targetA.Thing is Pawn)
                                {
                                    Pawn deliveree = (Pawn)actor.CurJob.targetA.Thing;
                                    if (deliveree.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier)
                                        return SoundDefOfAT.Recipe_ButcherCorpseMechanoid;
                                }
                                return soundDef;
                            }
                            catch (Exception)
                            {
                                return soundDef;
                            }
                         }, pitchFactor);

                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    return true;
                }
            }
        }
    }
}