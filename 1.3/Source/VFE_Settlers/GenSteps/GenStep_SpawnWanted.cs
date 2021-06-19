using RimWorld.BaseGen;
using System.Collections.Generic;
using Verse;
using System;
using RimWorld;
using Verse.AI.Group;
using System.Linq;

namespace VFE_Settlers.GenSteps
{
    public class GenStep_SpawnWanted : GenStep_Scatterer
    {
        public override int SeedPart
        {
            get
            {
                return 383313881;
            }
        }
        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            if (!base.CanScatterAt(c, map))
            {
                return false;
            }
            if (!map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors)))
            {
                return false;
            }
            return true;
        }

        protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int count = 1)
        {
            Faction faction = (map.ParentFaction != null && map.ParentFaction != Faction.OfPlayer) ? map.ParentFaction : Find.FactionManager.RandomEnemyFaction();
            CellRect cellRect = CellRect.CenteredOn(loc, 8, 8).ClipInsideMap(map);
            Pawn singlePawnToSpawn = (Pawn)parms.sitePart.things.Take(parms.sitePart.things[0]);

            ResolveParams resolveParams2 = default;
            resolveParams2.rect = cellRect;
            resolveParams2.faction = faction;
            resolveParams2.singlePawnToSpawn = singlePawnToSpawn;
            resolveParams2.postThingSpawn = delegate (Thing x)
            {
                MapGenerator.rootsToUnfog.Add(x.Position);
            };

            BaseGen.globalSettings.map = map;
            BaseGen.symbolStack.Push("pawn", resolveParams2);
            BaseGen.Generate();
            MapGenerator.SetVar("RectOfInterest", cellRect);
            map.lordManager.LordOf(map.mapPawns.FreeHumanlikesOfFaction(faction).First())?.AddPawn(singlePawnToSpawn);
        }
    }
}

