using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System;
using RimWorld.Planet;

namespace MOARANDROIDS
{
    internal class Pawn_HealthTracker_Patch
    {

        [HarmonyPatch(typeof(Pawn_HealthTracker), "AddHediff")]
        [HarmonyPatch(new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
        public class AddHediff_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref Pawn ___pawn, ref Hediff hediff, BodyPartRecord part)
            {
                try {
                    if (hediff == null)
                        return;

                    //If it is a VX0 then passing the pawn in surrogate mode
                    if (hediff.def == HediffDefOf.ATPP_HediffVX0Chip && (___pawn.Faction == Faction.OfPlayer || ___pawn.IsPrisoner || ___pawn.IsSlave))
                    {
                        CompAndroidState cas = Utils.getCachedCAS(___pawn);
                        //Prevent add VX0 to surrogates or M8Mech
                        if (cas == null || cas.isSurrogate || ___pawn.def == ThingDefOfAT.M8Mech)
                            return;

                        if (___pawn.Faction.IsPlayer && !Utils.preventVX0Thought)
                        {

                            //Simulation death surrogate organic
                            PawnDiedOrDownedThoughtsUtility.TryGiveThoughts(___pawn, null, PawnDiedOrDownedThoughtsKind.Died);

                            Pawn spouse = ___pawn.GetSpouse();
                            if (spouse != null && !spouse.Dead && spouse.needs.mood != null)
                            {
                                MemoryThoughtHandler memories = spouse.needs.mood.thoughts.memories;
                                memories.RemoveMemoriesOfDef(ThoughtDefOf.GotMarried);
                                memories.RemoveMemoriesOfDef(ThoughtDefOf.HoneymoonPhase);
                            }
                            Traverse.Create(___pawn.relations).Method("AffectBondedAnimalsOnMyDeath").GetValue();

                            ___pawn.health.NotifyPlayerOfKilled(null, null, null);
                        }
                        else
                        {
                            //If not from the player's faction then we will change his faction in simulate death
                            if (!___pawn.Faction.IsPlayer)
                                ___pawn.SetFaction(Faction.OfPlayer, null);
                        }

                        cas.initAsSurrogate();

                        //Blank skills
                        ___pawn.skills = new Pawn_SkillTracker(___pawn);
                        ___pawn.needs = new Pawn_NeedsTracker(___pawn);

                        //Erasing relationships
                        ___pawn.relations = new Pawn_RelationsTracker(___pawn);

                        //ALL SX are simple minded and have no other traits
                        TraitDef td = TraitDefOf.SimpleMindedAndroid;
                        Trait t = null;
                        if (td != null)
                            t = new Trait(td);

                        ___pawn.story.traits.allTraits.Clear();
                        if (t != null)
                            ___pawn.story.traits.allTraits.Add(t);
                        Utils.notifTraitsChanged(___pawn);

                        if (!___pawn.IsAndroidTier())
                        {
                            //Reset child and adulthood
                            if (!Settings.keepPuppetBackstory && ___pawn.story.childhood != null)
                            {
                                Backstory bs = null;

                                BackstoryDatabase.TryGetWithIdentifier("MercenaryRecruit", out bs);
                                if (bs != null)
                                    ___pawn.story.childhood = bs;
                            }

                            ___pawn.story.adulthood = null;
                        }


                        //Reset incapable of
                        Utils.ResetCachedIncapableOf(___pawn);

                        //New name definition
                        ___pawn.Name = new NameTriple("", "S" + 0 + "-" + Utils.GCATPP.getNextSXID(0), "");
                        Utils.GCATPP.incNextSXID(0);

                        if (!Utils.preventVX0Thought)
                        {
                            //Addition of the thought of PUPPET
                            foreach (Pawn current in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
                            {
                                current.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(Utils.thoughtDefVX0Puppet, 0), null);
                            }
                        }

                        if (___pawn.IsPrisoner)
                        {
                            if (___pawn.guest != null)
                                ___pawn.guest.SetGuestStatus(Faction.OfPlayer);
                        }
                        if (___pawn.workSettings == null)
                        {
                            ___pawn.workSettings = new Pawn_WorkSettings(___pawn);
                            ___pawn.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.Message("[ATPP] Pawn_HealthTracker.AddHediffPostfix " + ex.Message + " " + ex.StackTrace);
                }
            }
        }

        [HarmonyPatch(typeof(Pawn_HealthTracker), "AddHediff")]
        [HarmonyPatch(new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
        public class AddHediff_PatchPrefix
        {
            [HarmonyPrefix]
            public static bool Listener(ref Pawn ___pawn, ref Hediff hediff, BodyPartRecord part )
            {
                try
                {
                    //No hediff created for some reason, we stop the execution to prevent throwing error and blocking pawn creation
                    if (hediff == null)
                    {
                        return false;
                    }

                    if (___pawn.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier)
                    {
                        //If it is blacklisted Hediff 
                        if (Utils.BlacklistAndroidHediff.Contains(hediff.def.defName))
                            return false;
                    }
                    else
                    {
                        //PRevent "Part is null" issue for android tiers stuff added on non androids
                        if (part == null && Utils.ExceptionAndroidOnlyHediffs.Contains(hediff.def.defName))
                        {
                            return false;
                        }
                    }

                    //If surrogate prevent appending VX1-3 neural chips
                    CompAndroidState cas = Utils.getCachedCAS(___pawn);
                    if(cas != null && cas.isSurrogate 
                        && hediff.def.defName != HediffDefOf.ATPP_HediffVX0Chip.defName 
                        && Utils.ExceptionNeuralChip.Contains(hediff.def.defName))
                    {
                        Messages.Message("ATPP_CannotAddVXNeuralChipInSurrogates".Translate(), ___pawn, MessageTypeDefOf.NegativeEvent);
                        return false;
                    }

                    //manages the case of stacks of chips in order to restore those already present
                    if (Utils.ExceptionNeuralChip.Contains(hediff.def.defName))
                    {
                        //Prohibition added VX chips in a surrogate, we return the chip and we cross
                        if (cas != null && cas.isSurrogate)
                        {
                            IntVec3 pos1;
                            Map map1;
                            if (Utils.lastInstallImplantBillDoer != null && Utils.lastInstallImplantBillDoer.Map == ___pawn.Map)
                            {
                                pos1 = Utils.lastInstallImplantBillDoer.Position;
                                map1 = Utils.lastInstallImplantBillDoer.Map;
                            }
                            else
                            {
                                pos1 = ___pawn.Position;
                                map1 = ___pawn.Map;
                            }

                            GenSpawn.Spawn(hediff.def.spawnThingOnRemoved, pos1, map1, WipeMode.Vanish);
                            return false;
                        }

                        //We notify the hediff solarFlare that the host have a VXChip
                        Hediff heSF = ___pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.ATPP_SolarFlareAndroidImpact);
                        if(heSF != null)
                        {
                            ((Hediff_SolarFlare)heSF).withVXChip = true;
                        }

                        Hediff he = ___pawn.HaveNotStackableVXChip();
                        //Before the addition of the VXChip, the colonist already possessed a chip, we will restore it to the player
                        if (he != null)
                        {
                            ___pawn.health.RemoveHediff(he);

                            IntVec3 pos;
                            Map map;
                            if (Utils.lastInstallImplantBillDoer != null && Utils.lastInstallImplantBillDoer.Map == ___pawn.Map)
                            {
                                pos = Utils.lastInstallImplantBillDoer.Position;
                                map = Utils.lastInstallImplantBillDoer.Map;
                            }
                            else
                            {
                                pos = ___pawn.Position;
                                map = ___pawn.Map;
                            }

                            GenSpawn.Spawn(he.def.spawnThingOnRemoved, pos, map, WipeMode.Vanish);
                        }
                    }

                    return true;
                }
                catch(Exception ex)
                {
                    Log.Message("[ATPP] Pawn_HealthTracker.AddHediff " + ex.Message + " " + ex.StackTrace);
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(Pawn_HealthTracker), "RemoveHediff")]
        public class RemoveHedff_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Hediff hediff, Pawn ___pawn)
            {
                //If it is a VX0
                if (hediff != null && hediff.def == HediffDefOf.ATPP_HediffVX0Chip)
                {
                    CompAndroidState cas = Utils.getCachedCAS(___pawn);
                    if (cas == null)
                        return;

                    //Death of the host
                    ___pawn.Kill(null, null);
                }
            }
        }


        [HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
        public class MakeDowned
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, DamageInfo? dinfo, Hediff hediff)
            {
                try
                {
                    if (___pawn.IsSurrogateAndroid() && ___pawn.Faction.IsPlayer)
                    {
                        //Add surrogate to the list of downed surrogates
                        Utils.addDownedSurrogateToLister(___pawn);

                        // Disconnection of android surrogate used and owned by player ==== >>>> TO avoid ghost remodeling problems of surrogates from other factions in Lord which remains due to MakeDown removing them from the list but the disconnect will try to restart a CONNECT (in the context of external surrogates)
                        if (___pawn.IsSurrogateAndroid(true))
                        {
                            //Obtaining controller
                            CompAndroidState cas = Utils.getCachedCAS(___pawn);
                            if (cas == null)
                                return;

                            //Stop of the control mode at the controller
                            CompSurrogateOwner cso = Utils.getCachedCSO(cas.surrogateController);
                            //If surrogate downed due to another reason that skymind disconnection then we notice it to force map despawn on downed surrogate in hostile map 
                            bool downedViaDisconnect = true;
                            if (hediff != null && hediff.def != HediffDefOf.ATPP_NoHost)
                                downedViaDisconnect = false;

                            //Log.Message("=> Hediff => " + hediff.def.defName);
                            cso.stopControlledSurrogate(null, false, false, false, downedViaDisconnect);
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] Pawn_HealthTracker.MakeDowned : " + e.Message + " - " + e.StackTrace);
                }
            }
        }


        [HarmonyPatch(typeof(Pawn_HealthTracker), "NotifyPlayerOfKilled")]
        public class NotifyPlayerOfKilled
        {
            [HarmonyPrefix]
            public static bool Listener(DamageInfo? dinfo, Hediff hediff, Caravan caravan,Pawn ___pawn)
            {
                try
                {
                    //Blank android no death notification
                    if (___pawn.IsBlankAndroid())
                        return false;

                    //If surrogate
                    if (___pawn.IsSurrogateAndroid() && !___pawn.IsPrisoner)
                    {
                        Find.LetterStack.ReceiveLetter("ATPP_LetterSurrogateDisabled".Translate(___pawn.LabelShortCap), "ATPP_LetterSurrogateDisabledDesc".Translate(___pawn.LabelShortCap), LetterDefOf.Death, ___pawn, null, null);
                        return false;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Log.Message("[ATPP] Pawn_HealthTracker.NotifyPlayerOfKilled: " + e.Message + " - " + e.StackTrace);
                    return true;
                }
            }
        }


        /*
         * >>>>> SHOULD BE REMOVED LATER IF THE Psychology prefix works without issue (not already tested as psychology not already available for rw 1.3)
         * On va faker pour PSYCHOLOGY le fait que les andorids possédes déjà un hediff anxiety fake
        [HarmonyPatch(typeof(HediffSet), "GetFirstHediffOfDef")]
        public class GetFirstHediffOfDef
        {
            [HarmonyPostfix]
            public static void Listener(HediffSet __instance,ref Hediff __result, HediffDef def, bool mustBeVisible = false)
            {
                try
                {
                    //Si psychology mod pas présent on jerte
                    if (!Utils.PSYCHOLOGY_LOADED)
                        return;

                    Hediff find = null;
                    //Si Android et heddiff faisant partis des hediff blacklistés
                    if (__instance.pawn.IsAndroidTier() && Utils.BlacklistedHediffsForAndroids.Contains(def.defName))
                    {
                        //Recherche d'un dummyHediff deja ajouté
                        for (int i = 0; i < __instance.hediffs.Count; i++)
                        {
                            if (__instance.hediffs[i].def == def && (!mustBeVisible || __instance.hediffs[i].Visible))
                            {
                                find = __instance.hediffs[i];
                                break;
                            }
                        }

                        if(find == null)
                        {
                            find = __instance.pawn.health.AddHediff(DefDatabase<HediffDef>.GetNamed("ATPP_DummyHediff"));
                        }
                        //Log.Message("DummyHediff");
                        __result = find;
                    }
                }
                catch (Exception e)
                {
                    Log.Message("[ATPP] HediffSet.GetFirstHediffOfDef" + e.Message + " - " + e.StackTrace);
                }
            }
        }*/
    }
}