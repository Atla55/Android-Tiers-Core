using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    [HarmonyPatch(typeof(Building_TurretGun), "get_CanSetForcedTarget")]
    public static class CanSetForcedTarget_Patch
    {
        private static readonly FieldInfo mannableComp = AccessTools.Field(typeof(Building_TurretGun), "mannableComp");
        //private static readonly FieldInfo PlayerControlled = AccessTools.Field(typeof(Building_TurretGun), "PlayerControlled");

        [HarmonyPostfix]
        public static void Listener(Building_TurretGun __instance, ref bool __result)
        {
            CompRemotelyControlledTurret crt = __instance.TryGetComp<CompRemotelyControlledTurret>();
            //Si pas de controlleur alors on ne peut pas controller la tourelle
            if (crt == null || crt.controller == null)
                return;

            CompMannable mannable = (CompMannable)mannableComp.GetValue(__instance);
            // bool controlled = (bool)PlayerControlled.GetValue(__instance);
            __result = __result || mannable == null;
        }
    }

    [HarmonyPatch(typeof(Building_TurretGun), "DrawExtraSelectionOverlays")]
    public static class DrawExtraSelectionOverlays_Patch
    {
        [HarmonyPostfix]
        public static void Listener(Building_TurretGun __instance)
        {
            CompRemotelyControlledTurret crt = __instance.TryGetComp<CompRemotelyControlledTurret>();
            //Si pas de controlleur alors on ne peut pas controller la tourelle
            if (crt == null || crt.controller == null)
                return;

            CompSurrogateOwner csc = null;
            CompSkyMind csm = __instance.TryGetComp<CompSkyMind>();

            csc = crt.controller.TryGetComp<CompSurrogateOwner>();

            if (csm != null && csm.connected && crt.controller != null && csc != null && csc.skyCloudHost != null && csc.skyCloudHost.Map == __instance.Map)
            {
                GenDraw.DrawLineBetween(__instance.TrueCenter(), csc.skyCloudHost.TrueCenter(), SimpleColor.Red);
            }
        }
    }
}
