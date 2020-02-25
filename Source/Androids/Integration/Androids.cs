using System;
using UnityEngine;
using Verse;

namespace Androids.Integration
{
	// Token: 0x0200003C RID: 60
	public class Androids : Mod
	{
		// Token: 0x060000F7 RID: 247 RVA: 0x00009A78 File Offset: 0x00007C78
		public Androids(ModContentPack content) : base(content)
		{
			Androids.Instance = this;
			AndroidsModSettings.Instance = base.GetSettings<AndroidsModSettings>();
			bool flag = AndroidsModSettings.Instance != null;
			if (flag)
			{
				this.explosionRadiusBuffer = AndroidsModSettings.Instance.androidExplosionRadius.ToString();
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00009AD0 File Offset: 0x00007CD0
		public override string SettingsCategory()
		{
			return "Androids";
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00009AE8 File Offset: 0x00007CE8
		public override void DoSettingsWindowContents(Rect inRect)
		{
			int num = 0;
			float rowHeight = 24f;
			Rect inRect2 = new Rect(inRect);
			inRect2.width /= 2f;
			Rect rowRect = UIHelper.GetRowRect(inRect2, rowHeight, num);
			num++;
			Widgets.CheckboxLabeled(rowRect, "AndroidSettingsEyeGlow".Translate(), ref AndroidsModSettings.Instance.androidEyeGlow, false);
			Rect rowRect2 = UIHelper.GetRowRect(inRect2, rowHeight, num);
			num++;
			Widgets.CheckboxLabeled(rowRect2, "AndroidSettingsExplodeOnDeath".Translate(), ref AndroidsModSettings.Instance.androidExplodesOnDeath, false);
			Rect rowRect3 = UIHelper.GetRowRect(inRect2, rowHeight, num);
			num++;
			Widgets.TextFieldNumericLabeled<float>(rowRect3, "AndroidSettingsExplosionRadius".Translate(), ref AndroidsModSettings.Instance.androidExplosionRadius, ref this.explosionRadiusBuffer, 1.25f, GenRadial.MaxRadialPatternRadius);
			Rect rowRect4 = UIHelper.GetRowRect(inRect2, rowHeight, num);
			num++;
			Widgets.CheckboxLabeled(rowRect4, "AndroidSettingsDroidCompatibilityMode".Translate(), ref AndroidsModSettings.Instance.droidCompatibilityMode, false);
			TooltipHandler.TipRegion(rowRect4, "AndroidSettingsDroidCompatibilityModeTooltip".Translate());
			Rect rowRect5 = UIHelper.GetRowRect(inRect2, rowHeight, num);
			num++;
			Widgets.CheckboxLabeled(rowRect5, "AndroidSettingsDroidDetonationDialog".Translate(), ref AndroidsModSettings.Instance.droidDetonationConfirmation, false);
			TooltipHandler.TipRegion(rowRect5, "AndroidSettingsDroidDetonationDialogTooltip".Translate());
			Rect rowRect6 = UIHelper.GetRowRect(inRect2, rowHeight, num);
			num++;
			Widgets.CheckboxLabeled(rowRect6, "AndroidSettingsDroidWearDown".Translate(), ref AndroidsModSettings.Instance.droidWearDown, false);
			Rect rowRect7 = UIHelper.GetRowRect(inRect2, rowHeight, num);
			num++;
			Widgets.CheckboxLabeled(rowRect7, "AndroidSettingsDroidWearDownQuadrum".Translate(), ref AndroidsModSettings.Instance.droidWearDownQuadrum, false);
		}

		// Token: 0x04000093 RID: 147
		public static Androids Instance;

		// Token: 0x04000094 RID: 148
		private string explosionRadiusBuffer = "3.5";
	}
}
