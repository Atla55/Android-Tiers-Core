using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace MOARANDROIDS
{
    public class CompAutoDoor : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            doorRef = this.parent as Building_Door;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            //Si pas de serveur principal alimenté on retourne rien
            if (!Utils.GCATPP.isThereSkyCloudCore() || !Utils.GCATPP.isConnectedToSkyMind(parent))
            {
                yield break;
            }

            //Si hors-ligne on retourne rien
            if ( parent.TryGetComp<CompPowerTrader>() == null || !parent.TryGetComp<CompPowerTrader>().PowerOn
                || parent.IsBrokenDown())
            {
                yield break;
            }

            if (doorRef.Open)
            {
                yield return new Command_Action
                {
                    icon = Tex.texAutoDoorClose,
                    defaultLabel = "ATPP_AutoDoorClose".Translate(),
                    defaultDesc = "ATPP_AutoDoorCloseDescription".Translate(),
                    action = delegate ()
                    {
                        bool holdOpenInt = Traverse.Create(doorRef).Field("holdOpenInt").GetValue<bool>();
                        if (Traverse.Create(doorRef).Field("holdOpenInt").GetValue<bool>())
                            Traverse.Create(doorRef).Field("holdOpenInt").SetValue(false);

                        Traverse.Create(doorRef).Method("DoorTryClose", new object[0]).GetValue();
                        MoteMaker.ThrowText(doorRef.TrueCenter() + new Vector3(0.5f, 0f, 0.5f), doorRef.Map, "ATPP_AutoDoorCloseMoteText".Translate(), Color.white, -1f);
                        Utils.playVocal("soundDefSkyCloudDoorClosed");
                        
                    }
                };
            }
            else
            {
                yield return new Command_Action
                {
                    icon = Tex.texAutoDoorOpen,
                    defaultLabel = "ATPP_AutoDoorOpen".Translate(),
                    defaultDesc = "ATPP_AutoDoorOpenDescription".Translate(),
                    action = delegate ()
                    {
                        if (!Traverse.Create(doorRef).Field("holdOpenInt").GetValue<bool>())
                            Traverse.Create(doorRef).Field("holdOpenInt").SetValue(true);

                        doorRef.StartManualOpenBy(null);
                        MoteMaker.ThrowText(doorRef.TrueCenter() + new Vector3(0.5f, 0f, 0.5f), doorRef.Map, "ATPP_AutoDoorOpenMoteText".Translate(), Color.white, -1f);

                        Utils.playVocal("soundDefSkyCloudDoorOpened");
                    }
                };
            }
            yield break;
        }

        private Building_Door doorRef;
    }
}