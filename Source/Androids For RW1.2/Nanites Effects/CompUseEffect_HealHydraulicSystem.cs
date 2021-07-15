using System;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class CompUseEffect_HealHydraulicSystem : CompUseEffect
    {
        public static void Apply(Pawn user)
        {
            int nb = 0;
            bool chance = false;

            if (!Rand.Chance(Settings.percentageNanitesFail))
            {
                nb = user.health.hediffSet.hediffs.RemoveAll((Hediff h) => (Utils.AndroidOldAgeHediffHydraulic.Contains(h.def.defName)));
                if (nb > 0)
                {
                    Utils.refreshHediff(user);
                }
                chance = true;
            }

            if (nb == 0)
            {
                if (chance)
                    Messages.Message("ATPP_NoBrokenStuffFound".Translate(user.LabelShort), user, MessageTypeDefOf.NegativeEvent, true);
                else
                    Messages.Message("ATPP_BrokenStuffRepairFailed".Translate(user.LabelShort), user, MessageTypeDefOf.NegativeEvent, true);
            }
            else
                Messages.Message("ATPP_BrokenHydraulicSystemRepaired".Translate(user.LabelShort), user, MessageTypeDefOf.PositiveEvent, true);
        }

        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);

            Apply(user);
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if ( !Utils.ExceptionAndroidList.Contains(p.def.defName))
            {
                failReason = "ATPP_CanOnlyBeUsedByAndroid".Translate();
                return false;
            }
            else if (!p.haveAndroidOldAgeHediff(Utils.AndroidOldAgeHediffHydraulic))
            {
                failReason = "ATPP_CannotBeUsedBecauseNoOldAgeIssues".Translate();
                return false;
            }
            return base.CanBeUsedBy(p, out failReason);
        }
    }
}