using System;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Androids
{
	// Token: 0x02000026 RID: 38
	public class EnergySource_SolarComp : EnergySourceComp
	{
		// Token: 0x06000087 RID: 135 RVA: 0x000056D0 File Offset: 0x000038D0
		public override void RechargeEnergyNeed(Pawn targetPawn)
		{
			bool flag = GenLocalDate.DayPercent(targetPawn) < 0.2f || GenLocalDate.DayPercent(targetPawn) > 0.7f;
			bool flag2 = flag;
			if (!flag2)
			{
				bool inContainerEnclosed = targetPawn.InContainerEnclosed;
				if (!inContainerEnclosed)
				{
					bool flag3 = !targetPawn.IsCaravanMember() && targetPawn.Position.Roofed(targetPawn.Map);
					if (!flag3)
					{
						Need_Energy need_Energy = targetPawn.needs.TryGetNeed<Need_Energy>();
						need_Energy.CurLevel += base.EnergyProps.passiveEnergyGeneration;
					}
				}
			}
		}
	}
}
