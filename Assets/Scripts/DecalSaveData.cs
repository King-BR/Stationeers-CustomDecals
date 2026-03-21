using Assets.Scripts.Objects;

namespace CustomDecals.Prefabs
{
    public class DecalSaveData : StructureSaveData
    {
        public string DecalNameFront { get; set; }
        public string DecalNameBack { get; set; }
        public float HeightFront { get; set; }
        public float HeightBack { get; set; }
        public float WidthFront { get; set; }
        public float WidthBack { get; set; }
    }
}
