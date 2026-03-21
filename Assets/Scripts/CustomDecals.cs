using LaunchPadBooster;
using LaunchPadBooster.Utils;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx;
using Util.Commands;
using CustomDecals.Commands;
using CustomDecals.Prefabs;

namespace CustomDecals
{
    public class CustomDecals : MonoBehaviour
    {
        public static readonly string ModName = "CustomDecals";
        public static readonly string ModVersion = "1.0";
        public static readonly Mod MOD = new(ModName, ModVersion);
        public static readonly string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string decalsPath = Path.Combine(Paths.ConfigPath, ModName, "Decals");

        public static Dictionary<string, Texture2D> Decals = new();
        public static Dictionary<string, Material> DecalMaterials = new();

        public static void Log(string message, string type = "info")
        {
            switch (type.ToLower())
            {
                case "error":
                    Debug.LogError($"[{ModName}] {message}");
                    break;
                case "warning":
                    Debug.LogWarning($"[{ModName}] {message}");
                    break;
                default:
                    Debug.Log($"[{ModName}] {message}");
                    break;
            }
        }

        public void OnLoaded(List<GameObject> prefabs)
        {
            MOD.AddPrefabs(prefabs);

            LoadDecals();
            Log($"Loaded {Decals.Count} decals");

            MOD.SetupPrefabs() // run on all prefabs
              .SetBlueprintMaterials(); // fill in blueprint materials to match builtin blueprints

            MOD.AddSaveDataType<DecalSaveData>(); // add the custom save data type for decals
            MOD.SetupPrefabs<DecalBlock>() // run on decal prefab
                .AddToMultiConstructorKit("ItemKitSign") // add the decal prefab to the multiconstructor item kit sign
                .SetEntryTool("ItemKitSign") // set the entry tool to the item kit sign
                .SetExitTool(PrefabNames.Crowbar) // set the exit tool to the crowbar
                .RunFunc(prefab =>
                {
                    Sprite thumb = Decals.TryGetValue("warning", out var tex) ? Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(1f, 1f)) : null;
                    prefab.BuildStates[0].Tool.EntryQuantity = 1; // how many of the kit sign to consume when placing the decal
                    prefab.BuildStates[0].Thumbnail = thumb;
                    prefab.Thumbnail = thumb;
                });


            Log($"Loaded {prefabs.Count} prefabs");

            CommandLine.AddCommand("reloaddecals", new ReloadDecals());
        }

        public static void LoadDecals()
        {
            string defaultPath = Path.Combine(modPath, "Decals");
            string path = decalsPath;

            // check if path exists, if not, create it and copy default decals
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                if (Directory.Exists(defaultPath))
                {
                    foreach (var file in Directory.GetFiles(defaultPath, "*.png"))
                    {
                        string destFile = Path.Combine(path, Path.GetFileName(file));
                        File.Copy(file, destFile, true);
                    }
                    Log($"Copied default decals to {path}");
                }
                else
                {
                    Log($"Default decals not found at {defaultPath}", "warning");
                }
            }

            foreach (var file in Directory.GetFiles(path, "*.png"))
            {
                byte[] data = File.ReadAllBytes(file);

                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                tex.LoadImage(data);

                string name = (Path.GetFileNameWithoutExtension(file)).ToLower();

                Decals[name] = tex;
                DecalMaterials[name] = new Material(Shader.Find("Sprites/Default")) { mainTexture = tex };

                Log($"Loaded decal: {name}");
            }
        }
    }
}
