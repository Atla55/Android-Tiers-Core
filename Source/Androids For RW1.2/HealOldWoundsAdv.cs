using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class HediffComp_RegenWoundsAdv : HediffComp
    {

        public HediffCompProperties_RegenWoundsAdv HealingProps
        {
            get
            {
                return this.props as HediffCompProperties_RegenWoundsAdv;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.ResetTicksToHeal();
        }

        private void ResetTicksToHeal()
        {
            this.ticksToHeal = Rand.Range(this.HealingProps.Delay, (this.HealingProps.Delay+1)) * 50;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            this.ticksToHeal--;
            if (this.ticksToHeal <= 0)
            {
                this.TryHealRandomWound();
                this.ResetTicksToHeal();
            }
        }

        private void TryHealRandomWound()
        {
            IEnumerable<Hediff> hediffs = base.Pawn.health.hediffSet.hediffs;
            if (HediffComp_RegenWoundsAdv.stuff == null)
			{
                HediffComp_RegenWoundsAdv.stuff = new Func<Hediff, bool>(HediffUtility.IsTended);
            }
            if (!hediffs.Where(HediffComp_RegenWoundsAdv.stuff).TryRandomElement(out Hediff hediff))
            {
                return;
            }
            if (hediff.def != RimWorld.HediffDefOf.WoundInfection || hediff.def.makesSickThought)
            {
                hediff.Severity -= this.HealingProps.HealingAmount;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look<int>(ref this.ticksToHeal, "ticksToHeal", 0, false);
        }

        public override string CompDebugString()
        {
            return "ticksToHeal: " + this.ticksToHeal;
        }

        private int ticksToHeal;

        [CompilerGenerated]
        private static Func<Hediff, bool> stuff;
	}
}
