namespace Dropship.Commands;

[RegisterCommand]
public class RelodProfilesCommand : Command
{
    public override string Name => "reload_profiles";
    public override CommandCategory Category => CommandCategory.Profiles;
    public override string Description => "Reloads all profiles";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        ProfileManager.LoadProfiles();
        Console.WriteLine("Profiles have been reloaded");
        return true;
    }
}