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
    public class CompBuildingSkyMindLAN : ThingComp
    {
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            Building build = (Building)this.parent;
            Utils.GCATPP.popSkyMindServer(build, map);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            Utils.GCATPP.popSkyMindServer((Building)this.parent, previousMap);
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);

            Building build = (Building)this.parent;

            switch (signal)
            {
                case "PowerTurnedOn":
                    Utils.GCATPP.pushSkyMindServer(build);
                    break;
                case "PowerTurnedOff":
                    Utils.GCATPP.popSkyMindServer(build,build.Map);
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

            if (this.parent.TryGetComp<CompPowerTrader>().PowerOn)
                Utils.GCATPP.pushSkyMindServer((Building)this.parent);
        }
    }
}