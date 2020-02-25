using System;
using Verse;

namespace Androids
{
	// Token: 0x0200000D RID: 13
	public static class ButcherUtility
	{
		// Token: 0x06000036 RID: 54 RVA: 0x0000337C File Offset: 0x0000157C
		public static void SpawnDrops(Pawn pawn, IntVec3 position, Map map)
		{
			float coverageOfNotMissingNaturalParts = pawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(pawn.RaceProps.body.corePart);
			foreach (ThingCountClass thingCountClass in pawn.def.butcherProducts)
			{
				int num = (int)Math.Ceiling((double)((float)thingCountClass.count * coverageOfNotMissingNaturalParts));
				bool flag = num > 0;
				if (flag)
				{
					do
					{
						Thing thing = ThingMaker.MakeThing(thingCountClass.thingDef, null);
						thing.stackCount = Math.Min(num, thingCountClass.thingDef.stackLimit);
						num -= thing.stackCount;
						GenPlace.TryPlaceThing(thing, position, map, ThingPlaceMode.Near, null);
					}
					while (num > 0);
				}
			}
		}
	}
}
