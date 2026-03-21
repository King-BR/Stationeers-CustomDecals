using Util.Commands;

namespace CustomDecals.Commands
{
    public class ReloadDecals : CommandBase
    {
        public override string HelpText => "Reload the custom decals";
        public override string[] Arguments => new string[] { };
        public override bool IsLaunchCmd => false;

        public override string Execute(string[] args)
        {
            CustomDecals.Decals.Clear();
            CustomDecals.DecalMaterials.Clear();
            CustomDecals.Log("Reloading custom decals...");
            CustomDecals.LoadDecals();

            return $"Reloaded {CustomDecals.Decals.Count} custom decals";
        }
    }
}
