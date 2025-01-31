using System.Text.RegularExpressions;

namespace Dropship.Commands;

[RegisterCommand]
public class ChangeVersionCommand : Command
{
    public override string Name => "change_version";
    public override CommandCategory Category => CommandCategory.Profiles;
    public override string Description => "Changes profile's among us version";
    public override string Arguments => "<version>";

    public override bool Execute(string[] args)
    {
        if (args.Count() != 1)
        {
            InvalidArguments();
            return false;
        }
        if (string.IsNullOrWhiteSpace(args[0]))
        {
            InvalidArguments();
            return false;
        }

        bool validVersion = Regex.IsMatch(args[0], @"^\d{4}\.\d{1,2}\.\d{1,2}$");
        if (!validVersion && args[0] != "steam")
        {
            Console.WriteLine($"The selected version \"{args[0]}\" is not in a correct format");
            return false;
        }

        ProfileManager.Profiles[ProfileManager.EditProfile].AmongUsVersion = args[0];
        ProfileManager.SaveProfiles();
        ProfileManager.LoadProfiles();

        Console.WriteLine($"Version changed to {args[0]}");

        return true;
    }
}