using System;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;
using System.Collections.Generic;

namespace MOARANDROIDS
{
    public class CompRemotelyControlledTurret : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_References.Look<Pawn>(ref controller, "ATPP_RemoteTurretController");
        }

        public override void PostDraw()
        {
            Material avatar = null;
            Vector3 vector;

            Designator_AndroidToControl desi = null;
            bool isConnected = (csm != null && csm.connected);

            if (Find.DesignatorManager.SelectedDesignator is Designator_AndroidToControl)
                desi = (Designator_AndroidToControl)Find.DesignatorManager.SelectedDesignator;

            if (desi != null && desi.fromSkyCloud)
            {
                avatar = Tex.SelectableSX;
            }

            if (isConnected)
            {
                if (controller != null)
                    avatar = Tex.RemotelyControlledNode;
            }

            if (avatar != null)
            {
                vector = this.parent.TrueCenter();
                vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28125f;
                vector.z += 1.4f;
                vector.x += this.parent.def.size.x / 2;

                Graphics.DrawMesh(MeshPool.plane08, vector, Quaternion.identity, avatar, 0);
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            disconnectConnectedMind();
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            CompSurrogateOwner csc = null;

            if(controller != null)
            {
                csc = controller.TryGetComp<CompSurrogateOwner>();
            }

            if (csm != null && csm.connected && controller != null && csc != null && csc.skyCloudHost != null && csc.skyCloudHost.Map == parent.Map)
            {
                GenDraw.DrawLineBetween(parent.TrueCenter(), csc.skyCloudHost.TrueCenter(), SimpleColor.Red);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            csm = parent.TryGetComp<CompSkyMind>();
        }

        public override void CompTick()
        {
            base.CompTick();
            int GT = Find.TickManager.TicksGame;

        }

        public override void ReceiveCompSignal(string signal)
        {
            Building host = (Building)parent;
            if (signal == "FlickedOff" || signal == "ScheduledOff" || signal == "Breakdown" || signal == "PowerTurnedOff" || signal == "SkyMindNetworkUserDisconnected")
            {
                //Deconnection le cas echeant du controller
                if(controller != null)
                {

                }
            }
        }

        private void disconnectConnectedMind()
        {
            if(controller != null)
            {
                CompSurrogateOwner cso = controller.TryGetComp<CompSurrogateOwner>();
                if (cso != null)
                {
                    if (cso.skyCloudHost != null)
                    {
                        CompSkyCloudCore csc = cso.skyCloudHost.TryGetComp<CompSkyCloudCore>();
                        if (csc != null)
                        {
                            csc.stopRemotelyControlledTurret(controller);
                        }
                    }
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Building build = (Building)parent;
            Texture2D tex;

            if (controller != null)
            {
                //Boutton permettant deconnection de la tourelle du pawn controller
                yield return new Command_Action
                {
                    icon = Tex.AndroidToControlTargetDisconnect,
                    defaultLabel = "ATPP_AndroidToControlTargetDisconnect".Translate(),
                    defaultDesc = "ATPP_AndroidToControlTargetDisconnectDesc".Translate(),
                    action = delegate ()
                    {
                        disconnectConnectedMind();
                    }
                };
            }

            yield break;
        }

        public override string CompInspectStringExtra()
        {
            string ret = "";

            if(controller != null)
                ret += "ATPP_RemotelyControlledBy".Translate(controller.LabelShortCap) + "\n";

            return ret.TrimEnd('\r', '\n') + base.CompInspectStringExtra();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            

        }

        CompSkyMind csm;
        public Pawn controller;
    }
}
