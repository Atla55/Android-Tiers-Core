using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;

namespace MOARANDROIDS
{
    /*
     * Handle surrogate enslavement
     */
    internal class InteractionWorker_EnslaveAttempt_Patch
    {
        [HarmonyPatch(typeof(InteractionWorker_EnslaveAttempt), "Interacted")]
        public class InteractionWorker_EnslaveAttempt_Interacted
        {
            [HarmonyPostfix]
            public static void Listener(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, string letterText, string letterLabel, LetterDef letterDef, LookTargets lookTargets)
            {
                if (recipient.IsSlave)
                {
                    CompAndroidState cas = Utils.getCachedCAS(recipient);
                    if (cas != null && cas.isSurrogate && cas.externalController != null)
                    {
                        if (cas.surrogateController != null)
                        {
                            recipient.guest.SetGuestStatus(null);
                            Find.LetterStack.ReceiveLetter("ATPP_LetterExternalSurrogateDisconnect".Translate(), "ATPP_LetterExternalSurrogateDisconnectDesc".Translate(recipient.LabelShortCap), LetterDefOf.NeutralEvent, recipient);

                            //Disconnection of the external surrogate controller
                            CompSurrogateOwner cso = Utils.getCachedCSO(cas.surrogateController);
                            if (cso != null)
                                cso.disconnectControlledSurrogate(null);
                        }

                        cas.externalController = null;
                    }
                }
            }
        }
    }
}