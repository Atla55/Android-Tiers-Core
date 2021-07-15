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

namespace MOARANDROIDS
{
    public class CompBuildingSkyMindWAN : ThingComp
    {
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            Building build = (Building)this.parent;

            //if (this.parent.TryGetComp<CompPowerTrader>().PowerOn)
                Utils.GCATPP.popSkyMindWANServer(build, map);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            Utils.GCATPP.popSkyMindWANServer((Building)this.parent, previousMap);
        }


        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);

            Building build = (Building)this.parent;

            switch (signal)
            {
                case "PowerTurnedOn":
                    Utils.GCATPP.pushSkyMindWANServer(build);
                    break;
                case "PowerTurnedOff":
                    Utils.GCATPP.popSkyMindWANServer(build, build.Map);
                    break;
            }
        }

        public override string CompInspectStringExtra()
        {
            string ret = "";

            if (parent.Map == null)
                return base.CompInspectStringExtra();

            ret += "ATPP_SkyMindAntennaSynthesis".Translate(Utils.GCATPP.getNbThingsConnected(), Utils.GCATPP.getNbSlotAvailable()) + "\n";

            return ret.TrimEnd('\r', '\n') + base.CompInspectStringExtra();
        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (this.parent.TryGetComp<CompPowerTrader>().PowerOn)
                Utils.GCATPP.pushSkyMindWANServer((Building)this.parent);
        }
    }
}