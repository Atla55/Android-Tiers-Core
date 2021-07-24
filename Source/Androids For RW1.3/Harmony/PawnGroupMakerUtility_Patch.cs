using Verse;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace MOARANDROIDS
{
    internal class PawnGroupMakerUtility_Patch

    {
        [HarmonyPatch(typeof(PawnGroupMakerUtility), "GeneratePawns")]
        public class GeneratePawns_Patch
        {
            [HarmonyPostfix]
            public static void Listener(PawnGroupMakerParms parms, bool warnOnZeroResults, ref IEnumerable<Pawn> __result)
            {
                try
                {

                    Map playerMap = Utils.getRandomMapOfPlayer();
                    //If active solar flare then no surrogates are generated to prevent ridiculous spawn of dawned surrogates
                    if (!Settings.otherFactionsCanUseSurrogate
                        || (parms.faction != null && Utils.ExceptionBlacklistedFactionNoSurrogate.Contains(parms.faction.def.defName))
                        || (playerMap != null && playerMap.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare)) || (Settings.androidsAreRare && Rand.Chance(0.95f)))
                        return;

                    int nbHumanoids = 0;

                    foreach (var e in __result)
                    {
                        //Si humanoide
                        if (e.def.race != null && e.def.race.Humanlike)
                        {
                            //Si pas commercant
                            if (e.trader == null && e.TraderKind == null)
                                nbHumanoids++;
                        }
                    }

                    //Faction de niveau industriel et plus ET nb pawn généré supérieur ou égal à 5
                    if (parms.faction.def.techLevel >= TechLevel.Industrial && nbHumanoids >= 5)
                    {
                        List<Pawn> other = new List<Pawn>();
                        List<Pawn> ret = new List<Pawn>();
                        ISet<Pawn> tmp = __result.ToHashSet();

                        //On supprime les non humains ET trader
                        //int nba = tmp.RemoveAll((Pawn x) => (x.def.race == null || !x.def.race.Humanlike || x.trader != null || x.TraderKind != null));
                        foreach (var x in tmp)
                        {
                            if (x.def.race == null || !x.def.race.Humanlike || x.trader != null || x.TraderKind != null)
                            {
                                other.Add(x);
                            }
                        }

                        //Purge dans list de travail
                        for (int i = 0; i < other.Count; i++)
                        {
                            Pawn e = other[i];
                            tmp.Remove(e);
                        }

                        //Log.Message(other.Count + " animaux et traders virés");

                        //Calcul nb pawn a recreer en mode surrogate
                        int nb = (int)(tmp.Count * Rand.Range(Settings.percentageOfSurrogateInAnotherFactionGroupMin, Settings.percentageOfSurrogateInAnotherFactionGroup));
                        if (nb <= 0)
                        {
                            if (Settings.percentageOfSurrogateInAnotherFactionGroupMin == 0.0f)
                                return;
                            else
                                nb = 1;
                        }

                        //On va supprimer aleatoirement N pawns pour arriver a nb
                        while (tmp.Count > nb)
                        {
                            Pawn p = tmp.RandomElement();
                            other.Add(p);
                            tmp.Remove(p);
                        }

                        List<Pawn> tmpList = tmp.FastToList();

                        //On va se servir des nb pawn pregénéré par la fonction patché comme controller
                        for (int i = 0; i != nb; i++)
                        {
                            //PawnGenerationRequest request = new PawnGenerationRequest(Utils.AndroidsPKD[3], parms.faction, PawnGenerationContext.NonPlayer, parms.tile, false, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
                            //Pawn surrogate = PawnGenerator.GeneratePawn(request);

                            PawnKindDef rpkd = null;
                            var tmpListElem = tmpList[i];

                            if (parms.groupKind == PawnGroupKindDefOf.Peaceful || parms.groupKind == PawnGroupKindDefOf.Trader)
                            {
                                if (Rand.Chance(0.10f))
                                {
                                    if (!Utils.TXSERIE_LOADED || Rand.Chance(0.5f))
                                        rpkd = Utils.AndroidsPKDNeutral[3];
                                    else if (Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsXISeriePKDNeutral[3];
                                    else
                                        rpkd = Utils.AndroidsXSeriePKDNeutral[3];
                                }
                                else if (Rand.Chance(0.35f))
                                {
                                    if (!Utils.TXSERIE_LOADED || Rand.Chance(0.5f))
                                        rpkd = Utils.AndroidsPKDNeutral[2];
                                    else if (Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsXISeriePKDNeutral[2];
                                    else
                                        rpkd = Utils.AndroidsXSeriePKDNeutral[2];
                                }
                                else if (Rand.Chance(0.55f))
                                {
                                    if (!Utils.TXSERIE_LOADED || Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsPKDNeutral[1];
                                    else if (Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsXISeriePKDNeutral[1];
                                    else
                                        rpkd = Utils.AndroidsXSeriePKDNeutral[1];
                                }
                                else
                                {
                                    if (!Utils.TXSERIE_LOADED || Rand.Chance(0.5f))
                                        rpkd = Utils.AndroidsPKDNeutral[0];
                                    else if (Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsXISeriePKDNeutral[0];
                                    else
                                        rpkd = Utils.AndroidsXSeriePKDNeutral[0];
                                }
                            }
                            else
                            {
                                if (Rand.Chance(0.10f))
                                {
                                    if (!Utils.TXSERIE_LOADED || Rand.Chance(0.5f))
                                        rpkd = Utils.AndroidsPKDHostile[3];
                                    else if (Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsXISeriePKDHostile[3];
                                    else
                                        rpkd = Utils.AndroidsXSeriePKDHostile[3];
                                }
                                else if (Rand.Chance(0.35f))
                                {
                                    if (!Utils.TXSERIE_LOADED || Rand.Chance(0.5f))
                                        rpkd = Utils.AndroidsPKDHostile[2];
                                    else if (Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsXISeriePKDHostile[2];
                                    else
                                        rpkd = Utils.AndroidsXSeriePKDHostile[2];
                                }
                                else if (Rand.Chance(0.55f))
                                {
                                    if (!Utils.TXSERIE_LOADED || Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsPKDHostile[1];
                                    else if (Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsXISeriePKDHostile[1];
                                    else
                                        rpkd = Utils.AndroidsXSeriePKDHostile[1];
                                }
                                else
                                {
                                    if (!Utils.TXSERIE_LOADED || Rand.Chance(0.5f))
                                        rpkd = Utils.AndroidsPKDHostile[0];
                                    else if (Rand.Chance(0.75f))
                                        rpkd = Utils.AndroidsXISeriePKDHostile[0];
                                    else
                                        rpkd = Utils.AndroidsXSeriePKDHostile[0];
                                }
                            }

                            //Utils.AndroidsPKD.RandomElement();
                            Pawn surrogate = Utils.generateSurrogate(parms.faction, rpkd, IntVec3.Invalid, null, false, true, parms.tile, true, parms.inhabitants);

                            //Si en mode rare alors override de la creation d'androides en ancient, on squeeze l'init du pawn
                            if (!surrogate.IsAndroidTier())
                            {
                                surrogate.Destroy();
                                ret.Add(tmpListElem);
                                continue;
                            }

                            //Log.Message("--Surrogate generated pour "+tmp[i].def.defName);

                            //On vire les equipements/inventaires/vetements du surrogate
                            if (surrogate.inventory != null && surrogate.inventory.innerContainer != null)
                                surrogate.inventory.innerContainer.Clear();

                            surrogate.apparel.DestroyAll();

                            surrogate.equipment.DestroyAllEquipment();


                            CompAndroidState cas = Utils.getCachedCAS(surrogate);
                            if (cas != null)
                            {
                                cas.externalController = tmpListElem;
                                CompSurrogateOwner cso = Utils.getCachedCSO(tmpListElem);
                                if (cso != null)
                                {
                                    cso.setControlledSurrogate(surrogate, true);
                                }
                            }

                            //Transfere equipement
                            //Log.Message("--Traitement des equipements");
                            if (tmpListElem.equipment != null && surrogate.equipment != null)
                            {
                                /*Pawn_EquipmentTracker pet = tmp[i].equipment;
                                pet.pawn = surrogate;
                                tmp[i].equipment = surrogate.equipment;
                                tmp[i].equipment.pawn = tmp[i];
                                surrogate.equipment = pet;*/
                                surrogate.equipment.DestroyAllEquipment();

                                List<ThingWithComps> list = tmpListElem.equipment.AllEquipmentListForReading.FastToList();
                                for (int i1 = 0; i1 < list.Count; i1++)
                                {
                                    ThingWithComps e = list[i1];
                                    try
                                    {
                                        tmpListElem.equipment.Remove(e);
                                        if (!(e.def.equipmentType == EquipmentType.Primary && surrogate.equipment.Primary != null))
                                            surrogate.equipment.AddEquipment(e);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Message("[ATPP] PawnGroupMakerUtility.GeneratePawns.transfertEquipment " + ex.Message + " " + ex.StackTrace);
                                    }
                                }


                                for (int i1 = 0; i1 < surrogate.equipment.AllEquipmentListForReading.Count; i1++)
                                {
                                    ThingWithComps e = surrogate.equipment.AllEquipmentListForReading[i1];
                                    try
                                    {
                                        if (Utils.CELOADED && e != null && e.def.IsRangedWeapon)
                                        {
                                            ThingComp ammoUser = Utils.TryGetCompByTypeName(e, "CompAmmoUser", "CombatExtended");
                                            if (ammoUser != null)
                                            {
                                                var props = Traverse.Create(ammoUser).Property("Props").GetValue();
                                                int magazineSize = Traverse.Create(props).Field("magazineSize").GetValue<int>();
                                                ThingDef def = Traverse.Create(ammoUser).Field("selectedAmmo").GetValue<ThingDef>();
                                                if (def != null)
                                                {
                                                    Traverse.Create(ammoUser).Method("ResetAmmoCount", new object[] { def }).GetValue();
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                            }

                            //Transfert vetements
                            if (tmpListElem.apparel != null)
                            {
                                try
                                {
                                    //Log.Message("--Traitement des vetements");
                                    List<Apparel> list = tmpListElem.apparel.WornApparel.FastToList();
                                    for (int i1 = 0; i1 < list.Count; i1++)
                                    {
                                        Apparel e = list[i1];
                                        //Check si vetement peut etre porté par le surrogate
                                        string path = "";
                                        if (e.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
                                        {
                                            path = e.def.apparel.wornGraphicPath;
                                        }
                                        else
                                        {
                                            path = $"{e.def.apparel.wornGraphicPath}_{surrogate.story.bodyType.defName}_south";
                                        }

                                        Texture2D appFoundTex = null;
                                        Texture2D appTex = null;
                                        //CHeck dans les mods de la texture
                                        for (int j = LoadedModManager.RunningModsListForReading.Count - 1; j >= 0; j--)
                                        {
                                            appTex = LoadedModManager.RunningModsListForReading[j].GetContentHolder<Texture2D>().Get(path);
                                            if (appTex != null)
                                            {
                                                appFoundTex = appTex;
                                                break;
                                            }
                                        }
                                        //Check RW mods  de la texture
                                        if (appFoundTex == null)
                                        {
                                            path = GenFilePaths.ContentPath<Texture2D>() + path;
                                            appFoundTex = (Texture2D)(object)Resources.Load<Texture2D>(path);
                                        }

                                        //SI pb avec texture on ne l'ajoute pas
                                        if (appFoundTex != null)
                                        {
                                            tmpListElem.apparel.Remove(e);
                                            surrogate.apparel.Wear(e);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Message("[ATPP] PawnGroupMakerUtility.TransfertApparel " + ex.Message + " " + ex.StackTrace);
                                }
                            }


                            //Log.Message((tmp[i].inventory == null)+" "+(tmp[i].inventory.innerContainer == null)+" "+(surrogate.inventory == null)+" "+(surrogate.inventory.innerContainer== null));
                            //Inventaire
                            if (tmpListElem.inventory != null && tmpListElem.inventory.innerContainer != null && surrogate.inventory != null && surrogate.inventory.innerContainer != null)
                            {
                                try
                                {
                                    tmpListElem.inventory.innerContainer.TryTransferAllToContainer(surrogate.inventory.innerContainer);

                                    var elementsToRemove = new List<Thing>();
                                    //Suppression des drogues 
                                    IList<Thing> list = surrogate.inventory.innerContainer;
                                    for (int i1 = 0; i1 < list.Count; i1++)
                                    {
                                        Thing el = list[i1];
                                        if (el.def.IsDrug)
                                            elementsToRemove.Add(el);
                                    }

                                    foreach (var elementToRemove in elementsToRemove)
                                        surrogate.inventory.innerContainer.Remove(elementToRemove);
                                }
                                catch (Exception ex)
                                {
                                    Log.Message("[ATPP] PawnGroupMakerUtility.GeneratePawns.transfertInventory " + ex.Message + " " + ex.StackTrace);
                                }
                            }

                            ret.Add(surrogate);
                        }

                        //On remet dans le ret les autres pawns non "surogatisés"
                        __result = other.Concat(ret);
                    }
                }
                catch (Exception ex)
                {
                    Log.Message("[ATPP] PawnGroupMakerUtility.GeneratePawns " + ex.Message + " " + ex.StackTrace);
                }
            }
        }
    }
}