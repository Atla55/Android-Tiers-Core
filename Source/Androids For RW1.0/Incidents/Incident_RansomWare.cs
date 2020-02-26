using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
using RimWorld;
using Verse.AI;
using RimWorld.Planet;
using UnityEngine;

namespace MOARANDROIDS
{
    public class Incident_RansomWare : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return !Settings.disableSkyMindSecurityStuff && !Utils.isThereSolarFlare()
                    && Utils.GCATPP.getNbSkyMindUsers() > 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (Settings.disableSkyMindSecurityStuff)
                return false;

            Pawn victim;
            string title = "ATPP_LetterFactionRansomware".Translate();
            string msg = "";
            string ransomMsg = "";
            int nbConnectedClients = Utils.GCATPP.getNbThingsConnected();
            int nbUnsecurisedClients = nbConnectedClients - Utils.GCATPP.getNbSlotSecurisedAvailable();
            //Déduction faction ennemis au hasard
            Faction faction = Find.FactionManager.RandomEnemyFaction();

            LetterDef letter = LetterDefOf.ThreatBig;
            int fee = 0;

            //Si pas de config insécurisé alors on dégage
            if (nbUnsecurisedClients < 0)
                return false;

            victim = Utils.GCATPP.getRandomSkyMindUser();
            if (victim == null)
                return false;

            CompSurrogateOwner cso = victim.TryGetComp<CompSurrogateOwner>();
            if (cso == null)
                return false;

            cso.clearRansomwareVar();

            //Bad traits added
            if (Rand.Chance(0.5f))
            {
                List<TraitDef> tr = Utils.RansomAddedBadTraits.ToList();

                //Purge des traits deja possédé par la victime ET incompatibles avec ceux present
                foreach(var t in Utils.RansomAddedBadTraits)
                {
                    foreach(var t2 in victim.story.traits.allTraits)
                    {
                        if (t2.def == t || (t.conflictingTraits != null && t.conflictingTraits.Contains(t2.def)))
                        {
                            tr.Remove(t2.def);
                            break;
                        }
                            
                    }
                }

                //Selection trait aleatoire ajouté
                cso.ransomwareTraitAdded = tr.RandomElement();
                victim.story.traits.GainTrait(new Trait(cso.ransomwareTraitAdded, 0, true));

                fee = Rand.Range(Settings.ransomwareMinSilverToPayForBasTrait, Settings.ransomwareMaxSilverToPayForBasTrait);

                string traitLabel="";

                if(cso.ransomwareTraitAdded.degreeDatas != null && cso.ransomwareTraitAdded.degreeDatas.First() != null)
                    traitLabel = cso.ransomwareTraitAdded.degreeDatas.First().label;

                //Log.Message("=======>"+cso.ransomwareTraitAdded.defName);

                msg = "ATPP_LetterFactionRansomwareBadTraitDownloadedDesc".Translate(faction.Name, victim.LabelShortCap, traitLabel);
                ransomMsg = "ATPP_RansomNeedPayRansomDownloadedTrait".Translate(faction.Name, traitLabel, victim.LabelShortCap, fee);
            }
            else
            {
                //Skill enlevé

                SkillDef find = null;
                SkillRecord sel = null;
                int v = -1;
                //Check tu plus gros skill de la victime
                foreach (var s in victim.skills.skills)
                {
                    if(s.levelInt >= v)
                    {
                        v = s.levelInt;
                        find = s.def;
                        sel = s;
                    }
                }

                //APplication effet négatif
                sel.levelInt = 0;

                //Sauvegarde infos de skill pour restauration
                cso.ransomwareSkillStolen = find;
                cso.ransomwareSkillValue = v;

                fee = v * Settings.ransomwareSilverToPayToRestoreSkillPerLevel;

                msg = "ATPP_LetterFactionRansomwareSkillStolenDesc".Translate(faction.Name, cso.ransomwareSkillStolen.LabelCap, victim.LabelShortCap);
                ransomMsg = "ATPP_RansomNeedPayRansomCorruptedSKill".Translate(faction.Name, cso.ransomwareSkillStolen.LabelCap, victim.LabelShortCap, fee);
            }



            Find.LetterStack.ReceiveLetter(title, msg, letter, (LookTargets) victim, null, null);

            ChoiceLetter_RansomwareDemand ransom = (ChoiceLetter_RansomwareDemand) LetterMaker.MakeLetter(DefDatabase<LetterDef>.GetNamed("ATPP_CLPayRansomwareRansom"));
            ransom.label = "ATPP_RansomNeedPayRansomTitle".Translate();
            ransom.text = ransomMsg;
            ransom.faction = faction;
            ransom.victim = victim;
            ransom.radioMode = true;
            ransom.StartTimeout(60000);
            ransom.fee = fee;
            Find.LetterStack.ReceiveLetter(ransom, null);

            return true;
        }

    }
}
