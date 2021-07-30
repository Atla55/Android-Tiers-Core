using System;
using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Verse.AI.Group;
using System.Linq;
using HarmonyLib;
using System.Reflection;
using RimWorld.Planet;

namespace MOARANDROIDS
{
    public class CompBuildingSkyMindLAN : ThingComp
    {
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            Utils.GCATPP.popSkyMindServer(this.parent);

            if(parentPawn != null && parentPawn.IsCaravanMember())
            {
                Utils.GCATPP.pushSkyMindServer(this.parent, "caravan");
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (Utils.GCATPP == null)
                return;

            Utils.GCATPP.popSkyMindServer(this.parent);
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);

            switch (signal)
            {
                case "PowerTurnedOn":
                    Utils.GCATPP.pushSkyMindServer(this.parent, this.parent.Map.GetUniqueLoadID());
                    break;
                case "PowerTurnedOff":
                    Utils.GCATPP.popSkyMindServer(this.parent);
                    break;
                case "AndroidTiers_CaravanInit":
                    if(parent is Pawn)
                    {
                        Pawn cp = (Pawn)parent;
                        if (!init)
                            initComp();
                    }
                    break;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder ret = new StringBuilder();

            if (parent.Map == null)
                return base.CompInspectStringExtra();

            ret.AppendLine("ATPP_SkyMindAntennaSynthesis".Translate(Utils.GCATPP.getNbThingsConnected(), Utils.GCATPP.getNbSlotAvailable()));

            return ret.TrimEnd().Append(base.CompInspectStringExtra()).ToString();
        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            init = false;
            initComp();
        }

        public bool isPowerOn()
        {
            if (parentPawn == null)
            {
                return parentCPT != null && parentCPT.PowerOn && !parent.IsBrokenDown();
            }
            else
                return !parentPawn.Dead;
        }

        public void initComp()
        {
            if (init)
                return;
            init = true;

            if (parent is Pawn)
            {
                parentPawn = (Pawn)parent;
                parentBuilding = null;

                parentCSC = Utils.getCachedCSC(parent);

                if (parentCSC != null && !parentCSC.isOnline())
                {
                    return;
                }

                if (this.parent.Map == null)
                    Utils.GCATPP.pushSkyMindServer(this.parent, "caravan");
                else
                    Utils.GCATPP.pushSkyMindServer(this.parent, this.parent.Map.GetUniqueLoadID());
            }
            else
            {
                parentPawn = null;
                parentBuilding = (Building)parent;
                parentCPT = Utils.getCachedCPT(this.parent);
                if (parentCPT.PowerOn)
                    Utils.GCATPP.pushSkyMindServer(this.parent, this.parent.Map.GetUniqueLoadID());
            }
        }

        private CompSkyCloudCore parentCSC;
        private CompPowerTrader parentCPT;
        private Pawn parentPawn;
        private Building parentBuilding;
        private bool init = false;
    }
}