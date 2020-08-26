using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace VFE_Settlers.WorkGivers
{
   public class WorkGiver_TakeChemshineOutOfChemshineBarrel : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Defs.ThingDefOf.Building_ChemshineBarrel);

		public override PathEndMode PathEndMode => PathEndMode.Touch;

		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			//Log.Message("TEST");
			List<Thing> list = pawn.Map.listerThings.ThingsOfDef(Defs.ThingDefOf.Building_ChemshineBarrel);
			for (int i = 0; i < list.Count; i++)
			{
				//Log.Message("TEST2");
				if (((Buildings.Building_ChemshineBarrel)list[i]).Fermented)
				{
					//Log.Message("TEST3");
					return false;
				}
			}
			return true;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Buildings.Building_ChemshineBarrel building_ChemshineBarrel = t as Buildings.Building_ChemshineBarrel;
			if (building_ChemshineBarrel == null || !building_ChemshineBarrel.Fermented)
			{
				return false;
			}
			if (t.IsBurning())
			{
				return false;
			}
			if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
			{
				return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return JobMaker.MakeJob(Defs.JobDefOf.TakeChemshineOutOfChemshineBarrel, t);
		}
	}
}
