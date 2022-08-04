using System;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using Verse.Sound;
using RimWorld.Planet;

namespace MOARANDROIDS
{
    public class CompSkyCloudCore : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look<int>(ref SID, "ATPP_SID", -1);
            Scribe_Values.Look<int>(ref bootGT, "ATPP_bootGT", -2);
            Scribe_Values.Look<bool>(ref isKidnapped, "ATPP_isKidnapped", false);
            Scribe_Values.Look<int>(ref KidnappedPendingDisconnectionGT, "ATPP_KidnappedPendingDisconnectionGT", -1);
            
            Scribe_Collections.Look(ref this.storedMinds, false, "ATPP_storedMinds", LookMode.Deep);
            Scribe_Collections.Look(ref this.assistingMinds, false, "ATPP_assistingMinds", LookMode.Reference);
            Scribe_Collections.Look(ref this.replicatingMinds, false, "ATPP_replicatingMinds", LookMode.Reference);
            Scribe_Collections.Look(ref this.pendingUploads, false, "ATPP_pendingUploads", LookMode.Reference);

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
                CompSurrogateOwner cso = Utils.getCachedCSO(p);

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

            //Allow to reset kidnapped state if the M8 come back (spawn with isKinapped enabled but not Kidnapped infact)
            if (isKidnapped)
            {
                if (parent is Pawn)
                {
                    Pawn cp = (Pawn)parent;

                    if (!cp.IsKidnapped())
                    {
                        isKidnapped = false;
                        KidnappedPendingDisconnectionGT = -1;
                        init = false;
                    }
                }
            }

            generateInitialSurvivorsMinds();

            if(!init)
                initComp();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            //If despawn but it's an M8Mech host and he is alive then minds can continue their activities 
            if (parentPawn == null || parentPawn.Dead)
            {
                stopAllMindsActivities();
                Utils.GCATPP.popSkyCloudCore(this.parent);
                Utils.GCATPP.popSkyCloudCoreAbs(this.parent);
            }
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
                Find.LetterStack.ReceiveLetter("ATPP_destroyedMindsDueToDestroyedSkyCloudCore".Translate(storedMinds.Count), "ATPP_destroyedMindsDueToDestroyedSkyCloudCoreDesc".Translate(getName(), storedMinds.Count), LetterDefOf.ThreatBig);

                foreach (var p in storedMinds)
                {
                    p.Kill(null, null);
                }
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);

            switch (signal)
            {
                case "PowerTurnedOn":
                    //Definition sec ou le core démarrera vraiment
                    bootGT = Find.TickManager.TicksGame + (Settings.secToBootSkyCloudCore * 60);
                    break;
                case "PowerTurnedOff":
                    //Su systeme booté le serveur dit le power Failure
                    if (bootGT == -1)
                        Utils.playVocal("soundDefSkyCloudPowerFailure");

                    bootGT = -2;
                    stopAllMindsActivities(true);
                    Utils.GCATPP.popSkyCloudCore(parent);
                    break;
                case "AndroidTiers_CaravanInit":
                    if (parent is Pawn)
                    {
                        Pawn cp = (Pawn)parent;
                            if (!init)
                                initComp();
                    }
                    break;
            }
        }

        public void generateInitialSurvivorsMinds()
        {
            //If scenario apocalypse then generate 3 human conscioussness
            if (Current.Game.tickManager.TicksGame == 0 && Current.Game.Scenario.name == "Androids apocalypse")
            {
                Pawn cp = (Pawn)parent;
                //Set the initial M8 damage
                Hediff he = cp.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.DecayedFrame);
                if(he == null)
                    cp.health.AddHediff(HediffDefOf.DecayedFrame);

                Pawn mind;
                CompSurrogateOwner cso;
                PawnGenerationRequest pgr = new PawnGenerationRequest(PawnKindDefOf.AncientSoldier, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, newborn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, colonistRelationChanceFactor:1f, forceAddFreeWarmLayerIfNeeded: true, allowGay: true, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, biocodeWeaponChance: 0f, forceNoBackstory:false);
                //Generate the 3 humans minds
                for (int i = 0; i != 3; i++)
                {
                    mind = PawnGenerator.GeneratePawn(pgr);
                    setInitialChildhood(mind, "ApocalypseSurvivor23");
                    mind.story.adulthood = null;
                    cso = Utils.getCachedCSO(mind);
                    cso.skyCloudHost = parent;
                    Utils.ResetCachedIncapableOf(mind);
                    Utils.removeMindBlacklistedTrait(mind);
                    Current.Game.tickManager.DeRegisterAllTickabilityFor(mind);
                    storedMinds.Add(mind);
                }

                //Generate the defense Bot
                pgr = new PawnGenerationRequest(PawnKindDefOf.AndroidT2Colonist, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: true, newborn: false, allowDead: false, allowDowned: false, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, colonistRelationChanceFactor: 1f, forceAddFreeWarmLayerIfNeeded: true, allowGay: true, allowFood: false, allowAddictions: false, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, biocodeWeaponChance: 0f, forceNoBackstory: false);
                mind = PawnGenerator.GeneratePawn(pgr);
                setInitialChildhood(mind,"");
                mind.story.adulthood = null;
                cso = Utils.getCachedCSO(mind);
                cso.skyCloudHost = parent;
                Utils.ResetCachedIncapableOf(mind);
                Utils.removeMindBlacklistedTrait(mind);
                int skillbot = Rand.Range(8, 15);

                foreach (var sk in DefDatabase<SkillDef>.AllDefs)
                {
                    SkillRecord sr = mind.skills.GetSkill(sk);
                    if (sr != null)
                    {
                        if (sk == SkillDefOf.Shooting || sk == SkillDefOf.Melee)
                        {
                            sr.Level = skillbot;
                        }
                        else
                        {
                            sr.Level = 0;
                            sr.passion = Passion.None;
                            sr.levelInt = 0;
                            sr.xpSinceMidnight = 0;
                            sr.xpSinceLastLevel = 0;
                        }
                    }
                }
                //Basic robot so simple minded trait
                if(!mind.story.traits.HasTrait(TraitDefOf.SimpleMindedAndroid))
                    mind.story.traits.GainTrait(new Trait(TraitDefOf.SimpleMindedAndroid, 0, true));

                mind.Name = new NameTriple("", "CERBERUS-BOT", "");

                Current.Game.tickManager.DeRegisterAllTickabilityFor(mind);
                storedMinds.Add(mind);

                //Unit talk about the fact there is physical damages
                forceIntegrityWarning = 5;
            }
        }

        private void setInitialChildhood(Pawn mind, string id)
        {
            Backstory bs = null;
            BackstoryDatabase.TryGetWithIdentifier(id, out bs);
            if (bs != null)
                mind.story.childhood = bs;
        }

        public void initComp()
        {
            init = true;
            initSelfReference();

            if (parentBuilding != null)
            {
                parentCPT = Utils.getCachedCPT((Building)parent);
            }

            if (SID == -1)
            {
                SID = Utils.GCATPP.getNextSkyCloudID();
                Utils.GCATPP.incNextSkyCloudID();
            }

            if (!isKidnapped)
            {
                if (Booted())
                {
                    Utils.GCATPP.pushSkyCloudCore(parent);
                }

                Utils.GCATPP.pushSkyCloudCoreAbs(parent);
            }


            //Application retroactive de la surppression de traits blacklistés pour les minds
            foreach (var m in storedMinds)
            {
                Utils.removeMindBlacklistedTrait(m);
                Current.Game.tickManager.DeRegisterAllTickabilityFor(m);
            }
        }

        public bool isFull()
        {
            if (parentBuilding != null)
                return false;
            else
                return (10 - nbMindsAbsolute() <= 0);
        }

        public int nbMindsAbsolute()
        {
            return (storedMinds.Count + replicatingMinds.Count + pendingUploads.Count);
        }

        public void initSelfReference()
        {
            if (parentBuilding == null && parentPawn == null)
            {
                if (parent is Building)
                    parentBuilding = (Building)parent;
                else
                    parentPawn = (Pawn)parent;
            }
        }

        public bool isOnline()
        {
            initSelfReference();

            if (parentBuilding != null)
            {
                return !parent.Destroyed && parentCPT.PowerOn;
            }
            else
            {
                //PowerOn if M8 not dead And not kidnapped Or kidnapped but within the exception time
                return !parentPawn.Dead && (!isKidnapped || (KidnappedPendingDisconnectionGT != -1));
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            //If no minds stored
            if (storedMinds.Count == 0 || !isOnline() || !Booted())
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
                            
                            if(parentPawn == null)
                                Utils.playVocal("soundDefSkyCloudMindDeletionCompleted");
                            else
                                Utils.playVocal("soundDefM8MindDeletionCompleted");

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
                        CompSurrogateOwner cso = Utils.getCachedCSO(p);
                        if (cso == null)
                            return;

                        int GT = Find.TickManager.TicksGame;

                        if (isFull())
                        {
                            Messages.Message("ATPP_ProcessDuplicateFailed".Translate(p.LabelShortCap), parent, MessageTypeDefOf.NegativeEvent);
                        }
                        else
                        {
                            cso.replicationStartGT = GT;
                            cso.replicationEndingGT = GT + (Settings.mindReplicationHours * 2500);

                            replicatingMinds.Add(p);
                            stopMindActivities(p);

                            Messages.Message("ATPP_ProcessDuplicateOK".Translate(p.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);
                        }

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

                                Utils.GCATPP.checkAssistingMindsBonus();

                                if (nb > 0)
                                    Messages.Message("ATPP_ProcessMassAssist".Translate(nb), parent, MessageTypeDefOf.PositiveEvent);

                            }, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }

                        showFloatMenuMindsStored(delegate (Pawn p)
                        {
                            stopMindActivities(p);
                            assistingMinds.Add(p);

                            Utils.GCATPP.checkAssistingMindsBonus();

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
                        Utils.ShowFloatMenuSkyCloudCores(delegate (Thing core)
                        {
                            CompSurrogateOwner cso = Utils.getCachedCSO(p);
                            stopMindActivities(p);
                            cso.startMigration( core);
                        }, parent);
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
                                CompSurrogateOwner cso = Utils.getCachedCSO(p);
                                if (cso == null)
                                    return;

                                if (!Utils.GCATPP.isConnectedToSkyMind(sSX, true))
                                {
                                    Messages.Message("ATPP_CannotConnectToSkyMind".Translate(), parent, MessageTypeDefOf.NegativeEvent);
                                    return;
                                }
                                cso.setControlledSurrogate(sSX);
                            });
                        }, true, true, false, false, null, true);
                    }
                };
            }

            if (getNbMindsConnectedToSurrogate() != 0 || controlledTurrets.Count != 0)
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
                            if(parentPawn == null)
                                Utils.playVocal("soundDefSkyCloudAllMindDisconnected");
                            else
                                Utils.playVocal("soundDefM8AllMindDisconnected");
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));

                        showFloatMenuMindsStored(delegate (Pawn p)
                        {
                            CompSurrogateOwner cso = Utils.getCachedCSO(p);
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
            StringBuilder ret = new StringBuilder();

            if (parent.Map == null)
                return base.CompInspectStringExtra();

            string limitStorage = "";
            if (parentPawn != null)
                limitStorage = "/10";

            ret.AppendLine(getName())
               .AppendLine("ATPP_CentralCoreNbStoredMind".Translate(storedMinds.Count+limitStorage))
               .AppendLine("ATPP_CentralCoreNbAssistingMinds".Translate(assistingMinds.Count));

            if(isOnline())
            {

                if (!Booted())
                {
                    ret.Append("ATPP_SkyCloudCoreBooting".Translate((int)Math.Max(0, bootGT - Find.TickManager.TicksGame).TicksToSeconds()));
                }
                else
                {

                    //Check migration en cours de mind
                    foreach (var m in storedMinds)
                    {
                        CompSurrogateOwner cso = Utils.getCachedCSO(m);

                        if (cso == null)
                            continue;

                        if (cso.replicationEndingGT != -1)
                        {
                            float p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - cso.replicationStartGT) / (float)(cso.replicationEndingGT - cso.replicationStartGT));

                            ret.Append("=>").AppendLine("ATPP_CentralCoreReplicationInProgress".Translate(m.LabelShortCap, ((int)(p * (float)100)).ToString()));
                        }
                        //ATPP_CentralCoreReplicationInProgress

                        else if (cso.migrationEndingGT != -1 && cso.migrationSkyCloudHostDest != null)
                        {
                            CompSkyCloudCore csc2 = Utils.getCachedCSC(cso.migrationSkyCloudHostDest);
                            float p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - cso.migrationStartGT) / (float)(cso.migrationEndingGT - cso.migrationStartGT));

                            ret.Append("=>").AppendLine("ATPP_CentralCoreMigrationInProgress".Translate(m.LabelShortCap, csc2.getName(), ((int)(p * (float)100)).ToString()));
                        }

                        else if (inMentalBreak.ContainsKey(m))
                        {
                            ret.Append("=>").AppendLine("ATPP_CentralCoreProcessInMentalBreak".Translate(m.LabelShortCap));
                        }
                    }
                }
            }

            return ret.TrimEnd().Append(base.CompInspectStringExtra()).ToString();
        }

        public bool Booted()
        {
            if (parentPawn != null)
                return !parentPawn.Dead;
            else
                return bootGT == -1;
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            //Allow init of the core if pawn SkyCore and is in an caravan 
            if (!init)
                initComp();

            //Kidnapped M8 ? no more interpretation to minds
            if (parentPawn != null && isKidnapped)
            {
                //Check if we need to proceed to the disconnection
                if (KidnappedPendingDisconnectionGT != -1 && KidnappedPendingDisconnectionGT <= Find.TickManager.TicksGame)
                {
                    KidnappedPendingDisconnectionGT = -1;
                    //Stop all minds activities
                    stopAllMindsActivities();
                    //Stop skymind abilities
                    Utils.GCATPP.popSkyMindServer(parent);

                    Find.LetterStack.ReceiveLetter("ATPP_LetterKidnappedM8Disconnected".Translate(), "ATPP_LetterKidnappedM8DisconnectedDesc".Translate(parentPawn.LabelCap, getName()), LetterDefOf.NegativeEvent);
                }

                return;
            }

            //Minds processing stuff
            checkMindsStuffTick();
            checkCoreHealthStatus();
        }

        public override void CompTick()
        {
            if (!this.parent.Spawned)
            {
                return;
            }

            int CGT = Find.TickManager.TicksGame;
            if(CGT % 120 == 0)
            {
                //Rafraichissement qt de courant consommé
                refreshPowerConsumed();

                //Check si demarrage du core
                if (parentBuilding != null && bootGT > 0 && bootGT < CGT)
                {
                    //On rend accessible les controles
                    bootGT = -1;

                    if (parentCPT.PowerOn)
                        Utils.GCATPP.pushSkyCloudCore(parent);

                    //ATPP_SkyCloudCoreBooted
                    Find.LetterStack.ReceiveLetter("ATPP_SkyCloudCoreBooted".Translate(getName()), "ATPP_SkyCloudCoreBootedDesc".Translate(getName()), LetterDefOf.NeutralEvent, parent);

                    //Play sound
                    Utils.playVocal("soundDefSkyCloudPrimarySystemsOnline");
                }
            }

            if(parentBuilding != null && CGT % 250 == 0)
            {
                checkMindsStuffTick();
                checkCoreHealthStatus();
            }
        }

        public void checkCoreHealthStatus()
        {
            int CGT = Find.TickManager.TicksGame;
            if (CGT >= nextCoreHealthWarningGT)
            {
                if (parentPawn != null)
                {
                    if (forceIntegrityWarning != -1)
                        forceIntegrityWarning--;

                    if (forceIntegrityWarning == 0 || (!parentPawn.Dead && parentPawn.health.summaryHealth.SummaryHealthPercent < 0.55f))
                    {
                        forceIntegrityWarning = -1;
                        nextCoreHealthWarningGT = CGT + Rand.Range(2700, 5400);
                        Utils.playVocal("soundDefM8IntegrityCompromised");
                    }
                }
                else
                {
                    float num = (float)parentBuilding.HitPoints / (float)parentBuilding.MaxHitPoints;
                    if (num < 0.55f)
                    {
                        nextCoreHealthWarningGT = CGT + Rand.Range(2700, 5400);
                        Utils.playVocal("soundDefSkyCoreIntegrityCompromised");
                    }
                }
            }
        }

        public void checkMindsStuffTick()
        {
            int CGT = Find.TickManager.TicksGame;
            for (int i = storedMinds.Count - 1; i >= 0; i--)
            {
                Pawn m = storedMinds[i];
                CompSurrogateOwner cso = Utils.getCachedCSO(m);
                if (cso == null)
                    continue;

                cso.checkInterruptedUpload();

                //Atteinte fin attente de la replication d'un mind
                if (cso.replicationEndingGT != -1 && cso.replicationEndingGT < CGT)
                    replicationDone.Add(cso);

                if (cso.migrationEndingGT != -1 && cso.migrationEndingGT < CGT)
                    migrationsDone.Add(cso);
            }

            if (migrationsDone.Count != 0)
            {
                foreach (var md in migrationsDone)
                {
                    md.OnMigrated();
                }
                migrationsDone.Clear();
                if (parentPawn == null)
                    Utils.playVocal("soundDefSkyCloudMindMigrationCompleted");
                else
                    Utils.playVocal("soundDefM8MindMigrationCompleted");
            }
            if (replicationDone.Count != 0)
            {
                foreach (var md in replicationDone)
                {
                    md.OnReplicate();
                }
                replicationDone.Clear();
                if (parentPawn == null)
                    Utils.playVocal("soundDefSkyCloudMindReplicationCompleted");
                else
                    Utils.playVocal("soundDefM8MindReplicationCompleted");
            }

            //CHECK of the end of the mental breaks of the stored minds - decrementation time
            if (isOnline() && inMentalBreak.Count > 0)
            {
                var keys = new List<Pawn>(inMentalBreak.Keys);
                foreach (var ck in keys)
                {
                    inMentalBreak[ck] -= 3600;
                    if (inMentalBreak[ck] <= 0)
                    {
                        inMentalBreak.Remove(ck);

                        Messages.Message("ATPP_ProcessNoLongerInMentalBreak".Translate(ck.LabelShortCap, getName()), parent, MessageTypeDefOf.PositiveEvent);
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
                CompAndroidState cas = Utils.getCachedCAS(mind);
                if (cas == null || cas.surrogateController == null)
                    return;

                mind = cas.surrogateController;
            }

            //Arret des activitées
            stopMindActivities(mind);

            inMentalBreak[mind] = Rand.Range(Settings.minDurationMentalBreakOfDigitisedMinds, Settings.maxDurationMentalBreakOfDigitisedMinds) * 2500;
            if(parentPawn == null)
                Utils.playVocal("soundDefSkyCloudMindQuarantineMentalState");
            else
                Utils.playVocal("soundDefM8MindQuarantineMentalState");
        }

        public int getNbMindsConnectedToSurrogate()
        {
            int ret = 0;
            foreach (var m in storedMinds)
            {
                CompSurrogateOwner cso = Utils.getCachedCSO(m);
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
            CompSurrogateOwner cso = Utils.getCachedCSO(mind);
            if (cso != null && cso.isThereSX())
            {
                cso.disconnectControlledSurrogate(null);
            }

            if (!serverShutdown && assistingMinds.Contains(mind))
                assistingMinds.Remove(mind);
        }

        public bool mindIsBusy(Pawn mind)
        {
            CompSurrogateOwner cso = Utils.getCachedCSO(mind);
            return controlledTurrets.ContainsKey(mind) || replicatingMinds.Contains(mind) || inMentalBreak.ContainsKey(mind) || assistingMinds.Contains(mind) || (cso != null && cso.migrationEndingGT != -1);
        }

        public bool isRunning()
        {
            if (parentPawn != null)
                return !parentPawn.Dead;
            else
                return !parentBuilding.Destroyed && !parentBuilding.IsBrokenDown() && parentCPT.PowerOn;
        }

        public string getName()
        {
            return "Core-" + SID;
        }

        public float getPowerConsumed()
        {
            CompPowerTrader cpt = Utils.getCachedCPT((Building)this.parent);
            return (storedMinds.Count*Settings.powerConsumedByStoredMind) + cpt.Props.basePowerConsumption;
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
            if(parentBuilding != null)
                parentCPT.powerOutputInt = -(getPowerConsumed());
        }


        public void setRemotelyControlledTurret(Pawn mind, Building turret)
        {
            if (!storedMinds.Contains(mind) || controlledTurrets.ContainsKey(mind))
                return;

            CompRemotelyControlledTurret crt = turret.TryGetComp<CompRemotelyControlledTurret>();
            crt.controller = mind;
            controlledTurrets[mind] = turret;

            SoundDefOfAT.ATPP_SoundTurretConnection.PlayOneShot(null);
            FleckMaker.ThrowDustPuffThick(turret.Position.ToVector3Shifted(), turret.Map, 4.0f, Color.blue);

            Messages.Message("ATPP_SurrogateConnectionOK".Translate(mind.LabelShortCap, turret.LabelShortCap), turret, MessageTypeDefOf.PositiveEvent);
        }

        public void setMindInReplicationModeOn(Pawn m)
        {
            if (storedMinds.Contains(m) && !replicatingMinds.Contains(m))
            {
                stopMindActivities(m);
                replicatingMinds.Add(m);
            }
        }

        public void setMindInReplicationModeOff(Pawn m)
        {
            if (storedMinds.Contains(m) && replicatingMinds.Contains(m))
            {
                replicatingMinds.Remove(m);
            }
        }

        public void stopRemotelyControlledTurret(Pawn mind)
        {
            if (!controlledTurrets.ContainsKey(mind))
                return;

            CompRemotelyControlledTurret crt = controlledTurrets[mind].TryGetComp<CompRemotelyControlledTurret>();
            crt.controller =null;

            controlledTurrets.Remove(mind);
            SoundDefOfAT.ATPP_SoundTurretDisconnect.PlayOneShot(null);
        }

        private void disconnectAllRemotelyControlledTurrets()
        {
            foreach(var e in controlledTurrets)
            {
                stopRemotelyControlledTurret(e.Key);
            }
        }

        private void disconnectAllSurrogates()
        {
            foreach (var m in storedMinds)
            {
                CompSurrogateOwner cso = Utils.getCachedCSO(m);
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
                opts.AddRange(supOpts);
            }

            //Listing des SkyCloud Cores
            for (int i = storedMinds.Count-1; i >= 0; i--)
            {
                Pawn m = storedMinds[i];
                //Log.Message("Suspended => " +m.LabelCap+" "+ m.Suspended);
                CompSurrogateOwner cso = Utils.getCachedCSO(m);
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

        Pawn parentPawn;
        Building parentBuilding;

        CompPowerTrader parentCPT;
        List<CompSurrogateOwner> migrationsDone = new List<CompSurrogateOwner>();
        List<CompSurrogateOwner> replicationDone = new List<CompSurrogateOwner>();

        public List<Pawn> storedMinds = new List<Pawn>();
        public Dictionary<Pawn, Building> controlledTurrets = new Dictionary<Pawn, Building>();
        public List<Pawn> assistingMinds = new List<Pawn>();
        public List<Pawn> replicatingMinds = new List<Pawn>();
        public List<Pawn> pendingUploads = new List<Pawn>();
        public Dictionary<Pawn, int> inMentalBreak = new Dictionary<Pawn, int>();

        public List<Pawn> controlledTurretsKeys = new List<Pawn>();
        public List<Building> controlledTurretsValues = new List<Building>();

        public List<Pawn> inMentalBreakKeys = new List<Pawn>();
        public List<int> inMentalBreakValues = new List<int>();

        public int SID = -1;

        public int bootGT = -2;
        private int nextCoreHealthWarningGT = -1;
        public bool init = false;

        public int forceIntegrityWarning = -1;
        public bool isKidnapped = false;
        public int KidnappedPendingDisconnectionGT = -1;
    }
}