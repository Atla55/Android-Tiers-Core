using System;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace MOARANDROIDS
{
    public class CompHeatSensitive : ThingComp
    {
        public CompProperties_HeatSensitive Props
        {
            get
            {
                return (CompProperties_HeatSensitive)this.props;
            }
        }

        public int hotLevel
        {
            get
            {
                return this.hotLevelInt;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.hotLevelInt, "hotLevelInt", 0, false);
            Scribe_Values.Look<int>(ref this.ticksBeforeMelt, "ticksBeforeMelt", 0, false);
            Scribe_Values.Look<int>(ref this.nbTicksSinceHot3, "nbTicksSinceHot3", 0, false);
        }

        public override void PostDraw()
        {
            Material iconMat = null;

            //Si temperature du device chaude
            if (this.powerComp.PowerOn && !this.parent.IsBrokenDown() && this.hotLevelInt != 0)
            {
                if (this.hotLevelInt == 1)
                    iconMat = Tex.matHotLevel1;
                else if (this.hotLevelInt == 2)
                    iconMat = Tex.matHotLevel2;
                else if (this.hotLevelInt == 3)
                    iconMat = Tex.matHotLevel3;

                Vector3 vector = this.parent.TrueCenter();
                vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28125f;
                vector.x += this.parent.def.size.x / 4;

                vector.z -= 1;

                var num = (Time.realtimeSinceStartup + 397f * (float)(this.parent.thingIDNumber % 571)) * 4f;
                var num2 = ((float)Math.Sin((double)num) + 1f) * 0.5f;
                num2 = 0.3f + num2 * 0.7f;
                var material = FadedMaterialPool.FadedVersionOf(iconMat, num2);
                Graphics.DrawMesh(MeshPool.plane05, vector, Quaternion.identity, material, 0);

            }
        }

        // Token: 0x06002851 RID: 10321 RVA: 0x00133CC9 File Offset: 0x001320C9
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            System.Random rnd = new System.Random();

            base.PostSpawnSetup(respawningAfterLoad);
            if (this.parent != null)
                this.powerComp = this.parent.GetComp<CompPowerTrader>();

            if (Utils.ExceptionSkyCloudCores.Contains(parent.def.defName))
                isSkyCloudCore = true;

            //this.nbTicksSinceHot3 = 0;
            //this.hotLevelInt = 0;
            //Génération durée aléatoire 
            if (ticksBeforeMelt == 0)
                setNewExplosionThreshold();
            //this.soundDefHot = SoundDef.Named(this.Props.hotSoundDef);

            //Enregistrement dans la liste  dispositifs senseible a la temperature
            if (this.parent != null)
                Utils.GCATPP.pushHeatSensitiveDevices((Building)this.parent);
        }

        public void setNewExplosionThreshold()
        {
            if(isSkyCloudCore)
                this.ticksBeforeMelt = Rand.Range(Settings.nbHoursMinSkyCloudServerRunningHotBeforeExplode * 2500, Settings.nbHoursMaxSkyCloudServerRunningHotBeforeExplode * 2500);
            else
                this.ticksBeforeMelt = Rand.Range(Settings.nbHoursMinServerRunningHotBeforeExplode * 2500, Settings.nbHoursMaxServerRunningHotBeforeExplode * 2500);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (Find.TickManager.TicksGame % 250 == 0)
            {
                CheckTemperature();
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            //Arret manuel ou  composant endommagé => arret ambiance
            if (signal == "FlickedOff" || signal == "Breakdown" || signal == "PowerTurnedOff")
            {
                if (this.hotLevelInt == 3)
                    this.StopSustainerHot();

                this.hotLevelInt = 0;
                this.nbTicksSinceHot3 = 0;
            }
        }


        private void CheckTemperature()
        {
            int currLevel = this.hotLevelInt;

            if (!this.powerComp.PowerOn || this.parent.IsBrokenDown())
            {
                if (currLevel == 3)
                    this.StopSustainerHot();
                return;
            }

            float ambientTemperature = this.parent.AmbientTemperature;

            //Détermine si le niveau de temperature du dispositif influe la hotLevelInt en fonction de ses props
            if (ambientTemperature >= Props.hot3)
            {
                this.hotLevelInt = 3;
                this.nbTicksSinceHot3 += 250;

                //On plait le bip bip que si nouveau level différent du level précédent
                if (currLevel != this.hotLevelInt)
                {
                    this.StartSustainerHot();
                }
            }
            else
            {
                if (currLevel == 3)
                    this.StopSustainerHot();

                this.nbTicksSinceHot3 = 0;
                if (ambientTemperature >= Props.hot2)
                {
                    this.hotLevelInt = 2;
                }
                else if (ambientTemperature >= Props.hot1)
                {
                    this.hotLevelInt = 1;
                }
                else
                {
                    this.hotLevelInt = 0;
                }
            }

            //Meltingdown condition remplie on fait péter le serveur
            if (this.nbTicksSinceHot3 >= this.ticksBeforeMelt)
            {
                System.Random rnd = new System.Random();
                //Reset le meltdown counter
                this.nbTicksSinceHot3 = 0;
                //Définition nouveau seuil d'explosion
                setNewExplosionThreshold();

                makeExplosion();
                Find.LetterStack.ReceiveLetter("ATPP_ComptHeatSensitiveComputerMeltTitle".Translate(), "ATPP_ComptHeatSensitiveComputerMeltDesc".Translate(), LetterDefOf.NegativeEvent, new TargetInfo(this.parent.Position, this.parent.Map, false), null, null);
            }
        }

        public void makeExplosion()
        {
            //Passe le dispositif en broken
            if (this.parent != null)
            {
                CompBreakdownable bd = this.parent.TryGetComp<CompBreakdownable>();
                if (bd != null)
                    bd.DoBreakdown();

                Building b = (Building)parent;
                b.HitPoints -= (int)(b.HitPoints * Rand.Range(0.10f, 0.45f));

                if (isSkyCloudCore)
                {
                    GenExplosion.DoExplosion(this.parent.Position, this.parent.Map, 8, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                }
                else
                    GenExplosion.DoExplosion(this.parent.Position, this.parent.Map, 2, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
            }
        }

        public override void PostDeSpawn(Map map)
        {
            this.StopSustainerHot();

            //Desenregistrement dans la liste  dispositifs senseible a la temperature
            Utils.GCATPP.popHeatSensitiveDevices((Building)this.parent, map);
        }

        public override string CompInspectStringExtra()
        {
            if (this.parent == null)
                return "";

            if (this.powerComp != null && !this.powerComp.PowerOn)
                return "";

            if (this.hotLevelInt == 3)
                return "ATPP_CompHotSensitiveHot3Text".Translate();
            else if (this.hotLevelInt == 2)
                return "ATPP_CompHotSensitiveHot2Text".Translate();
            else if (this.hotLevelInt == 1)
                return "ATPP_CompHotSensitiveHot1Text".Translate();
            else
                return "ATPP_CompHotSensitiveHot0Text".Translate();
        }


        private void StartSustainerHot()
        {
            if (this.sustainerHot == null && !Settings.disableServersAlarm)
            {
                SoundInfo info = SoundInfo.InMap(this.parent, MaintenanceType.None);
                this.sustainerHot = this.Props.hotSoundDef.TrySpawnSustainer(info);
            }
        }

        private void StopSustainerHot()
        {
            if (this.sustainerHot != null)
            {
                this.sustainerHot.End();
                this.sustainerHot = null;
            }
        }

        //private SoundDef soundDefHot;
        private Sustainer sustainerHot;

        private int hotLevelInt;

        private int ticksBeforeMelt = 0;

        private int nbTicksSinceHot3 = 0;

        private CompPowerTrader powerComp;

        private bool isSkyCloudCore = false;

    }
}
