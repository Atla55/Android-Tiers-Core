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
    public class CompReloadStation : ThingComp
    {
        protected CompProperties_ReloadStation PropsPowerCollector
        {
            get
            {
                return (CompProperties_ReloadStation)this.props;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            Utils.GCATPP.pushReloadStation((Building)parent);

            //Au demarrage si l'emetteur on réapplique le retrait de ce courant convertis en sans fil
            //substractPowerTransmitted();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            //Retire de la liste des emetteurs de la map
            Utils.GCATPP.popReloadStation((Building)this.parent, map);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            
        }

        public override void ReceiveCompSignal(string signal)
        {
            //Quand un WPN sans va ou revient on raffraichis les offres des factions uniquement
            if (signal == "PowerTurnedOff" || signal == "PowerTurnedOn")
            {

            }
        }

        public override string CompInspectStringExtra()
        {
            string ret = base.CompInspectStringExtra();

            if (parent == null)
                return ret;
            if (ret == null)
                ret = "";

            if (ret != "")
                ret += "\n";

            //ret += "\n" + "ARKPPP_StormPerturbationInfo".Translate((int)(perturbation * 100));

            return ret;
        }

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

            if (CGT % 360 == 0)
            {
                //Augmentation énergie (barre de faim) des androids présents
                incAndroidPower();
            }
        }

        public float getPowerConsumed()
        {
            return getNbAndroidReloading() + this.parent.TryGetComp<CompPowerTrader>().Props.basePowerConsumption;
        }


        public void refreshPowerConsumed()
        {
            this.parent.TryGetComp<CompPowerTrader>().powerOutputInt = -(getPowerConsumed());
        }

        public void incAndroidPower()
        {
            IntVec3 parentPos = parent.Position;
            foreach (IntVec3 adjPos in ((Building)parent).CellsAdjacent8WayAndInside())
            {
                List<Thing> thingList = adjPos.GetThingList(parent.Map);
                if (thingList != null)
                {
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        Pawn cp = thingList[i] as Pawn;

                        /*if (cp != null && cp.IsColonist)
                            Log.Message("=>" + cp.LabelShortCap + " "+ cp.CurJobDef.defName);*/

                        //Il sagit d'un android  et il execute le jobDriver "ATPP_GoReloadBattery"
                        if (cp != null && Utils.ExceptionAndroidCanReloadWithPowerList.Contains(cp.def.defName) && cp.CurJobDef.defName == "ATPP_GoReloadBattery")
                        {
                            if (cp.needs.food.CurLevelPercentage < 1.0)
                            {
                                cp.needs.food.CurLevelPercentage += Settings.percentageOfBatteryChargedEach6Sec;
                                Utils.throwChargingMote(cp);
                            }
                        }
                    }
                }
            }
        }

        public IntVec3 getFreeReloadPlacePos(Pawn android)
        {
            bool ok = true;
            IntVec3 parentPos = parent.Position;
            foreach (IntVec3 adjPos in ((Building)parent).CellsAdjacent8WayAndInside())
            {
                ok = true;
                List<Thing> thingList = adjPos.GetThingList(parent.Map);
                if (thingList != null)
                {
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        Pawn cp = thingList[i] as Pawn;

                        //Il sagit d'un android 
                        if (cp != null && cp.IsColonist && Utils.ExceptionAndroidList.Contains(cp.def.defName))
                        {
                            ok = false;
                            break;
                        }
                    }

                    //Si pas déjà d'android dessus
                    if (ok)
                    {
                        //Check si atteignable par l'android
                        if (android.CanReach(adjPos, Verse.AI.PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
                        {
                            //Check si emplacement pas deja réservé par un android 
                            if( !android.Map.pawnDestinationReservationManager.IsReserved(adjPos))
                                return adjPos;
                        }
                    }
                }
            }

            return IntVec3.Invalid;
        }

        //Comptabilisation du nombre d'androids sur les bords directes du batiments en train de recharger
        public int getNbAndroidReloading(bool countOnly = false)
        {
            int ret = 0;
            IntVec3 parentPos = parent.Position;
            foreach(IntVec3 adjPos in ((Building)parent).CellsAdjacent8WayAndInside())
            {
                List<Thing> thingList = adjPos.GetThingList(parent.Map);
                if (thingList != null)
                {
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        Pawn cp = thingList[i] as Pawn;

                        //Il sagit d'un android 
                        if (cp != null && cp.IsColonist && Utils.ExceptionAndroidList.Contains(cp.def.defName) && cp.CurJobDef.defName == "ATPP_GoReloadBattery")
                        {
                            if (countOnly)
                            {
                                ret++;
                            }
                            else
                            {
                                //Si son job est "reloading" alors incrémentation (il ne sagit pas d'un android ne faisant que passer)
                                ret += Utils.getConsumedPowerByAndroid(cp.def.defName);
                            }
                        }
                    }
                }
            }

            return ret;
        }
    }
}