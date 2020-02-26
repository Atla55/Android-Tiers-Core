﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;
using RimWorld;

namespace MOARANDROIDS
{
    public class IncidentWorker_DownedT5Android : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && this.TryFindTile(out int num) && SiteMakerHelper.TryFindRandomFactionFor(SiteCoreDefOf.DownedT5Android, null, out Faction faction, true, null);
        }

        private bool TryFindTile(out int tile)
        {
            return TileFinder.TryFindNewSiteTile(out tile, 7, 15, true, false, -1);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!this.TryFindFactions(out Faction faction, out Faction faction2))
            {
                return false;
            }
            if (!TileFinder.TryFindNewSiteTile(out int tile, 8, 30, false, true, -1))
            {
                return false;
            }
            Site site = SiteMaker.MakeSite(SiteCoreDefOf.DownedT5Android, SitePartDefOf.Turrets, tile, faction2);
            site.Tile = tile;
            int randomInRange = IncidentWorker_DownedT5Android.TimeoutDaysRange.RandomInRange;
            site.GetComponent<TimeoutComp>().StartTimeout(randomInRange * 60000);
            Find.WorldObjects.Add(site);

            string labelText = this.def.letterLabel;
            string letterText = this.def.letterText;

            Find.LetterStack.ReceiveLetter(labelText, letterText, this.def.letterDef, site, null, null);
            return true;
        }

        private bool TryFindFactions(out Faction alliedFaction, out Faction enemyFaction)
        {
            if ((from x in Find.FactionManager.AllFactions
                 where !x.def.hidden && !x.defeated && !x.IsPlayer && !x.HostileTo(Faction.OfPlayer) && this.CommonHumanlikeEnemyFactionExists(Faction.OfPlayer, x) && !this.AnyQuestExistsFrom(x)
                 select x).TryRandomElement(out alliedFaction))
            {
                enemyFaction = this.CommonHumanlikeEnemyFaction(Faction.OfPlayer, alliedFaction);
                return true;
            }
            alliedFaction = null;
            enemyFaction = null;
            return false;
        }

        private bool AnyQuestExistsFrom(Faction faction)
        {
            List<Site> sites = Find.WorldObjects.Sites;
            for (int i = 0; i < sites.Count; i++)
            {
                DefeatAllEnemiesQuestComp component = sites[i].GetComponent<DefeatAllEnemiesQuestComp>();
                if (component != null && component.Active && component.requestingFaction == faction)
                {
                    return true;
                }
            }
            return false;
        }
        
        private bool CommonHumanlikeEnemyFactionExists(Faction f1, Faction f2)
        {
            return this.CommonHumanlikeEnemyFaction(f1, f2) != null;
        }

        private Faction CommonHumanlikeEnemyFaction(Faction f1, Faction f2)
        {
            if ((from x in Find.FactionManager.AllFactions
                 where x != f1 && x != f2 && !x.def.hidden && x.def.humanlikeFaction && !x.defeated && x.HostileTo(f1) && x.HostileTo(f2)
                 select x).TryRandomElement(out Faction result))
            {
                return result;
            }
            return null;
        }


        private static readonly IntRange TimeoutDaysRange = new IntRange(12, 20);
    }
}