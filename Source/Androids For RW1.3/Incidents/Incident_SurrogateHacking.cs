using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
using RimWorld;
using Verse.AI;
using RimWorld.Planet;
using UnityEngine;
using Verse.Sound;

namespace MOARANDROIDS
{
    public class Incident_SurrogateHacking : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return !Settings.disableSkyMindSecurityStuff && !Utils.isThereSolarFlare()
                    && Utils.GCATPP.getNbSurrogateAndroids() > 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (Settings.disableSkyMindSecurityStuff)
                return false;

            List<Pawn> victims;
            List<string> cryptolockedThings = new List<string>();
            string title = "";
            string msg = "";
            int nbConnectedClients = Utils.GCATPP.getNbThingsConnected();
            int nbSurrogates = Utils.GCATPP.getNbSurrogateAndroids();
            int nbUnsecurisedClients = nbConnectedClients - Utils.GCATPP.getNbSlotSecurisedAvailable();

            LetterDef letter;
            //Selection type virus 
            int attackType=1;
            int fee = 0;

            //Check si sur lensemble des clients connecté il y a quand meme des surrogates
            if (nbSurrogates <= 0)
                return false;

            //Attaque virale faible
            if (nbUnsecurisedClients <= 0)
            {
                if (!Rand.Chance(Settings.riskSecurisedSecuritySystemGetVirus))
                    return false;

                int nb = 0;
                

                nb = nbSurrogates / 2;
                if (nb != 0)
                {
                    nb = Rand.Range(1, nb + 1);
                }
                else
                    nb = 1;

                letter = LetterDefOf.ThreatSmall;
                //Obtention des victimes
                victims = Utils.GCATPP.getRandomSurrogateAndroids(nb);
                if (victims.Count == 0)
                {
                    return false;
                }

                foreach (var v in victims)
                {
                    CompSkyMind csm = v.TryGetComp<CompSkyMind>();
                    CompAndroidState cas = v.TryGetComp<CompAndroidState>();
                    if (csm == null || cas == null)
                        continue;

                    csm.Infected = 4;

                    //Deconnection du contorlleur le cas echeant
                    if (cas.surrogateController != null)
                    {
                        CompSurrogateOwner cso = cas.surrogateController.TryGetComp<CompSurrogateOwner>();
                        if (cso != null)
                            cso.disconnectControlledSurrogate(null);
                    }

                    Hediff he = v.health.hediffSet.GetFirstHediffOfDef(Utils.hediffNoHost);
                    if (he != null)
                        v.health.RemoveHediff(he);

                    Utils.ignoredPawnNotifications = v;
                    Utils.VirusedRandomMentalBreak.RandomElement().Worker.TryStart(v, null, false);
                    Utils.ignoredPawnNotifications = null;
                    //v.mindState.mentalStateHandler.TryStartMentalState(  , null, false, false, null, false);
                }


                title = "ATPP_IncidentSurrogateHackingVirus".Translate();
                msg = "ATPP_IncidentSurrogateHackingLiteDesc".Translate(nb);
            }
            else
            {
                letter = LetterDefOf.ThreatBig;

                attackType = Rand.Range(1, 4);

                int nb = 0;
                LordJob_AssaultColony lordJob;
                Lord lord = null;

                if (attackType != 3)
                {
                    //Attaque virale douce
                    //Obtention des victimes (qui peut allez de 1 victime a N/2 victimes
                    nb = nbSurrogates / 2;
                    if (nb != 0)
                    {
                        nb = Rand.Range(1, nb + 1);
                    }
                    else
                        nb = 1;

                    lordJob = new LordJob_AssaultColony(Faction.OfAncientsHostile, false, false, false, false, false);

                    if (lordJob != null)
                        lord = LordMaker.MakeNewLord(Faction.OfAncientsHostile, lordJob, Current.Game.CurrentMap, null);
                }
                else
                    nb = nbSurrogates;

                msg = "ATPP_IncidentSurrogateHackingHardDesc".Translate(nb) + "\n";

                switch (attackType)
                {
                    case 1:
                        title = "ATPP_IncidentSurrogateHackingVirus".Translate();
                        msg += "ATPP_IncidentVirusedDesc".Translate();
                        break;
                    case 2:
                        title = "ATPP_IncidentSurrogateHackingExplosiveVirus".Translate();
                        msg += "ATPP_IncidentVirusedExplosiveDesc".Translate();
                        break;
                    case 3:
                        title = "ATPP_IncidentSurrogateHackingCryptolocker".Translate();
                        msg += "ATPP_IncidentCryptolockerDesc".Translate();
                        break;
                }

                victims = Utils.GCATPP.getRandomSurrogateAndroids(nb);
                if (victims.Count != nb)
                    return false;

                foreach (var v in victims)
                {
                    CompSkyMind csm = v.TryGetComp<CompSkyMind>();

                    v.mindState.canFleeIndividual = false;
                    csm.Infected = attackType;
                    if (v.jobs != null)
                    {
                        v.jobs.StopAll();
                        v.jobs.ClearQueuedJobs();
                    }
                    if (v.mindState != null)
                        v.mindState.Reset(true);


                    switch (attackType)
                    {
                        //Virus
                        case 1:
                            //Devient hostile
                            if(lord != null)
                                lord.AddPawn(v);
                            break;
                        //Virus explosif
                        case 2:
                            //Devient hostile
                            if (lord != null)
                                lord.AddPawn(v);
                            break;
                        //Virus cryptolocker
                        case 3:
                            cryptolockedThings.Add(v.GetUniqueLoadID());
                            
                            switch (v.def.defName)
                            {
                                case Utils.M7:
                                    fee += Settings.ransomCostT5;
                                    break;
                                case Utils.HU:
                                    fee += Settings.ransomCostT4;
                                    break;
                                case Utils.T2:
                                    fee += Settings.ransomCostT2;
                                    break;
                                case Utils.T3:
                                    fee += Settings.ransomCostT3;
                                    break;
                                case Utils.T4:
                                    fee += Settings.ransomCostT4;
                                    break;
                                case Utils.T5:
                                    fee += Settings.ransomCostT5;
                                    break;
                                case Utils.T1:
                                default:
                                    fee += Settings.ransomCostT1;
                                    break;
                            }
                            break;
                    }

                    if(attackType ==1 || attackType == 2)
                    {
                        //On va attribuer aleatoirement des poids d'attaque aux surrogate
                        SkillRecord shooting = v.skills.GetSkill(SkillDefOf.Shooting);
                        if(shooting != null && !shooting.TotallyDisabled)
                        {
                            shooting.levelInt = Rand.Range(3, 19);
                        }
                        SkillRecord melee = v.skills.GetSkill(SkillDefOf.Melee);
                        if (melee != null && !melee.TotallyDisabled)
                        {
                            melee.levelInt = Rand.Range(3, 19);
                        }
                    }
                }
            }

            Find.LetterStack.ReceiveLetter(title, msg, letter, (LookTargets) victims, null, null);

            
            if (attackType == 3)
            {
                //Déduction faction ennemis au hasard
                Faction faction = Find.FactionManager.RandomEnemyFaction();

                ChoiceLetter_RansomDemand ransom = (ChoiceLetter_RansomDemand) LetterMaker.MakeLetter(DefDatabase<LetterDef>.GetNamed("ATPP_CLPayCryptoRansom"));
                ransom.label = "ATPP_CryptolockerNeedPayRansomTitle".Translate();
                ransom.text = "ATPP_CryptolockerNeedPayRansom".Translate(faction.Name, fee);
                ransom.faction = faction;
                ransom.radioMode = true;
                ransom.fee = fee;
                ransom.cryptolockedThings = cryptolockedThings;
                ransom.StartTimeout(60000);
                Find.LetterStack.ReceiveLetter(ransom, null);
            }

            return true;
        }

    }
}
