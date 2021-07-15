using System;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using Verse.Sound;

namespace MOARANDROIDS
{
    public class CompSkyCloudCore : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look<int>(ref SID, "ATPP_SID", -1);
            Scribe_Values.Look<int>(ref bootGT, "ATPP_bootGT", -2);
            Scribe_Collections.Look(ref this.storedMinds, false, "ATPP_storedMinds", LookMode.Deep);
            Scribe_Collections.Look(ref this.assistingMinds, false, "ATPP_assistingMinds", LookMode.Reference);
            Scribe_Collections.Look(ref this.replicatingMinds, false, "ATPP_replicatingMinds", LookMode.Reference);

            Scribe_Collections.Look<Pawn, int>(ref this.inMentalBreak, "ATPP_inMentalBreak", LookMode.Reference, LookMode.Value, ref inMentalBreakKeys, ref inMentalBreakValues);


            Scribe_Collections.Look<Pawn, Building>(ref this.controlledTurrets, "ATPP_controlledTurrets", LookMode.Reference, LookMode.Reference, ref controlledTurretsKeys, ref controlledTurretsValues);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (storedMinds == null)
                    storedMinds = new List<Pawn>();

                if (controlledTurrets == null)
                    controlledTurrets = new Dictionary<Pawn, Building>();

                if (inMentalBreak == null)
                    inMentalBreak = new Dictionary<Pawn, int>();

                storedMinds.RemoveAll(item => item == null);
            }
        }


        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();

            //Affichage minds connectés
            foreach (var p in storedMinds)
            {
                //Colon digitalisé connecté à un surrogate on trace le lien
                CompSurrogateOwner cso = p.TryGetComp<CompSurrogateOwner>();

                foreach(var csx in cso.availableSX)
                {
                    if (parent.Map == csx.Map)
                    {
                        GenDraw.DrawLineBetween(parent.TrueCenter(), csx.TrueCenter(), SimpleColor.Red);
                    }

                }
            }

            //Affichage turets connectés
            foreach(var p in controlledTurrets)
            {
                if(parent.Map == p.Value.Map)
                {
                    GenDraw.DrawLineBetween(parent.TrueCenter(), p.Value.TrueCenter(), SimpleColor.Red);
                }
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if(SID == -1)
            {
                SID = Utils.GCATPP.getNextSkyCloudID();
                Utils.GCATPP.incNextSkyCloudID();
            }
            if (Booted())
            {
                Utils.GCATPP.pushSkyCloudCore((Building)parent);
            }

            Utils.GCATPP.pushSkyCloudCoreAbs((Building)parent);


            //Application retroactive de la surppression de traits blacklistés pour les minds
            foreach(var m in storedMinds)
            {
                Utils.removeMindBlacklistedTrait(m);
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            stopAllMindsActivities();

            //Retire de la liste des emetteurs de la map
            Utils.GCATPP.popSkyCloudCore((Building)this.parent);
            Utils.GCATPP.popSkyCloudCoreAbs((Building)this.parent);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            //Kill de tous les hotes stockés
            if (storedMinds.Count != 0)
            {
                disconnectAllSurrogates();
                disconnectAllRemotelyControlledTurrets();
                //ATPP_destroyedMindsDueToDestroyedSkyCloudCore
                Find.LetterStack.ReceiveLetter("ATPP_destroyedMindsDueToDestroyedSkyCloudCore".Translate(storedMinds.Count), "ATPP_destroyedMindsDueToDestroyedSkyCloudCoreDesc".Translate(storedMinds.Count, getName()), LetterDefOf.ThreatBig);

                foreach (var p in storedMinds)
                {
                    p.Kill(null, null);
                }
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            if (signal == "PowerTurnedOff")
            {
                //Su systeme booté le serveur dit le power Failure
                if (bootGT == -1)
                    Utils.playVocal("soundDefSkyCloudPowerFailure");

                bootGT = -2;
                stopAllMindsActivities(true);
                Utils.GCATPP.popSkyCloudCore((Building)parent);
            }

            //Redemarrage ambiance
            if (signal == "PowerTurnedOn")
            {
                //Definition sec ou le core démarrera vraiment
                bootGT = Find.TickManager.TicksGame + (Settings.secToBootSkyCloudCore * 60);
            }
        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Building build = (Building)parent;

            //Si aucun mind stocké
            if (storedMinds.Count() == 0 || !build.TryGetComp<CompPowerTrader>().PowerOn || !Booted())
            {
                yield break;
            }

            yield return new Command_Action
            {
                icon = Tex.processInfo,
                defaultLabel = "ATPP_ProcessInfo".Translate(),
                defaultDesc = "ATPP_ProcessInfoDesc".Translate(),
                action = delegate ()
                {
                    showFloatMenuMindsStored(delegate (Pawn p)
                    {
                        Find.WindowStack.Add(new Dialog_InfoCard(p));
                    },false,false,false,true);
                }
            };

            yield return new Command_Action
            {
                icon = Tex.processRemove,
                defaultLabel = "ATPP_ProcessRemove".Translate(),
                defaultDesc = "ATPP_ProcessRemoveDesc".Translate(),
                action = delegate ()
                {
                    showFloatMenuMindsStored(delegate (Pawn p)
                    {
                        Find.WindowStack.Add(new Dialog_Msg("ATPP_ProcessRemove".Translate(), "ATPP_ProcessRemoveDescConfirm".Translate(p.LabelShortCap, getName()), delegate
                        {
                            stopMindActivities(p);

                            RemoveMind(p);
                            p.Kill(null, null);

                            Messages.Message("ATPP_ProcessRemoveOK".Translate(p.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);
                            
                            Utils.playVocal("soundDefSkyCloudMindDeletionCompleted");

                        }, false));
                    }, false, false, false, false);
                }
            };

            yield return new Command_Action
            {
                icon = Tex.processDuplicate,
                defaultLabel = "ATPP_ProcessDuplicate".Translate(),
                defaultDesc = "ATPP_ProcessDuplicateDesc".Translate(),
                action = delegate ()
                {
                    showFloatMenuMindsStored(delegate (Pawn p)
                    {
                        CompSurrogateOwner cso = p.TryGetComp<CompSurrogateOwner>();
                        if (cso == null)
                            return;

                        int GT = Find.TickManager.TicksGame;

                        cso.replicationStartGT = GT;
                        cso.replicationEndingGT = GT + (Settings.mindReplicationHours * 2500);

                        replicatingMinds.Add(p);
                        stopMindActivities(p);

                        Messages.Message("ATPP_ProcessDuplicateOK".Translate(p.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);

                    }, false, false, false, false);
                }
            };

            yield return new Command_Action
            {
                icon = Tex.processAssist,
                defaultLabel = "ATPP_ProcessAssist".Translate(),
                defaultDesc = "ATPP_ProcessAssistDesc".Translate(),
                action = delegate ()
                {
                    List<FloatMenuOption> opts = new List<FloatMenuOption>();
                    //Affichage des minds affectés à l'assistement
                    opts.Add(new FloatMenuOption("ATPP_ProcessAssistAssignedMinds".Translate(), delegate
                    {
                        List<FloatMenuOption> optsAdd = null;

                        //Check s'il y a lieu d'jaouter l'option (il y a au moin 1+ minds assigné à supprimer
                        if (assistingMinds.Count > 0)
                        {
                            optsAdd = new List<FloatMenuOption>();
                            optsAdd.Add(new FloatMenuOption("-"+("ATPP_ProcessAssistUnassignAll".Translate()), delegate
                            {
                                int nb = 0;
                                foreach (var m in storedMinds)
                                {
                                    if (assistingMinds.Contains(m))
                                    {
                                        assistingMinds.Remove(m);
                                        nb++;
                                    }
                                }

                                if(nb> 0)
                                    Messages.Message("ATPP_ProcessMassUnassist".Translate(nb), parent, MessageTypeDefOf.PositiveEvent);

                            }, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }

                        showFloatMenuMindsStored(delegate (Pawn p)
                        {
                            assistingMinds.Remove(p);

                            Messages.Message("ATPP_ProcessUnassistOK".Translate(p.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);

                        }, false, false, false, false, optsAdd, false, true);

                    }, MenuOptionPriority.Default, null, null, 0f, null, null));

                    //Affichage des minds non affectés à l'assistement
                    opts.Add(new FloatMenuOption("ATPP_ProcessAssistUnassignedMinds".Translate(), delegate
                    {
                        List<FloatMenuOption> optsAdd = null;

                        //Check s'il y a lieu d'jaouter l'option (il y a des minds et des minds non ajoutés)
                        if (storedMinds.Count > 0 && getNbUnassistingMinds() > 0)
                        {
                            optsAdd = new List<FloatMenuOption>();
                            optsAdd.Add(new FloatMenuOption("-"+("ATPP_ProcessAssistAssignAll".Translate()), delegate
                            {
                                int nb = 0;
                                foreach (var m in storedMinds)
                                {
                                    if (!assistingMinds.Contains(m))
                                    {
                                        stopMindActivities(m);
                                        assistingMinds.Add(m);
                                        nb++;
                                    }
                                }

                                if (nb > 0)
                                    Messages.Message("ATPP_ProcessMassAssist".Translate(nb), parent, MessageTypeDefOf.PositiveEvent);

                            }, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }

                        showFloatMenuMindsStored(delegate (Pawn p)
                        {
                            stopMindActivities(p);
                            assistingMinds.Add(p);

                            Messages.Message("ATPP_ProcessAssistOK".Translate(p.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);

                        }, false, false, false, false, optsAdd, true);

                    }, MenuOptionPriority.Default, null, null, 0f, null, null));

                    FloatMenu floatMenuMap = new FloatMenu(opts);
                    Find.WindowStack.Add(floatMenuMap);
                }
            };

            yield return new Command_Action
            {
                icon = Tex.processMigrate,
                defaultLabel = "ATPP_ProcessMigrate".Translate(),
                defaultDesc = "ATPP_ProcessMigrateDesc".Translate(),
                action = delegate ()
                {
                    showFloatMenuMindsStored(delegate (Pawn p)
                    {
                        Utils.ShowFloatMenuSkyCloudCores(delegate (Building core)
                        {
                            CompSurrogateOwner cso = p.TryGetComp<CompSurrogateOwner>();
                            stopMindActivities(p);
                            cso.startMigration( core);
                        }, (Building)parent);
                    }, false, false, false, false);
                }
            };

            yield return new Command_Action
            {
                icon = Tex.processSkillUp,
                defaultLabel = "ATPP_Skills".Translate(),
                defaultDesc = "ATPP_SkillsDesc".Translate(),
                action = delegate ()
                {
                    showFloatMenuMindsStored(delegate (Pawn p)
                    {
                        Find.WindowStack.Add(new Dialog_SkillUp(p,true));

                    }, false, false, false, true);
                }
            };

            yield return new Command_Action
            {
                icon = Tex.AndroidToControlTarget,
                defaultLabel = "ATPP_AndroidToControlTarget".Translate(),
                defaultDesc = "ATPP_AndroidToControlTargetDesc".Translate(),
                action = delegate ()
                {
                    showFloatMenuMindsStored(delegate (Pawn p)
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
                                Designator_AndroidToControl x = new Designator_AndroidToControl(p, true);
                                Find.DesignatorManager.Select(x);

                            }, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                        if (opts.Count != 0)
                        {
                            if (opts.Count == 1)
                            {
                                Designator_AndroidToControl x = new Designator_AndroidToControl(p, true);
                                Find.DesignatorManager.Select(x);
                            }
                            else
                            {
                                FloatMenu floatMenuMap = new FloatMenu(opts);
                                Find.WindowStack.Add(floatMenuMap);
                            }
                        }
                    }, true, true,false, false, null, true );
                    
                }
            };

            if (Utils.isThereNotControlledSurrogateInCaravan())
            {
                //Si drones SX no controllés dans une caravane
                yield return new Command_Action
                {
                    icon = Tex.AndroidToControlTargetRecovery,
                    defaultLabel = "ATPP_AndroidToControlTargetRecoverCaravan".Translate(),
                    defaultDesc = "ATPP_AndroidToControlTargetRecoverCaravanDesc".Translate(),
                    action = delegate ()
                    {
                        showFloatMenuMindsStored(delegate (Pawn p)
                        {
                            Utils.ShowFloatMenuNotCOntrolledSurrogateInCaravan(p, delegate (Pawn sSX)
                            {
                                CompSurrogateOwner cso = p.TryGetComp<CompSurrogateOwner>();
                                if (cso == null)
                                    return;

                                if (!Utils.GCATPP.isConnectedToSkyMind(sSX))
                                {
                                    //Tentative connection au skymind 
                                    if (!Utils.GCATPP.connectUser(sSX))
                                        return;
                                }
                                cso.setControlledSurrogate(sSX);
                            });
                        }, true, true, false, false, null, true);
                    }
                };
            }

            if (getNbMindsConnectedToSurrogate() != 0 || controlledTurrets.Count() != 0)
            {
                yield return new Command_Action
                {
                    icon = Tex.AndroidToControlTargetDisconnect,
                    defaultLabel = "ATPP_AndroidToControlTargetDisconnect".Translate(),
                    defaultDesc = "ATPP_AndroidToControlTargetDisconnectDesc".Translate(),
                    action = delegate ()
                    {
                        List<FloatMenuOption> opts = new List<FloatMenuOption>();
                        opts.Add(new FloatMenuOption("ATPP_ProcessDisconnectAllSurrogates".Translate(), delegate
                        {
                            disconnectAllSurrogates();
                            disconnectAllRemotelyControlledTurrets();
                            Utils.playVocal("soundDefSkyCloudAllMindDisconnected");
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));

                        showFloatMenuMindsStored(delegate (Pawn p)
                        {
                            CompSurrogateOwner cso = p.TryGetComp<CompSurrogateOwner>();
                            if (cso != null && cso.isThereSX())
                            {
                                 cso.disconnectControlledSurrogate(null);
                            }
                            stopRemotelyControlledTurret(p);

                        }, false, false,  true, false, opts);

                    }
                };
            }

            yield break;
        }

        public override string CompInspectStringExtra()
        {
            string ret = "";
            Building build = (Building)parent;

            if (parent.Map == null)
                return base.CompInspectStringExtra();

            ret += getName()+"\n";
            ret += "ATPP_CentralCoreNbStoredMind".Translate(storedMinds.Count)+"\n";
            ret += "ATPP_CentralCoreNbAssistingMinds".Translate(assistingMinds.Count) + "\n";

            if (build.TryGetComp<CompPowerTrader>().PowerOn)
            {

                if (!Booted())
                {
                    ret += "ATPP_SkyCloudCoreBooting".Translate((int)(Math.Max(0, bootGT - Find.TickManager.TicksGame)).TicksToSeconds());
                }
                else
                {

                    //Check migration en cours de mind
                    foreach (var m in storedMinds)
                    {
                        CompSurrogateOwner cso = m.TryGetComp<CompSurrogateOwner>();

                        if (cso == null)
                            continue;

                        if (cso.replicationEndingGT != -1)
                        {
                            float p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - cso.replicationStartGT) / (float)(cso.replicationEndingGT - cso.replicationStartGT));

                            ret += "=>" + ("ATPP_CentralCoreReplicationInProgress".Translate(m.LabelShortCap, ((int)(p * (float)100)).ToString())) + "\n";
                        }
                        //ATPP_CentralCoreReplicationInProgress

                        else if (cso.migrationEndingGT != -1 && cso.migrationSkyCloudHostDest != null)
                        {
                            CompSkyCloudCore csc2 = cso.migrationSkyCloudHostDest.TryGetComp<CompSkyCloudCore>();
                            float p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - cso.migrationStartGT) / (float)(cso.migrationEndingGT - cso.migrationStartGT));

                            ret += "=>" + ("ATPP_CentralCoreMigrationInProgress".Translate(m.LabelShortCap, csc2.getName(), ((int)(p * (float)100)).ToString())) + "\n";
                        }

                        else if (inMentalBreak.ContainsKey(m))
                        {
                            ret += "=>" + "ATPP_CentralCoreProcessInMentalBreak".Translate(m.LabelShortCap) + "\n";
                        }
                    }
                }
            }

            return ret.TrimEnd('\r', '\n') + base.CompInspectStringExtra();
        }

        public bool Booted()
        {
            return bootGT == -1;
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

                //Check si demarrage du core
                if(bootGT > 0 && bootGT < CGT)
                {
                    //On rend accessible les controles
                    bootGT = -1;

                    if (((Building)parent).TryGetComp<CompPowerTrader>().PowerOn)
                        Utils.GCATPP.pushSkyCloudCore((Building)parent);

                    //ATPP_SkyCloudCoreBooted
                    Find.LetterStack.ReceiveLetter("ATPP_SkyCloudCoreBooted".Translate(getName()), "ATPP_SkyCloudCoreBootedDesc".Translate(getName()), LetterDefOf.NeutralEvent, parent);

                    //Play sound
                    Utils.playVocal("soundDefSkyCloudPrimarySystemsOnline");
                }
            }

            if(CGT % 120 == 0)
            {
                List<CompSurrogateOwner> migrationsDone = null;
                List<CompSurrogateOwner> replicationDone = null;

                foreach (var m in storedMinds)
                {
                    CompSurrogateOwner cso = m.TryGetComp<CompSurrogateOwner>();
                    if (cso == null)
                        continue;

                    cso.checkInterruptedUpload();

                    //Atteinte fin attente de la replication d'un mind
                    if (cso.replicationEndingGT != -1 && cso.replicationEndingGT < CGT)
                    {
                        if (replicationDone == null)
                            replicationDone = new List<CompSurrogateOwner>();
                        replicationDone.Add(cso);
                    }

                    if (cso.migrationEndingGT != -1 && cso.migrationEndingGT < CGT)
                    {
                        if (migrationsDone == null)
                            migrationsDone = new List<CompSurrogateOwner>();
                        migrationsDone.Add(cso);
                    }
                }

                if(migrationsDone != null)
                {
                    foreach(var md in migrationsDone)
                    {
                        md.OnMigrated();
                    }
                    
                    Utils.playVocal("soundDefSkyCloudMindMigrationCompleted");
                }
                if (replicationDone != null)
                {
                    foreach (var md in replicationDone)
                    {
                        md.OnReplicate();
                    }
                    
                    Utils.playVocal("soundDefSkyCloudMindReplicationCompleted");
                }
            }

            if(CGT % 600 == 0)
            {
                //CHECK de la fin des mental breaks des minds stockés --  decrementation temps 
                if (((Building)parent).TryGetComp<CompPowerTrader>().PowerOn && inMentalBreak.Count > 0)
                {
                    var keys = new List<Pawn>(inMentalBreak.Keys);
                    foreach (var ck in keys)
                    {
                        inMentalBreak[ck] -= 600;
                        if (inMentalBreak[ck] <= 0)
                        {
                            inMentalBreak.Remove(ck);

                            Messages.Message("ATPP_ProcessNoLongerInMentalBreak".Translate(ck.LabelShortCap, getName()), parent, MessageTypeDefOf.PositiveEvent);
                        }
                    }
                }
            }
        }

        public void setMentalBreak(Pawn mind)
        {
            if (!storedMinds.Contains(mind) || inMentalBreak.ContainsKey(mind))
                return;

            //Arret du mental state
            mind.mindState.Reset();

            //Si surrogate founris resolution du controleur
            if (mind.IsSurrogateAndroid())
            {
                CompAndroidState cas = mind.TryGetComp<CompAndroidState>();
                if (cas == null || cas.surrogateController == null)
                    return;

                mind = cas.surrogateController;
            }

            //Arret des activitées
            stopMindActivities(mind);

            inMentalBreak[mind] = Rand.Range(Settings.minDurationMentalBreakOfDigitisedMinds, Settings.maxDurationMentalBreakOfDigitisedMinds) * 2500;
            Utils.playVocal("soundDefSkyCloudMindQuarantineMentalState");
        }

        public int getNbMindsConnectedToSurrogate()
        {
            int ret = 0;
            foreach (var m in storedMinds)
            {
                CompSurrogateOwner cso = m.TryGetComp<CompSurrogateOwner>();
                if (cso != null && cso.isThereSX())
                    ret++;

            }

            return ret;
        }

        public void stopAllMindsActivities(bool serverShutdown=false)
        {
            foreach(var m in storedMinds)
            {
                stopMindActivities(m, serverShutdown);
            }
        }

        public void stopMindActivities(Pawn mind, bool serverShutdown=false)
        {
            stopRemotelyControlledTurret(mind);
            CompSurrogateOwner cso = mind.TryGetComp<CompSurrogateOwner>();
            if (cso != null && cso.isThereSX())
            {
                cso.disconnectControlledSurrogate(null);
            }

            if (!serverShutdown && assistingMinds.Contains(mind))
                assistingMinds.Remove(mind);
        }

        public bool mindIsBusy(Pawn mind)
        {
            CompSurrogateOwner cso = mind.TryGetComp<CompSurrogateOwner>();
            return controlledTurrets.ContainsKey(mind) || replicatingMinds.Contains(mind) || inMentalBreak.ContainsKey(mind) || assistingMinds.Contains(mind) || (cso != null && cso.migrationEndingGT != -1);
        }

        public bool isRunning()
        {
            Building build = (Building)parent;

            return !build.Destroyed && !build.IsBrokenDown() && build.TryGetComp<CompPowerTrader>().PowerOn;
        }

        public string getName()
        {
            return "Core-" + SID;
        }

        public float getPowerConsumed()
        {
            return (storedMinds.Count()*Settings.powerConsumedByStoredMind) + this.parent.TryGetComp<CompPowerTrader>().Props.basePowerConsumption;
        }


        public int getNbUnassistingMinds()
        {
            int ret = 0;
            foreach(var m in storedMinds)
            {
                if (assistingMinds.Contains(m))
                    continue;
                ret++;
            }

            return ret;
        }

        public void refreshPowerConsumed()
        {
            this.parent.TryGetComp<CompPowerTrader>().powerOutputInt = -(getPowerConsumed());
        }


        public void setRemotelyControlledTurret(Pawn mind, Building turret)
        {
            if (!storedMinds.Contains(mind) || controlledTurrets.ContainsKey(mind))
                return;

            CompRemotelyControlledTurret crt = turret.TryGetComp<CompRemotelyControlledTurret>();
            crt.controller = mind;
            controlledTurrets[mind] = turret;

            Utils.soundDefTurretConnection.PlayOneShot(null);
            MoteMaker.ThrowDustPuffThick(turret.Position.ToVector3Shifted(), turret.Map, 4.0f, Color.blue);

            Messages.Message("ATPP_SurrogateConnectionOK".Translate(mind.LabelShortCap, turret.LabelShortCap), turret, MessageTypeDefOf.PositiveEvent);
        }

        public void stopRemotelyControlledTurret(Pawn mind)
        {
            if (!controlledTurrets.ContainsKey(mind))
                return;

            CompRemotelyControlledTurret crt = controlledTurrets[mind].TryGetComp<CompRemotelyControlledTurret>();
            crt.controller =null;

            controlledTurrets.Remove(mind);
            Utils.soundDefTurretConnectionStopped.PlayOneShot(null);
        }

        private void disconnectAllRemotelyControlledTurrets()
        {
            foreach (var e in controlledTurrets.ToList())
            {
                stopRemotelyControlledTurret(e.Key);
            }
        }

        private void disconnectAllSurrogates()
        {
            foreach (var m in storedMinds)
            {
                CompSurrogateOwner cso = m.TryGetComp<CompSurrogateOwner>();
                if (cso != null)
                    cso.stopControlledSurrogate(null);
            }
        }

        public void RemoveMind(Pawn mind)
        {
            stopRemotelyControlledTurret(mind);

            if (assistingMinds.Contains(mind))
                assistingMinds.Remove(mind);
            if (replicatingMinds.Contains(mind))
                replicatingMinds.Remove(mind);

            storedMinds.Remove(mind);
        }

        public void showFloatMenuMindsStored(Action<Pawn> onClick, bool hideSurrogate=false, bool hideTurretController=false, bool showOnlyConnectedDevices = false,bool resolveSurrogates=false, List<FloatMenuOption> supOpts=null, bool hideAssistingMinds =false, bool showOnlyAssistingMinds=false)
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            FloatMenu floatMenuMap;

            if(supOpts != null)
            {
                opts = opts.Concat(supOpts).ToList();
            }

            //Listing des SkyCloud Cores
            foreach (var m in storedMinds)
            {
                //Log.Message("Suspended => " +m.LabelCap+" "+ m.Suspended);
                CompSurrogateOwner cso = m.TryGetComp<CompSurrogateOwner>();
                bool isSurrogateController = cso != null && cso.isThereSX();
                bool turretController = controlledTurrets.ContainsKey(m);
                bool isAssistingMind = assistingMinds.Contains(m);

                //Si mind dans un mental break
                if (inMentalBreak.ContainsKey(m))
                    continue;

                //Si mind en cours de replication on le masque
                if (replicatingMinds.Contains(m))
                    continue;

                //Si migration en cours sur un mind on le jerte de la liste des minds consultables
                if (cso != null && cso.migrationEndingGT != -1)
                    continue;

                if ((hideAssistingMinds && isAssistingMind) || showOnlyAssistingMinds && !isAssistingMind)
                    continue;

                if (hideTurretController && turretController)
                    continue;

                if (hideSurrogate && isSurrogateController)
                    continue;

                string name = m.LabelShortCap;

                if (isSurrogateController)
                {
                    //On affiche le nom du colon numérisé car il est permuté avec le surrogate
                    if (!cso.isThereSX())
                    {
                        name = m.LabelShortCap;
                    }
                    else
                    {
                        if(cso.SX != null)
                            name = cso.SX.LabelShortCap;
                        else
                            name = m.LabelShortCap;
                    }
                }

                if ( showOnlyConnectedDevices )
                {
                    if (!isSurrogateController && !turretController)
                        continue;
                }
               

                opts.Add(new FloatMenuOption(name, delegate
                {
                    Pawn sel = m;
                    if (resolveSurrogates && isSurrogateController)
                    {
                        if (cso.isThereSX())
                        {
                            if (cso.isThereSX())
                            {
                                if (cso.SX != null)
                                    sel = cso.SX;
                                else
                                    sel = m;
                            }
                            else
                                sel = m;
                        }
                        else
                            sel = m;
                    }

                    onClick(sel);
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            opts.SortBy((x) => x.Label);

            if (opts.Count == 0)
                return;

            floatMenuMap = new FloatMenu(opts, "ATPP_SkyCloudFloatMenuMindsStoredSelect".Translate());
            Find.WindowStack.Add(floatMenuMap);
        }


        public List<Pawn> storedMinds = new List<Pawn>();
        public Dictionary<Pawn, Building> controlledTurrets = new Dictionary<Pawn, Building>();
        public List<Pawn> assistingMinds = new List<Pawn>();
        public List<Pawn> replicatingMinds = new List<Pawn>();
        public Dictionary<Pawn, int> inMentalBreak = new Dictionary<Pawn, int>();

        public List<Pawn> controlledTurretsKeys = new List<Pawn>();
        public List<Building> controlledTurretsValues = new List<Building>();

        public List<Pawn> inMentalBreakKeys = new List<Pawn>();
        public List<int> inMentalBreakValues = new List<int>();

        public int SID = -1;

        public int bootGT = -2;
    }
}