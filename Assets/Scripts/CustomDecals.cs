using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Pipes;
using BepInEx;
using CustomDecals.Prefabs;
using LaunchPadBooster;
using LaunchPadBooster.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Util.Commands;

namespace CustomDecals
{
    public class CustomDecals : MonoBehaviour
    {
        public static readonly string ModName = "CustomDecals";
        public static readonly string ModVersion = "1.1";
        public static readonly Mod MOD = new(ModName, ModVersion);
        public static readonly string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string DecalsPath = Path.Combine(Paths.ConfigPath, ModName, "Decals");
        public static readonly string DefaultDecalsPath = Path.Combine(modPath, "Decals");

        public static Dictionary<string, Texture2D> Decals = new();
        public static Dictionary<string, Material> DecalMaterials = new();
        public static Sprite[] GameSprites;

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

            // register commands dynamically
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(ExtendedCommandBase)) && !type.IsAbstract)
                {
                    ExtendedCommandBase cmd = (ExtendedCommandBase) Activator.CreateInstance(type);
                    CommandLine.AddCommand(cmd.Name, (CommandBase) cmd);
                    Log($"Registered command: {cmd.Name}");
                }
            }
        }

        public void OnGUI()
        {
            DecalUI.Draw();
        }

        public static bool TryGetSpriteByName(string name, out Sprite result)
        {
            result = null;

            if (GameSprites == null || string.IsNullOrEmpty(name)) return false;

            result = GameSprites.FirstOrDefault(s => !string.IsNullOrEmpty(s.name) && string.Equals(s.name, name, StringComparison.OrdinalIgnoreCase));
            return result != null;
        }

        public static void LoadDecals()
        {
            // Clear existing decals and materials before loading new ones
            GameSprites = new Sprite[] { };
            Decals.Clear();
            DecalMaterials.Clear();

            // Load all game sprites into a dictionary for easy access when creating materials
            GameSprites = Resources.FindObjectsOfTypeAll<Sprite>();

            // check if DecalsPath exists, if not, create it and copy default decals
            if (!Directory.Exists(DecalsPath))
            {
                Directory.CreateDirectory(DecalsPath);
                if (Directory.Exists(DefaultDecalsPath))
                {
                    foreach (var file in Directory.GetFiles(DefaultDecalsPath, "*.png"))
                    {
                        string destFile = Path.Combine(DecalsPath, Path.GetFileName(file));
                        File.Copy(file, destFile, true);
                    }
                    Log($"Copied default decals to {DecalsPath}");
                }
                else
                {
                    Log($"Default decals not found at {DefaultDecalsPath}", "warning");
                }
            }

            foreach (var file in Directory.GetFiles(DecalsPath, "*.png"))
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

    public abstract class ExtendedCommandBase : CommandBase
    {
        public abstract string Name { get; }
    }
}
