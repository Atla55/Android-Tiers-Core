using System;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;

namespace MOARANDROIDS
{
    public class CompAndroidPod : ThingComp
    {
        public override void CompTick()
        {
            if (!this.parent.Spawned)
            {
                return;
            }

            int CGT = Find.TickManager.TicksGame;
            if (CGT % 60 == 0)
            {
                //Rafraichissement qt de courant consommé
                refreshPowerConsumed();
            }
        }

        public float getPowerConsumed()
        {
            CompPowerTrader cpt = Utils.getCachedCPT(this.parent);

            if (cpt == null)
                return 0;
            else
                return getCurrentAndroidPowerConsumed() + cpt.Props.basePowerConsumption;
        }


        public void refreshPowerConsumed()
        {
            CompPowerTrader cpt = Utils.getCachedCPT(this.parent);
            cpt.powerOutputInt = -(getPowerConsumed());
        }

        public int getCurrentAndroidPowerConsumed()
        {
            int ret = 0;

            Building_Bed bed = (Building_Bed)parent;

            foreach (var cp in bed.CurOccupants)
            {
                //Il sagit d'un android 
                if (cp != null && Utils.ExceptionAndroidList.Contains(cp.def.defName))
                {
                    ret += Utils.getConsumedPowerByAndroid(cp.def.defName);
                }
            }
            return ret;
        }
    }
}