using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

namespace MOARANDROIDS
{
    internal class PawnGenerator_Patch

    {
        [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn")]
        [HarmonyPatch(new Type[] { typeof(PawnGenerationRequest)}, new ArgumentType[] { ArgumentType.Normal })]
        public class GeneratePawn_Patch
        {
            [HarmonyPostfix]
            public static void Listener(PawnGenerationRequest request, ref Pawn __result)
            {
                try
                {
                    bool isAndroidTier = __result.IsAndroidTier();

                    //Pas d'application de filtrage de creation d'androide si le mode est playerStarter avec le pawnkindDef de base du scenario de AT
                    if (!(request.Context == PawnGenerationContext.PlayerStarter && Utils.ExceptionPlayerStartingAndroidPawnKindList.Contains(request.KindDef.defName))) {
                        //Vire chance generation android
                        if (Settings.androidsAreRare
                            && __result.IsAndroidTier()
                            && ((Current.ProgramState == ProgramState.Entry) || (Current.ProgramState == ProgramState.Playing && request.Faction != Faction.OfPlayer))
                            && Rand.Chance(0.95f))
                        {
                            PawnGenerationRequest r = new PawnGenerationRequest(PawnKindDefOf.AncientSoldier, request.Faction, request.Context, request.Tile, request.ForceGenerateNewPawn, request.Newborn,
                                request.AllowDead, request.AllowDowned, request.CanGeneratePawnRelations, request.MustBeCapableOfViolence, request.ColonistRelationChanceFactor,
                                request.ForceAddFreeWarmLayerIfNeeded, request.AllowGay, request.AllowFood, request.AllowAddictions,request.Inhabitant, request.CertainlyBeenInCryptosleep,
                                request.ForceRedressWorldPawnIfFormerColonist, request.WorldPawnFactionDoesntMatter, request.BiocodeWeaponChance, request.ExtraPawnForExtraRelationChance,
                                request.RelationWithExtraPawnChanceFactor, request.ValidatorPreGear, request.ValidatorPostGear, request.ForcedTraits, request.ProhibitedTraits, request.MinChanceToRedressWorldPawn, request.FixedBiologicalAge, request.FixedChronologicalAge, request.FixedGender, request.FixedMelanin, request.FixedLastName, request.FixedBirthName, request.FixedTitle);

                            __result = PawnGenerator.GeneratePawn(r);
                        }
                    }

                    //Remove illogiocal traits with androids
                    if (isAndroidTier)
                    {
                        if (__result.gender == Gender.Male)
                        {

                            BodyTypeDef bd = DefDatabase<BodyTypeDef>.GetNamed("Male", false);
                            if (bd != null)
                                __result.story.bodyType = bd;
                        }
                        else
                        {
                            BodyTypeDef bd = DefDatabase<BodyTypeDef>.GetNamed("Female", false);
                            if (bd != null)
                                __result.story.bodyType = bd;
                        }


                        bool isAndroidWithSkin = Utils.ExceptionAndroidWithSkinList.Contains(__result.def.defName);

                        if (isAndroidWithSkin)
                        {
                            //force not damaged face for skinned androids
                            Utils.changeHARCrownType(__result, "Average_Normal");

                            if (Utils.RIMMSQOL_LOADED && Utils.lastResolveAllGraphicsHeadGraphicPath != null)
                            {
                                __result.story.GetType().GetField("headGraphicPath", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__result.story, Utils.lastResolveAllGraphicsHeadGraphicPath);
                                Utils.lastResolveAllGraphicsHeadGraphicPath = null;
                            }
                        }

                        Utils.removeMindBlacklistedTrait(__result);
                        //Chance that android can be painted (skinned androids excluded)
                        if (!isAndroidWithSkin && Rand.Chance(Settings.chanceGeneratedAndroidCanBePaintedOrRust))
                        {
                            CompAndroidState cas = __result.TryGetComp<CompAndroidState>();
                            if (cas != null)
                            {
                                if (Utils.forceGeneratedAndroidToBeDefaultPainted)
                                {
                                    cas.paintingIsRusted = false;
                                    cas.paintingRustGT = (Rand.Range(Settings.minDaysAndroidPaintingCanRust, Settings.maxDaysAndroidPaintingCanRust) * 60000);
                                    cas.customColor = (int)AndroidPaintColor.Default;
                                }
                                else
                                {
                                    if (Settings.androidsCanRust && Rand.Chance(0.35f))
                                    {
                                        cas.setRusted();
                                    }
                                    else
                                    {
                                        cas.customColor = Rand.Range((int)AndroidPaintColor.Black, ((int)AndroidPaintColor.Khaki) + 1);
                                    }
                                }
                            }
                        }
                    }

                    //Prevent generation M7/T5 dans ecran style EBDPrep carefully
                    if(Settings.preventM7T5AppearingInCharacterScreen &&  Current.ProgramState == ProgramState.Entry )
                    {
                        if(__result.def.defName == Utils.M7 || __result.def.defName == Utils.T5)
                        {
                            PawnGenerationRequest r = new PawnGenerationRequest(Utils.AndroidsPKDNeutral.RandomElement(), request.Faction, request.Context, request.Tile, request.ForceGenerateNewPawn, request.Newborn,
                                request.AllowDead, request.AllowDowned, request.CanGeneratePawnRelations, request.MustBeCapableOfViolence, request.ColonistRelationChanceFactor,
                                request.ForceAddFreeWarmLayerIfNeeded, request.AllowGay, request.AllowFood, request.AllowAddictions, request.Inhabitant, request.CertainlyBeenInCryptosleep,
                                request.ForceRedressWorldPawnIfFormerColonist, request.WorldPawnFactionDoesntMatter, request.BiocodeWeaponChance, request.ExtraPawnForExtraRelationChance,
                                request.RelationWithExtraPawnChanceFactor, request.ValidatorPreGear, request.ValidatorPostGear, request.ForcedTraits, request.ProhibitedTraits, request.MinChanceToRedressWorldPawn, request.FixedBiologicalAge, request.FixedChronologicalAge, request.FixedGender, request.FixedMelanin, request.FixedLastName, request.FixedBirthName, request.FixedTitle);

                            __result = PawnGenerator.GeneratePawn(r);
                        }
                    }

                    if (!Settings.notRemoveAllSkillPassionsForBasicAndroids)
                    {
                        //Si T1/T2
                        if (__result.IsBasicAndroidTier() && __result.def.defName != "M7Mech" && __result.skills != null && __result.skills.skills != null)
                        {
                            foreach(var sr in __result.skills.skills)
                            {
                                sr.passion = Passion.None;
                            }
                        }
                    }
                    if (!Settings.notRemoveAllTraitsFromT1T2)
                    {
                        //Si T1/T2
                        if (__result.IsBasicAndroidTier() && __result.def.defName != "M7Mech")
                        {
                            Utils.removeAllTraits(__result);
                        }
                    }

                    //Si GYNOID chargé changement sex android en fonction chance
                    if (Utils.ANDROIDTIERSGYNOID_LOADED
                        && isAndroidTier
                        && (__result.def.defName == Utils.T1 || __result.def.defName == Utils.T2 || __result.def.defName == Utils.T3 || __result.def.defName == Utils.T4 )
                        && Current.ProgramState == ProgramState.Playing && __result.Faction == Faction.OfPlayer)
                    {
                        if (Rand.Chance(Settings.percentageChanceMaleAndroidModel))
                        {
                            __result.gender = Gender.Male;
                            __result.story.bodyType = BodyTypeDefOf.Male;
                        }
                        else
                        {
                            __result.gender = Gender.Female;
                            __result.story.bodyType = BodyTypeDefOf.Female;
                        }

                    }

                    //Définiton des traits pour les androides générés a destination du player
                    if (Current.ProgramState == ProgramState.Playing && !Settings.basicAndroidsRandomSKills && __result.Faction == Faction.OfPlayer)
                    {
                        SkillRecord sr=null;

                        if (__result.def.defName == Utils.T1)
                        {
                            sr = __result.skills.GetSkill(SkillDefOf.Animals);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Animals;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Artistic);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Artistic;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Construction);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Construction;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Cooking);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Cooking;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Crafting);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Crafting;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Intellectual);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Intellectual;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Medicine);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Medical;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Melee);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Melee;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Mining);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Mining;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Plants);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Plants;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Shooting);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Shoot;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Social);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT1Social;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                        }
                        else if (__result.def.defName == Utils.T2)
                        {
                            sr = __result.skills.GetSkill(SkillDefOf.Animals);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Animals;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Artistic);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Artistic;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Construction);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Construction;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Cooking);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Cooking;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Crafting);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Crafting;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Intellectual);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Intellectual;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Medicine);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Medical;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Melee);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Melee;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Mining);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Mining;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Plants);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Plants;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Shooting);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Shoot;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                            sr = __result.skills.GetSkill(SkillDefOf.Social);
                            if (sr != null)
                            {
                                sr.levelInt = Settings.defaultSkillT2Social;
                                sr.xpSinceLastLevel = 0;
                                sr.xpSinceMidnight = 0;
                            }
                        }
                    }


                    //If TX3/4 then force not damaged head
                    if (__result.def.defName == Utils.TX3 || __result.def.defName == Utils.TX4)
                    {
                        Utils.changeHARCrownType(__result, "Average_Normal");

                        __result.Drawer.renderer.graphics.ResolveAllGraphics();
                    }

                }
                catch(Exception ex)
                {
                    Log.Message("[ATPP] PawnGenerator.GeneratePawn " + ex.Message + " " + ex.StackTrace);
                }
            }
        }
    }
}