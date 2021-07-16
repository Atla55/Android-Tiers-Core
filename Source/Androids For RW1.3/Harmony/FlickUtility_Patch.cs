using System.Collections.Generic;
using System.Linq;
using System;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using System.Reflection;
using UnityEngine;
using System.Text;
using Verse.Sound;

namespace MOARANDROIDS
{
    /// <summary>
    /// Allow colonists to talk to guests randomly
    /// </summary>
    internal static class FlickUtility_Patch
    {
        [HarmonyPatch(typeof(FlickUtility), "UpdateFlickDesignation")]
        public class UpdateFlickDesignation
        {
            [HarmonyPrefix]
            public static bool UpdateFlickDesignation_Prefix(Thing t)
            {
                try
                {
                    int CGT = Find.TickManager.TicksGame;
                    CompSkyMind csm = t.TryGetComp<CompSkyMind>();
                    if(csm == null)
                        return true;

                    //Eviter les mods qui ont des doublons sur leur Comp_PropertieFlickable (cf Vanilla truc muche)
                    if (csm.lastRemoteFlickGT == CGT)
                        return false;

                    String txt;
                    
                    //Si serveur principal installé sur la map alors automatisation du flick
                    if (Utils.GCATPP.isThereSkyCloudCore())
                    {
                        if (!csm.connected)
                            return true;

                        CompFlickable cf = t.TryGetComp<CompFlickable>();
                        if (cf != null)
                        {
                            
                            //Affichage texte
                            if (cf.SwitchIsOn)
                            {
                                txt = "ATPP_FlickDisable".Translate();
                                Utils.playVocal("soundDefSkyCloudDeviceDeactivated");
                            }
                            else
                            {
                                txt = "ATPP_FlickEnable".Translate();
                                Utils.playVocal("soundDefSkyCloudDeviceActivated");
                            }

                            MoteMaker.ThrowText(t.TrueCenter() + new Vector3(0.5f, 0f, 0.5f), t.Map, txt, Color.white, -1f);

                            cf.DoFlick();
                            csm.lastRemoteFlickGT = CGT;
                        }

                        return false;
                    }
                    return true;
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] FlickUtility.UpdateFlickDesignation "+e.Message+" "+e.StackTrace);
                    return true;
                }
            }
        }
    }
}