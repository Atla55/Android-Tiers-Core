using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class ChoiceLetter_RansomwareDemand : ChoiceLetter
    {
        public Faction faction;
        public int fee;
        public Pawn victim;

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                if (base.ArchivedOnly)
                {
                    yield return base.Option_Close;
                }
                else
                {
                    DiaOption accept = new DiaOption("RansomDemand_Accept".Translate());
                    accept.action = delegate
                    {
                        Utils.anyPlayerColonnyPaySilver(fee);

                        //Check si la faction tient parole
                        if (Rand.Chance(1.0f - Settings.riskCryptolockerScam))
                        {

                            CompSurrogateOwner cso = victim.TryGetComp<CompSurrogateOwner>();
                            
                            if(cso.ransomwareTraitAdded != null)
                            {
                                if (victim.story.traits.HasTrait(cso.ransomwareTraitAdded))
                                {
                                    Trait ct = null;
                                    foreach(var t in victim.story.traits.allTraits)
                                    {
                                        if (t.def == cso.ransomwareTraitAdded) {
                                            ct = t;
                                            break;
                                        }
                                    }

                                    if (ct != null)
                                        victim.story.traits.allTraits.Remove(ct);
                                }
                            }
                            else
                            {
                                foreach(var s in victim.skills.skills)
                                {
                                    if(s.def == cso.ransomwareSkillStolen)
                                    {
                                        s.levelInt = cso.ransomwareSkillValue;
                                        break;
                                    }
                                }
                            }
                            cso.clearRansomwareVar();

                            Messages.Message("ATPP_RansomwareClearedByFaction".Translate(faction.Name, victim.LabelShortCap), MessageTypeDefOf.PositiveEvent);
                        }
                        else
                        {
                            //ATPP_LetterFactionScamCryptolocker
                            Find.LetterStack.ReceiveLetter("ATPP_LetterFactionScam".Translate(), "ATPP_LetterFactionScamRansomwareDesc".Translate(faction.Name), LetterDefOf.ThreatBig);
                        }

                        Find.LetterStack.RemoveLetter(this);
                    };
                    accept.resolveTree = true;
                    if (!Utils.anyPlayerColonnyHasEnoughtSilver(fee))
                    {
                        accept.Disable("NeedSilverLaunchable".Translate(this.fee.ToString()));
                    }
                    yield return accept;
                    yield return base.Option_Reject;
                    yield return base.Option_Postpone;
                }
            }
        }

        public override bool CanShowInLetterStack
        {
            get
            {
                return base.CanShowInLetterStack;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Faction>(ref this.faction, "faction", false);
            Scribe_References.Look<Pawn>(ref this.victim, "victim", false);
            Scribe_Values.Look<int>(ref this.fee, "fee", 0, false);
        }
    }
}