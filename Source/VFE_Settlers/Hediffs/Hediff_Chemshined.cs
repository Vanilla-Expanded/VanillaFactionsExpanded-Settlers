using RimWorld;
using Verse;

namespace VFE_Settlers.Hediffs
{
    public class Hediff_Chemshined : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();
            if (pawn.IsHashIntervalTick(300))
            {
                if (Severity >= 0.75f)
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hangover, false);
                    if (hediff != null)
                    {
                        hediff.Severity = 1f;
                        return;
                    }
                    else
                    {
                        hediff = HediffMaker.MakeHediff(HediffDefOf.Hangover, pawn, null);
                        hediff.Severity = 1f;
                        pawn.health.AddHediff(hediff, null, null, null);
                        return;
                    }
                }
                if (Severity < 0.05f)
                {
                    pawn.health.RemoveHediff(this);
                }
            }
        }
    }
}