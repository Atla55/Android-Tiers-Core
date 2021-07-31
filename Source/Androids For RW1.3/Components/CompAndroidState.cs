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
    public class CompAndroidState : ThingComp
    {
        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look<bool>(ref this.autoPaintStarted, "ATPP_autoPaintStarted", false);
            
            Scribe_Values.Look<bool>(ref this.connectedLWPNActive, "ATPP_connectedLWPNActive", false);
            Scribe_References.Look(ref this.connectedLWPN, "ATPP_connectedLWPN", false);
            Scribe_Values.Look<bool>(ref this.isBlankAndroid, "ATPP_isBlankAndroid", false);
            Scribe_Values.Look<bool>(ref this.showUploadProgress, "ATPP_showUploadProgress", false);
            Scribe_Values.Look<bool>(ref this.useBattery, "ATPP_useBattery", (Settings.defaultGeneratorMode==1)?false:true, true);
            Scribe_Values.Look<int>(ref this.uploadEndingGT, "ATPP_uploadEndingGT", -1);
            Scribe_Values.Look<int>(ref this.uploadStartGT, "ATPP_uploadStartGT", 0);
            Scribe_Values.Look<bool>(ref this.isSurrogate, "ATPP_isSurrogate", false);
            Scribe_References.Look(ref surrogateController, "ATPP_surrogateController");
            Scribe_References.Look(ref lastController, "ATPP_lastController");
            Scribe_Values.Look<string>(ref this.savedName, "ATPP_savedName", "");
            Scribe_Values.Look<int>(ref this.frameworkNaniteEffectGTEnd, "ATPP_frameworkNaniteEffectGTEnd", -1);
            Scribe_Values.Look<int>(ref this.frameworkNaniteEffectGTStart, "ATPP_frameworkNaniteEffectGTStart", -1);
            Scribe_Values.Look<int>(ref paintingRustGT, "ATPP_paintingRustGT", -2);
            Scribe_Values.Look<bool>(ref this.paintingIsRusted, "ATPP_paintingIsRusted", false);
            Scribe_Values.Look<bool>(ref this.downedViaDisconnect, "ATPP_downedViaDisconnect", true);
            Scribe_Values.Look<bool>(ref this.lastSkymindDisconnectIsManual, "ATPP_lastSkymindDisconnectIsManualCAS", false);
            Scribe_References.Look(ref droppedWeapon, "ATPP_droppedWeapon");

            Scribe_Values.Look<int>(ref batteryExplosionEndingGT, "ATPP_batteryExplosionEndingGT", -1);


            Scribe_Values.Look<int>(ref customColor, "ATPP_customColor", (int)AndroidPaintColor.Default);

            Scribe_Deep.Look<Pawn>(ref this.externalController, "ATPP_externalController", new object[0]);

            Scribe_References.Look<Pawn>(ref this.uploadRecipient, "ATPP_uploadRecipient", false);

            Scribe_Defs.Look<HairDef>(ref this.hair, "ATPP_hair");
        }

        public override void PostDraw()
        {
            Material avatar = null;
            Vector3 vector;

            if (uploadEndingGT != -1 || showUploadProgress)
                avatar = Tex.UploadInProgress;
            else if (Find.DesignatorManager.SelectedDesignator is Designator_AndroidToControl && isSurrogate && surrogateController == null && (csm != null && csm.Infected == -1))
                avatar = Tex.SelectableSX;
            else if (Find.DesignatorManager.SelectedDesignator is Designator_SurrogateToHack && isSurrogate && parent.Faction != Faction.OfPlayer)
                avatar = Tex.SelectableSXToHack;
            else if (isSurrogate && surrogateController != null && !Settings.hideRemotelyControlledDeviceIcon)
                avatar = Tex.RemotelyControlledNode;

            if (avatar != null)
            {
                vector = this.parent.TrueCenter();
                vector.y = Altitudes.AltitudeFor(AltitudeLayer.MetaOverlays) + 0.28125f;
                vector.z += 1.4f;
                vector.x += this.parent.def.size.x / 2;

                Graphics.DrawMesh(MeshPool.plane08, vector, Quaternion.identity, avatar, 0);
            }
        }

        public override void PostDrawExtraSelectionOverlays()

        {
            base.PostDrawExtraSelectionOverlays();

            //Dessin liaison entre controlleur et SX
            if ( (surrogateController != null && isSurrogate && surrogateController.Map == parent.Map)  )
            {
                GenDraw.DrawLineBetween(parent.TrueCenter(), surrogateController.TrueCenter(), SimpleColor.Blue);
            }

            CompSurrogateOwner cso = Utils.getCachedCSO(surrogateController);
            if (cso != null 
                && cso.skyCloudHost != null 
                && cso.skyCloudHost.Map == parent.Map)
            {
                GenDraw.DrawLineBetween(parent.TrueCenter(), cso.skyCloudHost.TrueCenter(), SimpleColor.Red);
            }

            if((uploadEndingGT != -1 && uploadRecipient != null) || showUploadProgress)
            {
                GenDraw.DrawLineBetween(parent.TrueCenter(), uploadRecipient.TrueCenter(), SimpleColor.Green);
            }
        }

        private void initComp()
        {
            if (init)
                return;

            init = true;
            currentPawn = (Pawn)parent;
            csm = Utils.getCachedCSM(currentPawn);
            isAndroidWithSkin = Utils.ExceptionAndroidWithSkinList.Contains(currentPawn.def.defName);
            dontRust = Utils.ExceptionAndroidsDontRust.Contains(currentPawn.def.defName);

            bool isAndroidTier = currentPawn.IsAndroidTier();
            isAndroidOrAnimalTier = currentPawn.IsAndroidOrAnimalTier();

            if (!isAndroidTier && !Utils.ExceptionAndroidAnimalPowered.Contains(currentPawn.def.defName))
            {
                isOrganic = true;
            }

            if (isOrganic)
                useBattery = false;

            Utils.GCATPP.pushSurrogateAndroid(currentPawn);

            //Suppression traits blacklistés le cas echeant
            if (isAndroidTier && (!isSurrogate || (isSurrogate && surrogateController != null && surrogateController.IsAndroidTier())))
                Utils.removeMindBlacklistedTrait(currentPawn);

            this.isAndroidTIer = isAndroidTier;

            checkInfectionFix();

            if (isAndroidTier)
            {
                //Remove ideo if basic android or surrogate
                if (currentPawn.IsBasicAndroidTier() || isSurrogate)
                {
                    currentPawn.ideo = null;
                }

                if (isAndroidWithSkin)
                {
                    if (currentPawn.gender == Gender.Male)
                    {
                        BodyTypeDef bd = DefDatabase<BodyTypeDef>.GetNamed("Male", false);
                        if (bd != null)
                            currentPawn.story.bodyType = bd;
                    }
                    else
                    {
                        BodyTypeDef bd = DefDatabase<BodyTypeDef>.GetNamed("Female", false);
                        if (bd != null)
                            currentPawn.story.bodyType = bd;
                    }
                }

                if (currentPawn.ownership != null && currentPawn.ownership.OwnedBed != null)
                {
                    if (currentPawn.ownership.OwnedBed.ForPrisoners != currentPawn.IsPrisoner)
                    {
                        currentPawn.ownership.UnclaimBed();
                    }
                }
                //Starting du délais de rusting
                if (!dontRust)
                {
                    if (paintingRustGT == -2)
                    {
                        paintingRustGT = (Rand.Range(Settings.minDaysAndroidPaintingCanRust, Settings.maxDaysAndroidPaintingCanRust) * 60000);
                    }

                    if (paintingRustGT == -1 && paintingIsRusted && currentPawn.health != null)
                    {
                        Hediff he = currentPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_Rusted);
                        if (he == null)
                        {
                            currentPawn.health.AddHediff(HediffDefOf.ATPP_Rusted);
                        }
                    }
                }
            }
            else
            {
                if (currentPawn.health != null)
                {
                    paintingIsRusted = false;
                    Hediff he = currentPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_Rusted);
                    if (he != null)
                        currentPawn.health.RemoveHediff(he);
                }

                Pawn cpawn = currentPawn;

                //Si VX0 dans une session en cours alors on chope le pawn permuté controleur
                if (surrogateController != null)
                    cpawn = surrogateController;

                //Reset du child et adulthood si VX0 organic
                if (isSurrogate && isOrganic && cpawn.story != null && cpawn.story.adulthood != null)
                {
                    if (cpawn.story.childhood != null)
                    {
                        Backstory bs = null;

                        BackstoryDatabase.TryGetWithIdentifier("MercenaryRecruit", out bs);
                        if (bs != null)
                            cpawn.story.childhood = bs;
                    }

                    cpawn.story.adulthood = null;
                    //Reset incapable of
                    Utils.ResetCachedIncapableOf(cpawn);
                }

            }

            if (Utils.POWERPP_LOADED)
            {
                if (connectedLWPN == null)
                    connectedLWPNActive = false;
                else
                {
                    connectedLWPNCPT = Utils.getCachedCPT(connectedLWPN);
                }
            }

            addLowSignalHediff();
            addSolarFlareImpactHediff();
            checkTXWithSkinFacialTextureUpdate();

            if (uploadRecipient != null)
                uploadRecipientCAS = Utils.getCachedCAS(uploadRecipient);
            if (surrogateController != null)
                surrogateControllerCAS = Utils.getCachedCAS(surrogateController);

            //Reconexion auto au LWPN le cas echeant
            if (Utils.POWERPP_LOADED)
            {
                if (connectedLWPN != null && connectedLWPNActive)
                {
                    if (!Utils.GCATPP.pushLWPNAndroid(connectedLWPN, (Pawn)parent))
                    {
                        connectedLWPNActive = false;
                    }
                }
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            //Init for pawns in caravans
            if (!init)
            {
                initComp();
            }

        }

        public override void CompTick()
        {
            base.CompTick();
            if (parent.Map == null || !parent.Spawned)
                return;

            int GT = Find.TickManager.TicksGame;

            if (!init)
            {
                initComp();
            }


            //Reconnection auto de l'etranger à son surrogate que si pas de solar flare en cours et toujours dans un Lord (Si le cas d'un ennemis check de l'etat de son Lord)
            if ( (GT % 120 == 0))
            {
                checkUploadInterruptStuffTick(GT);

                //Check android battery explosion (overload)
                if (batteryExplosionEndingGT != -1 && batteryExplosionEndingGT < GT)
                {
                    Utils.makeAndroidBatteryOverload(currentPawn);
                    return;
                }
                checkNaniteApplyEffectEnd(GT);
                checkExternalControllerReconnection();
            }

            if(GT % 300 == 0)
            {
                checkInfectionFix();
                checkTXWithSkinFacialTextureUpdate();
                reloadBatteryTick();
                checkRusted();
                checkBlankAndroid();
                powerPPStuffTick();
            }
        }

        /*
         * If the surrogateis is controlled by an external controller and disconnected from it, check if we can reconnect him and re-add him into the lord
         */
        public void checkExternalControllerReconnection()
        {
            if (externalController != null)
            {
                //Disconnection of externalControler if he is connected and there is a solarFlare (SkyMind infrastructures powered-off)
                if (surrogateController != null && Find.World.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare))
                {
                    CompSurrogateOwner cso = Utils.getCachedCSO(externalController);
                    cso.stopControlledSurrogate((Pawn)parent, true);
                }
                else if (surrogateController == null
                 && csm != null && csm.hacked != 3
                     //&& !externalController.Faction.HostileTo(Faction.OfPlayer)
                     && !Find.World.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare))
                {
                    Lord lordInvolved = null;
                    if (currentPawn.Map.mapPawns.SpawnedPawnsInFaction(currentPawn.Faction).Any((Pawn p) => p != currentPawn))
                    {
                        Pawn p2 = (Pawn)GenClosest.ClosestThing_Global(currentPawn.Position, currentPawn.Map.mapPawns.SpawnedPawnsInFaction(currentPawn.Faction), 99999f, (Thing p) => p != currentPawn && ((Pawn)p).GetLord() != null, null);
                        lordInvolved = p2.GetLord();
                    }
                    if (lordInvolved == null && !(currentPawn.IsPrisoner || currentPawn.IsSlave))
                    {
                        LordJob_ExitMapBest lordJob = new LordJob_ExitMapBest();
                        lordInvolved = LordMaker.MakeNewLord(currentPawn.Faction, lordJob, Find.CurrentMap, null);
                    }


                    //If non-player controller of the surrogate id dead OR hacked surrogate had one when it still exists but it is no longer active
                    if (externalController.Dead || (csm != null && csm.hackOrigFaction.HostileTo(Faction.OfPlayer) && lordInvolved == null && !currentPawn.IsPrisoner))
                    {
                        //Addition of NoHost because as in externalController mode we have not reset the hediff to avoid the weird bug that when attempting to integrate enemies hacked has a lord its bug when it was down
                        addNoRemoteHostHediff();
                        externalController = null;
                    }
                    else
                    {
                        try
                        {
                            //Try auto-reconnect to surrogate to his external controller (with this time the connection init malus)
                            CompSurrogateOwner cso = Utils.getCachedCSO(externalController);
                            cso.setControlledSurrogate((Pawn)parent, true, true);
                            currentPawn.mindState.Reset();
                            currentPawn.mindState.duty = null;
                            currentPawn.jobs.StopAll();
                            currentPawn.jobs.ClearQueuedJobs();
                            currentPawn.ClearAllReservations();
                            if (currentPawn.drafter != null)
                                currentPawn.drafter.Drafted = false;

                            if (lordInvolved != null && !currentPawn.Downed)
                                lordInvolved.AddPawn(currentPawn);
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            //If the lord is a siege lord, then proceed to some custom adaptations
                            if (lordInvolved != null && lordInvolved.CurLordToil is LordToil_Siege)
                            {
                                LordToil_Siege st = (LordToil_Siege)lordInvolved.CurLordToil;

                                //Job defender attribution to the pawn
                                Pawn p = (Pawn)parent;
                                //Traverse.Create( st ).Method("SetAsDefender").GetValue((Pawn)parent);
                                LordToilData_Siege data = (LordToilData_Siege)Traverse.Create(st).Property("Data").GetValue();
                                p.mindState.duty = new PawnDuty(DutyDefOf.Defend, data.siegeCenter, -1f);
                                p.mindState.duty.radius = data.baseRadius;
                                st.UpdateAllDuties();
                            }
                        }
                        catch (Exception)
                        {

                        }
                        //Log.Message("Current duty ==>"+cp.mindState.duty.def.defName);
                        //Log.Message("Current job ==>" + cp.CurJobDef.defName);

                        //Ordering surrogate to go search his dropped weapon (if applicable)
                        if(droppedWeapon != null)
                        {
                            if(currentPawn.CanReach(droppedWeapon, PathEndMode.OnCell, Danger.Deadly))
                            {
                                if (!currentPawn.CanReserve(droppedWeapon))
                                    currentPawn.Map.reservationManager.ReleaseAllForTarget(droppedWeapon);

                                Job job = JobMaker.MakeJob(JobDefOf.Equip, droppedWeapon);
                                currentPawn.jobs.StartJob(job, JobCondition.None);
                            }
                            droppedWeapon = null;
                        }
                    }
                }
            }
        }

        /*
         * Check if the android reached some nanite effecter
         */
        public void checkNaniteApplyEffectEnd(int GT)
        {
            if (frameworkNaniteEffectGTEnd != -1 && GT >= frameworkNaniteEffectGTEnd && !currentPawn.Dead)
            {
                bool chance = false;
                int nb = 0;

                //Chance que nanite fail
                if (!Rand.Chance(Settings.percentageNanitesFail))
                {
                    //Le cas echeant on enleve le rusting
                    clearRusted();

                    nb = currentPawn.health.hediffSet.hediffs.RemoveAll((Hediff h) => (Utils.AndroidOldAgeHediffFramework.Contains(h.def.defName)));
                    nb += currentPawn.health.hediffSet.hediffs.RemoveAll((Hediff h) => (h.def == HediffDefOf.MissingBodyPart || (Utils.ExceptionRepairableFrameworkHediff.Contains(h.def) && h.IsPermanent())));
                    if (nb > 0)
                    {
                        Utils.refreshHediff(currentPawn);
                    }
                    chance = true;
                }

                if (nb == 0)
                {
                    if (chance)
                        Messages.Message("ATPP_NoBrokenStuffFound".Translate(currentPawn.LabelShort), currentPawn, MessageTypeDefOf.NegativeEvent, true);
                    else
                        Messages.Message("ATPP_BrokenStuffRepairFailed".Translate(currentPawn.LabelShort), currentPawn, MessageTypeDefOf.NegativeEvent, true);
                }
                else
                    Messages.Message("ATPP_BrokenFrameworkRepaired".Translate(currentPawn.LabelShort), currentPawn, MessageTypeDefOf.PositiveEvent, true);


                frameworkNaniteEffectGTEnd = -1;
                frameworkNaniteEffectGTStart = -1;
            }
        }

        /*
         * Check mind upload stuff (if interrupted or if success)
         */
        public void checkUploadInterruptStuffTick(int GT)
        {
            if (uploadEndingGT != -1)
            {
                checkInterruptedUpload();

                //Atteinte d'un chargement d'upload de conscience
                if (uploadRecipient != null && uploadEndingGT != -1 && uploadEndingGT < GT)
                {
                    uploadEndingGT = -1;
                    CompAndroidState cas = Utils.getCachedCAS(uploadRecipient);
                    cas.uploadEndingGT = -1;

                    Utils.removeUploadHediff(currentPawn, uploadRecipient);

                    Find.LetterStack.ReceiveLetter("ATPP_LetterUploadOK".Translate(), "ATPP_LetterUploadOKDesc".Translate(currentPawn.LabelShortCap, uploadRecipient.LabelShortCap), LetterDefOf.PositiveEvent, parent);

                    if (currentPawn.def.defName == Utils.T1 && uploadRecipient.def.defName != Utils.T1)
                        Utils.removeSimpleMindedTrait(currentPawn);
                    else
                        Utils.addSimpleMindedTraitForT1(uploadRecipient);

                    //On realise effectivement la permutation puis le kill de la source
                    Utils.PermutePawn(currentPawn, uploadRecipient);

                    Utils.clearBlankAndroid(uploadRecipient);

                    //Report du blankAndroid pour le flagger dans la routine de kill
                    isBlankAndroid = true;


                    //Si destinataire de la duplication prisonnier Et emetteur pas prisonier on enleve la condition 
                    if (!currentPawn.IsPrisoner && uploadRecipient.IsPrisoner)
                    {
                        if (uploadRecipient.Faction != Faction.OfPlayer)
                        {
                            uploadRecipient.SetFaction(Faction.OfPlayer, null);
                        }

                        if (uploadRecipient.guest != null)
                        {
                            uploadRecipient.guest.SetGuestStatus(null);
                        }
                    }

                    //SI destinataire de la duplication colon regular et emetteur prisonnier 
                    if (currentPawn.IsPrisoner && !uploadRecipient.IsPrisoner)
                    {
                        if (uploadRecipient.Faction != currentPawn.Faction)
                        {
                            uploadRecipient.SetFaction(currentPawn.Faction, null);
                        }

                        if (uploadRecipient.guest != null)
                        {
                            if (uploadRecipient.IsSlave)
                                uploadRecipient.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
                            else
                                uploadRecipient.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Prisoner);
                        }
                    }

                    if (!currentPawn.Dead)
                        currentPawn.Kill(null, null);


                    resetUploadStuff();
                }
            }
        }


        /*
         * If loaded, do Power++ stuff by tick
         */
        public void powerPPStuffTick()
        {
            if (Utils.POWERPP_LOADED)
            {
                //Tentative de reco auto
                if (!connectedLWPNActive && connectedLWPN != null)
                {
                    if (Utils.GCATPP.pushLWPNAndroid(connectedLWPN, currentPawn))
                        connectedLWPNActive = true;
                }
            }
        }

        /*
         * Reaload the energy (food) need if android resting in an valid android pod
         */
        public void reloadBatteryTick()
        {
            if (csm != null && csm.Infected == -1)
            {
                //Recharge surrogate
                if (Utils.androidIsValidPodForCharging(currentPawn) && !isOrganic)
                {
                    //cpawn.needs.food.CurLevel = cpawn.needs.food.MaxLevel;
                    currentPawn.needs.food.CurLevelPercentage += Settings.percentageOfBatteryChargedEach6Sec;
                    Utils.throwChargingMote(currentPawn);
                }
                if (isSurrogate && surrogateController == null)
                    addNoRemoteHostHediff();
            }
        }

        /*
         * Essentially usefull to fix visual bugged state of lite virused androids (correct cases where the patch is not executed at the end of the mental break and the state not cleared)
         */
        public void checkInfectionFix()
        {
            Pawn cp = (Pawn)parent;

            if (csm != null && csm.Infected == 4 && !cp.InMentalState)
            {
                csm.Infected = -1;
                Hediff he = cp.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_NoHost);
                if (he == null)
                {
                    cp.health.AddHediff(HediffDefOf.ATPP_NoHost);
                }
            }
        }

        public void checkTXWithSkinFacialTextureUpdate()
        {
            try
            {
                Pawn cp = (Pawn)parent;


                if (isAndroidWithSkin)
                {
                    Utils.lastResolveAllGraphicsHeadGraphicPath = null;

                    //Changement tete
                    if ((!TXHurtedHeadSet && (cp.health.summaryHealth.SummaryHealthPercent <= 0.85f && cp.health.summaryHealth.SummaryHealthPercent > 0.45f)) || forcedDamageLevel == 1)
                    {
                        TXHurtedHeadSet = true;

                        if (TXHurtedHeadSet2)
                        {
                            TXHurtedHeadSet2 = false;
                            if(hair != null)
                                cp.story.hairDef = hair;
                            hair = null;
                        }

                        Utils.changeTXBodyType(cp, 1);
                        Utils.changeHARCrownType(cp, "Average_Hurted");
                        cp.Drawer.renderer.graphics.ResolveAllGraphics();
                        PortraitsCache.SetDirty(cp);
                    }
                    else if ((!TXHurtedHeadSet2 && (cp.health.summaryHealth.SummaryHealthPercent <= 0.45f)) || forcedDamageLevel == 2)
                    {
                        TXHurtedHeadSet = false;
                        TXHurtedHeadSet2 = true;
                        if(hair == null)
                            hair = cp.story.hairDef;
                        cp.story.hairDef = DefDatabase<HairDef>.GetNamed("Shaved",false);
                        Utils.changeTXBodyType(cp, 2);
                        Utils.changeHARCrownType(cp, "Average_Hurted2");
                        cp.Drawer.renderer.graphics.ResolveAllGraphics();
                        PortraitsCache.SetDirty(cp);
                    }
                    else
                    {
                        if ((TXHurtedHeadSet || !init) && cp.health.summaryHealth.SummaryHealthPercent > 0.85f )
                        {
                            TXHurtedHeadSet = false;
                            TXHurtedHeadSet2 = false;
                            if (hair != null)
                            {
                                cp.story.hairDef = hair;
                                hair = null;
                            }
                            Utils.changeTXBodyType(cp, 0);
                            Utils.changeHARCrownType(cp, "Average_Normal");
                            cp.Drawer.renderer.graphics.ResolveAllGraphics();
                            PortraitsCache.SetDirty(cp);
                        }
                        else if ((TXHurtedHeadSet2 || !init) && cp.health.summaryHealth.SummaryHealthPercent > 0.45f)
                        {
                            TXHurtedHeadSet2 = false;
                            TXHurtedHeadSet = false;

                            if (cp.health.summaryHealth.SummaryHealthPercent <= 0.85f)
                                TXHurtedHeadSet = true;

                            cp.story.hairDef = hair;
                            hair = null;
                            if (cp.health.summaryHealth.SummaryHealthPercent <= 0.85f)
                            {
                                Utils.changeHARCrownType(cp, "Average_Hurted");
                                Utils.changeTXBodyType(cp, 1);
                            }
                            else
                            {
                                Utils.changeHARCrownType(cp, "Average_Normal");
                                Utils.changeTXBodyType(cp, 0);
                            }
                            cp.Drawer.renderer.graphics.ResolveAllGraphics();
                            PortraitsCache.SetDirty(cp);
                        }
                        //(string)Traverse.Create(p1.story).Field("headGraphicPath").GetValue();
                    }

                    if(Utils.RIMMSQOL_LOADED && Utils.lastResolveAllGraphicsHeadGraphicPath != null)
                    {
                        cp.story.GetType().GetField("headGraphicPath", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(cp.story, Utils.lastResolveAllGraphicsHeadGraphicPath);
                        Utils.lastResolveAllGraphicsHeadGraphicPath = null;
                        /*cp.Drawer.renderer.graphics.ResolveAllGraphics();
                        PortraitsCache.SetDirty(cp);*/
                    }
                }
            }
            catch(Exception e)
            {
                Log.Message("[ATPP] CompAndroidState.checkTXWithSkinFacialTextureUpdate " + e.Message + " " + e.StackTrace);
            }
        }

        public void checkBlankAndroid()
        {
            Pawn cp = (Pawn)parent;

            if (!cp.Dead && isBlankAndroid)
            {
                Hediff he = cp.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_BlankAndroid);
                if (he == null && cp.health != null)
                    cp.health.AddHediff(he);
            }
        }

        public void checkRusted()
        {
            try
            {
                Pawn cp = (Pawn)parent;

                //Entitées qui ne rust pas on degage et check avant de faire le menage des rust malplacés
                if (!isAndroidTIer || isAndroidWithSkin || dontRust)
                {
                    Hediff he = cp.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_Rusted);
                    if (he != null)
                        cp.health.RemoveHediff(he);

                    return;
                }


                if (!Settings.androidsCanRust)
                {
                    if (paintingIsRusted)
                    {
                        paintingIsRusted = false;
                        paintingRustGT = -3;    
                    }

                    Hediff he = cp.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_Rusted);
                    if (he != null)
                        cp.health.RemoveHediff(he);

                }
                else
                {
                    //Reprise de la rouille interrompue
                    if (paintingRustGT == -3 && !paintingIsRusted)
                    {
                        setRusted();
                    }

                    if (paintingRustGT != -1)
                    {
                        paintingRustGT -= 300;
                        if (paintingRustGT < 0)
                            paintingRustGT = 0;
                    }

                    //Lancement paint auto 1jour avant la fin d'expiration du timeout
                    if (Settings.allowAutoRepaint && (!cp.IsPrisoner || Settings.allowAutoRepaintForPrisoners) && !autoPaintStarted && paintingRustGT <= 60000)
                    {
                        //Déduction recipeDef
                        AndroidPaintColor color = (AndroidPaintColor)customColor;
                        string paintRecipeDefname = "";

                        switch (color)
                        {
                            case AndroidPaintColor.Black:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkBlack";
                                break;
                            case AndroidPaintColor.Blue:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkBlue";
                                break;
                            case AndroidPaintColor.Cyan:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkCyan";
                                break;
                            case AndroidPaintColor.None:
                            case AndroidPaintColor.Default:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkDefault";
                                break;
                            case AndroidPaintColor.Gray:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkGray";
                                break;
                            case AndroidPaintColor.Green:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkGreen";
                                break;
                            case AndroidPaintColor.Khaki:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkKhaki";
                                break;
                            case AndroidPaintColor.Orange:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkOrange";
                                break;
                            case AndroidPaintColor.Pink:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkPink";
                                break;
                            case AndroidPaintColor.Purple:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkPurple";
                                break;
                            case AndroidPaintColor.Red:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkRed";
                                break;
                            case AndroidPaintColor.White:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkWhite";
                                break;
                            case AndroidPaintColor.Yellow:
                                paintRecipeDefname = "ATPP_PaintAndroidFrameworkYellow";
                                break;
                        }

                        RecipeDef recipe = DefDatabase<RecipeDef>.GetNamed(paintRecipeDefname, false);
                        if (recipe != null)
                        {
                            //Renouvellement auto de la peinture (ajout operation auto)
                            cp.health.surgeryBills.AddBill(new Bill_Medical(recipe));
                            autoPaintStarted = true;
                        }
                    }

                    //Rouille de la peinture ?
                    if (paintingRustGT == 0 || (paintingRustGT == -1 && cp.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_Rusted) == null))
                    {
                        paintingIsRusted = true;
                        cp.Drawer.renderer.graphics.ResolveAllGraphics();
                        PortraitsCache.SetDirty(cp);
                        cp.health.AddHediff(HediffDefOf.ATPP_Rusted);

                        paintingRustGT = -1;
                    }
                    else
                    {
                        if (!paintingIsRusted)
                        {
                            //Cas aberrant (possede hediff de rusted alors que pas rusted)
                            Hediff cRusted = cp.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_Rusted);
                            if (cRusted != null)
                            {
                                cp.health.RemoveHediff(cRusted);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Message("[ATPP] CompAndroidState.CheckRusted "+e.Message+" "+e.StackTrace);
            }
        }
        
        public void clearPPPState()
        {
            connectedLWPN = null;
            connectedLWPNActive = false;
        }

        public void setRusted()
        {
            Pawn cp = (Pawn)parent;

            paintingIsRusted = true;
            paintingRustGT = -1;
            cp.health.AddHediff(HediffDefOf.ATPP_Rusted);
        }

        public void clearRusted()
        {
            Pawn pawn = (Pawn)parent;

            paintingIsRusted = false;
            autoPaintStarted = false;
            paintingRustGT = (Rand.Range(Settings.minDaysAndroidPaintingCanRust, Settings.maxDaysAndroidPaintingCanRust) * 60000);

            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            if (Find.ColonistBar != null)
            {
                PortraitsCache.SetDirty(pawn);
            }


            //Retire du hediff de rouille
            Hediff he = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_Rusted);
            if (he != null)
                pawn.health.RemoveHediff(he);
        }

        /*public void checkSolarFlareStuff()
        {
            //Androids avec une peau pas affectés par le solarflare
            if (Settings.disableSolarFlareEffect || currentPawn.health == null || currentPawn.def.defName == Utils.TX2 || currentPawn.def.defName == Utils.TX3 || currentPawn.def.defName == Utils.TX4)
                return;

            if (!isOrganic || currentPawn.VXAndVX0ChipPresent())
            {
                bool solarFlareRunning = Find.World.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare);

                if (currentPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_SolarFlareAndroidImpact) == null)
                    currentPawn.health.AddHediff(HediffDefOf.ATPP_SolarFlareAndroidImpact);
            }
        }*/

        public void addSolarFlareImpactHediff()
        {
            if (!(isAndroidOrAnimalTier ||  currentPawn.VXAndVX0ChipPresent()))
                return;
            Hediff he = currentPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_SolarFlareAndroidImpact);
            //Remove previous AT2.x SolarFlare hediff
            if (he != null && !(he is Hediff_SolarFlare))
            {
                currentPawn.health.RemoveHediff(he);
                he = null;
            }

            if (he == null)
                currentPawn.health.AddHediff(HediffDefOf.ATPP_SolarFlareAndroidImpact);
        }

        public void addLowSignalHediff()
        {
            if (!isSurrogate || currentPawn.NonHumanlikeOrWildMan())
                return;

            Hediff he = currentPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_LowNetworkSignal);
            //Remove previous AT2.x LowNetworkSignal hediff
            if (he != null && !(he is Hediff_LowNetworkSignal))
            {
                currentPawn.health.RemoveHediff(he);
                he = null;
            }

            if (he == null)
                currentPawn.health.AddHediff(HediffDefOf.ATPP_LowNetworkSignal);
        }

        public void addNoRemoteHostHediff()
        {
            //Check si surrogate et pas de controlleur ET possede pas de noHost alors on l'ajoute (===> effet d'un item externe cleanant les heddifs)
            if (currentPawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_NoHost) == null)
                currentPawn.health.AddHediff(HediffDefOf.ATPP_NoHost);
        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            
        }


        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);

            if(Utils.POWERPP_LOADED && connectedLWPN != null)
            {
                if (connectedLWPNActive)
                    Utils.GCATPP.popLWPNAndroid(connectedLWPN,(Pawn)parent);

                connectedLWPN = null;
                connectedLWPNCPT = null;
                connectedLWPNActive = false;
            }

            //Si surrogate on notifis le changement de map de ce dernier pour qu'il soit correctement traqué
            if (isSurrogate)
            {
                Pawn pawn = (Pawn)parent;
                Utils.GCATPP.pushSurrogateAndroid((Pawn)this.parent);

                Utils.removeDownedSurrogateToLister(pawn);
            }
        }





        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            Utils.removeUploadHediff((Pawn)parent, uploadRecipient);

            if (uploadEndingGT != -1)
                checkInterruptedUpload();

            /*if (isSurrogate)
                Utils.GCATPP.popSurrogateAndroid((Pawn)parent);*/

            if (isSurrogate && previousMap == null)
            {
                Utils.GCATPP.pushSurrogateAndroid((Pawn)this.parent);
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

                    //On va  invoquer le checkInterruption pour les duplicate et permutation 
                    checkInterruptedUpload();
                    break;
            }
        }

        private bool isRegularM7()
        {
            //Les M7Mech standard ne sont pas controlables
            return (!isSurrogate && isM7());
        }

        private bool isM7()
        {
            //Les M7Mech standard ne sont pas controlables
            return parent.def.defName == "M7Mech";
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Pawn pawn = (Pawn)parent;
            bool isPrisoner = pawn.IsPrisoner;
            bool transfertAllowed = Utils.mindTransfertsAllowed((Pawn)parent);

            //Si androide virusé (hacking) ajout boutton permettant de le shutdown
            if(csm != null && csm.Hacked == 1)
            {
                yield return new Command_Action
                {
                    icon = Tex.StopVirused,
                    defaultLabel = "ATPP_StopVirused".Translate(),
                    defaultDesc = "ATPP_StopVirusedDesc".Translate(),
                    action = delegate ()
                    {
                        pawn.Kill(null, null);
                    }
                };

                yield break;
            }

            //Prevent battery overload on organic, only player's pawn allowed but disabled for temp hacked androids
            if (!isOrganic && pawn.Faction == Faction.OfPlayer && (csm == null || csm.hacked != 3))
            {
                //Ajout possibilité de lancer l'explosion d'un androide a distance

                if (Utils.ResearchAndroidBatteryOverload.IsFinished)
                {
                    Texture2D tex = Tex.ForceAndroidToExplode;

                    if (batteryExplosionEndingGT != -1)
                        tex = Tex.ForceAndroidToExplodeDisabled;

                    yield return new Command_Action
                    {
                        icon = tex,
                        defaultLabel = "ATPP_OverloadAndroid".Translate(),
                        defaultDesc = "ATPP_OverloadAndroidDesc".Translate(),
                        action = delegate ()
                        {
                            if (batteryExplosionEndingGT != -1)
                                return;
                            Find.WindowStack.Add(new Dialog_Msg("ATPP_UploadMakeAndroidBatteryOverloadConfirm".Translate(), "ATPP_UploadMakeAndroidBatteryOverloadConfirmDesc".Translate(), delegate
                            {
                                batteryExplosionStartingGT = Find.TickManager.TicksGame;
                                batteryExplosionEndingGT = batteryExplosionStartingGT + 930;
                            }, false));
                        }
                    };
                }

                //Si POWER++ chargé ajout possibilité de rattaché android à un LWPN
                if (Utils.POWERPP_LOADED && useBattery)
                {
                    Texture2D tex = Tex.LWPNConnected;
                    
                    if (connectedLWPN == null || !connectedLWPNActive || connectedLWPN.Destroyed || connectedLWPNCPT == null || !connectedLWPNCPT.PowerOn)
                        tex = Tex.LWPNNotConnected;

                    yield return new Command_Action
                    {
                        icon = tex,
                        defaultLabel = "ARKPPP_LWPSel".Translate(),
                        defaultDesc = "",
                        action = delegate ()
                        {
                            List<FloatMenuOption> opts = new List<FloatMenuOption>();
                            FloatMenu floatMenuMap;

                            foreach (var build in parent.Map.listerBuildings.allBuildingsColonist)
                            {
                                CompPowerTrader cpt2 = Utils.getCachedCPT(build);
                                if ( (build.def.defName == "ARKPPP_LocalWirelessPowerEmitter" || build.def.defName == "ARKPPP_LocalWirelessPortablePowerEmitter") 
                                    && !build.IsBrokenDown()
                                    && cpt2.PowerOn)
                                {
                                    ThingComp compLWPNEmitter = Utils.TryGetCompByTypeName(build, "CompLocalWirelessPowerEmitter", "Power++");
                                    if (compLWPNEmitter != null)
                                    {
                                        string lib = getConnectedLWPNLabel(build);

                                        opts.Add(new FloatMenuOption("ARKPPP_WPNListRow".Translate(lib,((int)Utils.getCurrentAvailableEnergy(build.PowerComp.PowerNet)).ToString()), delegate
                                        {
                                            if(connectedLWPN  != null)
                                            {
                                                Utils.GCATPP.popLWPNAndroid(connectedLWPN, pawn);
                                                connectedLWPNActive = false;
                                                connectedLWPNCPT = null;
                                                connectedLWPN = null;
                                            }

                                            if (Utils.GCATPP.pushLWPNAndroid(build, pawn))
                                            {
                                                connectedLWPN = build;
                                                connectedLWPNCPT = Utils.getCachedCPT(connectedLWPN);
                                                connectedLWPNActive = true;
                                            }
                                            else
                                            {
                                                Messages.Message("ATPP_MessageLWPNNoSlotAvailable".Translate(),MessageTypeDefOf.NegativeEvent);
                                            }

                                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                                    }
                                }
                            }

                            //SI pas choix affichage de la raison 
                            if (opts.Count == 0)
                                opts.Add(new FloatMenuOption("ATPP_NoAvailableLWPN".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));

                            //Si le recepteur est configuré pour se connecter a un LWPN définis on ajoute une option de deconnexion
                            if (connectedLWPN != null)
                            {
                                opts.Add(new FloatMenuOption("ARKPPP_ClearCurrentWPNConnection".Translate(), delegate
                                {
                                    if(connectedLWPN != null)
                                        Utils.GCATPP.popLWPNAndroid(connectedLWPN, (Pawn)parent);

                                    connectedLWPN = null;
                                    connectedLWPNCPT = null;
                                    connectedLWPNActive = false;

                                }, MenuOptionPriority.Default, null, null, 0f, null, null));
                            }

                            floatMenuMap = new FloatMenu(opts, "ARKPPP_LocalWirelessPowerSelectorListTitle".Translate());
                            Find.WindowStack.Add(floatMenuMap);
                        }
                    };
                }
            }

            if (!isM7())
            {

                if (!isOrganic)
                {
                    yield return new Command_Toggle
                    {
                        icon = Tex.Battery,
                        defaultLabel = "ATPP_UseBattery".Translate(),
                        defaultDesc = "ATPP_UseBatteryDesc".Translate(),
                        isActive = (() => useBattery),
                        toggleAction = delegate ()
                        {
                            useBattery = !useBattery;
                            if(!useBattery && connectedLWPNActive)
                            {
                                Utils.GCATPP.popLWPNAndroid(connectedLWPN, (Pawn)parent);
                                connectedLWPN = null;
                                connectedLWPNActive = false;
                            }
                        }
                    };
                }


                if (!Utils.GCATPP.isConnectedToSkyMind(pawn) || isPrisoner)
                    yield break;

                bool uploadInProgress = showUploadProgress || uploadEndingGT != -1;

                if (!isOrganic && !isSurrogate && Utils.anySkyMindNetResearched())
                {
                    Texture2D selTex = Tex.UploadConsciousness;


                    if (!transfertAllowed)
                        selTex = Tex.UploadConsciousnessDisabled;

                    yield return new Command_Action
                    {
                        icon = selTex,
                        defaultLabel = "ATPP_UploadConsciousness".Translate(),
                        defaultDesc = "ATPP_UploadConsciousnessDesc".Translate(),
                        action = delegate ()
                        {
                            if (!transfertAllowed)
                                return;
                            Utils.ShowFloatMenuAndroidCandidate((Pawn)parent, delegate (Pawn target)
                            {
                                Find.WindowStack.Add(new Dialog_Msg("ATPP_UploadConsciousnessConfirm".Translate(parent.LabelShortCap, target.LabelShortCap), "ATPP_UploadConsciousnessConfirmDesc".Translate(parent.LabelShortCap, target.LabelShortCap) + "\n" + ("ATPP_WarningSkyMindDisconnectionRisk").Translate(), delegate
                                    {
                                        OnPermuteConfirmed((Pawn)parent, target);
                                    }, false));
                            });
                        }
                    };
                }
            }

            //Permet de deconnecter l'utilisateur connecté sur le robot
            if(isSurrogate )
            {
                if (surrogateController != null)
                {
                    yield return new Command_Action
                    {
                        icon = Tex.AndroidToControlTargetDisconnect,
                        defaultLabel = "ATPP_AndroidToControlTargetDisconnect".Translate(),
                        defaultDesc = "ATPP_AndroidToControlTargetDisconnectDesc".Translate(),
                        action = delegate ()
                        {
                            CompSurrogateOwner cso = Utils.getCachedCSO(surrogateController);
                            if (cso != null)
                                cso.disconnectControlledSurrogate((Pawn)parent);
                        }
                    };
                }
                else
                {
                    if (lastController != null)
                    { 
                        //Permet au surrogate de se relier au dernier controller
                        yield return new Command_Action
                        {
                            icon = Tex.AndroidSurrogateReconnectToLastController,
                            defaultLabel = "ATPP_AndroidSurrogateReconnectToLastController".Translate(),
                            defaultDesc = "ATPP_AndroidSurrogateReconnectToLastControllerDesc".Translate(),
                            action = delegate ()
                            {
                                Pawn cSurrogate = (Pawn)parent;
                                if (lastController == null || lastController.Dead)
                                {
                                    Messages.Message("ATPP_CannotReconnectToLastSurrogateController".Translate(), MessageTypeDefOf.NegativeEvent);
                                    return;
                                }

                                bool VX3Owner = lastController.VX3ChipPresent();
                                CompSurrogateOwner cso = Utils.getCachedCSO(lastController);
                                if (cso != null)
                                {
                                    bool isMind = false;
                                    //Check so lastController est un mind dans ce cas check qu'il ne fait pas deja autre chose
                                    if (cso.skyCloudHost != null)
                                    {
                                        isMind = true;
                                        CompSkyCloudCore csc = Utils.getCachedCSC(cso.skyCloudHost);
                                        if (csc == null || csc.mindIsBusy(lastController))
                                        {
                                            Messages.Message("ATPP_CannotReconnectToLastSurrogateController".Translate(), MessageTypeDefOf.NegativeEvent);
                                            return;
                                        }
                                    }

                                    //Si controller deconnecté tenttive reconnection au SkyMind
                                    bool isConnected = true;
                                    if (!Utils.GCATPP.isConnectedToSkyMind(lastController, true))
                                        isConnected = false;

                                    //If surrogate disconnected try to reconnect
                                    if (!Utils.GCATPP.isConnectedToSkyMind(cSurrogate, true))
                                        isConnected = false;

                                    //If regular pawn check the controlMode BUT if stored mind no check (senseless) just set as if the controlmode is enabled
                                    bool genControlMode = cso.controlMode;
                                    if (isMind)
                                        genControlMode = true;

                                    //Deja en session le lastUser on jerte
                                    if ( !isConnected || ((!VX3Owner && cso.isThereSX()) || (VX3Owner && cso.availableSX.Count+1 > Settings.VX3MaxSurrogateControllableAtOnce) ) || !genControlMode)
                                    {
                                        Messages.Message("ATPP_CannotReconnectToLastSurrogateController".Translate(), MessageTypeDefOf.NegativeEvent);
                                        return;
                                    }

                                    cso.setControlledSurrogate(cSurrogate);
                                }
                                else
                                {
                                    Messages.Message("ATPP_CannotReconnectToLastSurrogateController".Translate(), MessageTypeDefOf.NegativeEvent);
                                    return;
                                }
                            }
                        };
                    }
                    if (Utils.isThereFreeControllerInCaravan())
                    {
                        yield return new Command_Action
                        {
                            icon = Tex.AndroidToControlTargetRecovery,
                            defaultLabel = "ATPP_FreeControllerInCaravan".Translate(),
                            defaultDesc = "ATPP_FreeControllerInCaravanDesc".Translate(),
                            action = delegate ()
                            {
                                Utils.ShowFloatMenuNotControllingSurrogateControllerInCaravan((Pawn)parent, delegate (Pawn controller)
                                {
                                    CompSurrogateOwner cso = Utils.getCachedCSO(controller);
                                    //OK only if controller can be connected OR reside in a M8 (no check for connection on SkyMind)
                                    if (cso != null && (cso.skyCloudHost != null || Utils.GCATPP.isConnectedToSkyMind(controller, true)))
                                    {
                                        cso.setControlledSurrogate(currentPawn);
                                    }
                                });
                            }
                        };
                    }
                }
            }
            else
            {
                //Si pas un surrogate

                if (Utils.GCATPP.isConnectedToSkyMind(parent) && !isBlankAndroid)
                {
                    CompSurrogateOwner cso = Utils.getCachedCSO((Pawn)parent);

                    //Pas d'organique ou de controlleur de surrogate en corus de session peuvent faire l'operation d'augmentation de points
                    if ( !isOrganic && (cso == null ||  !cso.isThereSX()) )
                    {
                        yield return new Command_Action
                        {
                            icon = Tex.SkillUp,
                            defaultLabel = "ATPP_Skills".Translate(),
                            defaultDesc = "ATPP_SkillsDesc".Translate(),
                            action = delegate ()
                            {
                                Find.WindowStack.Add(new Dialog_SkillUp((Pawn)parent));
                            }
                        };
                    }
                }

            }


            yield break;
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder ret = new StringBuilder();
            try
            {

                if (parent.Map == null || isRegularM7())
                    return base.CompInspectStringExtra();

                int lvl = 0;
                Pawn cp = (Pawn)parent;

                if (cp.needs.food != null)
                    lvl = (int)(cp.needs.food.CurLevelPercentage * 100);

                if (!isOrganic)
                {
                    ret.AppendLine("ATPP_BatteryLevel".Translate(lvl));

                    if (Utils.POWERPP_LOADED && connectedLWPN != null)
                    {
                        if (connectedLWPNActive)
                            ret.AppendLine("ATPP_LWPNConnected".Translate(getConnectedLWPNLabel(connectedLWPN)));
                        else
                            ret.AppendLine("ATPP_LWPNDisconnected".Translate(getConnectedLWPNLabel(connectedLWPN)));
                    }

                    if (batteryExplosionEndingGT != -1)
                    {
                        float p;
                        p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - batteryExplosionStartingGT) / (float)(batteryExplosionEndingGT - batteryExplosionStartingGT));
                        ret.AppendLine("ATPP_BatteryExplodeInProgress".Translate(((int)(p * (float)100)).ToString()));
                    }

                    if (uploadEndingGT != -1 || showUploadProgress)
                    {
                        //Calcul pourcentage de transfert
                        float p;
                        string action;

                        if (uploadEndingGT == -1)
                        {
                            CompAndroidState cab = Utils.getCachedCAS(uploadRecipient);
                            p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - cab.uploadStartGT) / (float)(cab.uploadEndingGT - cab.uploadStartGT));
                            action = "ATPP_DownloadPercentage";
                        }
                        else
                        {
                            p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - uploadStartGT) / (float)(uploadEndingGT - uploadStartGT));
                            action = "ATPP_UploadPercentage";
                        }


                        ret.AppendLine(action.Translate(((int)(p * (float)100)).ToString()));
                    }

                    if (frameworkNaniteEffectGTEnd != -1)
                    {
                        float p;
                        p = Math.Min(1.0f, (float)(Find.TickManager.TicksGame - frameworkNaniteEffectGTStart) / (float)(frameworkNaniteEffectGTEnd - frameworkNaniteEffectGTStart));
                        ret.AppendLine("ATPP_NaniteFrameworkRepairingInProgress".Translate(((int)(p * (float)100)).ToString()));
                    }

                    if (paintingIsRusted)
                    {
                        ret.AppendLine("ATPP_Rusted".Translate());
                    }
                }

                if (isSurrogate)
                {
                    if (surrogateController != null)
                        ret.AppendLine("ATPP_RemotelyControlledBy".Translate(((Pawn)parent).LabelShortCap));

                    if (lastController != null && externalController == null)
                        ret.AppendLine("ATPP_PreviousSurrogateControllerIs".Translate(lastController.LabelShortCap));


                    if (surrogateController != null)
                    {
                        CompSurrogateOwner cso = Utils.getCachedCSO(surrogateController);
                        if (cso != null && surrogateController.VX3ChipPresent())
                        {
                            if (cso.SX == parent)
                            {
                                ret.AppendLine("ATPP_VX3SurrogateTypePrimary".Translate());
                            }
                            else
                            {
                                ret.AppendLine("ATPP_VX3SurrogateTypeSecondary".Translate());
                            }
                        }
                    }
                }

                return ret.TrimEnd().Append(base.CompInspectStringExtra()).ToString();
            }
            catch (Exception e)
            {
                Log.Message("[ATPP] CompAndroidState.CompInspectStringExtra " + e.Message + " " + e.StackTrace);
                return ret.TrimEnd().Append(base.CompInspectStringExtra()).ToString();
            }
        }


        public string getConnectedLWPNLabel(Building LWPNEmitter)
        {
            if (LWPNEmitter == null)
                return "";

            string ret = "";
            GameComponent GC_PPP = Utils.TryGetGameCompByTypeName("GC_PPP");
            Traverse GCPPP = Traverse.Create(GC_PPP);
            ThingComp compLWPNEmitter = Utils.TryGetCompByTypeName(LWPNEmitter, "CompLocalWirelessPowerEmitter", "Power++");
            if (compLWPNEmitter != null)
            {
                string LWPNID = (string)Traverse.Create(compLWPNEmitter).Field("LWPNID").GetValue();
                ret = (string)GCPPP.Method("getLWPNLabel", new object[] { LWPNID, false }).GetValue();
            }

            return ret;
        }

        /*
         * Detecte un cas d'interruption est le cas echeant tue les protagoniste de l'upload tous en affichant un message d'erreur
         */
        public void checkInterruptedUpload()
        {
            bool killSelf = false;
            bool uploadRecipientLastSkymindDisconnectIsManual = true;
            bool recipientDeadOrNull = uploadRecipient == null || uploadRecipient.Dead;
            bool recipientConnected = false;
            bool emitterConnected = false;

            if (uploadRecipient != null)
            {
                if (uploadRecipientCAS == null)
                    uploadRecipientCAS = Utils.getCachedCAS(uploadRecipient);

                if (uploadRecipientCAS != null)
                    uploadRecipientLastSkymindDisconnectIsManual = uploadRecipientCAS.lastSkymindDisconnectIsManual;

                if (Utils.GCATPP.isConnectedToSkyMind(uploadRecipient, !uploadRecipientLastSkymindDisconnectIsManual))
                    recipientConnected = true;
            }

            if (Utils.GCATPP.isConnectedToSkyMind(currentPawn))
                emitterConnected = true;

            if (isSurrogate && surrogateController != null)
            {
                CompSurrogateOwner cso = Utils.getCachedCSO(surrogateController);
                if (cso == null)
                    return;

                //Surrogate en cours on check si clones toujours connecté
                if (cso.isThereSX() && cso.availableSX != null)
                {
                    bool hostBadConn = false;
                    bool surrogateControllerLastSkymindDisconnectIsManual = true;

                    if (surrogateControllerCAS == null)
                        surrogateControllerCAS = Utils.getCachedCAS(surrogateController);

                    if (surrogateControllerCAS != null)
                        surrogateControllerLastSkymindDisconnectIsManual = surrogateControllerCAS.lastSkymindDisconnectIsManual;

                    //Si surrogateController stoclé dans le skyCloud
                    if (cso.skyCloudHost != null)
                        hostBadConn = !Utils.getCachedCSC(cso.skyCloudHost).isRunning();
                    else
                        hostBadConn = !Utils.GCATPP.isConnectedToSkyMind(surrogateController, !surrogateControllerLastSkymindDisconnectIsManual);

                    bool surrogateBadConn = !Utils.GCATPP.isConnectedToSkyMind(currentPawn, !lastSkymindDisconnectIsManual);

                    if (hostBadConn || surrogateBadConn)
                    {
                        //Log.Message("DDDDD==>"+ (!Utils.GCATPP.isConnectedToSkyMind(cpawn))+" "+ (!Utils.GCATPP.isConnectedToSkyMind(SX)));

                        Pawn disconnectedPawn = currentPawn;
                        Pawn invertedPawn = surrogateController;
                        if (hostBadConn)
                        {
                            disconnectedPawn = surrogateController;
                            invertedPawn = currentPawn;
                        }

                        //Notification de la deconnexion accidentelle
                        if (disconnectedPawn != null && invertedPawn != null && disconnectedPawn.Faction == Faction.OfPlayer)
                            Messages.Message("ATPP_SurrogateUnexpectedDisconnection".Translate(invertedPawn.LabelShortCap), disconnectedPawn, MessageTypeDefOf.NegativeEvent);

                        //un ou les deux des composantes sont déconnectés ===> on lance la deconnection du SX
                        cso.stopControlledSurrogate(currentPawn);
                    }
                }
            }

            //Si hote plus valide alors on arrete le processus et on kill les deux androids
            if (uploadEndingGT != -1 && (recipientDeadOrNull || currentPawn.Dead || !emitterConnected || !recipientConnected))
            {
                string reason = "";
                if (recipientDeadOrNull)
                {
                    reason = "ATPP_LetterInterruptedUploadDescCompHostDead".Translate();
                    killSelf = true;
                }

                if(currentPawn.Dead)
                {
                    reason = "ATPP_LetterInterruptedUploadDescCompSourceDead".Translate();
                    if (uploadRecipient != null && !uploadRecipient.Dead)
                    {
                        uploadRecipient.Kill(null, null);
                    }
                }

                if(reason == "")
                {
                    if (!recipientConnected || !emitterConnected)
                    {
                        reason = "ATPP_LetterInterruptedUploadDescCompDiconnectionError".Translate();

                        killSelf = true;
                        if (uploadRecipient != null && !uploadRecipient.Dead)
                        {
                            uploadRecipient.Kill(null, null);
                        }

                    }
                }

                resetUploadStuff();

                if (killSelf)
                {
                    if(!currentPawn.Dead)
                        currentPawn.Kill(null, null);
                }

                Utils.showFailedLetterMindUpload(reason);
            }
        }

        public void initAsSurrogate()
        {
            if (!init)
                initComp();

            isSurrogate = true;
            addNoRemoteHostHediff();
            addLowSignalHediff();
        }

        public void resetInternalState()
        {
            resetUploadStuff();
        }

        private void resetUploadStuff()
        {
            if (uploadRecipient != null)
            {
                CompAndroidState cab = Utils.getCachedCAS(uploadRecipient);
                cab.showUploadProgress = false;
                cab.uploadRecipient = null;
                cab.uploadRecipientCAS = null;
            }

            uploadStartGT = 0;
            uploadEndingGT = -1;
            uploadRecipient = null;
        }

        private void OnPermuteConfirmed(Pawn source, Pawn dest)
        {
            //Ajout hediff de transfert aux deux androids
            source.health.AddHediff(HediffDefOf.ATPP_ConsciousnessUpload);
            dest.health.AddHediff(HediffDefOf.ATPP_ConsciousnessUpload);

            int CGT = Find.TickManager.TicksGame;
            uploadRecipient = dest;
            uploadStartGT = CGT;
            uploadEndingGT = CGT + Settings.mindUploadHour*2500;

            CompAndroidState cab = Utils.getCachedCAS(dest);
            cab.showUploadProgress = true;
            cab.uploadRecipient = (Pawn)parent;
            cab.uploadRecipientCAS = this;
            //Cachin CAS of recipient
            uploadRecipientCAS = cab;

            Messages.Message("ATPP_StartUpload".Translate(source.LabelShortCap, dest.LabelShortCap), parent, MessageTypeDefOf.PositiveEvent);
        }

        public bool UseBattery
        {
            get
            {
                //SI M7 surrogate forcé à utilsier la recharge en directe
                if (parent.def.defName == Utils.M7 && isSurrogate)
                    return true;

                return useBattery;
            }
        }

        
        Pawn currentPawn = null;
        //Stock le signal indiquant si le pawn à été attribué par le systeme de job pour faire du guarding ou non
        public bool useBattery = false;
        public int uploadEndingGT = -1;
        public int uploadStartGT = 0;
        public Pawn uploadRecipient;
        public CompAndroidState uploadRecipientCAS;
        public bool isSurrogate = false;
        public Pawn surrogateController;
        public CompAndroidState surrogateControllerCAS;

        public bool isAndroidOrAnimalTier = false;
        //Sert a identifier les surrogates biologiques
        public bool isOrganic = false;
        public bool isAndroidTIer = false;
        public bool isAndroidWithSkin = false;

        public bool showUploadProgress = false;

        //Stocke le pawn externe (n'appartenant pas au joueur) controllant le surrogate (le cas des groupes de factions alliés/neutre/ennemis)
        public Pawn externalController;
        public int externalControllerConvertedJoinGT = 0;
        public Pawn lastController;

        private CompSkyMind csm;

        public string savedName = "";

        public int frameworkNaniteEffectGTEnd = -1;
        public int frameworkNaniteEffectGTStart = -1;

        //AndroidPaintColor
        public int customColor = (int)AndroidPaintColor.Default;

        public int paintingRustGT = -2;
        public bool paintingIsRusted = false;

        public int batteryExplosionEndingGT = -1;
        public int batteryExplosionStartingGT = -1;

        public bool isBlankAndroid = false;

        public Building connectedLWPN;
        public CompPowerTrader connectedLWPNCPT;
        public bool connectedLWPNActive = false;
        public bool autoPaintStarted = false;

        public bool TXHurtedHeadSet = false;
        public bool TXHurtedHeadSet2 = false;

        public HairDef hair;

        public bool init = false;

        public bool dontRust = false;
        public int forcedDamageLevel = -1;

        //Store if for the last downing of the surrogate, if it was downed via disconnect or other way
        public bool downedViaDisconnect = true;
        public bool lastSkymindDisconnectIsManual = false;

        public Thing droppedWeapon;
    }
}