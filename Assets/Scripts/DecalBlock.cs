using Assets.Scripts.Objects;
using System.Text;
using UnityEngine;

namespace CustomDecals.Prefabs
{
    public class DecalBlock : SmallDevice
    {
        public string DecalNameFront = "warning";
        public string DecalNameBack = "warning";
        public float HeightFront = 1f;
        public float HeightBack = 1f;
        public float WidthFront = 1f;
        public float WidthBack = 1f;
        public MeshRenderer FrontRenderer;
        public MeshRenderer BackRenderer;
        private Material _materialFront;
        private Material _materialBack;

        public override void Awake()
        {
            base.Awake();

            FrontRenderer = transform.Find("Front")?.GetComponent<MeshRenderer>();
            BackRenderer = transform.Find("Back")?.GetComponent<MeshRenderer>();

            if (string.IsNullOrEmpty(CustomName) || !CustomName.Contains(";")) CustomName = $"{DecalNameFront};{WidthFront};{HeightFront}";

            this.ApplyDecals();
        }

        public override void OnRenamed()
        {
            base.OnRenamed();
            this.ApplyDecals();
        }


        public new string ToTooltip()
        {
            StringBuilder sb = new StringBuilder();

            this.ToTooltip(sb);

            return sb.ToString();
        }

        public new void ToTooltip(StringBuilder sb)
        {
            sb.Append("Front Decal: <color=green>" + DecalNameFront + "</color>");
            sb.Append("\nFront Scale: <color=green>" + WidthFront + "x" + HeightFront + "</color>");
            sb.Append("\nBack Decal: <color=green>" + DecalNameBack + "</color>");
            sb.Append("\nBack Scale: <color=green>" + WidthBack + "x" + HeightBack + "</color>");
        }

        public void ParseDisplayName(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                CustomDecals.Log("DisplayName empty", "error");
                return;
            }

            var parts = data.Split(';');

            if (parts.Length == 3)
            {
                parts = new string[] { parts[0], parts[0], parts[1], parts[1], parts[2], parts[2] };
            }

            if (parts.Length < 6)
            {
                CustomDecals.Log($"Invalid format: {data}", "error");
                return;
            }

            DecalNameFront = parts[0].ToLower();
            DecalNameBack = parts[1].ToLower();

            float.TryParse(parts[2], out WidthFront);
            float.TryParse(parts[3], out HeightFront);
            float.TryParse(parts[4], out WidthBack);
            float.TryParse(parts[5], out HeightBack);
        }

        public void ApplyDecals()
        {
            this.ParseDisplayName(this.DisplayName);

            // FRONT
            if (CustomDecals.DecalMaterials.TryGetValue(DecalNameFront, out var frontMat))
            {
                _materialFront = new Material(frontMat);

                if (FrontRenderer != null)
                {
                    FrontRenderer.material = _materialFront;
                    FrontRenderer.transform.localScale = new Vector2(WidthFront, HeightFront);
                }
            }
            else
            {
                CustomDecals.Log($"Decal material not found: {DecalNameFront}", "error");
            }

            // BACK
            if (CustomDecals.DecalMaterials.TryGetValue(DecalNameBack, out var backMat))
            {
                _materialBack = new Material(backMat);

                if (BackRenderer != null)
                {
                    BackRenderer.material = _materialBack;
                    BackRenderer.transform.localScale = new Vector2(WidthBack, HeightBack);
                }
            }
            else
            {
                CustomDecals.Log($"Decal material not found: {DecalNameBack}", "error");
            }

            // TODO: OFFSET
            // FrontRenderer.transform.localPosition = new Vector3(0, 0, FrontRenderer.transform.localPosition.z + 0.1f);
            // BackRenderer.transform.localPosition = new Vector3(0, 0, BackRenderer.transform.localPosition.z + 0.1f);
        }

        public override ThingSaveData SerializeSave()
        {
            ThingSaveData savedData = (ThingSaveData) new DecalSaveData();
            this.InitialiseSaveData(ref savedData);
            return savedData;
        }

        public override void DeserializeSave(ThingSaveData data)
        {
            base.DeserializeSave(data);
            if (data is DecalSaveData decalSaveData)
            {
                HeightFront = decalSaveData.HeightFront;
                HeightBack = decalSaveData.HeightBack;
                WidthFront = decalSaveData.WidthFront;
                WidthBack = decalSaveData.WidthBack;

                this.ApplyDecals();
            }
        }

        protected override void InitialiseSaveData(ref ThingSaveData savedData)
        {
            base.InitialiseSaveData(ref savedData);
            if (savedData is DecalSaveData decalSaveData)
            {
                decalSaveData.DecalNameFront = this.DecalNameFront;
                decalSaveData.DecalNameBack = this.DecalNameFront;
                decalSaveData.HeightFront = this.HeightFront;
                decalSaveData.HeightBack = this.HeightBack;
                decalSaveData.WidthFront = this.WidthFront;
                decalSaveData.WidthBack = this.WidthBack;
            }
        }
    }
}