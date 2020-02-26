using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class Alert_UnsecurisedClients : Alert
    {
        public Alert_UnsecurisedClients()
        {
            this.defaultPriority = AlertPriority.High;
        }


        public override AlertReport GetReport()
        {
            if(Settings.disableSkyMindSecurityStuff)
                return false;

            int nbSecurisedSlot = Utils.GCATPP.getNbSlotSecurisedAvailable();
            int nbClient = Utils.GCATPP.getNbThingsConnected();
            int nbUnsecurised = nbClient - nbSecurisedSlot;

            if (nbUnsecurised > 0)
            {
                this.defaultLabel = "ATPP_AlertUnsecurisedClients".Translate(nbUnsecurised);
                this.defaultExplanation = "ATPP_AlertUnsecurisedClientsDesc".Translate(nbUnsecurised);
                return true;
            }
            else
                return false;
        }
    }
}
