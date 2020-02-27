using System;
using RimWorld;
using Verse;

namespace Androids
{
	// Token: 0x0200003B RID: 59
	public class ThoughtWorker_DroidAlways : ThoughtWorker
	{
		// Token: 0x060000F5 RID: 245 RVA: 0x00009A40 File Offset: 0x00007C40
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			bool flag = p.def.HasModExtension<MechanicalPawnProperties>();
			ThoughtState result;
			if (flag)
			{
				result = ThoughtState.ActiveAtStage(0);
			}
			else
			{
				result = ThoughtState.Inactive;
			}
			return result;
		}
	}
}
