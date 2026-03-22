using BepInEx;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using CustomDecals.Prefabs;

namespace CustomDecals
{
    [BepInPlugin("kingbr.customdecals_patchs", "CustomDecals_Patchs", "1.0")]
    public class CustomDecals_Patchs : BaseUnityPlugin
    {
        public static Harmony harmony;
        public static CustomDecals_Patchs Instance;

        private void Awake()
        {
            harmony = new Harmony("kingbr.customdecals_patchs");
            Instance = this;
            harmony.PatchAll();
            foreach (MethodBase methodBase in harmony.GetPatchedMethods().ToList<MethodBase>())
                CustomDecals.Log("Patched: " + methodBase.DeclaringType.FullName + "." + methodBase.Name, "info");
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Labeller), "Rename")]
        [HarmonyPrefix]
        public static bool RenamePrefix(Labeller __instance, Thing thing)
        {
            if (thing is not DecalBlock)
                return true;

            string newName = "test";

            //DecalUI.ShowUI = !DecalUI.ShowUI;

            thing.CustomName = newName;
            thing.RenameThing(newName);
            return false;
        }
    }
}