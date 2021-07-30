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
    public class CompBuildingSkyMindRelay : ThingComp
    {
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            
                Utils.GCATPP.popRelayTower(this.parent, map.GetUniqueLoadID());
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            Utils.GCATPP.popRelayTower((Building)this.parent, previousMap.GetUniqueLoadID());
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);

            Building build = (Building)this.parent;

            switch (signal)
            {
                case "PowerTurnedOn":
                    Utils.GCATPP.pushRelayTower(build, build.Map.GetUniqueLoadID());
                    break;
                case "PowerTurnedOff":
                    Utils.GCATPP.popRelayTower(build, build.Map.GetUniqueLoadID());
                    break;
            }
        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            CompPowerTrader cpt = Utils.getCachedCPT(this.parent);
            if (cpt.PowerOn)
                Utils.GCATPP.pushRelayTower(this.parent, this.parent.Map.GetUniqueLoadID());
        }
    }
}