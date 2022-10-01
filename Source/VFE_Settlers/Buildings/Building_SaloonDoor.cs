using RimWorld;
using Verse;

namespace VFE_Settlers.Buildings
{
    public class Building_SaloonDoor : Building_Door
    {
        public new bool FreePassage => true;
        public new bool SlowsPawns => false;

        public new bool CanPhysicallyPass(Pawn p) => true;

        public new bool PawnCanOpen(Pawn p) => true;

        public override void TickRare()
        {
            GenTemperature.EqualizeTemperaturesThroughBuilding(this, 14f, twoWay: true);
        }
    }
}