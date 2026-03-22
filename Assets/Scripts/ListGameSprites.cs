using UnityEngine;
using System;

namespace CustomDecals.Commands
{
    public class ListGameSprites : ExtendedCommandBase
    {
        public override string Name => "listgamesprites";
        public override string HelpText => "List game sprites, use with [searchString] to filter the sprites (case insensitive), Example: listgamesprites liquid";
        public override bool IsLaunchCmd => false;
        public override string[] Arguments => new string[] { "searchString" };

        public override string Execute(string[] args)
        {
            Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();

            if (args.Length <= 1)
            {
                if (args.Length == 1) sprites = Array.FindAll(sprites, s => s.name.ToLower().Contains(args[0].ToLower()));
                string result = $"Found {sprites.Length} sprites:\n";

                foreach (Sprite sprite in sprites)
                {
                    result += $"{sprite.name} - {sprite}\n";
                }

                return result;
            } else
            {
                return ERROR_INVALID_SYNTAX;
            }
        }
    }
}
