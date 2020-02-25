using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace Androids
{
	// Token: 0x02000009 RID: 9
	public class WorkGiver_GiveEnergySourceConsumableToPatient : WorkGiver_Scanner
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002CAD File Offset: 0x00000EAD
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000026 RID: 38 RVA: 0x000029D6 File Offset: 0x00000BD6
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002CB8 File Offset: 0x00000EB8
		public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			bool downed = pawn.Downed;
			bool result;
			if (downed)
			{
				result = false;
			}
			else
			{
				bool flag = thing.IsForbidden(pawn) || !thing.Position.InAllowedArea(pawn);
				if (flag)
				{
					bool flag2 = !pawn.CanReach(new LocalTargetInfo(thing), PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn);
					if (flag2)
					{
						return false;
					}
				}
				bool flag3 = HealthAIUtility.ShouldSeekMedicalRest(pawn);
				if (flag3)
				{
					result = false;
				}
				else
				{
					Pawn pawn2 = thing as Pawn;
					bool flag4 = pawn2 == null;
					if (flag4)
					{
						result = false;
					}
					else
					{
						bool flag5 = !pawn.CanReserve(new LocalTargetInfo(pawn2), 1, -1, null, false);
						if (flag5)
						{
							result = false;
						}
						else
						{
							bool? flag6;
							if (pawn2 == null)
							{
								flag6 = null;
							}
							else
							{
								Faction faction = pawn2.Faction;
								flag6 = ((faction != null) ? new bool?(!faction.IsPlayer) : null);
							}
							bool flag7 = flag6 ?? true;
							if (flag7)
							{
								result = false;
							}
							else
							{
								bool flag8 = !pawn2.InBed() || !pawn2.Downed;
								if (flag8)
								{
									result = false;
								}
								else
								{
									bool flag9 = !HealthAIUtility.ShouldSeekMedicalRest(pawn2);
									if (flag9)
									{
										result = false;
									}
									else
									{
										Need_Energy need_Energy = pawn2.needs.TryGetNeed<Need_Energy>();
										bool flag10 = need_Energy == null;
										if (flag10)
										{
											result = false;
										}
										else
										{
											bool flag11 = !forced && need_Energy.CurLevelPercentage > 0.5f;
											if (flag11)
											{
												result = false;
											}
											else
											{
												Thing thing2 = this.TryFindBestEnergySource(pawn);
												bool flag12 = thing2 == null;
												if (flag12)
												{
													result = false;
												}
												else
												{
													bool flag13 = !pawn.CanReserve(new LocalTargetInfo(thing2), 1, -1, null, false);
													result = !flag13;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002E68 File Offset: 0x00001068
		public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			Pawn pawn2 = thing as Pawn;
			Thing thing2 = this.TryFindBestEnergySource(pawn);
			bool flag = thing2 != null;
			if (flag)
			{
				Need_Energy need_Energy = pawn2.needs.TryGetNeed<Need_Energy>();
				EnergySourceComp energySourceComp = thing2.TryGetComp<EnergySourceComp>();
				int num = (int)Math.Ceiling((double)((need_Energy.MaxLevel - need_Energy.CurLevel) / energySourceComp.EnergyProps.energyWhenConsumed));
				num = Math.Min(num, thing2.stackCount);
				bool flag2 = num > 0;
				if (flag2)
				{
					return new Job(JobDefOf.ChJAndroidRechargeEnergyComp, new LocalTargetInfo(thing2), new LocalTargetInfo(pawn2))
					{
						count = num
					};
				}
			}
			return null;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002F0C File Offset: 0x0000110C
		public Thing TryFindBestEnergySource(Pawn pawn)
		{
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, (Thing searchThing) => searchThing.TryGetComp<EnergySourceComp>() != null && !searchThing.IsForbidden(pawn) && pawn.CanReserve(searchThing, 1, -1, null, false) && searchThing.Position.InAllowedArea(pawn) && pawn.CanReach(new LocalTargetInfo(searchThing), PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn), null, 0, -1, false, RegionType.Set_Passable, false);
		}
	}
}
