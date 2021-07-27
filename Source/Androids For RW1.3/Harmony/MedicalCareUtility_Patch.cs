using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Verse.Sound;

namespace MOARANDROIDS
{
    internal class MedicalCareUtility_Patch
    {

        /*
         * Permet de savoir dans AllowsMedicine si le patient est un android
         */
        [HarmonyPatch(typeof(MedicalCareUtility), "AllowsMedicine")]
        public class AllowsMedicine_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(MedicalCareCategory cat, ThingDef meds, ref bool __result)
            {
                if (Utils.FindBestMedicinePatient == null || !Utils.SMARTMEDICINE_LOADED)
                    return;

                //Log.Message("HERE2 "+ Utils.FindBestMedicinePatient.LabelShortCap);
                //Si androide slection que des nanokits
                if (Utils.FindBestMedicinePatient.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier)
                {
                    if (!Utils.ExceptionNanoKits.Contains(meds.defName))
                        __result = false;
                }
                else
                {
                    //Humain pas de nanokits
                    if (Utils.ExceptionNanoKits.Contains(meds.defName))
                        __result = false;
                }

                //Utils.FindBestMedicinePatient = null;
            }
        }


        /*
         * PostFix évitant d'attribuer de need comfort et outdoor aux T1 et T2 et l'hygiene a l'ensemble des robots
         */
        [HarmonyPatch(typeof(MedicalCareUtility), "MedicalCareSelectButton")]
        public class MedicalCareSelectButton_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Rect rect, Pawn pawn)
            {
                try
                {
                    if (pawn.RaceProps.FleshType == FleshTypeDefOfAT.AndroidTier)
                    {
                        Func<Pawn, MedicalCareCategory> getPayload = new Func<Pawn, MedicalCareCategory>(MedicalCareSelectButton_GetMedicalCare);
                        Func<Pawn, IEnumerable<Widgets.DropdownMenuElement<MedicalCareCategory>>> menuGenerator = new Func<Pawn, IEnumerable<Widgets.DropdownMenuElement<MedicalCareCategory>>>(MedicalCareSelectButton_GenerateMenu);
                        Texture2D buttonIcon;
                        int index = (int)pawn.playerSettings.medCare;
                        if (index == 0)
                            buttonIcon = Tex.NoCare;
                        else if (index == 1)
                            buttonIcon = Tex.NoMed;
                        else if (index == 2)
                            buttonIcon = Tex.NanoKitBasic;
                        else if (index == 3)
                            buttonIcon = Tex.NanoKitIntermediate;
                        else
                            buttonIcon = Tex.NanoKitAdvanced;

                        Widgets.Dropdown<Pawn, MedicalCareCategory>(rect, pawn, getPayload, menuGenerator, null, buttonIcon, null, null, null, true);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch(Exception e)
                {
                    Log.Message("[ATPP] MedicalCareUtility.MedicalCareSelectButton : " + e.Message + " - " + e.StackTrace);
                    return true;
                }
            }

            private static MedicalCareCategory MedicalCareSelectButton_GetMedicalCare(Pawn pawn)
            {
                return pawn.playerSettings.medCare;
            }

            private static IEnumerable<Widgets.DropdownMenuElement<MedicalCareCategory>> MedicalCareSelectButton_GenerateMenu(Pawn p)
            {
                for (int i = 0; i < 5; i++)
                {
                    MedicalCareCategory mc = (MedicalCareCategory)i;
                    yield return new Widgets.DropdownMenuElement<MedicalCareCategory>
                    {
                        option = new FloatMenuOption(mc.GetLabel(), delegate
                        {
                            p.playerSettings.medCare = mc;
                        }, MenuOptionPriority.Default, null, null, 0f, null, null),
                        payload = mc
                    };
                }
            }
        }
        
        private static MedicalCareCategory MedicalCareSelectButton_GetMedicalCare(Pawn pawn)
        {
            return pawn.playerSettings.medCare;
        }

        private static IEnumerable<Widgets.DropdownMenuElement<MedicalCareCategory>> MedicalCareSelectButton_GenerateMenu(Pawn p)
        {
            for (int i = 0; i < 5; i++)
            {
                MedicalCareCategory mc = (MedicalCareCategory)i;
                yield return new Widgets.DropdownMenuElement<MedicalCareCategory>
                {
                    option = new FloatMenuOption(mc.GetLabel(), delegate
                    {
                        p.playerSettings.medCare = mc;
                    }, MenuOptionPriority.Default, null, null, 0f, null, null),
                    payload = mc
                };
            }
        }
    }




}