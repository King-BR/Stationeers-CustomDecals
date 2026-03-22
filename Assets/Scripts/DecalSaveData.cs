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
        public float? OffsetXFront { get; set; }
        public float? OffsetXBack { get; set; }
        public float? OffsetYFront { get; set; }
        public float? OffsetYBack { get; set; }
        public float? OffsetZFront { get; set; }
        public float? OffsetZBack { get; set; }
        public float? RotationXFront { get; set; }
        public float? RotationXBack { get; set; }
        public float? RotationYFront { get; set; }
        public float? RotationYBack { get; set; }
        public float? RotationZFront { get; set; }
        public float? RotationZBack { get; set; }
    }
}
