using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;

namespace MOARANDROIDS
{
    public class Designator_AndroidToControl : Designator
    {
        public Designator_AndroidToControl(Pawn controller, bool fromSkyCloud=false)
        {
            this.defaultLabel = "ATPP_AndroidToControlTarget".Translate();
            this.defaultDesc = "ATPP_AndroidToControlTargetDesc".Translate();
            this.soundDragSustain = SoundDefOf.Designate_DragAreaDelete;
            this.soundDragChanged = null;
            this.soundSucceeded = SoundDefOf.Designate_ZoneDelete;
            this.useMouseIcon = true;
            this.icon = Tex.AndroidToControlTarget;
            this.hotKey = KeyBindingDefOf.Misc4;
            this.controller = controller;
            this.fromSkyCloud = fromSkyCloud;
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
                if (fromSkyCloud)
                {
                    if (!this.TurretInCell(c))
                        return "ATPP_DesignatorNeedSelectSX".Translate();
                }
                else
                    return "ATPP_DesignatorNeedSelectSX".Translate();
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

            if (t is Pawn)
            {
                Pawn cp = (Pawn)t;
                CompSkyMind csm = cp.TryGetComp<CompSkyMind>();
                CompAndroidState cas = cp.TryGetComp<CompAndroidState>();

                //Si pas clone ou clone deja utilisé on degage
                if (cas == null || !cas.isSurrogate || cas.surrogateController != null || csm.Infected != -1)
                    return false;

                if (!Utils.GCATPP.isConnectedToSkyMind(cp))
                {
                    //Tentative connection au skymind 
                    if (!Utils.GCATPP.connectUser(cp))
                        return false;
                }

                target = cp;
                kindOfTarget = 1;
                return true;
            }
            else if (fromSkyCloud && (t.def.thingClass == typeof(Building_Turret) || t.def.thingClass.IsSubclassOf(typeof(Building_Turret))))
            {
                Building build = (Building)t;
                CompRemotelyControlledTurret crt = t.TryGetComp<CompRemotelyControlledTurret>();

                if (crt != null && crt.controller == null && !t.IsBrokenDown() && t.TryGetComp<CompPowerTrader>().PowerOn)
                {
                    if (!Utils.GCATPP.isConnectedToSkyMind(t))
                    {
                        //Tentative connection au skymind 
                        if (!Utils.GCATPP.connectUser(t))
                            return false;
                    }

                    target = t;
                    kindOfTarget = 2;
                    return true;
                }
            }

            return false;
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
            

            CompSurrogateOwner cso = controller.TryGetComp<CompSurrogateOwner>();
            if (cso != null) {
                if (kindOfTarget == 1)
                {
                    cso.controlMode = true;
                    cso.setControlledSurrogate((Pawn)target);
                }
                else if (kindOfTarget == 2)
                {
                    if(cso.skyCloudHost != null)
                    {
                        CompSkyCloudCore csc = cso.skyCloudHost.TryGetComp<CompSkyCloudCore>();
                        if(csc != null)
                        {
                            csc.setRemotelyControlledTurret(controller, (Building)target);
                        }
                    }
                }
            }

            if(!controller.VX3ChipPresent())
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

        private bool TurretInCell(IntVec3 c)
        {
            if (!c.Fogged(base.Map))
            {
                List<Thing> thingList = c.GetThingList(base.Map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    if (thingList[i] != null && (thingList[i].def.thingClass == typeof(Building_Turret) || thingList[i].def.thingClass.IsSubclassOf(typeof(Building_Turret)) ) && this.CanDesignateThing(thingList[i]).Accepted)
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

        private int kindOfTarget = 0;
        private IntVec3 pos;
        private Thing target;
        private Map cmap;
        private Pawn controller;
        public bool fromSkyCloud = false;
    }
}
