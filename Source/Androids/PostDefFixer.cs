using System;
using Verse;

namespace Androids
{
	// Token: 0x0200001F RID: 31
	[StaticConstructorOnStartup]
	public static class PostDefFixer
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00005250 File Offset: 0x00003450
		static PostDefFixer()
		{
			foreach (RecipeDef recipeDef in DefDatabase<RecipeDef>.AllDefs)
			{
				bool flag = recipeDef.defName.StartsWith("Administer_");
				if (flag)
				{
					recipeDef.recipeUsers.RemoveAll((ThingDef thingDef) => thingDef.HasModExtension<MechanicalPawnProperties>());
				}
			}
		}
	}
}
