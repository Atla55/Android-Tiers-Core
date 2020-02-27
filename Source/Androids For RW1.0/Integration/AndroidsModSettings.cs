using System;
using Verse;

namespace Androids.Integration
{
	// Token: 0x0200003D RID: 61
	public class AndroidsModSettings : ModSettings
	{
		// Token: 0x060000FA RID: 250 RVA: 0x00009C90 File Offset: 0x00007E90
		public AndroidsModSettings()
		{
			AndroidsModSettings.Instance = this;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00009CE0 File Offset: 0x00007EE0
		public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.androidEyeGlow, "androidEyeGlow", true, false);
			Scribe_Values.Look<bool>(ref this.androidExplodesOnDeath, "androidExplodesOnDeath", true, false);
			Scribe_Values.Look<float>(ref this.androidExplosionRadius, "androidExplosionRadius", 3.5f, false);
			Scribe_Values.Look<bool>(ref this.droidCompatibilityMode, "droidCompatibilityMode", false, false);
			Scribe_Values.Look<bool>(ref this.droidDetonationConfirmation, "droidDetonationConfirmation", true, false);
			Scribe_Values.Look<bool>(ref this.droidWearDown, "droidWearDown", true, false);
			Scribe_Values.Look<bool>(ref this.droidWearDownQuadrum, "droidWearDownQuadrum", true, false);
		}

		// Token: 0x04000095 RID: 149
		public static AndroidsModSettings Instance;

		// Token: 0x04000096 RID: 150
		public bool androidEyeGlow = true;

		// Token: 0x04000097 RID: 151
		public bool androidExplodesOnDeath = true;

		// Token: 0x04000098 RID: 152
		public float androidExplosionRadius = 3.5f;

		// Token: 0x04000099 RID: 153
		public bool droidCompatibilityMode = false;

		// Token: 0x0400009A RID: 154
		public bool droidDetonationConfirmation = true;

		// Token: 0x0400009B RID: 155
		public bool droidWearDown = true;

		// Token: 0x0400009C RID: 156
		public bool droidWearDownQuadrum = true;
	}
}
