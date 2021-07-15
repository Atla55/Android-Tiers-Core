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
using Verse.Sound;

namespace MOARANDROIDS
{
    public class CompSkyMind : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.connected, "ATPP_connected", false);
            Scribe_Values.Look<bool>(ref this.autoconn, "ATPP_autoconn", false);

            Scribe_Values.Look<int>(ref this.infectedExplodeGT, "ATPP_infectedExplodeGT", -1);
            Scribe_Values.Look<int>(ref this.infected, "ATPP_infected", -1);
            Scribe_Values.Look<int>(ref this.infectedEndGT, "ATPP_infectedEndGT", -1);
            Scribe_Values.Look<int>(ref this.hackEndGT, "ATPP_hackEndGT", -1);
            Scribe_Values.Look<int>(ref this.hacked, "ATPP_hacked", -1);
            Scribe_Values.Look<int>(ref this.lastRemoteFlickGT, "ATPP_lastRemoteFlickGT", 0);
            
            Scribe_Values.Look<bool>(ref this.hackWasPrisoned, "ATPP_hackWasPrisoned", false);
            
            Scribe_Values.Look<string>(ref this.infectedPreviousState, "ATPP_infectedPreviousState", "");
            
            Scribe_References.Look<Faction>(ref this.hackOrigFaction, "ATPP_hackOrigFaction", false);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            Utils.GCATPP.popVirusedThing(parent);
            Utils.GCATPP.popSkyMindable(parent);
            Utils.GCATPP.disconnectUser(parent);
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            Utils.GCATPP.popSkyMindable(parent);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (connected)
            {
                if (!Utils.GCATPP.connectUser(parent))
                    connected = false;
            }

            if(infected != -1 || hacked != -1)
            {
                Utils.GCATPP.pushVirusedThing(parent);
            }

            Utils.GCATPP.pushSkyMindable(parent);
        }


        public override void PostDraw()
        {
            Material avatar = null;
            Vector3 vector;

            if (Utils.antennaSelected() && connected)
                avatar = Tex.ConnectedUser;

            if (infected == 1)
                avatar = Tex.virus;
            else if (infected == 2)
                avatar = Tex.explosiveVirus;
            else if (infected == 3)
                avatar = Tex.cryptolocker;
            else if (infected == 4)
                avatar = Tex.virusLite;
            else if (hacked == 1)
                avatar = Tex.Virused;
            else if (hacked == 2)
                avatar = Tex.ExplosiveVirused;
            else if (hacked == 3)
                avatar = Tex.HackedTemp;

            if (avatar != null)
            {
                vector = this.parent.TrueCenter();
                vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28125f;
                vector.z += 1.4f;
                vector.x += this.parent.def.size.x / 2;

                Graphics.DrawMesh(MeshPool.plane08, vector, Quaternion.identity, avatar, 0);
            }
        }


        public bool canBeConnectedToSkyMind()
        {
            if (parent is Pawn)
            {
                Pawn pawn = (Pawn)parent;
                return pawn.VXChipPresent() || pawn.IsAndroidTier();
            }
            else
            {
                CompPowerTrader c = parent.TryGetComp<CompPowerTrader>();
                return c != null && c.PowerOn;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            CompAndroidState casx = parent.TryGetComp<CompAndroidState>();

            //Si infecté ou un surrogate ennemis alors pas de possibilité de le connecté/déconnecté du réseau du joueur
            if (infected != -1 || (parent.Faction != Faction.OfPlayer && casx != null && casx.isSurrogate))
                yield break;

            if (parent is Pawn)
            {
                Pawn pawn = (Pawn)parent;

                //Si ni un humain ou robot pucé ET pas un android Tier alors pas de possibilité de connection au SkyMind
                if (!pawn.VXChipPresent() && !pawn.IsAndroidTier())
                {
                    yield break;
                }

                //Les M7Mech standard ne sont pas controlables
                CompAndroidState cas = parent.TryGetComp<CompAndroidState>();
                if (cas != null && !cas.isSurrogate && parent.def.defName == "M7Mech")
                    yield break;

            }

            //Si batiment et il n'y a pas de skyCloud placé on masque les controles sur les batiments
            if(parent is Building && !Utils.GCATPP.isThereSkyCloudCore())
            {
                yield break;
            }

            yield return new Command_Toggle
            {
                icon = Tex.SkyMindAutoConn,
                defaultLabel = "ATPP_SkyMindAutoConn".Translate(),
                defaultDesc = "ATPP_SkyMindAutoConnDesc".Translate(),
                isActive = (() => autoconn),
                toggleAction = delegate ()
                {
                    autoconn = !autoconn;
                }
            };

            yield return new Command_Toggle
            {
                icon = Tex.SkyMindConn,
                defaultLabel = "ATPP_SkyMindConn".Translate(),
                defaultDesc = "ATPP_SkyMindConnDesc".Translate(),
                isActive = (() => connected),
                toggleAction = delegate ()
                {
                    if (!connected)
                    {
                        if (!Utils.GCATPP.connectUser(parent))
                        {
                            if (Utils.GCATPP.getNbSlotAvailable() == 0)
                                Messages.Message("ATPP_CannotConnectToSkyMindNoNet".Translate(), parent, MessageTypeDefOf.NegativeEvent);
                            else
                                Messages.Message("ATPP_CannotConnectToSkyMind".Translate(), parent, MessageTypeDefOf.NegativeEvent);

                            return;
                        }
                    }
                    else
                    {
                        Utils.GCATPP.disconnectUser(parent);
                    }
                }
            };


            yield break;
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);

            switch (signal)
            {
                case "SkyMindNetworkUserConnected":
                    //Log.Message(parent.LabelCap + " => SkyMindConnectedUser");
                    connected = true;
                    break;
                case "SkyMindNetworkUserDisconnected":
                    //Log.Message(parent.LabelCap + " => SkyMindDisconnectedUser");
                    connected = false;
                    break;
            }
        }

        public int Infected
        {
            get
            {
                return infected;
            }

            set
            {
                int pVal = infected;
                infected = value;
                if(value == -1 && pVal != -1)
                {
                    parent.SetFaction(Faction.OfPlayer);

                    if (Utils.ExceptionCooler.Contains(parent.def.defName) || Utils.ExceptionHeater.Contains(parent.def.defName))
                    {
                        parent.TryGetComp<CompTempControl>().targetTemperature = (float)Convert.ToDouble(infectedPreviousState);
                    }
                    else
                    {
                        CompFlickable cf = parent.TryGetComp<CompFlickable>();
                        if (cf != null)
                        {
                            cf.SwitchIsOn = true;
                        }
                    }

                    Utils.GCATPP.popVirusedThing(parent);
                }
                else
                {
                    switch (value)
                    {
                        //Virus
                        case 1:
                            //Devient hostile
                            parent.SetFaction(Faction.OfAncientsHostile);
                            break;
                        //Virus explosif
                        case 2:
                            //Devient hostile
                            parent.SetFaction(Faction.OfAncientsHostile);
                            infectedExplodeGT = Find.TickManager.TicksGame + (Settings.nbSecExplosiveVirusTakeToExplode * 60);
                            break;
                        //Virus cryptolocker
                        case 3:
                            //Rend inutilisable batiment
                            parent.SetFaction(Faction.OfAncientsHostile);
                            break;
                    }

                    if (parent is Pawn)
                    {
                        Pawn p = (Pawn)parent;

                        CompAndroidState cas = parent.TryGetComp<CompAndroidState>();
                        if (cas == null)
                            return;

                        //Deconnection du contorlleur le cas echeant
                        if (cas.surrogateController != null)
                        {
                            CompSurrogateOwner cso = cas.surrogateController.TryGetComp<CompSurrogateOwner>();
                            if (cso != null)
                            {
                                //Cryptolocker on force la remise du NoHost de down du surrogate
                                if (value == 3)
                                    cso.disconnectControlledSurrogate(p);
                                else
                                {
                                    //AUssinon on evite qu'il down pour eviter  quil fasse tomber son arme dans le cadre des virus lambda
                                    cso.disconnectControlledSurrogate(p,false, true);
                                }
                            }
                        }

                        if (value != 3)
                        {
                            //On eneleve le hediff NoHostConnected
                            Hediff he = p.health.hediffSet.GetFirstHediffOfDef(Utils.hediffNoHost);
                            if (he != null)
                                p.health.RemoveHediff(he);
                        }

                        switch (value)
                        {
                            //Virus
                            case 1:
                                //Devient hostile
                                parent.SetFactionDirect(Faction.OfAncientsHostile);
                                break;
                            //Virus explosif
                            case 2:
                                //Devient hostile
                                parent.SetFactionDirect(Faction.OfAncientsHostile);
                                infectedExplodeGT = Find.TickManager.TicksGame + (Settings.nbSecExplosiveVirusTakeToExplode * 60);
                                break;
                        }

                        Utils.soundDefSurrogateHacked.PlayOneShot(null);
                    }
                    else
                    {
                        //Si device emerdant alors application effet negatif
                        if (Utils.ExceptionCooler.Contains(parent.def.defName))
                        {
                            infectedPreviousState = parent.TryGetComp<CompTempControl>().targetTemperature.ToString();
                            parent.TryGetComp<CompTempControl>().targetTemperature = +200;
                        }
                        else if (Utils.ExceptionHeater.Contains(parent.def.defName))
                        {
                            infectedPreviousState = parent.TryGetComp<CompTempControl>().targetTemperature.ToString();
                            parent.TryGetComp<CompTempControl>().targetTemperature = -200;
                        }
                        else if ((parent.def.thingClass == typeof(Building_Turret) || parent.def.thingClass.IsSubclassOf(typeof(Building_Turret))))
                        {

                        }
                        else
                        {
                            //Sinon le truc enmerdant ces que le dispositif est desactivé
                            CompFlickable cf = parent.TryGetComp<CompFlickable>();
                            if (cf != null)
                            {
                                cf.SwitchIsOn = false;
                            }
                        }
                    }

                    Utils.GCATPP.pushVirusedThing(parent);
                }
            }
        }

        public int Hacked
        {
            get
            {
                return hacked;
            }

            set
            {
                hacked = value;
                if (value == -1)
                {
                    Utils.GCATPP.popVirusedThing(parent);
                }
                else
                {
                    Utils.GCATPP.pushVirusedThing(parent);
                }
            }
        }

        public void tempHackingEnding()
        {
            if (hacked != 3)
                return;
            //Fin du hack temporaire du surrogate on deconnecte le joueur
            Pawn cp = (Pawn)parent;

            CompAndroidState cas = cp.TryGetComp<CompAndroidState>();
            if (cas.surrogateController != null)
            {
                CompSurrogateOwner cso = cas.surrogateController.TryGetComp<CompSurrogateOwner>();
                cso.stopControlledSurrogate(cp, true);
            }

            cp.SetFaction(hackOrigFaction, null);
            //Si le surrogate été un prisonnier on le restaure en tant que tel
            if (hackWasPrisoned)
            {
                if (cp.guest != null)
                {
                    cp.guest.SetGuestStatus(Faction.OfPlayer, true);
                    if (cp.workSettings == null)
                    {
                        cp.workSettings = new Pawn_WorkSettings(cp);
                        cp.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
                    }
                }
            }

            if (cp.jobs != null) {
                cp.jobs.StopAll();
                cp.jobs.ClearQueuedJobs();
            }
            if(cp.mindState != null)
                cp.mindState.Reset(true);

            PawnComponentsUtility.AddAndRemoveDynamicComponents(cp, false);
            hacked = -1;
            hackEndGT = -1;
            hackWasPrisoned = false;

            cp.Map.attackTargetsCache.UpdateTarget(cp);

            foreach (var p in cp.Map.mapPawns.AllPawnsSpawned)
            {
                if (p.mindState == null)
                    continue;

                Thing aim = p.mindState.enemyTarget;

                if (aim != null && aim is Pawn)
                {
                    //Log.Message("=>" + aim.def.defName);

                    Pawn pawnTarget = (Pawn)aim;
                    if (pawnTarget == cp)
                    {

                        if (p.jobs != null)
                        {
                            p.jobs.StopAll();
                            p.jobs.ClearQueuedJobs();
                        }
                    }
                }
            }

            Find.ColonistBar.MarkColonistsDirty();
        }

        public override string CompInspectStringExtra()
        {
            string ret = "";

            if (parent.Map == null)
                return base.CompInspectStringExtra();

            if (infected == 1 && infectedEndGT != -1)
            {
                ret += "ATPP_TempInfection".Translate((infectedEndGT- Find.TickManager.TicksGame).ToStringTicksToPeriodVerbose()) + "\n";
            }

            if (infected == 3)
            {
                ret += "ATPP_CryptoLockedSurrogate".Translate() + "\n";
            }

            if (infected == 2)
            {

                ret += "ATPP_ExplosiveVirus".Translate((int)(Math.Max(0, (infectedExplodeGT - Find.TickManager.TicksGame)).TicksToSeconds())) + "\n";
            }

            if (hacked == 3)
            {
                ret += "ATPP_TempHackingLosingControlIn".Translate((int)(Math.Max(0, (hackEndGT - Find.TickManager.TicksGame)).TicksToSeconds()))+"\n";
            }

            if (Utils.GCATPP.isConnectedToSkyMind(parent))
            {
                ret += "ATPP_SkyMindDetected".Translate() + "\n";
            }

            return ret.TrimEnd('\r', '\n') + base.CompInspectStringExtra();
        }

        public void resetInternalState()
        {
            infected = -1;
            infectedExplodeGT = -1;
        }

        public int infectedEndGT = -1;
        public int hacked = -1;
        public int hackEndGT = -1;
        public Faction hackOrigFaction;
        public bool hackWasPrisoned;
        private int infected = -1; // -1 : pas d'infection, 1: infection virus std, 2: virus explosif, 3: virus cryptolocker => android inutilisable
        public int infectedExplodeGT = -1;
        //Stocke le state precedent des composants piraté (du casting de type a prevoir)
        private string infectedPreviousState = "";
        public int lastRemoteFlickGT = 0;

        public bool autoconn;
        public bool connected;
    }
}