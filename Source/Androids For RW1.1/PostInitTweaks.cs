using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MOARANDROIDS
{
    /// <summary>
    /// Tweaks ThingDefs after the game has been made.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class PostInitializationTweaker
    {
        static PostInitializationTweaker()
        {
            //Start tweaking.
            //IEnumerable<ThingDef> corpseDefs = DefDatabase<ThingDef>.AllDefs.Where(thingDef => thingDef.defName.EndsWith("_Corpse"));

            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                //If the Def got a AndroidTweaker do stuff, otherwise do not bother.
                AndroidTweaker tweaker = thingDef.GetModExtension<AndroidTweaker>();
                if (tweaker != null)
                {
                    ThingDef corpseDef = thingDef?.race?.corpseDef;
                    if (corpseDef != null)
                    {
                        //Removes corpse rotting.
                        if (tweaker.tweakCorpseRot)
                        {
                            corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_Rottable);
                            corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_SpawnerFilth);
                        }

                        //Modifies the butchering products by importing the costs from a recipe.
                        RecipeDef recipeDef = tweaker.recipeDef;
                        if (tweaker.tweakCorpseButcherProducts && recipeDef != null)
                        {
                            corpseDef.butcherProducts.Clear();

                            foreach (IngredientCount ingredient in recipeDef.ingredients)
                            {
                                float finalCount = 0f;
                                ThingDef ingredientThingDef = ingredient.filter.AnyAllowedDef;
                                int requiredCount = ingredient.CountRequiredOfFor(ingredientThingDef, recipeDef);

                                if (tweaker.corpseButcherRoundUp)
                                {
                                    finalCount = (float)Math.Ceiling((float)requiredCount * tweaker.corpseButcherProductsRatio);
                                }
                                else
                                {
                                    finalCount = (float)Math.Floor((float)requiredCount * tweaker.corpseButcherProductsRatio);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
