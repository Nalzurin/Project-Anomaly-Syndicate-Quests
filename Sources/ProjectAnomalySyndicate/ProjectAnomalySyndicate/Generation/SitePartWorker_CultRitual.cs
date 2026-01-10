using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace ProjectAnomalySyndicate.Generation
{
    public class SitePartWorker_CultRitual : SitePartWorker
    {
        public override void Init(Site site, SitePart sitePart)
        {
            base.Init(site, sitePart);
            //site.customLabel = sitePart.def.label.Formatted(site.Faction.Named("FACTION"));
        }

        /* public override string GetPostProcessedThreatLabel(Site site, SitePart sitePart)
         {
             return (string)("Hostiles".Translate() + ": " + "Unknown".Translate().CapitalizeFirst()) + ("\n" + "Contains".Translate() + ": " + "Unknown".Translate().CapitalizeFirst());
         }*/

        private const int SpawnRadius = 20;
        public override void PostMapGenerate(Map map)
        {
            Site site = map.Parent as Site;
            PawnGroupKindDef pawnGroup = PawnGroupKindDefOf.PsychicRitualSiege;
            float num = Math.Max(Faction.OfHoraxCult.def.MinPointsToGeneratePawnGroup(pawnGroup), site.ActualThreatPoints);
            Log.Message(num);
            List<Pawn> cultists = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms
            {
                groupKind = pawnGroup,
                points = num,
                faction = Faction.OfHoraxCult
            }).ToList();
            Log.Message(cultists.Count);
            DistressCallUtility.SpawnPawns(map, cultists, map.Center, 20);
            Lord lord = LordMaker.MakeNewLord(Faction.OfHoraxCult, new LordJob_DefendBase(Faction.OfHoraxCult, map.Center, 180000), map, cultists);
            foreach (Thing allThing in map.listerThings.AllThings)
            {
                if (allThing.def.category == ThingCategory.Item)
                {
                    CompForbiddable compForbiddable = allThing.TryGetComp<CompForbiddable>();
                    if (compForbiddable != null && !compForbiddable.Forbidden)
                    {
                        allThing.SetForbidden(value: true, warnOnFail: false);

                    }
                }

            }

        }

    }
}
