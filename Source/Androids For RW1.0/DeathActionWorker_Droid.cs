using System;
using Verse;

namespace Androids
{
	// Token: 0x02000038 RID: 56
	public class DeathActionWorker_Droid : DeathActionWorker_Android
	{
		// Token: 0x060000ED RID: 237 RVA: 0x000097F0 File Offset: 0x000079F0
		public override void PawnDied(Corpse corpse)
		{
			base.PawnDied(corpse);
			bool flag = !corpse.Destroyed;
			if (flag)
			{
				ButcherUtility.SpawnDrops(corpse.InnerPawn, corpse.Position, corpse.Map);
				bool flag2 = corpse.InnerPawn.apparel != null;
				if (flag2)
				{
					corpse.InnerPawn.apparel.DropAll(corpse.PositionHeld, true);
				}
				corpse.Destroy(DestroyMode.Vanish);
			}
		}
	}
}
