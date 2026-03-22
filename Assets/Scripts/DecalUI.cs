using UnityEngine;
using System.Linq;

namespace CustomDecals
{
    public static class DecalUI
    {
        public static bool ShowUI = false;

        private static Vector2 scroll;
        private static string search = "";

        private const int CellSize = 80;
        private const int Columns = 5;

        public static void Draw()
        {
            if (!ShowUI)
                return;

            GUILayout.BeginArea(new Rect(50, 50, 500, 600), GUI.skin.box);

            GUILayout.Label("Decal Selector");

            // SEARCH BAR
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(50));
            search = GUILayout.TextField(search);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // FILTER
            var filtered = string.IsNullOrEmpty(search) ? CustomDecals.Decals.ToList() : CustomDecals.Decals
                .Where(kvp => kvp.Key.Contains(search.ToLower()))
                .ToList();

            scroll = GUILayout.BeginScrollView(scroll);

            int count = 0;

            foreach (var kvp in filtered)
            {
                if (count % Columns == 0)
                    GUILayout.BeginHorizontal();

                DrawCell(kvp.Key, kvp.Value);

                count++;

                if (count % Columns == 0)
                    GUILayout.EndHorizontal();
            }

            // close incomplete row
            if (count % Columns != 0)
                GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            if (GUILayout.Button("Close"))
                ShowUI = false;

            GUILayout.EndArea();
        }

        private static void DrawCell(string name, Texture2D tex)
        {
            GUILayout.BeginVertical(GUILayout.Width(CellSize));

            if (GUILayout.Button(tex, GUILayout.Width(CellSize), GUILayout.Height(CellSize)))
            {
                OnDecalSelected(name);
            }

            GUILayout.Label(name, GUILayout.Width(CellSize));

            GUILayout.EndVertical();
        }

        private static void OnDecalSelected(string name)
        {
            CustomDecals.Log($"Selected decal: {name}");

            // TODO: apply decal
        }
    }
}