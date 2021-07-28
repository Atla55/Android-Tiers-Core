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
    internal class InteractionWorker_RecruitAttempt_Patch
    {
        [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "Interacted")]
        public class InteractionWorker_EnslaveAttempt_Interacted
        {
            [HarmonyPostfix]
            public static void Listener(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, string letterText, string letterLabel, LetterDef letterDef, LookTargets lookTargets)
            {
                if (recipient.IsColonist)
                {
                    CompAndroidState cas = Utils.getCachedCAS(recipient);
                    if (cas != null && cas.isSurrogate && cas.externalController != null)
                    {
                        if (cas.surrogateController != null)
                        {
                            //Recruited surrogate controller disconnect from the surrogate then escape from his base
                            if (Rand.Chance(Settings.chanceRecruitedSurrogateControllerCanEscapeAndJoin))
                            {
                                Utils.GCATPP.externalSurrogateCJoiner.Add(cas.surrogateController);
                                CompAndroidState casSC = Utils.getCachedCAS(cas.surrogateController);
                                if (casSC != null)
                                {
                                    //Between 6h to 2d
                                    casSC.externalControllerConvertedJoinGT = Find.TickManager.TicksGame + Rand.Range(15000, 120000);
                                }
                                Find.LetterStack.ReceiveLetter("ATPP_LetterExternalSurrogateEscape".Translate(), "ATPP_LetterExternalSurrogateEscapeDesc".Translate(recipient.LabelShortCap), LetterDefOf.NeutralEvent, recipient);
                            }
                            else
                            {
                                //Recruited surrogate controller is arrested and disconnected
                                Find.LetterStack.ReceiveLetter("ATPP_LetterTraitorOffline".Translate(), "ATPP_LetterTraitorOfflineDesc".Translate(recipient.LabelShortCap), LetterDefOf.NegativeEvent, recipient);
                            }

                            //Disconnection of the external surrogate controller
                            CompSurrogateOwner cso = Utils.getCachedCSO(cas.surrogateController);
                            if (cso != null)
                                cso.disconnectControlledSurrogate(null);
                        }

                        //On vire l'external controller
                        cas.externalController = null;
                    }
                }
            }
        }
    }
}