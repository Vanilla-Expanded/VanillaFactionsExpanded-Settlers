using RimWorld;
using Verse;

namespace VFE_Settlers.Defs {
    [DefOf]
    public static class JobDefOf {
        public static JobDef FillChemshineBarrel;
        public static JobDef TakeChemshineOutOfChemshineBarrel;
        static JobDefOf() {
            DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
        }
    }
}
