using System;
using UnityEngine;

namespace Androids
{
	// Token: 0x02000036 RID: 54
	public static class UIHelper
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00009664 File Offset: 0x00007864
		public static Rect GetRowRect(Rect inRect, float rowHeight, int row)
		{
			float y = inRect.y + rowHeight * (float)row;
			Rect result = new Rect(inRect.x, y, inRect.width, rowHeight);
			return result;
		}
	}
}
