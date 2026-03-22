using System;

namespace CustomDecals.Commands
{
    public class ReloadDecals : ExtendedCommandBase
    {
        public override string Name => "reloaddecals";
        public override string HelpText => "Reload the custom decals";
        public override bool IsLaunchCmd => false;
        public override string[] Arguments => new string[] { };

        public override string Execute(string[] args)
        {
            CustomDecals.Log("Reloading custom decals...");
            CustomDecals.LoadDecals();

            return $"Reloaded {CustomDecals.Decals.Count} custom decals";
        }
    }
}
