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
    internal class Pawn_DraftController_Patch

    {
        [HarmonyPatch(typeof(Pawn_DraftController), "set_Drafted")]
        public class Drafted_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, bool value)
            {
                CompAndroidState cas = null;
                if (___pawn != null)
                    cas = ___pawn.TryGetComp<CompAndroidState>();

                //If android with glowing eyes we refresh the graphics cache, because glowing eyes with different colors are stored in cache (drafted => red, not-drafted => normal color depending of the android serie)
                if (Utils.ExceptionAndroidWithGlowingEyes.Contains(___pawn.def.defName) 
                        || (___pawn.def.defName == Utils.TX2K && (cas.TXHurtedHeadSet || cas.TXHurtedHeadSet2))
                        || (___pawn.def.defName == Utils.TX3 && (cas.TXHurtedHeadSet || cas.TXHurtedHeadSet2))
                        || (___pawn.def.defName == Utils.TX4 && (cas.TXHurtedHeadSet || cas.TXHurtedHeadSet2)))
                {
                    ___pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                }
            }
        }
    }
}