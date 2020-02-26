using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    // Token: 0x0200093B RID: 2363
    public class Verb_MechFall : Verb
    {
        // Token: 0x06003290 RID: 12944 RVA: 0x0017A8E0 File Offset: 0x00178CE0
        protected override bool TryCastShot()
        {
            if (this.currentTarget.HasThing && this.currentTarget.Thing.Map != this.caster.Map)
            {
                return false;
            }
            MechFall mechfall = (MechFall)GenSpawn.Spawn(ThingDefOf.MechFallBeam, this.currentTarget.Cell, this.caster.Map);
            mechfall.duration = 450;
            mechfall.instigator = this.caster;
            mechfall.weaponDef = ((base.EquipmentSource == null) ? null : base.EquipmentSource.def);
            mechfall.StartStrike();
            if (base.EquipmentSource != null && !base.EquipmentSource.Destroyed)
            {
                base.EquipmentSource.Destroy(DestroyMode.Vanish);
            }
            return true;
        }
        // Token: 0x06003291 RID: 12945 RVA: 0x0017A9AC File Offset: 0x00178DAC
        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = false;
            return 2f;
        }

        // Token: 0x0400210D RID: 8461
        private const int DurationTicks = 450;
    }
}
