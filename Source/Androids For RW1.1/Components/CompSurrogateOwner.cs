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

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (extraSX == null)
                    extraSX = new List<Pawn>();

                //Reconsitution du tableau de SX virtuelle (fusion extraSX et SX)
                if (SX != null)
                    availableSX.Add(SX);
                if(extraSX.Count >0)
                    availableSX = (List<Pawn>)availableSX.Concat(extraSX).ToList();

                availableSX.RemoveAll(item => item == null);
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (parent.Map == null || !parent.Spawned)
                return;

            int GT = Find.TickManager.TicksGame;

            if(GT % 600 == 0)
            {
                Pawn cpawn = (Pawn)parent;
                if (cpawn.IsKidnapped())
                {
                    Utils.GCATPP.disconnectUser(cpawn);
                }
            }

            if (GT % 120 == 0 && ( permuteEndingGT != -1 || duplicateEndingGT != -1 || uploadToSkyCloudEndingGT != -1 || migrationEndingGT != -1 || mindAbsorptionEndingGT != -1 || downloadFromSkyCloudEndingGT != -1 || (controlMode && SX != null)))
            {
                Pawn cpawn = (Pawn)parent;
                checkInterruptedUpload();

                //Atteinte d'un chargement de permutation de conscience
                if (permuteEndingGT != -1 && permuteEndingGT < GT)
                {
                    checkInterruptedUpload();
                    if (permuteEndingGT == -1)
                        return;
                    permuteEndingGT = -1;
                    permuteRecipient.TryGetComp<CompSurrogateOwner>().permuteEndingGT = -1;

                    Utils.removeUploadHediff(cpawn, permuteRecipient);

                    Find.LetterStack.ReceiveLetter("ATPP_LetterPermuteOK".Translate(), "ATPP_LetterPermuteOKDesc".Translate(cpawn.LabelShortCap, permuteRecipient.LabelShortCap), LetterDefOf.PositiveEvent, parent);
                    //On realise effectivement la permutation
                    Utils.PermutePawn(cpawn, permuteRecipient);

                    //Clear du status de blank andorid si destinataire blank android
                    Utils.clearBlankAndroid(permuteRecipient);

                    //Si destinataire T1 rajout simpleMinded
                    if (permuteRecipient.def.defName == Utils.T1)
                        Utils.addSimpleMindedTraitForT1(permuteRecipient);
                    else
                        Utils.removeSimpleMindedTrait(permuteRecipient);

                    //Si source T1 rajout simpleMinded
                    if (cpawn.def.defName == Utils.T1)
                        Utils.addSimpleMindedTraitForT1(cpawn);
                    else
                        Utils.removeSimpleMindedTrait(cpawn);

                    if(cpawn.IsPrisoner || permuteRecipient.IsPrisoner)
                        dealWithPrisonerRecipientPermute(cpawn, permuteRecipient);

                    resetUploadStuff();
                }

                //Atteinte d'un chargement de duplication de conscience
                if (duplicateEndingGT != -1 && duplicateEndingGT < GT)
                {
                    //Log.Message("DUPLICATION EFFECTIVE !!! ");
                    checkInterruptedUpload();
                    if (duplicateEndingGT == -1)
                        return;
                    duplicateEndingGT = -1;
                    duplicateRecipient.TryGetComp<CompSurrogateOwner>().duplicateEndingGT = -1;

                    Utils.removeUploadHediff(cpawn, duplicateRecipient);

                    Find.LetterStack.ReceiveLetter("ATPP_LetterDuplicateOK".Translate(), "ATPP_LetterDuplicateOKDesc".Translate(cpawn.LabelShortCap, duplicateRecipient.LabelShortCap), LetterDefOf.PositiveEvent, parent);
                    //On realise effectivement la permutation
                    Utils.Duplicate(cpawn, duplicateRecipient);

                    //Clear du status de blank andorid si destinataire blank android
                    Utils.clearBlankAndroid(duplicateRecipient);

                    //Si destinataire T1 rajout simpleMinded
                    if (duplicateRecipient.def.defName == Utils.T1)
                        Utils.addSimpleMindedTraitForT1(duplicateRecipient);
                    else
                        Utils.removeSimpleMindedTrait(duplicateRecipient);

                    
                    dealWithPrisonerRecipient(cpawn, duplicateRecipient);
                    

                    resetUploadStuff();
                }

                //Atteinte d'un chargement de upload vers SkyCloud
                if (uploadToSkyCloudEndingGT != -1 && uploadToSkyCloudEndingGT < GT)
                {
                    //Log.Message("DUPLICATION EFFECTIVE !!! ");
                    checkInterruptedUpload();
                    if (uploadToSkyCloudEndingGT == -1)
                        return;
                    uploadToSkyCloudEndingGT = -1;
                    Utils.removeUploadHediff(cpawn, null);

                    CompSkyCloudCore csc = skyCloudRecipient.TryGetComp<CompSkyCloudCore>();

                    Find.LetterStack.ReceiveLetter("ATPP_LetterSkyCloudUploadOK".Translate(), "ATPP_LetterSkyCloudUploadOKDesc".Translate(cpawn.LabelShortCap, csc.getName()), LetterDefOf.PositiveEvent, parent);
                    //On realise effectivement l'upload vers le Core

                    //Stockage pawn actuel
                    csc.storedMinds.Add(cpawn);

                    //Suppression traits blacklistés pour les esprits
                    Utils.removeMindBlacklistedTrait(cpawn);

                    //Si corp d'origine été un T1 alors on supprime le simpleminded le cas echeant
                    Utils.removeSimpleMindedTrait(cpawn);

                    //Copie du corps du pawn et placement sur la carte
                    bool killMode = false;
                    if (Settings.skyCloudUploadModeForSourceMind == 2)
                        killMode = true;

                    Pawn corpse = Utils.spawnCorpseCopy(cpawn,killMode);

                    //Reconfiguration en mode VX0 auto
                    if(Settings.skyCloudUploadModeForSourceMind == 1)
                    {
                        //On retire la VX2/VX3
                        foreach(var he in corpse.health.hediffSet.hediffs.ToList())
                        {
                            if (he.def == Utils.hediffHaveVX2Chip || he.def == Utils.hediffHaveVX3Chip)
                                corpse.health.RemoveHediff(he);
                        }

                        BodyPartRecord bpr = null;
                        bpr = corpse.health.hediffSet.GetBrain();

                        //On ajoute une VX0 en prevenant l'apparition de mauvaises pensées
                        Utils.preventVX0Thought = true;
                        corpse.health.AddHediff(Utils.hediffHaveVX0Chip, bpr);
                        Utils.preventVX0Thought = false;
                    }


                    //Deconnection de skymind le cas echeant
                    Utils.GCATPP.disconnectUser(cpawn);

                    //Despawn du pawn numérisé
                    cpawn.DeSpawn();

                    skyCloudHost = skyCloudRecipient;

                    //Simulation mort du corp en générant un corp identique

                    Utils.playVocal("soundDefSkyCloudMindDownloadCompleted");


                    resetUploadStuff();
                }

                //Atteinte d'un chargement de download depuis SkyCloud
                if (downloadFromSkyCloudEndingGT != -1 && downloadFromSkyCloudEndingGT < GT)
                {
                    //Log.Message("DUPLICATION EFFECTIVE !!! ");
                    checkInterruptedUpload();
                    if (downloadFromSkyCloudEndingGT == -1)
                        return;
                    downloadFromSkyCloudEndingGT = -1;
                    Utils.removeUploadHediff(cpawn, null);

                    CompSurrogateOwner cso = skyCloudDownloadRecipient.TryGetComp<CompSurrogateOwner>();
                    if (cso.skyCloudHost == null)
                        return;

                    CompSkyCloudCore csc = cso.skyCloudHost.TryGetComp<CompSkyCloudCore>();

                    //Arret des jobs d'un esprit
                    csc.stopMindActivities(skyCloudDownloadRecipient);


                    Find.LetterStack.ReceiveLetter("ATPP_LetterSkyCloudDownloadOK".Translate(), "ATPP_LetterSkyCloudDownloadOKDesc".Translate(skyCloudDownloadRecipient.LabelShortCap, cpawn.LabelShortCap), LetterDefOf.PositiveEvent, parent);
                    //On realise effectivement le download du core vers le cerveau
                    //Permutation
                    Utils.PermutePawn(skyCloudDownloadRecipient, cpawn);

                    //Report du blankAndroid pour le flagger dans la routine de kill
                    CompAndroidState cas = skyCloudDownloadRecipient.TryGetComp<CompAndroidState>();
                    if(cas != null)
                        cas.isBlankAndroid = true;

                    //Clear du status de blank andorid si destinataire blank android
                    Utils.clearBlankAndroid(cpawn);

                    //Si cpawn un T1 on lui ajoute le trait SimpleMinded
                    Utils.addSimpleMindedTraitForT1(cpawn);

                    

                    //Suppression de la memoire du core
                    csc.RemoveMind(skyCloudDownloadRecipient);

                    //Annulation status de prisonnier de l'esprit downloader
                    dealWithPrisonerRecipientPermute(cpawn, skyCloudDownloadRecipient);

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
                    foreach (var sr in cpawn.skills.skills)
                    {
                        sum += sr.levelInt;
                    }
                    average = (int)((float)sum / (float)cpawn.skills.skills.Count());

                    int nbp = (int)((float)((float)average / (float)20) * Rand.Range(1000, 5000));

                    Utils.GCATPP.incSkillPoints(nbp);

                    Find.LetterStack.ReceiveLetter("ATPP_MindAbsorptionDone".Translate(), "ATPP_MindAbsorptionDoneDesc".Translate(cpawn.LabelShortCap, nbp), LetterDefOf.PositiveEvent, cpawn);

                    resetUploadStuff();

                    cpawn.Kill(null, null);
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
                    recipient.guest.SetGuestStatus(null, false);
                }
            }

            //SI destinataire de la duplication colon regular et emetteur prisonnier 
            if (cpawn.IsPrisoner && !recipient.IsPrisoner)
            {
                if (recipient.Faction != cpawn.Faction)
                {
                    recipient.SetFaction(cpawn.Faction, null);
                }

                if (recipient.guest != null)
                {
                    recipient.guest.SetGuestStatus(Faction.OfPlayer, true);
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
                    recipient.guest.SetGuestStatus(null, false);

                if (cpawn.Faction != tmp)
                    cpawn.SetFaction(tmp, null);

                if (cpawn.guest != null)
                    cpawn.guest.SetGuestStatus(Faction.OfPlayer, true);

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
                    cpawn.guest.SetGuestStatus(null, false);

                if (recipient.Faction != tmp)
                    recipient.SetFaction(tmp, null);

                if (recipient.guest != null)
                    recipient.guest.SetGuestStatus(Faction.OfPlayer, true);

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
            CompSkyCloudCore csc = skyCloudHost.TryGetComp<CompSkyCloudCore>();

            PawnGenerationRequest request = new PawnGenerationRequest(cpawn.kindDef, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f, false, true, true, false, true, false, false, fixedBiologicalAge: cpawn.ageTracker.AgeBiologicalYearsFloat, fixedChronologicalAge: cpawn.ageTracker.AgeChronologicalYearsFloat, fixedGender: cpawn.gender, fixedMelanin: cpawn.story.melanin);
            Pawn clone = PawnGenerator.GeneratePawn(request);

            //surrogate.Name = new NameTriple("", "S" + prefix + SXVer + "-" + Utils.GCATPP.getNextSXID(SXVer), "");

            foreach (var h in clone.health.hediffSet.hediffs.ToList())
            {
                clone.health.RemoveHediff(h);
            }

            CompSurrogateOwner cso = clone.TryGetComp<CompSurrogateOwner>();
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

            Find.LetterStack.ReceiveLetter("ATPP_LetterSkyCloudReplicateOK".Translate(), "ATPP_LetterSkyCloudReplicateOKDesc".Translate(cpawn.LabelShortCap, skyCloudHost.TryGetComp<CompSkyCloudCore>().getName()), LetterDefOf.PositiveEvent, parent);

            resetUploadStuff();
        }

        public void OnMigrated()
        {
            Pawn cpawn = (Pawn)parent;
            checkInterruptedUpload();
            if (migrationEndingGT == -1)
                return;
            migrationEndingGT = -1;

            CompSkyCloudCore csc = skyCloudHost.TryGetComp<CompSkyCloudCore>();
            CompSkyCloudCore csc2 = migrationSkyCloudHostDest.TryGetComp<CompSkyCloudCore>();


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

            Hediff he = cpawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("ATPP_ConsciousnessUpload"));
            if (he != null)
            {
                cpawn.health.hediffSet.hediffs.Remove(he);
                he.PostRemoved();
            }
            
            he = cpawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("ATPP_InRemoteControlMode"));
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

        private void toggleControlMode()
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
            cpawn.health.AddHediff(DefDatabase<HediffDef>.GetNamed("ATPP_InRemoteControlMode"));
        }

        private void removeRemoteControlHediff()
        {
            Pawn cpawn = (Pawn)parent;
            Hediff he = cpawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("ATPP_InRemoteControlMode"));
            if (he != null)
                cpawn.health.RemoveHediff(he);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Pawn pawn = (Pawn)parent;
            bool isPrisonner = pawn.IsPrisoner;
            CompAndroidState cas = pawn.TryGetComp<CompAndroidState>();
            bool isBlankAndroid = (cas != null && cas.isBlankAndroid);

            //Si pas prisonier ET pas un surrogate non controlé && affecté au crafting
            if (!isPrisonner && !(cas != null && cas.isSurrogate && cas.surrogateController == null) && pawn.workSettings != null && pawn.workSettings.WorkIsActive(Utils.WorkTypeDefSmithing) && Settings.androidsCanOnlyBeHealedByCrafter)
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
            if (!pawn.VXChipPresent())
                yield break;

            //Les clones ne peuvent pas etre clone owner
            if (cas != null && cas.isSurrogate)
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

            if (!Utils.GCATPP.isConnectedToSkyMind(pawn))
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

                                Messages.Message("ATPP_MindAbsorptionStarted".Translate(pawn.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);

                            }, false));
                        }
                    };
                }
            }

            //Si en mode controle alors on affiche le selecteur de SX
            if (controlMode && !isPrisonner)
            {
                if ( (SX == null && availableSX.Count == 0) || (pawn.VX3ChipPresent() && availableSX.Count < Settings.VX3MaxSurrogateControllableAtOnce) )
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
            if (!pawn.VX2ChipPresent() && !pawn.VX3ChipPresent())
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

                        Utils.ShowFloatMenuSkyCloudCores( delegate (Building target)
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

                        Utils.ShowFloatMenuSkyCloudCores(delegate (Building target)
                        {
                            CompSkyCloudCore csc = target.TryGetComp<CompSkyCloudCore>();

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
                        CompSurrogateOwner cso = permuteRecipient.TryGetComp<CompSurrogateOwner>();
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
                        CompSurrogateOwner cso = duplicateRecipient.TryGetComp<CompSurrogateOwner>();
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
                case "SkyMindNetworkUserDisconnected":
                    //Si deconnection du reseau skyMind alors que controlMode en cours alors on arrete les choses en regle
                    if (controlMode)
                        disconnectControlledSurrogate(null);
                    //On va  invoquer le checkInterruption pour les duplicate et permutation 
                    checkInterruptedUpload();
                    break;
            }
        }


        public void setControlledSurrogate(Pawn controlled, bool external=false)
        {
            Pawn cp = (Pawn)parent;
            if (cp == null)
                return;
            bool VX3Host = cp.VX3ChipPresent();

            if (controlled == null)
                return;
            CompAndroidState cas = controlled.TryGetComp<CompAndroidState>();

            if (cas == null 
                || ((SX != null && !VX3Host) || (VX3Host && availableSX.Count+1 > Settings.VX3MaxSurrogateControllableAtOnce)) 
                || (!external && (!controlMode || !Utils.GCATPP.isConnectedToSkyMind(cp) || !Utils.GCATPP.isConnectedToSkyMind(controlled))))
                return;

                externalController = external;

            if (!external)
            {
                Utils.soundDefSurrogateConnection.PlayOneShot(null);
                MoteMaker.ThrowDustPuffThick(controlled.Position.ToVector3Shifted(), controlled.Map, 4.0f, Color.blue);
            }

            //Définition du controlleur
            cas.surrogateController = cp;
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
            Hediff he = controlled.health.hediffSet.GetFirstHediffOfDef(Utils.hediffNoHost);
            if (he != null)
                controlled.health.RemoveHediff(he);
            //On Ajout le hediff de remotely controlled
            controlled.health.AddHediff(DefDatabase<HediffDef>.GetNamed("ATPP_RemotelyControlled"));

            //On raffraichis la barre pour que le surrogate controlé y soit listé
            Find.ColonistBar.MarkColonistsDirty();

            //On duplique les prioritées du controleur sur le SX
            if (!externalController)
            {
                if (cp.workSettings != null && cp.workSettings.EverWork)
                {
                    foreach (var el in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                    {
                        controlled.workSettings.SetPriority(el, cp.workSettings.GetPriority(el));
                    }
                }
                controlled.playerSettings.AreaRestriction = cp.playerSettings.AreaRestriction;
                controlled.playerSettings.hostilityResponse = cp.playerSettings.hostilityResponse;

                //ON duplique l'agenda du controleur sur le SX
                for (int i = 0; i != 24; i++)
                {
                    controlled.timetable.SetAssignment(i, cp.timetable.GetAssignment(i));
                }
            }
        }

        /*
         * Check s'il y a au moin un SX de connecté a ce controller 
         */
        public bool isThereSX()
        {
            return SX != null || extraSX.Count > 0;
        }

        public void stopControlledSurrogate(Pawn surrogate, bool externalController=false, bool preventNoHost=false,bool noPrisonedSurrogateConversion=false)
        {
            //Log.Message("SetControlledSurrogate DEB");
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
                }
                if (SX != null)
                {
                    //Restauration nom SX
                    Utils.restoreSavedSurrogateName(cp);
                    Utils.PermutePawn(SX, cp);
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
                }
                else
                {
                    //Restauration nom SX
                    Utils.restoreSavedSurrogateName(cp);
                    Utils.PermutePawn(surrogate, cp);
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
                CompAndroidState cas = csurrogate.TryGetComp<CompAndroidState>();
                if (cas != null)
                    cas.surrogateController = null;

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
                        csurrogate.health.hediffSet.AddDirect(HediffMaker.MakeHediff(Utils.hediffNoHost, csurrogate, null));
                    else
                    {
                        csurrogate.health.AddHediff(Utils.hediffNoHost);
                    }
                }


                //On vire le potentiel Hediff de LowSignalSkyMind
                he = csurrogate.health.hediffSet.GetFirstHediffOfDef(Utils.hediffLowNetworkSignal);
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



                //Utils.disableGlobalKill = false;

                Utils.soundDefSurrogateConnectionStopped.PlayOneShot(null);

                //Si surrogate est prisonnier on le met comme non prisonnier et le controlleur comme prisonnier
                if (csurrogate.IsPrisoner && !noPrisonedSurrogateConversion)
                {
                    if (csurrogate.Faction != Faction.OfPlayer)
                    {
                        csurrogate.SetFaction(Faction.OfPlayer, null);
                    }

                    if (csurrogate.guest != null)
                    {
                        csurrogate.guest.SetGuestStatus(null, false);
                    }

                    if (cp.guest != null)
                    {
                        cp.guest.SetGuestStatus(Faction.OfPlayer, true);
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

                if (csurrogate.timetable != null)
                {
                    for (int i = 0; i != 24; i++)
                    {
                        csurrogate.timetable.SetAssignment(i, TimeAssignmentDefOf.Anything);
                    }
                }
            }
            //Log.Message("SetControlledSurrogate FIN");


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
            source.health.AddHediff(DefDatabase<HediffDef>.GetNamed("ATPP_ConsciousnessUpload"));
            dest.health.AddHediff(DefDatabase<HediffDef>.GetNamed("ATPP_ConsciousnessUpload"));

            int CGT = Find.TickManager.TicksGame;
            permuteRecipient = dest;
            permuteStartGT = CGT;
            permuteEndingGT = CGT + 60-(CGT%60) + Settings.mindPermutationHours * 2500;

            CompSurrogateOwner cso = dest.TryGetComp<CompSurrogateOwner>();
            cso.showPermuteProgress = true;
            cso.permuteRecipient = (Pawn)parent;

            Messages.Message("ATPP_StartPermute".Translate(source.LabelShortCap, dest.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);
        }

        private void OnDuplicateConfirmed(Pawn source, Pawn dest)
        {
            //Ajout hediff de transfert aux deux androids
            source.health.AddHediff(DefDatabase<HediffDef>.GetNamed("ATPP_ConsciousnessUpload"));
            dest.health.AddHediff(DefDatabase<HediffDef>.GetNamed("ATPP_ConsciousnessUpload"));

            int CGT = Find.TickManager.TicksGame;
            duplicateRecipient = dest;
            duplicateStartGT = CGT;
            duplicateEndingGT = CGT + 60-(CGT%60) + Settings.mindDuplicationHours * 2500;

            CompSurrogateOwner cso = dest.TryGetComp<CompSurrogateOwner>();
            cso.showDuplicateProgress = true;
            cso.duplicateRecipient = (Pawn)parent;

            //Log.Message("DUPLICATION STARTED !! ");
            Messages.Message("ATPP_StartDuplicate".Translate(source.LabelShortCap, dest.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);
        }

        private void OnMoveConsciousnessToSkyCloudCore(Pawn source, Building dest)
        {
            source.health.AddHediff(DefDatabase<HediffDef>.GetNamed("ATPP_ConsciousnessUpload"));

            int CGT = Find.TickManager.TicksGame;
            skyCloudRecipient = dest;
            uploadToSkyCloudStartGT = CGT;
            uploadToSkyCloudEndingGT = CGT + 60 - (CGT % 60) + Settings.mindUploadToSkyCloudHours * 2500;

            CompSkyCloudCore csc = dest.TryGetComp<CompSkyCloudCore>();

            Messages.Message("ATPP_StartSkyCloudUpload".Translate(source.LabelShortCap, csc.getName()), parent, MessageTypeDefOf.PositiveEvent);
        }

        private void OnMoveConsciousnessFromSkyCloudCore(Pawn source)
        {
            Pawn dest = (Pawn)parent;
            dest.health.AddHediff(DefDatabase<HediffDef>.GetNamed("ATPP_ConsciousnessUpload"));

            int CGT = Find.TickManager.TicksGame;
            skyCloudDownloadRecipient = source;
            downloadFromSkyCloudStartGT = CGT;
            downloadFromSkyCloudEndingGT = CGT + 60 - (CGT % 60) + Settings.mindUploadToSkyCloudHours * 2500;

            CompSkyCloudCore csc = source.TryGetComp<CompSkyCloudCore>();

            string name = source.LabelShortCap;
            CompSurrogateOwner cso = source.TryGetComp<CompSurrogateOwner>();

            if (cso != null && cso.isThereSX())
            {
                if(cso.SX != null)
                    name = cso.SX.LabelShortCap;
            }

            Messages.Message("ATPP_StartSkyCloudDownload".Translate(source.LabelShortCap, dest.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);
        }

        public void startMigration(Building dest)
        {
            CompSkyCloudCore csc2 = dest.TryGetComp<CompSkyCloudCore>();

            int CGT = Find.TickManager.TicksGame;
            migrationSkyCloudHostDest = dest;
            migrationStartGT = CGT;
            migrationEndingGT = CGT + 60 - (CGT % 60) + Settings.mindSkyCloudMigrationHours * 2500;

            Messages.Message("ATPP_StartSkyCloudMigration".Translate(((Pawn)parent).LabelShortCap, skyCloudHost.TryGetComp<CompSkyCloudCore>().getName(), csc2.getName()), parent, MessageTypeDefOf.PositiveEvent);
        }

        /*
         * Detecte un cas d'interruption est le cas echeant tue les protagoniste de l'upload tous en affichant un message d'erreur
         */
        public void checkInterruptedUpload()
        {
            bool killSelf = false;
            Pawn cpawn = (Pawn)parent;
           
            //Permutation ou duplication en cours on test si les conditions d'arret sont presentes
            if (permuteEndingGT != -1 || duplicateEndingGT != -1 || uploadToSkyCloudEndingGT != -1 || downloadFromSkyCloudEndingGT != -1 || mindAbsorptionEndingGT != -1 || migrationEndingGT != -1 || replicationEndingGT != -1)
            {
                bool permuteRecipientDead = false;
                if (permuteRecipient != null)
                    permuteRecipientDead = permuteRecipient.Dead;

                bool duplicateRecipientDead = false;
                if (duplicateRecipient != null)
                    duplicateRecipientDead = duplicateRecipient.Dead;

                bool recipientConnected = false;

                bool emitterConnected = false;

                if (permuteRecipient != null && Utils.GCATPP.isConnectedToSkyMind(permuteRecipient, true))
                    recipientConnected = true;

                if (duplicateRecipient != null && Utils.GCATPP.isConnectedToSkyMind(duplicateRecipient,true))
                    recipientConnected = true;

                if (Utils.GCATPP.isThereSkillServers())
                    recipientConnected = true;

                //L'équivalence du EST connecté sur le COre s'est si il est bien alimenté en elec
                if ( (skyCloudRecipient != null && skyCloudRecipient.TryGetComp<CompPowerTrader>().PowerOn)
                    || (replicationEndingGT != -1 && skyCloudHost.TryGetComp<CompPowerTrader>().PowerOn)
                    || (migrationSkyCloudHostDest != null && migrationSkyCloudHostDest.TryGetComp<CompPowerTrader>().PowerOn)
                    || (skyCloudDownloadRecipient != null && skyCloudDownloadRecipient.TryGetComp<CompSurrogateOwner>() != null && skyCloudDownloadRecipient.TryGetComp<CompSurrogateOwner>().skyCloudHost.TryGetComp<CompPowerTrader>().PowerOn))
                {
                    recipientConnected = true;
                }

                //L'équivalence en mode migration est le check de si le host du mind est branché
                if (Utils.GCATPP.isConnectedToSkyMind(cpawn,true)
                    || (replicationEndingGT != -1 && skyCloudHost.TryGetComp<CompPowerTrader>().PowerOn)
                    || (skyCloudHost != null && skyCloudHost.TryGetComp<CompPowerTrader>().PowerOn) )
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
                            CompSkyCloudCore csc = skyCloudHost.TryGetComp<CompSkyCloudCore>();
                            csc.replicatingMinds.Remove((Pawn)parent);
                            showMindUploadNotif = false;

                            Find.LetterStack.ReceiveLetter("ATPP_LetterInterruptedSkyCloudReplication".Translate(), "ATPP_LetterInterruptedSkyCloudReplicationDesc".Translate(cpawn.LabelShortCap), LetterDefOf.ThreatSmall);
                        }
                        else if (!recipientConnected || !emitterConnected)
                        {
                            reason = "ATPP_LetterInterruptedUploadDescCompDiconnectionError".Translate();

                            killSelf = true;
                            if (permuteEndingGT != -1)
                            {
                                if (permuteRecipient != null && !permuteRecipient.Dead)
                                {
                                    permuteRecipient.Kill(null, null);
                                }
                            }
                            else if (duplicateEndingGT != -1)
                            {
                                if (duplicateRecipient != null && !duplicateRecipient.Dead)
                                {
                                    duplicateRecipient.Kill(null, null);
                                }
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
                CompSurrogateOwner cso = duplicateRecipient.TryGetComp<CompSurrogateOwner>();
                cso.showDuplicateProgress = false;
                cso.duplicateRecipient = null;
            }

            if (permuteRecipient != null)
            {
                CompSurrogateOwner cso = permuteRecipient.TryGetComp<CompSurrogateOwner>();
                cso.showPermuteProgress = false;
                cso.permuteRecipient = null;
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
        public Building skyCloudRecipient;
        public Pawn permuteRecipient;
        public Pawn duplicateRecipient;

        public bool showPermuteProgress = false;
        public bool showDuplicateProgress = false;

        public TraitDef ransomwareTraitAdded;
        public int ransomwareSkillValue = -1;
        public SkillDef ransomwareSkillStolen;

        public bool externalController = true;

        public Building skyCloudHost;
        //Migration stuff related
        public Building migrationSkyCloudHostDest;
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
    }
}