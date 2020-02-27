using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace Androids
{
	// Token: 0x02000004 RID: 4
	public class JobDriver_RechargeEnergyFromConsumable : JobDriver
	{
		// Token: 0x06000010 RID: 16 RVA: 0x000022B5 File Offset: 0x000004B5
		public override void Notify_Starting()
		{
			base.Notify_Starting();
			this.energyNeed = base.GetActor().needs.TryGetNeed<Need_Energy>();
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000022D8 File Offset: 0x000004D8
		public override bool TryMakePreToilReservations()
		{
			return true;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000022EC File Offset: 0x000004EC
		public override string GetReport()
		{
			bool isValid = base.TargetB.IsValid;
			string result;
			if (isValid)
			{
				result = base.ReportStringProcessed(this.job.def.GetModExtension<ExtraReportStringProperties>().extraReportString);
			}
			else
			{
				result = base.GetReport();
			}
			return result;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002336 File Offset: 0x00000536
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
			bool flag = !base.TargetB.IsValid;
			if (flag)
			{
				base.AddFailCondition(() => this.energyNeed == null);
			}
			yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
			bool isValid = base.TargetB.IsValid;
			if (isValid)
			{
				yield return Toils_Reserve.Reserve(TargetIndex.B, 1, -1, null);
			}
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			yield return Toils_Reserve.Release(TargetIndex.A);
			yield return Toils_Haul.StartCarryThing(TargetIndex.A, false, true, false);
			bool isValid2 = base.TargetB.IsValid;
			if (isValid2)
			{
				yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnForbidden(TargetIndex.B);
				yield return Toils_General.Wait(100).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
				Toil rechargeToil = new Toil();
				rechargeToil.AddFinishAction(delegate
				{
					Thing carriedThing = this.pawn.carryTracker.CarriedThing;
					bool flag2 = carriedThing != null;
					if (flag2)
					{
						EnergySourceComp energySourceComp = carriedThing.TryGetComp<EnergySourceComp>();
						bool flag3 = energySourceComp != null;
						if (flag3)
						{
							energySourceComp.RechargeEnergyNeed((Pawn)base.TargetB.Thing);
						}
						this.pawn.carryTracker.DestroyCarriedThing();
					}
				});
				yield return rechargeToil;
				yield return Toils_Reserve.Release(TargetIndex.B);
				rechargeToil = null;
			}
			else
			{
				yield return Toils_General.Wait(100).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
				Toil rechargeToil2 = new Toil();
				rechargeToil2.AddFinishAction(delegate
				{
					Thing carriedThing = this.pawn.carryTracker.CarriedThing;
					bool flag2 = carriedThing != null;
					if (flag2)
					{
						EnergySourceComp energySourceComp = carriedThing.TryGetComp<EnergySourceComp>();
						bool flag3 = energySourceComp != null;
						if (flag3)
						{
							energySourceComp.RechargeEnergyNeed(this.pawn);
						}
						this.pawn.carryTracker.DestroyCarriedThing();
					}
				});
				yield return rechargeToil2;
				rechargeToil2 = null;
			}
			yield break;
		}

		// Token: 0x04000007 RID: 7
		private const TargetIndex PowerDestIndex = TargetIndex.A;

		// Token: 0x04000008 RID: 8
		private const TargetIndex OtherPawnIndex = TargetIndex.B;

		// Token: 0x04000009 RID: 9
		public Need_Energy energyNeed;
	}
}
