namespace Dropship.Commands;

[RegisterCommand]
public class CreateProfileCommand : Command
{
    public override string Name => "create_profile";
    public override CommandCategory Category => CommandCategory.Profiles;
    public override string Description => "Creates a profile. By default it's gonna use your steam-installed game";
    public override string Arguments => "<name> [among_version]";

    public override bool Execute(string[] args)
    {
        if (args.Count() == 1)
        {
            if (!string.IsNullOrWhiteSpace(args[0]))
            {
                ProfileManager.CreateProfile(args[0], "steam");
            }
            else
            {
                InvalidArguments();
                return false;
            }
        }
        else if (args.Count() == 2)
        {
            if (!string.IsNullOrWhiteSpace(args[0]) && !string.IsNullOrWhiteSpace(args[1]))
            {
                ProfileManager.CreateProfile(args[0], args[1]);
            }
            else
            {
                InvalidArguments();
                return false;
            }
        }
        else
        {
            InvalidArguments();
            return false;
        }
        return true;
    }
}