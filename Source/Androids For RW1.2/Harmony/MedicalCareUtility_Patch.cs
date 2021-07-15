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
                //Log.Message("HERE1");
                if (Utils.FindBestMedicinePatient == null || !Utils.SMARTMEDICINE_LOADED)
                    return;

                //Log.Message("HERE2 "+ Utils.FindBestMedicinePatient.LabelShortCap);
                //Si androide slection que des nanokits
                if (Utils.FindBestMedicinePatient.IsAndroidTier() || Utils.FindBestMedicinePatient.IsCyberAnimal())
                {
                    if (!Utils.ExceptionNanoKits.Contains(meds.defName))
                        __result = false;
                }
                else
                {
                    //Log.Message("BLOP");
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
                    if (Utils.ExceptionAndroidList.Contains(pawn.def.defName) || pawn.IsCyberAnimal())
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

        [HarmonyPatch(typeof(MedicalCareUtility), "MedicalCareSetter")]
        public class MedicalCareSetter_Patch
        {

            private static bool medicalCarePainting;


            [HarmonyPrefix]
            public static bool Listener(Rect rect, ref MedicalCareCategory medCare)
            {

                    List<object> obj = Find.Selector.SelectedObjects;
                    try
                    {
                        if (obj.Count == 1 && (obj[0] is Pawn))
                        {
                            Pawn pawn = (Pawn)obj[0];

                            if (Utils.ExceptionAndroidList.Contains(pawn.def.defName) || pawn.IsCyberAnimal())
                            {
                                Rect rect2 = new Rect(rect.x, rect.y, rect.width / 5f, rect.height);
                                for (int i = 0; i < 5; i++)
                                {
                                    MedicalCareCategory mc = (MedicalCareCategory)i;
                                    Widgets.DrawHighlightIfMouseover(rect2);
                                    Texture2D tex;
                                    string text;

                                    if (i == 0)
                                    {
                                        tex = Tex.NoCare;
                                        text = "ATPP_NanoKitsNoCare".Translate();
                                    }
                                    else if (i == 1)
                                    {
                                        tex = Tex.NoMed;
                                        text = "ATPP_NanoKitsNoKitJustVisit".Translate();
                                    }
                                    else if (i == 2)
                                    {
                                        tex = Tex.NanoKitBasic;
                                        text = "ATPP_NanoKitsBasic".Translate();
                                    }
                                    else if (i == 3)
                                    {
                                        tex = Tex.NanoKitIntermediate;
                                        text = "ATPP_NanoKitsIntermediate".Translate();
                                    }
                                    else
                                    {
                                        tex = Tex.NanoKitAdvanced;
                                        text = "ATPP_NanoKitsAdvanced".Translate();
                                    }

                                    GUI.DrawTexture(rect2, tex);
                                    Widgets.DraggableResult draggableResult = Widgets.ButtonInvisibleDraggable(rect2, false);
                                    if (draggableResult == Widgets.DraggableResult.Dragged)
                                    {
                                        medicalCarePainting = true;
                                    }
                                    if ((medicalCarePainting && Mouse.IsOver(rect2) && medCare != mc) || draggableResult.AnyPressed())
                                    {
                                        medCare = mc;
                                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                                    }
                                    if (medCare == mc)
                                    {
                                        Widgets.DrawBox(rect2, 3);
                                    }
                                    TooltipHandler.TipRegion(rect2, () => text, 632165 + i * 17);
                                    rect2.x += rect2.width;
                                }
                                if (!Input.GetMouseButton(0))
                                {
                                    medicalCarePainting = false;
                                }

                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                            return true;
                    }
                    catch(Exception e)
                    {
                        Log.Message("[ATPP] MedicalCareUtility.MedicalCareSetter(Error) : " + e.Message + " - " + e.StackTrace);
                        return true;
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