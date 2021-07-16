using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace MOARANDROIDS
{
    public class JobDriver_GoReloadBattery : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (this.pawn.Downed)
                return false;
            this.pawn.Map.pawnDestinationReservationManager.Reserve(this.pawn, this.job, this.job.targetA.Cell);
            return true;
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Check si TargetIndex.A est un Bed si oui alors juste un Toil_Bed.GotoBed suivant d'un LayDownCustomFood
            if (this.TargetThingA is Building_Bed)
            {
                Building_Bed pod = (Building_Bed)this.TargetThingA;

                yield return Toils_Bed.GotoBed(TargetIndex.A);
                //yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
                yield return Toils_LayDownPower.LayDown(TargetIndex.A, true, false, false, true);
            }
            else
            {
                Toil gotoCell = Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
                Toil nothing = new Toil();
                yield return gotoCell;
                Toil setSkin = new Toil();
                setSkin.initAction = delegate
                {
                    pawn.Rotation = Rot4.South;
                };
                yield return setSkin;
                yield return nothing;
                yield return Toils_General.Wait(50);
                yield return Toils_Jump.JumpIf(nothing, () => this.pawn.needs.food.CurLevelPercentage < 1.0f
                    && !this.job.targetB.ThingDestroyed && !((Building)this.job.targetB).IsBrokenDown()
                    && ((Building)this.job.targetB).TryGetComp<CompPowerTrader>().PowerOn);
            }
        }
    }
}