using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Androids
{
	// Token: 0x02000005 RID: 5
	public class JobGiver_GetEnergy : ThinkNode_JobGiver
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00002410 File Offset: 0x00000610
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			return (JobGiver_GetEnergy)base.DeepCopy(resolve);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002430 File Offset: 0x00000630
		public override float GetPriority(Pawn pawn)
		{
			Need_Energy need_Energy = pawn.needs.TryGetNeed<Need_Energy>();
			bool flag = need_Energy == null;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				bool flag2 = need_Energy.CurLevelPercentage < Need_Energy.rechargePercentage;
				if (flag2)
				{
					result = 11.5f;
				}
				else
				{
					result = 0f;
				}
			}
			return result;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000247C File Offset: 0x0000067C
		protected override Job TryGiveJob(Pawn pawn)
		{
			bool downed = pawn.Downed;
			Job result;
			if (downed)
			{
				result = null;
			}
			else
			{
				Need_Energy need_Energy = pawn.needs.TryGetNeed<Need_Energy>();
				bool flag = need_Energy == null;
				if (flag)
				{
					result = null;
				}
				else
				{
					bool flag2 = need_Energy.CurLevelPercentage >= Need_Energy.rechargePercentage;
					if (flag2)
					{
						result = null;
					}
					else
					{
						Thing thing3 = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, (Thing thing) => this.BestClosestPowerSource(pawn, thing), null, 0, -1, false, RegionType.Set_Passable, false);
						bool flag3 = thing3 != null;
						if (flag3)
						{
							Building building = thing3 as Building;
							bool flag4 = thing3 != null && building != null && building.PowerComp != null && building.PowerComp.PowerNet.CurrentStoredEnergy() > 50f;
							if (flag4)
							{
								IntVec3 position = thing3.Position;
								bool flag5 = position.Walkable(pawn.Map) && position.InAllowedArea(pawn) && pawn.CanReserve(new LocalTargetInfo(position), 1, -1, null, false) && pawn.CanReach(position, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
								if (flag5)
								{
									return new Job(JobDefOf.ChJAndroidRecharge, thing3);
								}
								IEnumerable<IntVec3> source = GenAdj.CellsAdjacentCardinal(building);
								Func<IntVec3, float> <>9__1;
								Func<IntVec3, float> keySelector;
								if ((keySelector = <>9__1) == null)
								{
									keySelector = (<>9__1 = ((IntVec3 selector) => selector.DistanceTo(pawn.Position)));
								}
								foreach (IntVec3 intVec in source.OrderByDescending(keySelector))
								{
									bool flag6 = intVec.Walkable(pawn.Map) && intVec.InAllowedArea(pawn) && pawn.CanReserve(new LocalTargetInfo(intVec), 1, -1, null, false) && pawn.CanReach(intVec, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
									if (flag6)
									{
										return new Job(JobDefOf.ChJAndroidRecharge, thing3, intVec);
									}
								}
							}
						}
						Thing thing2 = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, (Thing thing) => thing.TryGetComp<EnergySourceComp>() != null && !thing.IsForbidden(pawn) && pawn.CanReserve(new LocalTargetInfo(thing), 1, -1, null, false) && thing.Position.InAllowedArea(pawn) && pawn.CanReach(new LocalTargetInfo(thing), PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn), null, 0, -1, false, RegionType.Set_Passable, false);
						bool flag7 = thing2 != null;
						if (flag7)
						{
							EnergySourceComp energySourceComp = thing2.TryGetComp<EnergySourceComp>();
							bool flag8 = energySourceComp != null;
							if (flag8)
							{
								int num = (int)Math.Ceiling((double)((need_Energy.MaxLevel - need_Energy.CurLevel) / energySourceComp.EnergyProps.energyWhenConsumed));
								bool flag9 = num > 0;
								if (flag9)
								{
									return new Job(JobDefOf.ChJAndroidRechargeEnergyComp, new LocalTargetInfo(thing2))
									{
										count = num
									};
								}
							}
						}
						result = null;
					}
				}
			}
			return result;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000027AC File Offset: 0x000009AC
		public bool BestClosestPowerSource(Pawn pawn, Thing thing)
		{
			CompPower compPower;
			bool flag = thing.Faction == pawn.Faction && (compPower = thing.TryGetComp<CompPower>()) != null && compPower.PowerNet != null && compPower.PowerNet.CurrentStoredEnergy() > 50f && !thing.IsForbidden(pawn) && pawn.CanReserve(new LocalTargetInfo(thing), 1, -1, null, false) && thing.Position.InAllowedArea(pawn) && pawn.CanReach(new LocalTargetInfo(thing), PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				Building t = thing as Building;
				IntVec3 position = thing.Position;
				bool flag3 = position.Walkable(pawn.Map) && position.InAllowedArea(pawn) && pawn.CanReserve(new LocalTargetInfo(position), 1, -1, null, false) && pawn.CanReach(position, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn);
				if (flag3)
				{
					result = true;
				}
				else
				{
					IEnumerable<IntVec3> source = GenAdj.CellsAdjacentCardinal(t);
					Func<IntVec3, float> <>9__0;
					Func<IntVec3, float> keySelector;
					if ((keySelector = <>9__0) == null)
					{
						keySelector = (<>9__0 = ((IntVec3 selector) => selector.DistanceTo(pawn.Position)));
					}
					foreach (IntVec3 intVec in source.OrderByDescending(keySelector))
					{
						bool flag4 = intVec.Walkable(pawn.Map) && intVec.InAllowedArea(pawn) && pawn.CanReserve(new LocalTargetInfo(intVec), 1, -1, null, false) && pawn.CanReach(intVec, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn);
						if (flag4)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}
	}
}
