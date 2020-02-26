using System;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;
using System.Collections.Generic;

namespace MOARANDROIDS
{
    public class CompComputer : ThingComp
    {
        public CompProperties_Computer Props
        {
            get
            {
                return (CompProperties_Computer)this.props;
            }

        }

        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.powerComp = this.parent.GetComp<CompPowerTrader>();

            if (this.Props.ambiance != "None")
                this.ambiance = SoundDef.Named(this.Props.ambiance);

            if (Utils.ExceptionSkillServers.Contains(parent.def.defName))
            {
                isSkillServer = true;
                Utils.GCATPP.pushSkillServer((Building)this.parent);
            }

            if (Utils.ExceptionSecurityServers.Contains(parent.def.defName))
            {
                isSecurityServer = true;
                Utils.GCATPP.pushSecurityServer((Building)this.parent);
            }

            if (Utils.ExceptionHackingServers.Contains(parent.def.defName))
            {
                isHackingServer = true;
                Utils.GCATPP.pushHackingServer((Building)this.parent);
            }

            if (respawningAfterLoad)
            {
                if (this.powerComp.PowerOn)
                    this.StartSustainer();
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            int GT = Find.TickManager.TicksGame;

            //TOutes les 30sec incrémentation points de sécurité
            if(isHackingServer && GT % 1800 == 0 && !(((Building)parent).IsBrokenDown() || !((Building)parent).TryGetComp<CompPowerTrader>().PowerOn))
            {
                 Utils.GCATPP.incHackingPoints(Utils.nbHackingPointsGeneratedBy((Building)parent));
            }

            if (isSkillServer && GT % 1800 == 0 && !(((Building)parent).IsBrokenDown() || !((Building)parent).TryGetComp<CompPowerTrader>().PowerOn))
            {
                Utils.GCATPP.incSkillPoints(Utils.nbSkillPointsGeneratedBy((Building)parent));
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            Building host = (Building)parent;
            //Arret manuel ou  composant endommagé => arret ambiance
            if (signal == "FlickedOff" || signal == "ScheduledOff" || signal == "Breakdown" || signal == "PowerTurnedOff" )
            {
                if (isSkillServer)
                    Utils.GCATPP.popSkillServer(host);
                if (isSecurityServer)
                    Utils.GCATPP.popSecurityServer(host);
                if (isHackingServer)
                    Utils.GCATPP.popHackingServer(host);
                //if (signal != "Virused" && signal != "Hacked")
                this.StopSustainer();
            }

            //Redemarrage ambiance
            if (signal == "PowerTurnedOn")
            {
                if (isSkillServer)
                    Utils.GCATPP.pushSkillServer(host);
                if (isSecurityServer)
                    Utils.GCATPP.pushSecurityServer(host);
                if (isHackingServer)
                    Utils.GCATPP.pushHackingServer(host);
                this.StartSustainer();
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Building build = (Building)parent;

            //Si brokendown ou pas alimenté on degage ou partie sécurité désactivée
            if (Settings.disableSkyMindSecurityStuff || build.IsBrokenDown() || !build.TryGetComp<CompPowerTrader>().PowerOn)
                yield break;

            int nbp = Utils.GCATPP.getNbHackingPoints();

            bool canVirus, canVirusExplosive, canHack, canTempHack;
            canVirus = canVirusExplosive = canHack = canTempHack = false;

            bool powered = !parent.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare) && !build.IsBrokenDown() && build.TryGetComp<CompPowerTrader>().PowerOn;
            
            Texture2D tex;


            if (isHackingServer)
            {

                if (nbp - Settings.costPlayerVirus >= 0)
                    canVirus = true && powered;

                if (canVirus)
                    tex = Tex.PlayerVirus;
                else
                    tex = Tex.PlayerVirusDisabled;

                yield return new Command_Action
                {
                    icon = tex,
                    defaultLabel = "ATPP_UploadVirus".Translate(),
                    defaultDesc = "ATPP_UploadVirusDesc".Translate(),
                    action = delegate ()
                    {
                        if (canVirus)
                            showFloatMapHackMenu(1);
                        else
                            Messages.Message("ATPP_CannotHackNotEnoughtHackingPoints".Translate(Settings.costPlayerVirus), MessageTypeDefOf.NegativeEvent);
                    }
                };

                if (nbp - Settings.costPlayerExplosiveVirus >= 0)
                    canVirusExplosive = true && powered;

                if (canVirusExplosive)
                    tex = Tex.PlayerExplosiveVirus;
                else
                    tex = Tex.PlayerExplosiveVirusDisabled;

                yield return new Command_Action
                {
                    icon = tex,
                    defaultLabel = "ATPP_UploadExplosiveVirus".Translate(),
                    defaultDesc = "ATPP_UploadExplosiveVirusDesc".Translate(),
                    action = delegate ()
                    {
                        if (canVirusExplosive)
                            showFloatMapHackMenu(2);
                        else
                            Messages.Message("ATPP_CannotHackNotEnoughtHackingPoints".Translate(Settings.costPlayerExplosiveVirus), MessageTypeDefOf.NegativeEvent);
                    }
                };

                if (nbp - Settings.costPlayerHackTemp >= 0)
                    canTempHack = true && powered;

                if (canTempHack)
                    tex = Tex.PlayerHackingTemp;
                else
                    tex = Tex.PlayerHackingTempDisabled;


                yield return new Command_Action
                {
                    icon = tex,
                    defaultLabel = "ATPP_HackTemp".Translate(),
                    defaultDesc = "ATPP_HackTempDesc".Translate(),
                    action = delegate ()
                    {
                        if (canTempHack)
                            showFloatMapHackMenu(3);
                        else
                            Messages.Message("ATPP_CannotHackNotEnoughtHackingPoints".Translate(Settings.costPlayerHackTemp), MessageTypeDefOf.NegativeEvent);
                    }
                };

                if (nbp - Settings.costPlayerHack >= 0)
                    canHack = true && powered;

                if (canHack)
                    tex = Tex.PlayerHacking;
                else
                    tex = Tex.PlayerHackingDisabled;

                yield return new Command_Action
                {
                    icon = tex,
                    defaultLabel = "ATPP_Hack".Translate(),
                    defaultDesc = "ATPP_HackDesc".Translate(),
                    action = delegate ()
                    {
                        if (canHack)
                            showFloatMapHackMenu(4);
                        else
                            Messages.Message("ATPP_CannotHackNotEnoughtHackingPoints".Translate(Settings.costPlayerHack), MessageTypeDefOf.NegativeEvent);
                    }
                };
            }

            yield break;
        }

        private void showFloatMapHackMenu(int hackType)
        {
            //Listing map de destination
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            string lib = "";
            foreach (var m in Find.Maps)
            {
                if (m == Find.CurrentMap)
                    lib = "ATPP_ThisCurrentMap".Translate(m.Parent.Label);
                else
                    lib = m.Parent.Label;

                opts.Add(new FloatMenuOption(lib, delegate
                {
                    Current.Game.CurrentMap = m;
                    Designator_SurrogateToHack x = new Designator_SurrogateToHack(hackType);
                    Find.DesignatorManager.Select(x);

                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            if (opts.Count != 0)
            {
                if (opts.Count == 1)
                {
                    Designator_SurrogateToHack x = new Designator_SurrogateToHack(hackType);
                    Find.DesignatorManager.Select(x);
                }
                else
                {
                    FloatMenu floatMenuMap = new FloatMenu(opts);
                    Find.WindowStack.Add(floatMenuMap);
                }
            }

        }

        public override string CompInspectStringExtra()
        {
            string ret = "";

            if (parent.Map == null)
                return base.CompInspectStringExtra();

            if (isSecurityServer)
            {
                ret += "ATPP_SecurityServersSynthesis".Translate(Utils.GCATPP.getNbSlotSecurisedAvailable(), Utils.GCATPP.getNbThingsConnected()) + "\n";
                ret += "ATTP_SecuritySlotsAdded".Translate(Utils.nbSecuritySlotsGeneratedBy((Building)parent)) + "\n";
            }

            if (isHackingServer)
            {
                ret += "ATPP_HackingServersSynthesis".Translate(Utils.GCATPP.getNbHackingPoints(), Utils.GCATPP.getNbHackingSlotAvailable()) + "\n";
                ret += "ATTP_HackingProducedPoints".Translate(Utils.nbHackingPointsGeneratedBy((Building)parent)) + "\n";
                ret += "ATTP_HackingSlotsAdded".Translate(Utils.nbHackingSlotsGeneratedBy((Building)parent)) + "\n";
            }

            if (isSkillServer)
            {
                ret += "ATPP_SkillServersSynthesis".Translate(Utils.GCATPP.getNbSkillPoints(), Utils.GCATPP.getNbSkillSlotAvailable()) + "\n";
                ret += "ATTP_SkillProducedPoints".Translate(Utils.nbSkillPointsGeneratedBy((Building)parent)) + "\n";
                ret += "ATTP_SkillSlotsAdded".Translate(Utils.nbSkillSlotsGeneratedBy((Building)parent)) + "\n";
            }

            return ret.TrimEnd('\r', '\n') + base.CompInspectStringExtra();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            this.StopSustainer();

            if (isSecurityServer)
            {
                Utils.GCATPP.popSecurityServer((Building)this.parent);
            }

            if (isHackingServer)
            {
                Utils.GCATPP.popHackingServer((Building)this.parent);
            }

            if (isSkillServer)
            {
                Utils.GCATPP.popSkillServer((Building)this.parent);
            }
        }


        private void StartSustainer()
        {
            if (this.sustainer == null && this.Props.ambiance != "None" && !Settings.disableServersAmbiance)
            {
                SoundInfo info = SoundInfo.InMap(this.parent, MaintenanceType.None);
                this.sustainer = this.ambiance.TrySpawnSustainer(info);
                //this.sustainer = SoundDefOf.GeyserSpray.TrySpawnSustainer(info);

                //this.sustainerHot = SoundDefOf.GeyserSpray.TrySpawnSustainer(info);
            }
        }

        private void StopSustainer()
        {
            if (this.sustainer != null && this.Props.ambiance != "None")
            {
                this.sustainer.End();
                this.sustainer = null;
                // SoundInfo info = SoundInfo.InMap(this.parent, MaintenanceType.None);
                //this.ambiance.sustainStopSound.PlayOneShot(info);
            }
        }

        private Sustainer sustainer;
        private CompPowerTrader powerComp;
        private SoundDef ambiance;

        private bool isSecurityServer = false;
        private bool isHackingServer = false;
        private bool isSkillServer = false;
    }
}
