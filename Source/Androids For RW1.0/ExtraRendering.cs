using System;
using UnityEngine;
using Verse;

namespace Androids
{
	// Token: 0x02000011 RID: 17
	public static class ExtraRendering
	{
		// Token: 0x0600003C RID: 60 RVA: 0x0000391C File Offset: 0x00001B1C
		public static void DrawAdvanced(this Graphic graphic, Vector3 loc, Rot4 rot, float rotY, ThingDef thingDef, Thing thing)
		{
			Mesh mesh = graphic.MeshAt(rot);
			Quaternion rotation = graphic.GetQuatFromRot(rot) * Quaternion.AngleAxis(rotY, Vector3.up);
			Material material = graphic.MatAt(rot, thing);
			Graphics.DrawMesh(mesh, loc, rotation, material, 0);
			bool flag = graphic.ShadowGraphic != null;
			if (flag)
			{
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003970 File Offset: 0x00001B70
		public static Quaternion GetQuatFromRot(this Graphic graphic, Rot4 rot)
		{
			bool flag = graphic.data != null && !graphic.data.drawRotated;
			Quaternion result;
			if (flag)
			{
				result = Quaternion.identity;
			}
			else
			{
				bool shouldDrawRotated = graphic.ShouldDrawRotated;
				if (shouldDrawRotated)
				{
					result = rot.AsQuat;
				}
				else
				{
					result = Quaternion.identity;
				}
			}
			return result;
		}
	}
}
