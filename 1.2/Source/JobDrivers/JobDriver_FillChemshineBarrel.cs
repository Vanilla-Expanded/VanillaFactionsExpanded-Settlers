using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace VFE_Settlers.JobDrivers
{
	public class JobDriver_FillChemshineBarrel : JobDriver
	{
		private const TargetIndex BarrelInd = TargetIndex.A;

		private const TargetIndex WortInd = TargetIndex.B;

		private const int Duration = 200;


		protected Buildings.Building_ChemshineBarrel Barrel => (Buildings.Building_ChemshineBarrel)job.GetTarget(TargetIndex.A).Thing;

		protected Thing Chemfuel => job.GetTarget(TargetIndex.B).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.Reserve(Barrel, job, 1, -1, null, errorOnFailed))
			{
				return pawn.Reserve(Chemfuel, job, 1, -1, null, errorOnFailed);
			}
			return false;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.A);
			AddEndCondition(() => (Barrel.SpaceLeftForChemfuel > 0) ? JobCondition.Ongoing : JobCondition.Succeeded);
			yield return Toils_General.DoAtomic(delegate
			{
				job.count = Barrel.SpaceLeftForChemfuel;
			});
			Toil reserveWort = Toils_Reserve.Reserve(TargetIndex.B);
			yield return reserveWort;
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(TargetIndex.B);
			yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveWort, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.Wait(200).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A)
				.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
				.WithProgressBarToilDelay(TargetIndex.A);
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Barrel.AddChemfuel(Chemfuel);
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			yield return toil;
		}
	}
}
