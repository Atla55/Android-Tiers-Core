using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;
using Verse.AI.Group;

namespace MOARANDROIDS
{
    public class Designator_SurrogateToHack : Designator
    {
        public Designator_SurrogateToHack(int hackType)
        {
            this.hackType = hackType;

            switch (hackType)
            {
                case 1:
                    this.defaultLabel = "ATPP_UploadVirus".Translate();
                    this.defaultDesc = "ATPP_UploadVirusDesc".Translate();
                    this.icon = Tex.PlayerVirus;
                    break;
                case 2:
                    this.defaultLabel = "ATPP_UploadExplosiveVirus".Translate();
                    this.defaultDesc = "ATPP_UploadExplosiveVirusDesc".Translate();
                    this.icon = Tex.PlayerExplosiveVirus;
                    break;
                case 3:
                    this.defaultLabel = "ATPP_HackTemp".Translate();
                    this.defaultDesc = "ATPP_HackTempDesc".Translate();
                    this.icon = Tex.PlayerHackingTemp;
                    break;
                case 4:
                    this.defaultLabel = "ATPP_Hack".Translate();
                    this.defaultDesc = "ATPP_HackDesc".Translate();
                    this.icon = Tex.PlayerHacking;
                    break;
            }

            this.soundDragSustain = SoundDefOf.Designate_DragAreaDelete;
            this.soundDragChanged = null;
            this.soundSucceeded = SoundDefOf.Designate_ZoneDelete;
            this.useMouseIcon = true;
            this.hotKey = KeyBindingDefOf.Misc4;

        }

        public override void DrawMouseAttachments()
        {
            base.DrawMouseAttachments();
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(base.Map))
            {
                return false;
            }
            if (!this.SXInCell(c))
            {
                return "ATPP_DesignatorNeedSelectSXToHack".Translate();
            }
            return true;
        }

        public override void RenderHighlight(List<IntVec3> dragCells)
        {
            base.RenderHighlight(dragCells);

            DesignatorUtility.RenderHighlightOverSelectableThings(this, dragCells);
        }


        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            base.CanDesignateThing(t);

            if (!(t is Pawn))
                return false;

            Pawn cp = (Pawn)t;
            CompAndroidState cas = cp.TryGetComp<CompAndroidState>();

            //Si pas clone ou clone deja utilisé on degage
            if (cas == null || !cas.isSurrogate || cp.Faction == Faction.OfPlayer)
                return false;

            target = cp;

            return true;
        }

        public override int DraggableDimensions
        {
            get
            {
                return 0;
            }
        }

        public override bool DragDrawMeasurements
        {
            get
            {
                return false;
            }
        }

        public override void DesignateMultiCell(IEnumerable<IntVec3> cells)
        {
            throw new NotImplementedException();
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            this.pos = c;
            this.cmap = Current.Game.CurrentMap;
        }

        protected override void FinalizeDesignationSucceeded()
        {
            base.FinalizeDesignationSucceeded();

            CompSkyMind csm = target.TryGetComp<CompSkyMind>();
            CompAndroidState cas = target.TryGetComp<CompAndroidState>();
            string surrogateName = target.LabelShortCap;
            CompSurrogateOwner cso = null;

            if (cas.externalController != null)
            {
                surrogateName = cas.externalController.LabelShortCap;
                cso = cas.externalController.TryGetComp<CompSurrogateOwner>();
            }

            Lord clord = target.GetLord();
            int nbp = Utils.GCATPP.getNbHackingPoints();
            int nbpToConsume = 0;

            //Check points
            switch (hackType)
            {
                case 1:
                    nbpToConsume = Settings.costPlayerVirus;
                    break;
                case 2:
                    nbpToConsume = Settings.costPlayerExplosiveVirus;
                    break;
                case 3:
                    nbpToConsume = Settings.costPlayerHackTemp;
                    break;
                case 4:
                    nbpToConsume = Settings.costPlayerHack;
                    break;
            }

            if(nbpToConsume > nbp)
            {
                Messages.Message("ATPP_CannotHackNotEnoughtHackingPoints".Translate(), MessageTypeDefOf.NegativeEvent);
                return;
            }

            //Si faction alliée ou neutre ==> pénalitée
            if (target.Faction.RelationKindWith(Faction.OfPlayer) != FactionRelationKind.Hostile)
            {
                target.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -1*Rand.Range(5, 36));
            }

            //Application effet
            switch (hackType)
            {
                case 1:
                case 2:

                    csm.Hacked = hackType;
                    //Surrogate va attaquer la colonnie
                    target.SetFactionDirect(Faction.OfAncients);
                    LordJob_AssistColony lordJob;
                    Lord lord = null;

                    IntVec3 fallbackLocation;
                    RCellFinder.TryFindRandomSpotJustOutsideColony(target.PositionHeld, target.Map, out fallbackLocation);

                    target.mindState.Reset();
                    target.mindState.duty = null;
                    target.jobs.StopAll();
                    target.jobs.ClearQueuedJobs();
                    target.ClearAllReservations();
                    if (target.drafter != null)
                        target.drafter.Drafted = false;

                    lordJob = new LordJob_AssistColony(Faction.OfAncients, fallbackLocation);
                    if (lordJob != null)
                        lord = LordMaker.MakeNewLord(Faction.OfAncients, lordJob, Current.Game.CurrentMap, null);

                    if (clord != null)
                    {
                        if(clord.ownedPawns.Contains(target))
                            clord.Notify_PawnLost(target, PawnLostCondition.IncappedOrKilled, null);
                    }

                    lord.AddPawn(target);

                    //Si virus explosive enclenchement de la détonnation
                    if(hackType == 2)
                        csm.infectedExplodeGT = Find.TickManager.TicksGame + (Settings.nbSecExplosiveVirusTakeToExplode * 60);
                    break;
                case 3:
                case 4:
                    bool wasPrisonner = target.IsPrisoner;
                    Faction prevFaction = target.Faction;
                    target.SetFaction(Faction.OfPlayer);

                    if (target.workSettings == null)
                    {
                        target.workSettings = new Pawn_WorkSettings(target);
                        target.workSettings.EnableAndInitialize();
                    }

                    if (clord != null)
                    {
                        if (clord.ownedPawns.Contains(target))
                            clord.Notify_PawnLost(target, PawnLostCondition.ChangedFaction, null);
                    }

                    if (cso != null)
                        cso.disconnectControlledSurrogate(null);

                    if (hackType == 4)
                    {
                        //Contorle definitif on jerte l'externalController
                        if (cas != null)
                            cas.externalController = null;
                    }

                    target.Map.attackTargetsCache.UpdateTarget(target);
                    PawnComponentsUtility.AddAndRemoveDynamicComponents(target, false);
                    Find.ColonistBar.MarkColonistsDirty();

                    if (hackType == 3)
                    {
                        csm.Hacked = hackType;
                        csm.hackOrigFaction = prevFaction;
                        if (wasPrisonner)
                            csm.hackWasPrisoned = true;
                        else
                            csm.hackWasPrisoned = false;
                        csm.hackEndGT = Find.TickManager.TicksGame + (Settings.nbSecDurationTempHack * 60);
                    }
                    else
                    {
                        //Si le surrogate quon veux controlé est infecté alors on enleve l'infection et on reset ses stats
                        if (csm.Infected != -1)
                        {
                            csm.Infected = -1;

                            if (target.skills != null && target.skills.skills  != null)
                            {
                                foreach (var sr in target.skills.skills)
                                {
                                    sr.levelInt = 0;
                                }
                            }
                        }
                    }
                    

                    break;
            }

            Utils.GCATPP.decHackingPoints(nbpToConsume);

            Utils.soundDefSurrogateHacked.PlayOneShot(null);

            //Notif d'applciation de l'effet
            Messages.Message("ATPP_SurrogateHackOK".Translate(surrogateName), target, MessageTypeDefOf.PositiveEvent);

            //ANimation sonore et visuelle
            Utils.soundDefSurrogateConnection.PlayOneShot(null);
            MoteMaker.ThrowDustPuffThick(pos.ToVector3Shifted(), cmap, 4.0f, Color.red);

            Find.DesignatorManager.Deselect();
        }


        [DebuggerHidden]
        private bool SXInCell(IntVec3 c)
        {
            if (!c.Fogged(base.Map))
            {
                List<Thing> thingList = c.GetThingList(base.Map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    if (thingList[i] is Pawn && this.CanDesignateThing(thingList[i]).Accepted)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void FinalizeDesignationFailed()
        {
            base.FinalizeDesignationFailed();

        }

        private IntVec3 pos;
        private Pawn target;
        private Map cmap;
        private int hackType;
    }
}
