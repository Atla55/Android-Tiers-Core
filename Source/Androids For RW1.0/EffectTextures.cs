using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Androids
{
	// Token: 0x02000010 RID: 16
	[StaticConstructorOnStartup]
	public static class EffectTextures
	{
		// Token: 0x0600003B RID: 59 RVA: 0x0000388C File Offset: 0x00001A8C
		public static Graphic GetEyeGraphic(bool isFront, Color color)
		{
			Pair<bool, Color> key = new Pair<bool, Color>(isFront, color);
			bool flag = EffectTextures.eyeCache.ContainsKey(key);
			Graphic result;
			if (flag)
			{
				result = EffectTextures.eyeCache[key];
			}
			else
			{
				if (isFront)
				{
					EffectTextures.eyeCache[key] = GraphicDatabase.Get<Graphic_Single>(EffectTextures.Eyeglow_Front_Path, ShaderDatabase.MoteGlow, Vector2.one, color);
				}
				else
				{
					EffectTextures.eyeCache[key] = GraphicDatabase.Get<Graphic_Single>(EffectTextures.Eyeglow_Side_Path, ShaderDatabase.MoteGlow, Vector2.one, color);
				}
				result = EffectTextures.eyeCache[key];
			}
			return result;
		}

		// Token: 0x0400000F RID: 15
		public static string Eyeglow_Front_Path = "Effects/Eyeglow_front";

		// Token: 0x04000010 RID: 16
		public static string Eyeglow_Side_Path = "Effects/Eyeglow_side";

		// Token: 0x04000011 RID: 17
		public static Dictionary<Pair<bool, Color>, Graphic> eyeCache = new Dictionary<Pair<bool, Color>, Graphic>();
	}
}
