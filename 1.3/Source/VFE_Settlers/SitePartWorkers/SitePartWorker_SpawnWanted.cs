using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace VFE_Settlers.SitePartWorkers
{
    public class SitePartWorker_SpawnWanted : SitePartWorker
    {
		public override void Notify_GeneratedByQuestGen(SitePart part, Slate slate, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
		{
			base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);
			slate.TryGet("asker", out Pawn asker);
			Pawn pawn = Utilities.UtilityPawn.GenerateWanted(asker.Faction, part.site.Faction, Rand.Range(Settings.SettingsHelper.LatestVersion.MinReward, Settings.SettingsHelper.LatestVersion.MaxReward) * 2, out Hediffs.HediffComp_Wanted wanted);

			NameTriple name = pawn.Name as NameTriple;
			slate.Set<Pawn>("criminal", pawn);
			slate.Set<string>("bounty", (wanted.Reward * 10).ToString());
			slate.Set<string>("nickname", name.Nick);

			part.things = new ThingOwner<Pawn>(part, oneStackOnly: true);
			part.things.TryAdd(pawn);
			PawnRelationUtility.Notify_PawnsSeenByPlayer(Gen.YieldSingle(pawn), out _, informEvenIfSeenBefore: true, writeSeenPawnsNames: false);
		}
	}
}
