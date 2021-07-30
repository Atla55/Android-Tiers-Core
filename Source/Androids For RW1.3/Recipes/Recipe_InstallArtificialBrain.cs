using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MOARANDROIDS
{
    public class Recipe_InstallArtificialBrain : Recipe_InstallImplant
    {

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            CompAndroidState cas = Utils.getCachedCAS(pawn);

            if (cas == null)
                return;

            //Le cas echeant on deconnecte le controlleur s'il y en a un 
            if(cas.surrogateController != null)
            {
                CompSurrogateOwner cso = Utils.getCachedCSO(cas.surrogateController);
                if(cso != null)
                {
                    cso.disconnectControlledSurrogate(pawn);
                }
            }

            //On définis le fait qu'il ne sagit plus d'un surrogate mais d'un blank neural net andorid
            cas.isSurrogate = false;
            Hediff he = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_NoHost);
            if (he != null)
                pawn.health.RemoveHediff(he);
            he = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_LowNetworkSignal);
            if (he != null)
                pawn.health.RemoveHediff(he);

            cas.isBlankAndroid = true;
            pawn.health.AddHediff(HediffDefOf.ATPP_BlankAndroid);
            pawn.BroadcastCompSignal("ATPP_SurrogateConvertedToBlankNNAndroid");
            Utils.removeDownedSurrogateToLister(pawn);
        }

    }
}
