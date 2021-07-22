using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace VFE_Settlers.Utilities
{
    public static class UtilityAction
    {
        public static Thing GetDrunkenTarget(Pawn pawn, IntVec3 near, int maxDistance = 40)
        {
            List<Thing> outCandidates = new List<Thing>();
            if (pawn.CanReach(near, PathEndMode.OnCell, Danger.Deadly))
            {
                List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Plant);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Position.InHorDistOf(near, maxDistance) && CanSmash(pawn, list[i]))
                    {
                        outCandidates.Add(list[i]);
                    }
                }
                List<Thing> list2 = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);
                for (int j = 0; j < list2.Count; j++)
                {
                    if (list2[j].Position.InHorDistOf(near, maxDistance) && CanSmash(pawn, list2[j]))
                    {
                        outCandidates.Add(list2[j]);
                    }
                }
            }

            if (outCandidates.Count > 0)
                return outCandidates.RandomElement();
            return null;
        }

        public static bool CanSmash(Pawn pawn, Thing thing)
        {
            if ((thing.def.category == ThingCategory.Plant && !thing.def.defName.ToLower().Contains("grass")) || thing.def.IsDoor)
            {
                return !thing.Destroyed && thing.Spawned && thing != pawn && (pawn.CanReach(thing, PathEndMode.Touch, Danger.Deadly));
            }
            return false;
        }
    }
}