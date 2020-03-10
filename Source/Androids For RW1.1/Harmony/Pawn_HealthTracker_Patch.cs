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

                    //Si il sagit d'une VX0 alors passation du pawn en mode surrogate
                    if (hediff.def.defName == "ATPP_HediffVX0Chip" && (___pawn.Faction == Faction.OfPlayer || ___pawn.IsPrisoner))
                    {
                        CompAndroidState cas = ___pawn.TryGetComp<CompAndroidState>();
                        if (cas == null || cas.isSurrogate)
                            return;

                        if (___pawn.Faction.IsPlayer && !Utils.preventVX0Thought)
                        {

                            //Simulation mort surrogate organic
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
                            //Si pas de la faction du player alors on va changer sa faction dans simuler de mort
                            if (!___pawn.Faction.IsPlayer)
                                ___pawn.SetFaction(Faction.OfPlayer, null);
                        }

                        cas.initAsSurrogate();

                        //SKills vierges
                        ___pawn.skills = new Pawn_SkillTracker(___pawn);
                        ___pawn.needs = new Pawn_NeedsTracker(___pawn);

                        //Effacement des relations
                        ___pawn.relations = new Pawn_RelationsTracker(___pawn);

                        //TOuts les SX sont simple minded et ont aucuns autres traits
                        TraitDef td = DefDatabase<TraitDef>.GetNamed("SimpleMindedAndroid", false);
                        Trait t = null;
                        if (td != null)
                            t = new Trait(td);

                        ___pawn.story.traits.allTraits.Clear();
                        if (t != null)
                            ___pawn.story.traits.allTraits.Add(t);
                        Utils.notifTraitsChanged(___pawn);

                        if (!___pawn.IsAndroidTier())
                        {
                            //Reset du child et adulthood
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

                        //Définition nouveau nom
                        ___pawn.Name = new NameTriple("", "S" + 0 + "-" + Utils.GCATPP.getNextSXID(0), "");
                        Utils.GCATPP.incNextSXID(0);

                        if (!Utils.preventVX0Thought)
                        {
                            //Ajout du thought de PUPPET
                            foreach (Pawn current in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
                            {
                                current.needs.mood.thoughts.memories.TryGainMemory(ThoughtMaker.MakeThought(Utils.thoughtDefVX0Puppet, 0), null);
                            }
                        }

                        if (___pawn.IsPrisoner)
                        {
                            if (___pawn.guest != null)
                                ___pawn.guest.SetGuestStatus(Faction.OfPlayer, false);
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
                    if (___pawn.IsAndroidTier())
                    {
                        //S'il sagit d'Hediff blacklistés 
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

                    //gére le cas des empilements de chips afin de restituer celles déjà présentes
                    if (Utils.ExceptionNeuralChip.Contains(hediff.def.defName))
                    {
                        CompAndroidState cas = ___pawn.TryGetComp<CompAndroidState>();

                        //Interdiction ajouté VX puces dans un surrogate, on restitue la puce et on se barre
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

                        Hediff he = ___pawn.HaveNotStackableVXChip();
                        //Avant l'ajout de la VXChip le colon possédés déjà une puce on va la restaurer au joueur
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
                //Si il sagit d'une VX0 
                if (hediff.def.defName == "ATPP_HediffVX0Chip")
                {
                    CompAndroidState cas = ___pawn.TryGetComp<CompAndroidState>();
                    if (cas == null)
                        return;

                    //Mort de l'hote
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
                    // Deconnexion of Is surrogate android used et que appartenant au joueur ====>>>> POUR eviter les problemes de reminescence fantome de surrogates d'autres factions dans des Lord qui reste a cause du fait que le MakeDown les enleves de la liste mais le disconnect va essayer de relancer un CONNECT (dans le cadre des surrogates externes)
                    if (___pawn.IsSurrogateAndroid(true) && ___pawn.Faction.IsPlayer)
                    {
                        //Obtention controlleur
                        CompAndroidState cas = ___pawn.TryGetComp<CompAndroidState>();
                        if (cas == null)
                            return;

                        //Arret du mode de control chez le controller
                        CompSurrogateOwner cso = cas.surrogateController.TryGetComp<CompSurrogateOwner>();
                        cso.stopControlledSurrogate(null);
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
                    //Blank android pas de notification de la mort
                    if (___pawn.IsBlankAndroid())
                        return false;

                    //Si surrogate
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
         * On va faker pour PSYCHOLOGY le fait que les andorids possédes déjà un hediff anxiety fake
         */
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
        }
    }
}