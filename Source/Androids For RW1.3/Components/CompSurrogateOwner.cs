﻿using System;
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
using RimWorld.Planet;
using System.Text.RegularExpressions;

namespace MOARANDROIDS
{
    public class CompSurrogateOwner : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look<bool>(ref this.controlMode, "ATPP_controlMode", false);
            Scribe_References.Look(ref SX, "ATPP_SX");
            Scribe_References.Look(ref permuteRecipient, "ATPP_permuteRecipient");
            Scribe_References.Look(ref duplicateRecipient, "ATPP_duplicateRecipient");
            Scribe_References.Look(ref skyCloudRecipient, "ATPP_skyCloudRecipient");
            Scribe_References.Look(ref skyCloudDownloadRecipient, "ATPP_skyCloudDownloadRecipient");

            Scribe_Values.Look<bool>(ref this.repairAndroids, "ATPP_repairAndroids", false);

            Scribe_Values.Look<int>(ref this.migrationEndingGT, "ATPP_migrationEndingGT", -1);
            Scribe_Values.Look<int>(ref this.migrationStartGT, "ATPP_migrationStartGT", 0);
            Scribe_References.Look(ref migrationSkyCloudHostDest, "ATPP_migrationSkyCloudHostDest");


            Scribe_Values.Look<int>(ref this.mindAbsorptionEndingGT, "ATPP_mindAbsorptionEndingGT", -1);
            Scribe_Values.Look<int>(ref this.mindAbsorptionStartGT, "ATPP_mindAbsorptionStartGT", 0);

            Scribe_Values.Look<int>(ref this.downloadFromSkyCloudEndingGT, "ATPP_downloadFromSkyCloudEndingGT", -1);
            Scribe_Values.Look<int>(ref this.downloadFromSkyCloudStartGT, "ATPP_downloadFromSkyCloudStartGT", 0);
            Scribe_Values.Look<int>(ref this.uploadToSkyCloudEndingGT, "ATPP_uploadToSkyCloudEndingGT", -1);
            Scribe_Values.Look<int>(ref this.uploadToSkyCloudStartGT, "ATPP_uploadToSkyCloudStartGT", 0);
            Scribe_Values.Look<int>(ref this.permuteEndingGT, "ATPP_permuteEndingGT", -1);
            Scribe_Values.Look<int>(ref this.permuteStartGT, "ATPP_permuteStartGT", 0);
            Scribe_Values.Look<int>(ref this.duplicateEndingGT, "ATPP_duplicateEndingGT", -1);
            Scribe_Values.Look<int>(ref this.duplicateStartGT, "ATPP_duplicateStartGT", 0);
            Scribe_Values.Look<bool>(ref this.showPermuteProgress, "ATPP_showPermuteProgress", false);
            Scribe_Values.Look<bool>(ref this.showDuplicateProgress, "ATPP_showDuplicateProgress", false);

            Scribe_Values.Look<int>(ref this.replicationStartGT, "ATPP_replicationStartGT", 0);
            Scribe_Values.Look<int>(ref this.replicationEndingGT, "ATPP_replicationEndingGT", -1);



            Scribe_References.Look(ref skyCloudHost, "ATPP_skyCloudHost");
            Scribe_Defs.Look(ref this.ransomwareSkillStolen, "ATPP_ransomwareSkillStolen");
            Scribe_Values.Look<int>(ref this.ransomwareSkillValue, "ATPP_ransomwareSkillValue", -1);
            Scribe_Defs.Look(ref ransomwareTraitAdded, "ATPP_ransomwareTraitAdded");
            Scribe_Values.Look<bool>(ref this.externalController, "ATPP_CSIOExternalController", false);
            Scribe_Collections.Look(ref savedSkillsBecauseM7Control, "ATPP_savedSkillsBecauseM7Control", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref savedWorkAffectationBecauseM7Control, "ATPP_savedWorkAffectationBecauseM7Control", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref this.extraSX, false, "ATPP_extraSX", LookMode.Reference);
            Scribe_Values.Look<bool>(ref this.lastSkymindDisconnectIsManual, "ATPP_lastSkymindDisconnectIsManualCSO", false);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (extraSX == null)
                    extraSX = new List<Pawn>();

                //Reconsitution du tableau de SX virtuelle (fusion extraSX et SX)
                if (SX != null)
                    availableSX.Add(SX);
                if(extraSX.Count >0)
                    availableSX.AddRange(extraSX);

                availableSX.RemoveAll(item => item == null);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            currentPawn = (Pawn)parent;
            currentCAS = Utils.getCachedCAS(currentPawn);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (parent.Map == null || !parent.Spawned)
                return;

            int GT = Find.TickManager.TicksGame;

            if(GT % 600 == 0)
            {
                if (currentPawn.IsKidnapped())
                {
                    Utils.GCATPP.disconnectUser(currentPawn);
                }
            }

            if (GT % 180 == 0 && ( permuteEndingGT != -1 || permuteRecipient != null
                                   || duplicateEndingGT != -1 || duplicateRecipient != null 
                                   || uploadToSkyCloudEndingGT != -1 || migrationEndingGT != -1 
                                   || mindAbsorptionEndingGT != -1 || downloadFromSkyCloudEndingGT != -1 
                                   || (controlMode && SX != null)))
            {
                checkInterruptedUpload();

                //Atteinte d'un chargement de permutation de conscience
                if (permuteEndingGT != -1 && permuteEndingGT < GT)
                {
                    checkInterruptedUpload();
                    if (permuteEndingGT == -1)
                        return;
                    permuteEndingGT = -1;
                    CompSurrogateOwner cso = Utils.getCachedCSO(permuteRecipient);
                    cso.permuteEndingGT = -1;

                    Utils.removeUploadHediff(currentPawn, permuteRecipient);

                    Find.LetterStack.ReceiveLetter("ATPP_LetterPermuteOK".Translate(), "ATPP_LetterPermuteOKDesc".Translate(currentPawn.LabelShortCap, permuteRecipient.LabelShortCap), LetterDefOf.PositiveEvent, parent);
                    //On realise effectivement la permutation
                    Utils.PermutePawn(currentPawn, permuteRecipient);

                    //Clear du status de blank andorid si destinataire blank android
                    Utils.clearBlankAndroid(permuteRecipient);

                    //Si destinataire T1 rajout simpleMinded
                    if (permuteRecipient.def.defName == Utils.T1)
                        Utils.addSimpleMindedTraitForT1(permuteRecipient);
                    else
                        Utils.removeSimpleMindedTrait(permuteRecipient);

                    //Si source T1 rajout simpleMinded
                    if (currentPawn.def.defName == Utils.T1)
                        Utils.addSimpleMindedTraitForT1(currentPawn);
                    else
                        Utils.removeSimpleMindedTrait(currentPawn);

                    if(currentPawn.IsPrisoner || permuteRecipient.IsPrisoner)
                        dealWithPrisonerRecipientPermute(currentPawn, permuteRecipient);

                    resetUploadStuff();
                }

                //Atteinte d'un chargement de duplication de conscience
                if (duplicateEndingGT != -1 && duplicateEndingGT < GT)
                {
                    checkInterruptedUpload();
                    if (duplicateEndingGT == -1)
                        return;
                    duplicateEndingGT = -1;
                    Utils.getCachedCSO(duplicateRecipient).duplicateEndingGT = -1;

                    Utils.removeUploadHediff(currentPawn, duplicateRecipient);

                    Find.LetterStack.ReceiveLetter("ATPP_LetterDuplicateOK".Translate(), "ATPP_LetterDuplicateOKDesc".Translate(currentPawn.LabelShortCap, duplicateRecipient.LabelShortCap), LetterDefOf.PositiveEvent, parent);
                    //On realise effectivement la permutation
                    Utils.Duplicate(currentPawn, duplicateRecipient);

                    //Clear du status de blank andorid si destinataire blank android
                    Utils.clearBlankAndroid(duplicateRecipient);

                    //Si destinataire T1 rajout simpleMinded
                    if (duplicateRecipient.def.defName == Utils.T1)
                        Utils.addSimpleMindedTraitForT1(duplicateRecipient);
                    else
                        Utils.removeSimpleMindedTrait(duplicateRecipient);

                    
                    dealWithPrisonerRecipient(currentPawn, duplicateRecipient);
                    

                    resetUploadStuff();
                }

                //Atteinte d'un chargement de upload vers SkyCloud
                if (uploadToSkyCloudEndingGT != -1 && uploadToSkyCloudEndingGT < GT)
                {
                    checkInterruptedUpload();
                    if (uploadToSkyCloudEndingGT == -1)
                        return;
                    uploadToSkyCloudEndingGT = -1;
                    Utils.removeUploadHediff(currentPawn, null);

                    CompSkyCloudCore csc = Utils.getCachedCSC(skyCloudRecipient);

                    Find.LetterStack.ReceiveLetter("ATPP_LetterSkyCloudUploadOK".Translate(), "ATPP_LetterSkyCloudUploadOKDesc".Translate(currentPawn.LabelShortCap, csc.getName()), LetterDefOf.PositiveEvent, parent);
                    //On realise effectivement l'upload vers le Core

                    //Stockage pawn actuel
                    csc.storedMinds.Add(currentPawn);
                    Current.Game.tickManager.DeRegisterAllTickabilityFor(currentPawn);

                    //Suppression traits blacklistés pour les esprits
                    Utils.removeMindBlacklistedTrait(currentPawn);

                    //Si corp d'origine été un T1 alors on supprime le simpleminded le cas echeant
                    Utils.removeSimpleMindedTrait(currentPawn);

                    //Copie du corps du pawn et placement sur la carte
                    bool killMode = false;
                    if (Settings.skyCloudUploadModeForSourceMind == 2)
                        killMode = true;

                    Pawn corpse = Utils.spawnCorpseCopy(currentPawn, killMode);

                    //Reconfiguration en mode VX0 auto
                    if(Settings.skyCloudUploadModeForSourceMind == 1)
                    {
                        //On retire la VX2/VX3
                        foreach(var he in corpse.health.hediffSet.hediffs.ToList())
                        {
                            if (he.def == HediffDefOf.ATPP_HediffVX2Chip || he.def == HediffDefOf.ATPP_HediffVX3Chip)
                                corpse.health.RemoveHediff(he);
                        }

                        BodyPartRecord bpr = null;
                        bpr = corpse.health.hediffSet.GetBrain();

                        //On ajoute une VX0 en prevenant l'apparition de mauvaises pensées
                        Utils.preventVX0Thought = true;
                        corpse.health.AddHediff(HediffDefOf.ATPP_HediffVX0Chip, bpr);
                        Utils.preventVX0Thought = false;
                    }


                    //Deconnection de skymind le cas echeant
                    Utils.GCATPP.disconnectUser(currentPawn);

                    //Despawn du pawn numérisé
                    currentPawn.DeSpawn();

                    skyCloudHost = skyCloudRecipient;

                    //Simulation mort du corp en générant un corp identique

                    Utils.playVocal("soundDefSkyCloudMindDownloadCompleted");

                    resetUploadStuff();
                }

                //Atteinte d'un chargement de download depuis SkyCloud
                if (downloadFromSkyCloudEndingGT != -1 && downloadFromSkyCloudEndingGT < GT)
                {
                    checkInterruptedUpload();
                    if (downloadFromSkyCloudEndingGT == -1)
                        return;
                    downloadFromSkyCloudEndingGT = -1;
                    Utils.removeUploadHediff(currentPawn, null);

                    CompSurrogateOwner cso = Utils.getCachedCSO(skyCloudDownloadRecipient);
                    if (cso.skyCloudHost == null)
                        return;

                    CompSkyCloudCore csc = Utils.getCachedCSC(cso.skyCloudHost);

                    //Arret des jobs d'un esprit
                    csc.setMindInReplicationModeOff(skyCloudDownloadRecipient);


                    Find.LetterStack.ReceiveLetter("ATPP_LetterSkyCloudDownloadOK".Translate(), "ATPP_LetterSkyCloudDownloadOKDesc".Translate(skyCloudDownloadRecipient.LabelShortCap, currentPawn.LabelShortCap), LetterDefOf.PositiveEvent, parent);
                    //On realise effectivement le download du core vers le cerveau
                    //Permutation
                    Utils.PermutePawn(skyCloudDownloadRecipient, currentPawn);

                    //Report du blankAndroid pour le flagger dans la routine de kill
                    CompAndroidState cas = Utils.getCachedCAS(skyCloudDownloadRecipient);
                    if(cas != null)
                        cas.isBlankAndroid = true;

                    //Clear du status de blank andorid si destinataire blank android
                    Utils.clearBlankAndroid(currentPawn);

                    //Si cpawn un T1 on lui ajoute le trait SimpleMinded
                    Utils.addSimpleMindedTraitForT1(currentPawn);

                    

                    //Suppression de la memoire du core de l'esprit téléchargé
                    csc.RemoveMind(skyCloudDownloadRecipient);

                    //Annulation status de prisonnier de l'esprit downloader
                    dealWithPrisonerRecipientPermute(currentPawn, skyCloudDownloadRecipient);

                    Utils.playVocal("soundDefSkyCloudMindUploadCompleted");

                    //On kill le destinataire permuté
                    skyCloudDownloadRecipient.Kill(null,null);

                    resetUploadStuff();
                }

                if(mindAbsorptionEndingGT != -1 && mindAbsorptionEndingGT < GT)
                {
                    //Calcul moyenne point de skill du prisonnier
                    int sum = 0;
                    int average = 0;
                    foreach (var sr in currentPawn.skills.skills)
                    {
                        sum += sr.levelInt;
                    }
                    average = (int)((float)sum / (float)currentPawn.skills.skills.Count());

                    int nbp = (int)((float)((float)average / (float)20) * Rand.Range(1000, 5000));

                    Utils.GCATPP.incSkillPoints(nbp);

                    Find.LetterStack.ReceiveLetter("ATPP_MindAbsorptionDone".Translate(), "ATPP_MindAbsorptionDoneDesc".Translate(currentPawn.LabelShortCap, nbp), LetterDefOf.PositiveEvent, currentPawn);

                    resetUploadStuff();

                    currentPawn.Kill(null, null);
                }
            }
        }

        public void dealWithPrisonerRecipient(Pawn cpawn, Pawn recipient)
        {
            //Si destinataire de la duplication prisonnier Et emetteur pas prisonier on enleve la condition 
            if (!cpawn.IsPrisoner && recipient.IsPrisoner)
            {
                if (recipient.Faction != Faction.OfPlayer)
                {
                    recipient.SetFaction(Faction.OfPlayer, null);
                }

                if (recipient.guest != null)
                {
                    recipient.guest.SetGuestStatus(null, GuestStatus.Guest);
                }
            }

            //SI destinataire de la duplication colon regular et  prisonnier 
            if (cpawn.IsPrisoner && !recipient.IsPrisoner)
            {
                if (recipient.Faction != cpawn.Faction)
                {
                    recipient.SetFaction(cpawn.Faction, null);
                }

                if (recipient.guest != null)
                {
                    if (cpawn.IsSlave)
                        recipient.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
                    else
                        recipient.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Prisoner);
                }
            }
        }

        public void dealWithPrisonerRecipientPermute(Pawn cpawn, Pawn recipient)
        {
            if (!cpawn.IsPrisoner && recipient.IsPrisoner)
            {
                Faction tmp = recipient.Faction;

                if (recipient.Faction != Faction.OfPlayer)
                    recipient.SetFaction(Faction.OfPlayer, null);

                if (recipient.guest != null)
                    recipient.guest.SetGuestStatus(null, GuestStatus.Guest);

                if (cpawn.Faction != tmp)
                    cpawn.SetFaction(tmp, null);

                if (cpawn.guest != null)
                {
                    if(cpawn.IsSlave)
                        cpawn.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
                    else
                        cpawn.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Prisoner);
                }

                if (recipient.workSettings != null)
                    recipient.workSettings.EnableAndInitialize();
            }
            else
            // Prisonner <=> COLON
            {
                Faction tmp = cpawn.Faction;

                if (cpawn.Faction != Faction.OfPlayer)
                    cpawn.SetFaction(Faction.OfPlayer, null);

                if (cpawn.guest != null)
                    cpawn.guest.SetGuestStatus(null, GuestStatus.Guest);

                if (recipient.Faction != tmp)
                    recipient.SetFaction(tmp, null);

                if (recipient.guest != null)
                {
                    if (cpawn.IsSlave)
                        recipient.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
                    else
                        recipient.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Prisoner);
                }

                if (cpawn.workSettings != null)
                    cpawn.workSettings.EnableAndInitialize();
            }
        }

        public void OnReplicate()
        {
            Pawn cpawn = (Pawn)parent;
            checkInterruptedUpload();
            if (replicationEndingGT == -1)
                return;
            replicationEndingGT = -1;
            CompSkyCloudCore csc = Utils.getCachedCSC(skyCloudHost);

            PawnGenerationRequest request = new PawnGenerationRequest(cpawn.kindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f, false, true, true, false, false, false, false, fixedBiologicalAge: cpawn.ageTracker.AgeBiologicalYearsFloat, fixedChronologicalAge: cpawn.ageTracker.AgeChronologicalYearsFloat, fixedGender: cpawn.gender, fixedMelanin: cpawn.story.melanin);
            Pawn clone = PawnGenerator.GeneratePawn(request);

            //surrogate.Name = new NameTriple("", "S" + prefix + SXVer + "-" + Utils.GCATPP.getNextSXID(SXVer), "");

            List<Hediff> list = clone.health.hediffSet.hediffs.FastToList();
            for (int i = 0; i < list.Count; i++)
            {
                Hediff h = list[i];
                clone.health.RemoveHediff(h);
            }

            CompSurrogateOwner cso = Utils.getCachedCSO(clone);
            cso.skyCloudHost = skyCloudHost;

            Utils.Duplicate(cpawn, clone, false);


            //Définition nouveau nom
            try
            {
                string baseName = clone.LabelShort;
                int start = 1;
                int tmp = 0;
                //Tentative pour touver dernier indice numerique 
                foreach (var m in csc.storedMinds)
                {
                    if (m.LabelShort.StartsWith(baseName))
                    {
                        var result = Regex.Match(m.LabelShort, @" \d+$", RegexOptions.RightToLeft);
                        if (result.Success)
                        {
                            int tmp2 = 0;
                            if (Int32.TryParse(result.Value, out tmp2))
                            {
                                if (tmp2 > tmp)
                                    tmp = tmp2;
                            }
                        }
                    }
                }

                if (tmp > 0)
                {
                    start = tmp + 1;
                    //On garde uniquement la partie sans chiffre terminal
                    int idx = baseName.LastIndexOf(' ');
                    if (idx != -1)
                    {
                        baseName = baseName.Substring(0, idx);
                    }
                }

                for (int i = start; true; i++)
                {
                    bool ok = true;
                    string destName = baseName + " " + i.ToString();
                    foreach (var m in csc.storedMinds)
                    {
                        if (m.LabelShort == destName)
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (ok)
                    {
                        NameTriple nt = (NameTriple)clone.Name;
                        clone.Name = new NameTriple(nt.First, destName, nt.Last);
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Message("[ATPP] ReplicateMind.SetNewName " + e.Message + " " + e.StackTrace);
            }

            csc.storedMinds.Add(clone);

            //On enleve le mind de la liste des minds en cours de replication
            csc.replicatingMinds.Remove(cpawn);

            Find.LetterStack.ReceiveLetter("ATPP_LetterSkyCloudReplicateOK".Translate(), "ATPP_LetterSkyCloudReplicateOKDesc".Translate(cpawn.LabelShortCap, Utils.getCachedCSC(skyCloudHost).getName()), LetterDefOf.PositiveEvent, parent);

            resetUploadStuff();
        }

        public void OnMigrated()
        {
            Pawn cpawn = (Pawn)parent;
            checkInterruptedUpload();
            if (migrationEndingGT == -1)
                return;
            migrationEndingGT = -1;

            CompSkyCloudCore csc = Utils.getCachedCSC(skyCloudHost);
            CompSkyCloudCore csc2 = Utils.getCachedCSC(migrationSkyCloudHostDest);


            csc.RemoveMind(cpawn);
            csc2.storedMinds.Add(cpawn);
            skyCloudHost = migrationSkyCloudHostDest;

            Find.LetterStack.ReceiveLetter("ATPP_LetterSkyCloudMigrateOK".Translate(), "ATPP_LetterSkyCloudMigrateOKDesc".Translate(cpawn.LabelShortCap, csc.getName(), csc2.getName()), LetterDefOf.PositiveEvent, skyCloudHost);
            resetUploadStuff();
        }

        public override void PostDraw()
        {
            Material avatar = null;
            Vector3 vector;

            if ( (permuteEndingGT != -1 || showPermuteProgress) || (duplicateEndingGT != -1 || showDuplicateProgress) || uploadToSkyCloudEndingGT != -1 || downloadFromSkyCloudEndingGT != -1)
                avatar = Tex.UploadInProgress;

            if (avatar != null)
            {
                vector = this.parent.TrueCenter();
                vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28125f;
                vector.z += 1.4f;
                vector.x += this.parent.def.size.x / 2;

                Graphics.DrawMesh(MeshPool.plane08, vector, Quaternion.identity, avatar, 0);
            }
        }


        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            if (controlMode && SX != null)
            {
                disconnectControlledSurrogate(null);
                controlMode = false;
            }

            Pawn cpawn = (Pawn)parent;

            Hediff he = cpawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_ConsciousnessUpload);
            if (he != null)
            {
                cpawn.health.hediffSet.hediffs.Remove(he);
                he.PostRemoved();
            }
            
            he = cpawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_InRemoteControlMode);
            if (he != null)
            {
                cpawn.health.hediffSet.hediffs.Remove(he);
                he.PostRemoved();
            }
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();

            if( (permuteEndingGT != -1 && permuteRecipient != null) || showPermuteProgress)
            {
                GenDraw.DrawLineBetween(parent.TrueCenter(), permuteRecipient.TrueCenter(), SimpleColor.Green);
            }

            if ((duplicateEndingGT != -1 && duplicateRecipient != null) || showDuplicateProgress)
            {
                GenDraw.DrawLineBetween(parent.TrueCenter(), duplicateRecipient.TrueCenter(), SimpleColor.Green);
            }

            //Dessin liaison trasfert vers SkyCloud
            if(uploadToSkyCloudEndingGT != -1 && skyCloudRecipient.Map == parent.Map)
            {
                GenDraw.DrawLineBetween(parent.TrueCenter(), skyCloudRecipient.TrueCenter(), SimpleColor.Green);
            }

            //Dessin liaison entre controlleur et son/Ses SX
            if (controlMode) {
                if (SX != null && SX.Map == parent.Map)
                {
                    GenDraw.DrawLineBetween(parent.TrueCenter(), SX.Position.ToVector3(), SimpleColor.Blue);
                }
                if (extraSX.Count > 0)
                {
                    foreach (var e in extraSX)
                    {
                        if (e.Map == parent.Map)
                        {
                            GenDraw.DrawLineBetween(parent.TrueCenter(), e.Position.ToVector3(), SimpleColor.Blue);
                        }
                }
                }
            }
        }

        public void toggleControlMode()
        {
            Pawn cpawn = (Pawn)parent;

            //Si dupication ou permutation en cours on empeche le mode control
            if (duplicateEndingGT != -1 || permuteEndingGT != -1)
            {
                controlMode = false;
                return;
            }

            controlMode = !controlMode;
            if (controlMode)
            {
                addRemoteControlHediff();
            }
            else
            {
                disconnectControlledSurrogate(null);
                removeRemoteControlHediff();
            }
        }

        private void addRemoteControlHediff()
        {
            Pawn cpawn = (Pawn)parent;
            cpawn.health.AddHediff(HediffDefOf.ATPP_InRemoteControlMode);
        }

        private void removeRemoteControlHediff()
        {
            Pawn cpawn = (Pawn)parent;
            Hediff he = cpawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_InRemoteControlMode);
            if (he != null)
                cpawn.health.RemoveHediff(he);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            bool isPrisonner = currentPawn.IsPrisoner;
            bool isBlankAndroid = (currentCAS != null && currentCAS.isBlankAndroid);

            //Si pas prisonier ET pas un surrogate non controlé && affecté au crafting
            if (!isPrisonner && !(currentCAS != null && currentCAS.isSurrogate && currentCAS.surrogateController == null) && currentPawn.workSettings != null && currentPawn.workSettings.WorkIsActive(Utils.WorkTypeDefSmithing) && Settings.androidsCanOnlyBeHealedByCrafter)
            {
                yield return new Command_Toggle
                {
                    icon = Tex.RepairAndroid,
                    defaultLabel = "ATPP_RepairAndroids".Translate(),
                    defaultDesc = "ATPP_RepairAndroidsDesc".Translate(),
                    isActive = (() => repairAndroids),
                    toggleAction = delegate ()
                    {
                        repairAndroids = !repairAndroids;
                    }
                };
            }

            //Possibilitée réservé uniquement aux pucés ET connectés aux SkyMind
            if (!currentPawn.VXChipPresent())
                yield break;

            //Les clones ne peuvent pas etre clone owner
            if (currentCAS != null && currentCAS.isSurrogate)
                yield break;

            if (!isPrisonner && !isBlankAndroid)
            {
                yield return new Command_Toggle
                {
                    icon = Tex.SurrogateMode,
                    defaultLabel = "ATPP_EnableSurrogateControlMode".Translate(),// "ATPP_UseBattery".Translate(),
                    defaultDesc = "ATPP_EnableSurrogateControlModeDesc".Translate(),//"ATPP_UseBatteryDesc".Translate(),
                    isActive = (() => controlMode),
                    toggleAction = delegate ()
                    {
                        toggleControlMode();
                    }
                };
            }

            if (!Utils.GCATPP.isConnectedToSkyMind(currentPawn))
                yield break;


            bool transfertAllowed = Utils.mindTransfertsAllowed((Pawn)parent);

            if (isPrisonner)
            {
                //AJout bouton d'absorption d'esprit
                //ATPP_MindAbsorption
                if (mindAbsorptionEndingGT == -1 && Utils.GCATPP.isThereSkillServers())
                {
                    Texture2D tex = Tex.MindAbsorption;
                    if (!transfertAllowed)
                        tex = Tex.MindAbsorptionDisabled;

                    yield return new Command_Action
                    {
                        icon = tex,
                        defaultLabel = "ATPP_MindAbsorption".Translate(),
                        defaultDesc = "ATPP_MindAbsorptionDesc".Translate(),
                        action = delegate ()
                        {
                            if (!transfertAllowed)
                                return;

                            Find.WindowStack.Add(new Dialog_Msg("ATPP_MindAbsorptionConfirm".Translate(), "ATPP_MindAbsorptionConfirmDesc".Translate(parent.LabelShortCap) + "\n" + ("ATPP_WarningSkyMindDisconnectionRisk").Translate(), delegate
                            {
                                mindAbsorptionStartGT = Find.TickManager.TicksGame;
                                mindAbsorptionEndingGT = mindAbsorptionStartGT + 5000;

                                Messages.Message("ATPP_MindAbsorptionStarted".Translate(currentPawn.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);

                            }, false));
                        }
                    };
                }
            }

            //Si en mode controle alors on affiche le selecteur de SX
            if (controlMode && !isPrisonner)
            {
                if ( (SX == null && availableSX.Count == 0) || (currentPawn.VX3ChipPresent() && availableSX.Count < Settings.VX3MaxSurrogateControllableAtOnce) )
                {
                    yield return new Command_Action
                    {
                        icon = Tex.AndroidToControlTarget,
                        defaultLabel = "ATPP_AndroidToControlTarget".Translate(),
                        defaultDesc = "ATPP_AndroidToControlTargetDesc".Translate(),
                        action = delegate ()
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
                                    Designator_AndroidToControl x = new Designator_AndroidToControl((Pawn)parent);
                                    Find.DesignatorManager.Select(x);

                                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                            }
                            if (opts.Count != 0)
                            {
                                if (opts.Count == 1)
                                {
                                    Designator_AndroidToControl x = new Designator_AndroidToControl((Pawn)parent);
                                    Find.DesignatorManager.Select(x);
                                }
                                else
                                {
                                    FloatMenu floatMenuMap = new FloatMenu(opts);
                                    Find.WindowStack.Add(floatMenuMap);
                                }
                            }
                        }
                    };
                    //yield return new Designator_AndroidToControl((Pawn)parent);

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
                                Utils.ShowFloatMenuNotCOntrolledSurrogateInCaravan((Pawn)parent, delegate (Pawn sSX)
                                 {
                                     if (!Utils.GCATPP.isConnectedToSkyMind(sSX))
                                     {
                                         //Tentative connection au skymind 
                                         if (!Utils.GCATPP.connectUser(sSX))
                                             return;
                                     }
                                     setControlledSurrogate(sSX);
                                 });
                            }
                        };
                    }
                }

                if(availableSX.Count > 0)
                    yield return new Command_Action
                    {
                        icon = Tex.AndroidToControlTargetDisconnect,
                        defaultLabel = "ATPP_AndroidToControlTargetDisconnect".Translate(),
                        defaultDesc = "ATPP_AndroidToControlTargetDisconnectDesc".Translate(),
                        action = delegate ()
                        {
                            //Impossibilité de deconnexion si le controleur à un mental break
                            Pawn cpawn = (Pawn)parent;
                            disconnectControlledSurrogate(null);
                        }
                    };
            }

            //FOnctionnalitées réservées uniquement aux VX2
            if (!currentPawn.VX2ChipPresent() && !currentPawn.VX3ChipPresent())
                yield break;

            Texture2D selTex = Tex.Permute;


            //Si en mode surrogate alors desactivation possibilité de duplication/permutation
            if (controlMode)
                transfertAllowed = false;

            if (!transfertAllowed)
                selTex = Tex.PermuteDisabled;

            //Permutation
            yield return new Command_Action
            {
                icon = selTex,
                defaultLabel = "ATPP_Permute".Translate(),
                defaultDesc = "ATPP_PermuteDesc".Translate(),
                action = delegate ()
                {
                    if (!transfertAllowed)
                        return;

                    Utils.ShowFloatMenuPermuteOrDuplicateCandidate((Pawn)parent, delegate (Pawn target)
                    {
                        Find.WindowStack.Add(new Dialog_Msg("ATPP_PermuteConsciousnessConfirm".Translate(parent.LabelShortCap, target.LabelShortCap), "ATPP_PermuteConsciousnessConfirmDesc".Translate(parent.LabelShortCap, target.LabelShortCap) + "\n" + ("ATPP_WarningSkyMindDisconnectionRisk").Translate(), delegate
                        {
                            OnPermuteConfirmed((Pawn)parent, target);
                        }, false));
                    },true);
                }
            };

            if (transfertAllowed)
                selTex = Tex.Duplicate;
            else
                selTex = Tex.DuplicateDisabled;

            //Duplication
            yield return new Command_Action
            {
                icon = selTex,
                defaultLabel = "ATPP_Duplicate".Translate(),
                defaultDesc = "ATPP_DuplicateDesc".Translate(),
                action = delegate ()
                {
                    if (!transfertAllowed)
                        return;

                    Utils.ShowFloatMenuPermuteOrDuplicateCandidate((Pawn)parent, delegate (Pawn target)
                    {
                        Find.WindowStack.Add(new Dialog_Msg("ATPP_DuplicateConsciousnessConfirm".Translate(parent.LabelShortCap, target.LabelShortCap), "ATPP_DuplicateConsciousnessConfirmDesc".Translate(parent.LabelShortCap, target.LabelShortCap) + "\n" + ("ATPP_WarningSkyMindDisconnectionRisk").Translate(), delegate
                        {
                            OnDuplicateConfirmed((Pawn)parent, target);
                        }, false));
                    });
                }
            };


            if (Utils.GCATPP.isThereSkyCloudCore())
            {
                if (transfertAllowed && !isPrisonner)
                    selTex = Tex.UploadToSkyCloud;
                else
                    selTex = Tex.UploadToSkyCloudDisabled;

                //Transfert vers le SkyCloud si SkyCloudCore posé 
                //CompSkyCloudCore
                yield return new Command_Action
                {
                    icon = selTex,
                    defaultLabel = "ATPP_MoveToSkyCloud".Translate(),
                    defaultDesc = "ATPP_MoveToSkyCloudDesc".Translate(),
                    action = delegate ()
                    {
                        if ((!transfertAllowed || isPrisonner))
                            return;

                        Utils.ShowFloatMenuSkyCloudCores( delegate (Thing target)
                        {
                            Find.WindowStack.Add(new Dialog_Msg("ATPP_MoveToSkyCloud".Translate(), "ATPP_MoveToSkyCloudDesc".Translate() + "\n" + ("ATPP_WarningSkyMindDisconnectionRisk").Translate(), delegate
                            {
                                OnMoveConsciousnessToSkyCloudCore((Pawn)parent, target);
                            }, false));
                        });
                    }
                };

                bool transfertAllowed2 = Utils.mindTransfertsAllowed((Pawn)parent,false);
                if (controlMode)
                    transfertAllowed2 = false;

                if (transfertAllowed2)
                    selTex = Tex.DownloadFromSkyCloud;
                else
                    selTex = Tex.DownloadFromSkyCloudDisabled;

                yield return new Command_Action
                {
                    icon = selTex,
                    defaultLabel = "ATPP_MoveFromSkyCloud".Translate(),
                    defaultDesc = "ATPP_MoveFromSkyCloudDesc".Translate(),
                    action = delegate ()
                    {
                        if (!transfertAllowed2)
                            return;

                        Utils.ShowFloatMenuSkyCloudCores(delegate (Thing target)
                        {
                            CompSkyCloudCore csc = Utils.getCachedCSC(target);

                            csc.showFloatMenuMindsStored(delegate (Pawn mind)
                            {
                                Find.WindowStack.Add(new Dialog_Msg("ATPP_MoveFromSkyCloud".Translate(), "ATPP_MoveFromSkyCloudDesc".Translate() + "\n" + ("ATPP_WarningSkyMindDisconnectionRisk").Translate(), delegate
                                {
                                    OnMoveConsciousnessFromSkyCloudCore(mind);
                                }, false));
                            });
                        });
                    }
                };

            }

            yield break;
        }

        public void disconnectControlledSurrogate(Pawn surrogate, bool externalController=false, bool preventNoHost=false)
        {
            Pawn cpawn = (Pawn)parent;
            //Arret du control d'un potentiel clone
            stopControlledSurrogate(surrogate, externalController, preventNoHost);
        }

        public override string CompInspectStringExtra()
        {
            string ret = "";
            try
            {
                if (parent.Map == null)
                    return base.CompInspectStringExtra();

                if (permuteEndingGT != -1 || showPermuteProgress)
                {
                    //Calcul pourcentage de transfert
                    float p;
                    if (permuteEndingGT == -1)
                    {
                        CompSurrogateOwner cso = Utils.getCachedCSO(permuteRecipient);
                        p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - cso.permuteStartGT) / (float)(cso.permuteEndingGT - cso.permuteStartGT));
                    }
                    else
                    {
                        p = p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - permuteStartGT) / (float)(permuteEndingGT - permuteStartGT));
                    }
                    ret += "ATPP_PermutationPercentage".Translate(((int)(p * (float)100)).ToString()) + "\n";
                }


                if (duplicateEndingGT != -1 || showDuplicateProgress)
                {
                    //Calcul pourcentage de transfert
                    float p;
                    if (duplicateEndingGT == -1)
                    {
                        CompSurrogateOwner cso = Utils.getCachedCSO(duplicateRecipient);
                        p = p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - cso.duplicateStartGT) / (float)(cso.duplicateEndingGT - cso.duplicateStartGT));
                    }
                    else
                    {
                        p = p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - duplicateStartGT) / (float)(duplicateEndingGT - duplicateStartGT));
                    }
                    ret += "ATPP_DuplicationPercentage".Translate(((int)(p * (float)100)).ToString()) + "\n";
                }

                if (uploadToSkyCloudEndingGT != -1)
                {
                    //Calcul pourcentage de transfert
                    float p;

                    p = p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - uploadToSkyCloudStartGT) / (float)(uploadToSkyCloudEndingGT - uploadToSkyCloudStartGT));
                    ret += "ATPP_UploadSkyCloudPercentage".Translate(((int)(p * (float)100)).ToString()) + "\n";
                }

                if (downloadFromSkyCloudEndingGT != -1)
                {
                    //Calcul pourcentage de transfert
                    float p;

                    p = p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - downloadFromSkyCloudStartGT) / (float)(downloadFromSkyCloudEndingGT - downloadFromSkyCloudStartGT));
                    ret += "ATPP_DownloadFromSkyCloudPercentage".Translate(((int)(p * (float)100)).ToString()) + "\n";
                }

                if (mindAbsorptionEndingGT != -1)
                {
                    //Calcul pourcentage de transfert
                    float p;

                    p = p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - mindAbsorptionStartGT) / (float)(mindAbsorptionEndingGT - mindAbsorptionStartGT));
                    ret += "ATPP_MindAbsorptionProgress".Translate(((int)(p * (float)100)).ToString()) + "\n";
                }

                if (controlMode && availableSX.Count > 0)
                {
                    string lst = "";
                    foreach (var s in availableSX)
                    {
                        if (SX == s)
                        {
                            lst += Utils.getSavedSurrogateNameNick(SX) + ", ";
                        }
                        else
                        {
                            lst += Utils.getSavedSurrogateNameNick(s) + ", ";
                        }
                    }

                    ret += "ATPP_RemotelyControl".Translate(lst.TrimEnd(' ', ',')) + "\n";
                }

                return ret.TrimEnd('\r', '\n', ' ') + base.CompInspectStringExtra();
            }
            catch(Exception e)
            {
                Log.Message("[ATPP] CompSurrogateOwner.CompInspectStringExtra "+e.Message+" "+e.StackTrace);
                return ret.TrimEnd('\r', '\n', ' ') + base.CompInspectStringExtra();
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);

            switch (signal)
            {
                case "SkyMindNetworkUserConnected":
                    break;
                case "SkyMindNetworkUserDisconnectedManually":
                case "SkyMindNetworkUserDisconnected":
                    if (signal == "SkyMindNetworkUserDisconnectedManually")
                        lastSkymindDisconnectIsManual = true;
                    else
                        lastSkymindDisconnectIsManual = false;

                    //Si deconnection du reseau skyMind alors que controlMode en cours alors on arrete les choses en regle
                    if (controlMode)
                        disconnectControlledSurrogate(null);
                    //On va  invoquer le checkInterruption pour les duplicate et permutation 
                    checkInterruptedUpload();
                    break;
            }
        }

        /*
         * Check if there are remaining free slot for the controller
         */
        public bool controllingSurrogateSlotsFull()
        {
            Pawn cp = (Pawn)parent;
            bool VX3Host = cp.VX3ChipPresent();
            return (SX != null && !VX3Host) || (VX3Host && availableSX.Count >= Settings.VX3MaxSurrogateControllableAtOnce);
        }

        public void setControlledSurrogate(Pawn controlled, bool external = false, bool forceExternalConnectionInitMalus = false)
        {
            Pawn cp = (Pawn)parent;
            if (cp == null)
                return;
            bool VX3Host = cp.VX3ChipPresent();
            if (controlled == null)
                return;
            CompAndroidState cas = Utils.getCachedCAS(controlled);
            //If regular pawn check the controlMode BUT if stored mind no check (senseless) just set as if the controlmode is enabled (and not kidnapped)
            bool genControlMode = controlMode;
            if (skyCloudHost != null)
            {
                CompSkyCloudCore tcsc = Utils.getCachedCSC(skyCloudHost);
                if (tcsc != null)
                {
                    //Control allowed if SkyCore not kidnapped or kidnapped but within time between kidnapping and disconnection from faction
                    if (!tcsc.isKidnapped || tcsc.KidnappedPendingDisconnectionGT != -1)
                    {
                        genControlMode = true;
                    }
                    else
                    {
                        genControlMode = false;
                    }
                }
                else
                    genControlMode = false;
            }

            if (cas == null 
                || ((SX != null && !VX3Host) || (VX3Host && availableSX.Count+1 > Settings.VX3MaxSurrogateControllableAtOnce)) 
                || (!external && (!genControlMode || !Utils.GCATPP.isConnectedToSkyMind(cp) || !Utils.GCATPP.isConnectedToSkyMind(controlled))))
                return;

                externalController = external;

            if (!external)
            {
                SoundDefOfAT.ATPP_SoundSurrogateConnection.PlayOneShot(null);
                FleckMaker.ThrowDustPuffThick(controlled.Position.ToVector3Shifted(), controlled.Map, 4.0f, Color.blue);
            }
            //Définition du controlleur
            cas.surrogateController = cp;
            cas.surrogateControllerCAS = currentCAS;
            cas.lastController = cp;

            bool inMainSX = false;

            if (SX == null)
            {
                SX = controlled;
                inMainSX = true;
            }
            else
            {
                extraSX.Add(controlled);
            }

            availableSX.Add(controlled);
            if (!external)
                Messages.Message("ATPP_SurrogateConnectionOK".Translate(cp.LabelShortCap, controlled.LabelShortCap), cp, MessageTypeDefOf.PositiveEvent);

            //On ne sauvegarde les stats que si il sagit d'une laison vers un surrogate principale
            if (inMainSX)
            {
                //Si SX est un M7 on sauvegarde les skills (que si pas fait)
                if (controlled.def.defName == "M7Mech" && savedSkillsBecauseM7Control == null)
                {
                    savedSkillsBecauseM7Control = new List<string>();
                    savedWorkAffectationBecauseM7Control = new List<string>();

                    foreach (var s in cp.skills.skills)
                    {
                        string k = s.def.defName + "-" + s.levelInt.ToString() + "-" + s.xpSinceLastLevel.ToString() + "-" + s.xpSinceMidnight.ToString();
                        savedSkillsBecauseM7Control.Add(k);
                    }

                    if (cp.workSettings != null && cp.workSettings.EverWork)
                    {
                        foreach (var el in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                        {
                            string k = el.defName + "-" + cp.workSettings.GetPriority(el).ToString();
                            savedWorkAffectationBecauseM7Control.Add(k);
                        }
                    }
                }
            }
            //Si pas la premiere connecction alors duplication 
            if (!inMainSX)
            {
                //Sauevgarde du nom
                Utils.saveSurrogateName(controlled);
                Utils.Duplicate(SX, controlled, false);
            }
            else
            {
                //Aussinon permutation pour que les interactions soit authentiques
                Utils.PermutePawn(cp, controlled);

                //Sauvegarde nom du surrogate
                Utils.saveSurrogateName(cp);
                //Definition nom originel du controlleur
                NameTriple nam = (NameTriple)controlled.Name;
                cp.Name = new NameTriple(nam.First, nam.Nick, nam.Last);
            }
            //On enleve le hediff de no host
            Hediff he = controlled.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_NoHost);
            if (he != null)
                controlled.health.RemoveHediff(he);
            //On Ajout le hediff de remotely controlled
            controlled.health.AddHediff(HediffDefOf.ATPP_RemotelyControlled);

            //On raffraichis la barre pour que le surrogate controlé y soit listé
            Find.ColonistBar.MarkColonistsDirty();
            
            //Add surrogate in mapPawns
            if(!externalController)
            {
                //Remove from listing of surrogate downed to reduce overhead of processing the list
                Utils.removeDownedSurrogateToLister(controlled);

                if (Settings.hideInactiveSurrogates && controlled.Map != null && controlled.Map.IsPlayerHome)
                {
                    controlled.Map.mapPawns.RegisterPawn(controlled);
                }
            }

            //Connection init malus
            if(Settings.allowSurrogateConnectionInitMalus && (!externalController || forceExternalConnectionInitMalus))
            {
                controlled.health.AddHediff(HediffDefOf.ATPP_SurrogateInit);
            }
        }

        /*
         * Check if there is at least one SX controller connected to this
         */
        public bool isThereSX()
        {
            return SX != null || (extraSX != null && extraSX.Count > 0);
        }

        public void stopControlledSurrogate(Pawn surrogate, bool externalController=false, bool preventNoHost=false,bool noPrisonedSurrogateConversion=false, bool _downedViaDisconnect = true)
        {
            Pawn cp = (Pawn)parent;

            //Pas de surrogate controllé
            if (!isThereSX() || (surrogate != null && !availableSX.Contains(surrogate)))
                return;
            //Demande d'arret de toutes les connexions
            if (surrogate == null)
            {
                //Application effet de la  deconnexion au niveau mental
                foreach(var s in extraSX)
                {
                    //Reset stats
                    Utils.initBodyAsSurrogate(s);
                    //Restauration nom
                    Utils.restoreSavedSurrogateName(s);
                    if (!externalController && Settings.hideInactiveSurrogates && s != null && s.Map != null)
                    {
                        //hide only surrogate on player's map
                        if(s.Map.IsPlayerHome)
                            s.Map.mapPawns.DeRegisterPawn(s);
                    }
                }
                if (SX != null)
                {
                    //Restauration nom SX
                    Utils.restoreSavedSurrogateName(cp);
                    Utils.PermutePawn(SX, cp);

                    if (!externalController && Settings.hideInactiveSurrogates && SX != null && SX.Map != null)
                    {
                        //hide only surrogate on player's map
                        if (SX.Map.IsPlayerHome)
                            SX.Map.mapPawns.DeRegisterPawn(SX);
                    }
                }
            }
            else
            {
                //Application effet de la  deconnexion au niveau mental 
                if (extraSX.Contains(surrogate))
                {
                    //Reset stats
                    Utils.initBodyAsSurrogate(surrogate);
                    //Restauration nom
                    Utils.restoreSavedSurrogateName(surrogate);

                    if (!externalController && Settings.hideInactiveSurrogates && surrogate != null && surrogate.Map != null)
                    {
                        //hide only surrogate on player's map
                        if (surrogate.Map.IsPlayerHome)
                            surrogate.Map.mapPawns.DeRegisterPawn(surrogate);
                    }
                }
                else
                {
                    //Restauration nom SX
                    Utils.restoreSavedSurrogateName(cp);
                    Utils.PermutePawn(surrogate, cp);

                    if (!externalController && Settings.hideInactiveSurrogates && surrogate != null && surrogate.Map != null)
                    {
                        //hide only surrogate on player's map
                        if (surrogate.Map.IsPlayerHome)
                            surrogate.Map.mapPawns.DeRegisterPawn(surrogate);
                    }
                }
            }

            foreach (var csurrogate in availableSX)
            {
                if (surrogate != null && surrogate != csurrogate)
                    continue;

                try
                {
                    //Si SX est un M7 alors on va restituer en local les skills du controlleur (le du M7 n'affecte que le surrogate principale)
                    if (csurrogate == SX && csurrogate.def.defName == "M7Mech" && savedSkillsBecauseM7Control != null)
                    {
                        foreach (var k in savedSkillsBecauseM7Control)
                        {
                            //Explode
                            string[] vals = k.Split('-');
                            if (vals.Count() != 4)
                                continue;

                            SkillDef sd = DefDatabase<SkillDef>.GetNamed(vals[0], false);
                            if (sd == null)
                                continue;

                            int levelInt = 0;
                            float xpSinceLastLevel = 0;
                            float xpSinceMidnight = 0;

                            try
                            {
                                levelInt = int.Parse(vals[1]);
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                xpSinceLastLevel = float.Parse(vals[2]);
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                xpSinceMidnight = float.Parse(vals[3]);
                            }
                            catch (Exception)
                            {

                            }

                            foreach (var s in cp.skills.skills)
                            {
                                if (s.def == sd)
                                {
                                    s.levelInt = levelInt;
                                    s.xpSinceLastLevel = xpSinceLastLevel;
                                    s.xpSinceMidnight = xpSinceMidnight;
                                    break;
                                }
                            }
                        }

                        if (cp.workSettings != null)
                        {
                            foreach (var k in savedWorkAffectationBecauseM7Control)
                            {
                                //Explode
                                string[] vals = k.Split('-');
                                if (vals.Count() != 2)
                                    continue;

                                WorkTypeDef wtd = DefDatabase<WorkTypeDef>.GetNamed(vals[0], false);
                                if (wtd == null)
                                    continue;

                                int priority = 0;
                                try
                                {
                                    priority = int.Parse(vals[1]);
                                }
                                catch (Exception)
                                {

                                }

                                cp.workSettings.SetPriority(wtd, priority);
                            }
                        }

                        savedSkillsBecauseM7Control = null;
                        savedWorkAffectationBecauseM7Control = null;
                    }
                }
                catch (Exception e)
                {
                    Log.Message("[ATPP] stopControlledSurrogate.(savedSkillsBecauseM7Control) : " + e.Message + " - " + e.StackTrace);
                }

                //On enleve le controller
                CompAndroidState cas = Utils.getCachedCAS(csurrogate);
                if (cas != null)
                {
                    cas.surrogateController = null;
                    cas.surrogateControllerCAS = null;
                    //Store the last way the surrogate was disconnected
                    cas.downedViaDisconnect = _downedViaDisconnect;
                }

                //Utils.disableGlobalKill = true;
                //On effectue des operations directes sur les heddifs sans passer par AddHediff et RemoveHediff car cest surfonction peuvent checquer la mort du pawn et appelez Kill
                // (sachant que cette routine est elle meme appelée lors d'un Kill via notre patch cela duplique la mort de la creature courante) 
                //On enleve le hediff de remotely controled
                Hediff he = csurrogate.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("ATPP_RemotelyControlled"));
                if (he != null)
                {
                    if (Utils.insideKillFuncSurrogate)
                    {
                        csurrogate.health.hediffSet.hediffs.Remove(he);
                        he.PostRemoved();
                    }
                    else
                    {
                        csurrogate.health.RemoveHediff(he);
                    }
                }

                //On remet le hediff de no host au clone
                if (!externalController && !preventNoHost)
                {
                    if (Utils.insideKillFuncSurrogate)
                        csurrogate.health.hediffSet.AddDirect(HediffMaker.MakeHediff(HediffDefOf.ATPP_NoHost, csurrogate, null));
                    else
                    {
                        csurrogate.health.AddHediff(HediffDefOf.ATPP_NoHost);
                    }
                }


                //On vire le potentiel Hediff de LowSignalSkyMind
                /*he = csurrogate.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_LowNetworkSignal);
                if (he != null)
                {
                    if (Utils.insideKillFuncSurrogate)
                    {
                        csurrogate.health.hediffSet.hediffs.Remove(he);
                        he.PostRemoved();
                    }
                    else
                    {
                        csurrogate.health.RemoveHediff(he);
                    }
                }*/


                //Utils.disableGlobalKill = false;

                SoundDefOfAT.ATPP_SoundSurrogateDisconnect.PlayOneShot(null);

                //Si surrogate est prisonnier on le met comme non prisonnier et le controlleur comme prisonnier
                if (csurrogate.IsPrisoner && !noPrisonedSurrogateConversion)
                {
                    if (csurrogate.Faction != Faction.OfPlayer)
                    {
                        csurrogate.SetFaction(Faction.OfPlayer, null);
                    }

                    if (csurrogate.guest != null)
                    {
                        csurrogate.guest.SetGuestStatus(null, GuestStatus.Guest);
                    }

                    if (cp.guest != null)
                    {
                        if(cp.IsSlave)
                            cp.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
                        else
                            cp.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Prisoner);
                    }

                    //On va egalement forcer la fin du controlMode car le joueur na plus de controle pour l'enlever sur le colon
                    //SX = null;
                    removeRemoteControlHediff();
                    controlMode = false;
                    Utils.GCATPP.disconnectUser(cp);
                }

                //On restaure a zero les worksettings du SX
                if (csurrogate.def.defName != "M7Mech" && csurrogate.workSettings != null && csurrogate.workSettings.EverWork)
                {
                    foreach (var el in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                    {
                        if (csurrogate.workSettings.GetPriority(el) != 0)
                            csurrogate.workSettings.SetPriority(el, 3);
                    }
                }

                if (csurrogate.playerSettings != null)
                {
                    csurrogate.playerSettings.AreaRestriction = null;
                    csurrogate.playerSettings.hostilityResponse = HostilityResponseMode.Flee;
                }
                //Log.Message("S2");
                if (csurrogate.timetable != null)
                {
                    for (int i = 0; i != 24; i++)
                    {
                        csurrogate.timetable.SetAssignment(i, TimeAssignmentDefOf.Anything);
                    }
                }
            }
            //Retrait du referencement du surrogate mentionné
            if (surrogate == null)
            {
                extraSX.Clear();
                SX = null;
                availableSX.Clear();
            }
            else
            {
                if (extraSX.Contains(surrogate))
                {
                    extraSX.Remove(surrogate);
                    availableSX.Remove(surrogate);
                }
                else
                {
                    availableSX.Remove(SX);
                    SX = null;
                }
            }

            //If the controller is in a caravan, not a hosted mind and there is no other controlled surrogate then we stop the controlMode (=> no longer downed)
            if (!externalController && cp.Faction == Faction.OfPlayer && skyCloudHost == null && cp.IsCaravanMember() && !isThereSX())
            {
                if (controlMode)
                {
                    toggleControlMode();
                }
            }
        }

        public void clearRansomwareVar()
        {
            ransomwareSkillValue = -1;
            ransomwareSkillStolen = null;
            ransomwareTraitAdded = null;
        }

        private void OnPermuteConfirmed(Pawn source, Pawn dest)
        {
            //Ajout hediff de transfert aux deux androids
            source.health.AddHediff(HediffDefOf.ATPP_ConsciousnessUpload);
            dest.health.AddHediff(HediffDefOf.ATPP_ConsciousnessUpload);

            int CGT = Find.TickManager.TicksGame;
            permuteRecipient = dest;
            permuteStartGT = CGT;
            permuteEndingGT = CGT + 60-(CGT%60) + Settings.mindPermutationHours * 2500;
            
            CompSurrogateOwner cso = Utils.getCachedCSO(dest);
            cso.showPermuteProgress = true;
            cso.permuteRecipient = source;
            cso.permuteRecipientCSO = this;
            permuteRecipientCSO = cso;

            Messages.Message("ATPP_StartPermute".Translate(source.LabelShortCap, dest.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);
        }

        private void OnDuplicateConfirmed(Pawn source, Pawn dest)
        {
            //Ajout hediff de transfert aux deux androids
            source.health.AddHediff(HediffDefOf.ATPP_ConsciousnessUpload);
            dest.health.AddHediff(HediffDefOf.ATPP_ConsciousnessUpload);

            int CGT = Find.TickManager.TicksGame;
            duplicateRecipient = dest;
            duplicateStartGT = CGT;
            duplicateEndingGT = CGT + 60-(CGT%60) + Settings.mindDuplicationHours * 2500;

            CompSurrogateOwner cso = Utils.getCachedCSO(dest);
            cso.showDuplicateProgress = true;
            cso.duplicateRecipient = (Pawn)parent;
            cso.duplicateRecipientCSO = this;
            duplicateRecipientCSO = cso;

            //Log.Message("DUPLICATION STARTED !! ");
            Messages.Message("ATPP_StartDuplicate".Translate(source.LabelShortCap, dest.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);
        }

        private void OnMoveConsciousnessToSkyCloudCore(Pawn source, Thing dest)
        {
            int CGT = Find.TickManager.TicksGame;

            CompSkyCloudCore csc = Utils.getCachedCSC(dest);
            if (csc.isFull())
            {
                Messages.Message("ATPP_StartSkyCloudUploadFailedFull".Translate(source.LabelShortCap, csc.getName()), parent, MessageTypeDefOf.NegativeEvent);
            }
            else
            {
                source.health.AddHediff(HediffDefOf.ATPP_ConsciousnessUpload);
                skyCloudRecipient = dest;
                uploadToSkyCloudStartGT = CGT;
                uploadToSkyCloudEndingGT = CGT + 60 - (CGT % 60) + Settings.mindUploadToSkyCloudHours * 2500;
                csc.pendingUploads.Add(source);
                Messages.Message("ATPP_StartSkyCloudUpload".Translate(source.LabelShortCap, csc.getName()), parent, MessageTypeDefOf.PositiveEvent);
            }
        }

        private void OnMoveConsciousnessFromSkyCloudCore(Pawn source)
        {
            Pawn dest = (Pawn)parent;
            dest.health.AddHediff(HediffDefOf.ATPP_ConsciousnessUpload);

            int CGT = Find.TickManager.TicksGame;
            skyCloudDownloadRecipient = source;
            downloadFromSkyCloudStartGT = CGT;
            downloadFromSkyCloudEndingGT = CGT + 60 - (CGT % 60) + Settings.mindUploadToSkyCloudHours * 2500;

            string name = source.LabelShortCap;
            CompSurrogateOwner cso = Utils.getCachedCSO(source);

            CompSkyCloudCore csc = Utils.getCachedCSC(cso.skyCloudHost);
            if (csc != null)
            {

                if (cso != null && cso.isThereSX())
                {
                    if (cso.SX != null)
                        name = cso.SX.LabelShortCap;
                }
                csc.setMindInReplicationModeOn(source);

                Messages.Message("ATPP_StartSkyCloudDownload".Translate(source.LabelShortCap, dest.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);
            }
        }

        public void startMigration(Thing dest)
        {
            CompSkyCloudCore csc2 = Utils.getCachedCSC(dest);

            int CGT = Find.TickManager.TicksGame;

            if (csc2.isFull())
            {
                Messages.Message("ATPP_StartSkyCloudMigrationFailed".Translate(((Pawn)parent).LabelShortCap, csc2.getName()), parent, MessageTypeDefOf.NegativeEvent);
            }
            else
            {
                migrationSkyCloudHostDest = dest;
                migrationStartGT = CGT;
                migrationEndingGT = CGT + 60 - (CGT % 60) + Settings.mindSkyCloudMigrationHours * 2500;

                Messages.Message("ATPP_StartSkyCloudMigration".Translate(((Pawn)parent).LabelShortCap, Utils.getCachedCSC(skyCloudHost).getName(), csc2.getName()), parent, MessageTypeDefOf.PositiveEvent);
            }
        }

        /*
         * Detecte un cas d'interruption est le cas echeant tue les protagoniste de l'upload tous en affichant un message d'erreur
         */
        public void checkInterruptedUpload()
        {
            bool killSelf = false;
            bool permuteRecipientLastSkymindDisconnectIsManual = true;
            bool duplicateRecipientLastSkymindDisconnectIsManual = true;
            Pawn cpawn = (Pawn)parent;
           
            //Permutation ou duplication en cours on test si les conditions d'arret sont presentes
            if (permuteEndingGT != -1 || permuteRecipient != null 
                || duplicateEndingGT != -1 || duplicateRecipient != null
                || uploadToSkyCloudEndingGT != -1 || downloadFromSkyCloudEndingGT != -1 || mindAbsorptionEndingGT != -1 || migrationEndingGT != -1 || replicationEndingGT != -1)
            {
                
                bool permuteRecipientDead = false;
                if (permuteRecipient != null)
                    permuteRecipientDead = permuteRecipient.Dead;

                bool duplicateRecipientDead = false;
                if (duplicateRecipient != null)
                    duplicateRecipientDead = duplicateRecipient.Dead;

                bool recipientConnected = false;

                bool emitterConnected = false;

                if (permuteRecipient != null) {
                    if (permuteRecipientCSO == null)
                        permuteRecipientCSO = Utils.getCachedCSO(permuteRecipient);

                    if (permuteRecipientCSO != null)
                        permuteRecipientLastSkymindDisconnectIsManual = permuteRecipientCSO.lastSkymindDisconnectIsManual;

                    if (Utils.GCATPP.isConnectedToSkyMind(permuteRecipient, !permuteRecipientLastSkymindDisconnectIsManual, false))
                        recipientConnected = true;
                }

                if (duplicateRecipient != null)
                {
                    if (duplicateRecipientCSO == null)
                        duplicateRecipientCSO = Utils.getCachedCSO(duplicateRecipient);

                    if (duplicateRecipientCSO != null)
                        duplicateRecipientLastSkymindDisconnectIsManual = duplicateRecipientCSO.lastSkymindDisconnectIsManual;

                    if (Utils.GCATPP.isConnectedToSkyMind(duplicateRecipient, !duplicateRecipientLastSkymindDisconnectIsManual, false))
                        recipientConnected = true;
                }

                /*if (Utils.GCATPP.isThereSkillServers())
                    recipientConnected = true;*/

                CompSurrogateOwner csoSkyCloudRecipient = Utils.getCachedCSO(skyCloudDownloadRecipient);
                CompSkyCloudCore cscSkyCloudRecipient = Utils.getCachedCSC(skyCloudRecipient);
                CompSkyCloudCore cscSkyCloudHost = Utils.getCachedCSC(skyCloudHost);
                CompSkyCloudCore cscMigrationSkyCloudHostDest = Utils.getCachedCSC(migrationSkyCloudHostDest);
                CompSkyCloudCore cscCsoSkyCloudRecipientSkyCloudHost = null;

                if(csoSkyCloudRecipient != null)
                    cscCsoSkyCloudRecipientSkyCloudHost = Utils.getCachedCSC(csoSkyCloudRecipient.skyCloudHost);

                //L'équivalence du EST connecté sur le COre s'est si il est bien alimenté en elec
                if ( (skyCloudRecipient != null && cscSkyCloudRecipient != null && cscSkyCloudRecipient.isOnline())
                    || (replicationEndingGT != -1 && cscSkyCloudHost != null && cscSkyCloudHost.isOnline())
                    || (migrationSkyCloudHostDest != null && cscMigrationSkyCloudHostDest.isOnline())
                    || (skyCloudDownloadRecipient != null && csoSkyCloudRecipient != null && cscCsoSkyCloudRecipientSkyCloudHost.isOnline()))
                {
                    recipientConnected = true;
                }

                //L'équivalence en mode migration est le check de si le host du mind est branché
                if ( (replicationEndingGT != -1 && cscSkyCloudHost != null && cscSkyCloudHost.isOnline())
                    || (skyCloudHost != null && cscSkyCloudHost != null && cscSkyCloudHost.isOnline())
                    || (Utils.GCATPP.isConnectedToSkyMind(cpawn, !lastSkymindDisconnectIsManual,false)) )
                    emitterConnected = true;

                //Si hote plus valide alors on arrete le processus et on kill les deux androids
                if ((permuteRecipientDead || duplicateRecipientDead || cpawn.Dead || !emitterConnected || !recipientConnected))
                {
                    bool showMindUploadNotif = true;
                    string reason = "";
                    if (permuteRecipientDead || duplicateRecipientDead)
                    {
                        reason = "ATPP_LetterInterruptedUploadDescCompHostDead".Translate();
                        killSelf = true;
                    }

                    if (cpawn.Dead)
                    {
                        reason = "ATPP_LetterInterruptedUploadDescCompSourceDead".Translate();
                        if (permuteRecipient != null && !permuteRecipient.Dead)
                            permuteRecipient.Kill(null, null);

                        if (duplicateRecipient != null && !duplicateRecipient.Dead)
                            duplicateRecipient.Kill(null, null);
                    }

                    if (reason == "" )
                    {
                        if(mindAbsorptionEndingGT != -1)
                        {
                            if (!cpawn.Dead)
                                killSelf = true;

                            showMindUploadNotif = false;
                            Find.LetterStack.ReceiveLetter("ATPP_LetterInterruptedMindAbsorption".Translate(), "ATPP_LetterInterruptedMindAbsorptionDesc".Translate(cpawn.LabelShortCap), LetterDefOf.ThreatSmall);
                        }
                        else if (replicationEndingGT != -1)
                        {
                            CompSkyCloudCore csc = Utils.getCachedCSC(skyCloudHost);
                            csc.replicatingMinds.Remove((Pawn)parent);
                            showMindUploadNotif = false;

                            Find.LetterStack.ReceiveLetter("ATPP_LetterInterruptedSkyCloudReplication".Translate(), "ATPP_LetterInterruptedSkyCloudReplicationDesc".Translate(cpawn.LabelShortCap), LetterDefOf.ThreatSmall);
                        }
                        else if (migrationEndingGT != -1)
                        {
                            CompSkyCloudCore csc = Utils.getCachedCSC(skyCloudHost);
                            //Remove mind from the source SkyCore
                            csc.RemoveMind(cpawn);
                            //Remove mind from the destination SkyCore in pending uploads
                            if(cscMigrationSkyCloudHostDest != null)
                                cscMigrationSkyCloudHostDest.pendingUploads.Remove(cpawn);
                            //mind failing the migration are killed (corrupted)
                            cpawn.Kill(null,null);
                            showMindUploadNotif = false;
                            Find.LetterStack.ReceiveLetter("ATPP_LetterInterruptedSkyCloudMigration".Translate(), "ATPP_LetterInterruptedSkyCloudMigrationDesc".Translate(cpawn.LabelShortCap), LetterDefOf.ThreatSmall);
                        }
                        else if (!recipientConnected || !emitterConnected)
                        {
                            reason = "ATPP_LetterInterruptedUploadDescCompDiconnectionError".Translate();

                            //If mind upload then we remove it from the pending upload table of the destination SkyCore
                            if(uploadToSkyCloudEndingGT != -1 && cscSkyCloudRecipient != null)
                            {
                                cscSkyCloudRecipient.pendingUploads.Remove(cpawn);
                            }

                            killSelf = true;
                            if (permuteRecipient != null && !permuteRecipient.Dead)
                            {
                                permuteRecipient.Kill(null, null);
                            }
                            else if(duplicateRecipient != null && !duplicateRecipient.Dead)
                            { 
                                duplicateRecipient.Kill(null, null);
                            }
                        }
                    }

                    resetUploadStuff();

                    if (killSelf && !cpawn.Dead)
                    {
                        cpawn.Kill(null, null);
                    }

                    if(showMindUploadNotif)
                        Utils.showFailedLetterMindUpload(reason);
                }
            }
        }

        private void resetUploadStuff()
        {
            if (duplicateRecipient != null)
            {
                CompSurrogateOwner cso = Utils.getCachedCSO(duplicateRecipient);
                cso.showDuplicateProgress = false;
                cso.duplicateRecipient = null;
                cso.duplicateRecipientCSO = null;
            }

            if (permuteRecipient != null)
            {
                CompSurrogateOwner cso = Utils.getCachedCSO(permuteRecipient);
                cso.showPermuteProgress = false;
                cso.permuteRecipient = null;
                cso.permuteRecipientCSO = null;
            }

            permuteStartGT = 0;
            duplicateStartGT = 0;
            permuteEndingGT = -1;
            duplicateEndingGT = -1;
            duplicateRecipient = null;
            permuteRecipient = null;
            skyCloudRecipient = null;
            skyCloudDownloadRecipient = null;
            uploadToSkyCloudStartGT = 0;
            uploadToSkyCloudEndingGT = -1;
            downloadFromSkyCloudStartGT = 0;
            downloadFromSkyCloudEndingGT = -1;
            migrationSkyCloudHostDest = null;
            migrationEndingGT = -1;
            migrationStartGT = 0;
            replicationStartGT = 0;
            replicationEndingGT = -1;
            mindAbsorptionEndingGT = -1;
            mindAbsorptionStartGT = -1;
        }

        private Pawn currentPawn;
        private CompAndroidState currentCAS;
        //Indique si le colon est en mode controle de clone
        public bool controlMode = false;
        //Désigne le SX visé
        public Pawn SX = null;
        //Désigne les SX additionnelles (possesseur de VX3)
        public List<Pawn> extraSX = new List<Pawn>();

        public int mindAbsorptionStartGT = 0;
        public int mindAbsorptionEndingGT = -1;

        public int permuteStartGT = 0;
        public int permuteEndingGT = -1;

        public int duplicateStartGT = 0;
        public int duplicateEndingGT = -1;

        public int uploadToSkyCloudStartGT = 0;
        public int uploadToSkyCloudEndingGT = -1;

        public int downloadFromSkyCloudStartGT = 0;
        public int downloadFromSkyCloudEndingGT = -1;

        public Pawn skyCloudDownloadRecipient;
        public Thing skyCloudRecipient;
        public Pawn permuteRecipient;
        public CompSurrogateOwner permuteRecipientCSO;
        public Pawn duplicateRecipient;
        public CompSurrogateOwner duplicateRecipientCSO;

        public bool showPermuteProgress = false;
        public bool showDuplicateProgress = false;

        public TraitDef ransomwareTraitAdded;
        public int ransomwareSkillValue = -1;
        public SkillDef ransomwareSkillStolen;

        public bool externalController = true;

        public Thing skyCloudHost;
        //Migration stuff related
        public Thing migrationSkyCloudHostDest;
        public int migrationStartGT = 0;
        public int migrationEndingGT = -1;
        //Replication stuff related
        public int replicationStartGT = 0;
        public int replicationEndingGT = -1;

        public bool repairAndroids = false;

        //Permet de stocker une copies des levels de skills quand permutation avec un M7
        public List<string> savedSkillsBecauseM7Control;
        public List<string> savedWorkAffectationBecauseM7Control;

        public List<Pawn> availableSX = new List<Pawn>();
        public bool lastSkymindDisconnectIsManual = false;
    }
}