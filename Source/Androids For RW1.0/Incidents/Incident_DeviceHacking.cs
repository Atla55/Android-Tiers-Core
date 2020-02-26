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
    public class Incident_DeviceHacking : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return !Settings.disableSkyMindSecurityStuff && !Utils.isThereSolarFlare()
                    && Utils.GCATPP.getNbDevices() > 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (Settings.disableSkyMindSecurityStuff)
                return false;

            List<Thing> victims;
            string title = "";
            string msg = "";
            int nbConnectedClients = Utils.GCATPP.getNbThingsConnected();
            List<string> cryptolockedThings = new List<string>();
            int nbDevices = Utils.GCATPP.getNbDevices();
            int nbUnsecurisedClients = nbConnectedClients - Utils.GCATPP.getNbSlotSecurisedAvailable();

            LetterDef letter;
            //Selection type virus 
            int attackType=1;
            int fee = 0;

            //Check si sur lensemble des clients connecté il y a quand meme des devices
            if (nbDevices <= 0)
                return false;

            //Attaque virale faible
            if (nbUnsecurisedClients <= 0)
            {
                if (!Rand.Chance(Settings.riskSecurisedSecuritySystemGetVirus))
                    return false;

                int nb = 0;
                

                nb = nbDevices / 2;
                if (nb != 0)
                {
                    nb = Rand.Range(1, nb + 1);
                }
                else
                    nb = 1;

                letter = LetterDefOf.ThreatSmall;
                //Obtention des victimes
                victims = Utils.GCATPP.getRandomDevices(nb);
                if (victims.Count == 0)
                {
                    return false;
                }

                foreach (var v in victims)
                {
                    CompSkyMind csm = v.TryGetComp<CompSkyMind>();
                    CompAndroidState cas = v.TryGetComp<CompAndroidState>();
                    if (cas == null)
                        continue;

                    csm.Infected = 4;

                    //Piratage temporaire

                }


                title = "ATPP_IncidentDeviceHackingVirus".Translate();
                msg = "ATPP_IncidentDeviceHackingLiteDesc".Translate(nb);

                victims = Utils.GCATPP.getRandomDevices(nb);
                if (victims.Count != nb)
                    return false;

                foreach (var v in victims)
                {
                    CompSkyMind csm = v.TryGetComp<CompSkyMind>();
                    if (csm == null)
                        continue;

                    Utils.GCATPP.disconnectUser(v);
                    csm.Infected = attackType;
                    csm.infectedEndGT = Find.TickManager.TicksGame + (Rand.Range(Settings.nbHourLiteHackingDeviceAttackLastMin, Settings.nbHourLiteHackingDeviceAttackLastMax)* 2500);
                }

            }
            else
            {
                letter = LetterDefOf.ThreatBig;

                attackType = Rand.Range(1, 4);

                int nb = 0;

                //Attaque virale douce
                //Obtention des victimes (qui peut allez de 1 victime a N/2 victimes
                nb = nbDevices / 2;
                if (nb != 0)
                {
                    nb = Rand.Range(1, nb + 1);
                }
                else
                    nb = 1;

                msg = "ATPP_IncidentDeviceHackingHardDesc".Translate(nb) + "\n";

                switch (attackType)
                {
                    case 1:
                        title = "ATPP_IncidentDeviceHackingVirus".Translate();
                        msg += "ATPP_IncidentDeviceVirusedDesc".Translate();
                        break;
                    case 2:
                        title = "ATPP_IncidentDeviceHackingExplosiveVirus".Translate();
                        msg += "ATPP_IncidentDeviceVirusedExplosiveDesc".Translate();
                        break;
                    case 3:
                        title = "ATPP_IncidentDeviceHackingCryptolocker".Translate();
                        msg += "ATPP_IncidentDeviceCryptolockerDesc".Translate();
                        break;
                }

                victims = Utils.GCATPP.getRandomDevices(nb);
                if (victims.Count != nb)
                    return false;

                foreach (var v in victims)
                {
                    CompSkyMind csm = v.TryGetComp<CompSkyMind>();
                    if (csm == null)
                        continue;

                    Utils.GCATPP.disconnectUser(v);
                    csm.Infected = attackType;

                    //Virus cryptolocker
                    if (attackType == 3)
                    {
                        cryptolockedThings.Add(v.GetUniqueLoadID());
                        fee += (int)(v.def.BaseMarketValue * 0.25f);
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
                ransom.deviceType = true;
                ransom.StartTimeout(60000);
                Find.LetterStack.ReceiveLetter(ransom, null);
            }

            return true;
        }

    }
}
