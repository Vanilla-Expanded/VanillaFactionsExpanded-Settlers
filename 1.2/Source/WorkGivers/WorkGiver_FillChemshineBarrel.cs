using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace VFE_Settlers.WorkGivers
{
	public class WorkGiver_FillChemshineBarrel : WorkGiver_Scanner
	{
		private static string TemperatureTrans;

		private static string NoChemfuelTrans;

		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(Defs.ThingDefOf.Building_ChemshineBarrel);

		public override PathEndMode PathEndMode => PathEndMode.Touch;

		public static void ResetStaticData()
		{
			TemperatureTrans = "BadTemperature".Translate().ToLower();
			NoChemfuelTrans = "NoChemfuel".Translate();
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Buildings.Building_ChemshineBarrel building_ChemshineBarrel = t as Buildings.Building_ChemshineBarrel;
			if (building_ChemshineBarrel == null || building_ChemshineBarrel.Fermented || building_ChemshineBarrel.SpaceLeftForChemfuel <= 0)
			{
				return false;
			}
			float ambientTemperature = building_ChemshineBarrel.AmbientTemperature;
			CompProperties_TemperatureRuinable compProperties = building_ChemshineBarrel.def.GetCompProperties<CompProperties_TemperatureRuinable>();
			if (ambientTemperature < compProperties.minSafeTemperature + 2f || ambientTemperature > compProperties.maxSafeTemperature - 2f)
			{
				JobFailReason.Is(TemperatureTrans);
				return false;
			}
			if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
			{
				return false;
			}
			if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
			{
				return false;
			}
			if (FindChemfuel(pawn) == null)
			{
				JobFailReason.Is(NoChemfuelTrans);
				return false;
			}
			if (t.IsBurning())
			{
				return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Buildings.Building_ChemshineBarrel barrel = (Buildings.Building_ChemshineBarrel)t;
			Thing t2 = FindChemfuel(pawn);
			return JobMaker.MakeJob(Defs.JobDefOf.FillChemshineBarrel, t, t2);
		}

		private Thing FindChemfuel(Pawn pawn)
		{
			Predicate<Thing> validator = (Thing x) => (!x.IsForbidden(pawn) && pawn.CanReserve(x)) ? true : false;
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.Chemfuel), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator);
		}
	}
}
