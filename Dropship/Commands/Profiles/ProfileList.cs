namespace Dropship.Commands;

[RegisterCommand]
public class ProfileListCommand : Command
{
    public override string Name => "profilelist";
    public override CommandCategory Category => CommandCategory.Profiles;
    public override string Description => "Shows a list of all your profiles";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        Console.WriteLine("All profiles:");
        foreach (var profile in ProfileManager.Profiles)
        {
            Console.WriteLine(profile.Value.Name);
            Console.WriteLine($"    Uses {(profile.Value.AmongUsVersion == "steam" ? "Steam Installation" : $"Among Us {profile.Value.AmongUsVersion}")}");
            Console.WriteLine($"    Mods:");
            if (profile.Value.Mods.Count == 0) Console.WriteLine($"       None :(");
            foreach (var mod in profile.Value.Mods)
            {
                Console.WriteLine($"        - {mod}");
            }
        }

        return true;
    }
}