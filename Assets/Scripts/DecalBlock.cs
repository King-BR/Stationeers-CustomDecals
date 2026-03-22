using Assets.Scripts.Objects;
using System.Text;
using UnityEngine;
using Assets.Scripts.Objects.Items;

namespace CustomDecals.Prefabs
{
    public class DecalBlock : SmallDevice
    {
        private string DecalNameFront = "warning";
        private string DecalNameBack = "warning";
        private float HeightFront = 1f;
        private float HeightBack = 1f;
        private float WidthFront = 1f;
        private float WidthBack = 1f;
        private float? OffsetXFront = 0f;
        private float? OffsetXBack = 0f;
        private float? OffsetYFront = 0f;
        private float? OffsetYBack = 0f;
        private float? OffsetZFront = 0.65f;
        private float? OffsetZBack = 0.6f;
        private float? RotationXFront = 0f;
        private float? RotationXBack = 0f;
        private float? RotationYFront = 180f;
        private float? RotationYBack = 0f;
        private float? RotationZFront = 0f;
        private float? RotationZBack = 0f;
        private MeshRenderer FrontRenderer;
        private MeshRenderer BackRenderer;
        private Material _materialFront;
        private Material _materialBack;
        private float _lastUpdateInSeconds = 0f;

        public override void Awake()
        {
            base.Awake();

            FrontRenderer = transform.Find("Front")?.GetComponent<MeshRenderer>();
            BackRenderer = transform.Find("Back")?.GetComponent<MeshRenderer>();

            if (string.IsNullOrEmpty(CustomName)
                || !CustomName.Contains(";")) CustomName = $"{DecalNameFront};{WidthFront};{HeightFront}";

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
                parts = new string[] { parts[0], string.Empty, parts[1], parts[2], parts[1], parts[2] };
            }

            if (parts.Length < 6)
            {
                CustomDecals.Log($"Invalid format: {data}", "error");
                return;
            }

            DecalNameFront = parts[0].ToLower();
            DecalNameBack = parts[1].ToLower();

            CustomDecals.GameSprites = Resources.FindObjectsOfTypeAll<Sprite>();

            // Create materials of vanilla/modded sprites on demand to avoid impacting performance
            if (CustomDecals.TryGetSpriteByName(DecalNameFront, out var frontSprite))
            {
                if (frontSprite.texture != null)
                {
                    CustomDecals.Decals.TryAdd(DecalNameFront, frontSprite.texture);
                    CustomDecals.DecalMaterials[DecalNameFront] = new Material(Shader.Find("Sprites/Default"))
                    {
                        mainTexture = frontSprite.texture
                    };
                }
            }

            if (CustomDecals.TryGetSpriteByName(DecalNameBack, out var backSprite))
            {
                if (backSprite.texture != null)
                {
                    CustomDecals.Decals.TryAdd(DecalNameBack, backSprite.texture);
                    CustomDecals.DecalMaterials[DecalNameBack] = new Material(Shader.Find("Sprites/Default"))
                    {
                        mainTexture = backSprite.texture
                    };
                }
            }

            float.TryParse(parts[2], out WidthFront);
            float.TryParse(parts[3], out HeightFront);
            float.TryParse(parts[4], out WidthBack);
            float.TryParse(parts[5], out HeightBack);

            WidthFront = Mathf.Abs(WidthFront);
            HeightFront = Mathf.Abs(HeightFront);
            WidthBack = Mathf.Abs(WidthBack);
            HeightBack = Mathf.Abs(HeightBack);
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
            if (string.IsNullOrEmpty(DecalNameBack))
            {
                if (BackRenderer != null)
                {
                    BackRenderer.enabled = false;
                }
            }
            else if (CustomDecals.DecalMaterials.TryGetValue(DecalNameBack, out var backMat))
            {
                _materialBack = new Material(backMat);

                if (BackRenderer != null)
                {
                    BackRenderer.enabled = true;
                    BackRenderer.material = _materialBack;
                    BackRenderer.transform.localScale = new Vector2(WidthBack, HeightBack);
                }
            }
            else
            {
                CustomDecals.Log($"Decal material not found: {DecalNameBack}", "error");
            }

            // TODO: OFFSET
            /*
            FrontRenderer.transform.localPosition = new Vector3(OffsetXFront, OffsetYFront, OffsetZFront);
            BackRenderer.transform.localPosition = new Vector3(OffsetXFront, OffsetYFront, OffsetZFront);
            */

            // TODO: ROTATION
            /*
            FrontRenderer.transform.localRotation = Quaternion.Euler(RotationXFront, RotationYFront, RotationZFront);
            BackRenderer.transform.localRotation = Quaternion.Euler(RotationXBack, RotationYBack, RotationZBack);
            */

            // Scale bound according to the largest dimension of each decal
            this.Bounds.extents = new Vector3(Mathf.Max(WidthFront, WidthBack) / 4f, Mathf.Max(HeightFront, HeightBack) / 4f, 0.3f);
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
                HeightFront = decalSaveData.WidthBack;
                HeightBack = decalSaveData.HeightBack;
                WidthFront = decalSaveData.WidthFront;
                WidthBack = decalSaveData.WidthBack;
                OffsetXFront = decalSaveData.OffsetXFront == null ? 0f : decalSaveData.OffsetXFront;
                OffsetXBack = decalSaveData.OffsetXBack == null ? 0f : decalSaveData.OffsetXBack;
                OffsetYFront = decalSaveData.OffsetYFront == null ? 0f : decalSaveData.OffsetYFront;
                OffsetYBack = decalSaveData.OffsetYBack == null ? 0f : decalSaveData.OffsetYBack;
                OffsetZFront = decalSaveData.OffsetZFront == null ? 0.65f : decalSaveData.OffsetZFront;
                OffsetZBack = decalSaveData.OffsetZBack == null ? 0.6f : decalSaveData.OffsetZBack;
                RotationXFront = decalSaveData.RotationXFront == null ? 0f : decalSaveData.RotationXFront;
                RotationXBack = decalSaveData.RotationXBack == null ? 0f : decalSaveData.RotationXBack;
                RotationYFront = decalSaveData.RotationYFront == null ? 180f : decalSaveData.RotationYFront;
                RotationYBack = decalSaveData.RotationYBack == null ? 0f : decalSaveData.RotationYBack;
                RotationZFront = decalSaveData.RotationZFront == null ? 0f : decalSaveData.RotationZFront;
                RotationZBack = decalSaveData.RotationZBack == null ? 0f : decalSaveData.RotationZBack;

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
                decalSaveData.OffsetXFront = this.OffsetXFront;
                decalSaveData.OffsetXBack = this.OffsetXBack;
                decalSaveData.OffsetYFront = this.OffsetYFront;
                decalSaveData.OffsetYBack = this.OffsetYBack;
                decalSaveData.OffsetZFront = this.OffsetZFront;
                decalSaveData.OffsetZBack = this.OffsetZBack;
                decalSaveData.RotationXFront = this.RotationXFront;
                decalSaveData.RotationXBack = this.RotationXBack;
                decalSaveData.RotationYFront = this.RotationYFront;
                decalSaveData.RotationYBack = this.RotationYBack;
                decalSaveData.RotationZFront = this.RotationZFront;
                decalSaveData.RotationZBack = this.RotationZBack;
            }
        }
    }
}