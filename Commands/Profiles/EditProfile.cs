namespace Dropship.Commands;

[RegisterCommand]
public class EditProfile : Command
{
    public override string Name => "edit_profile";
    public override CommandCategory Category => CommandCategory.Profiles;
    public override string Description => "Starts editing the selected profile. Required for adding/removing mods";
    public override string Arguments => "<name>";

    public override bool Execute(string[] args)
    {
        if (args.Count() == 1)
        {
            if (!string.IsNullOrWhiteSpace(args[0]))
            {
                if (!ProfileManager.Profiles.ContainsKey(args[0]))
                {
                    Console.WriteLine($"Profile \"{args[0]}\" does not exist");
                    return false;
                }

                ProfileManager.EditProfile = args[0];
                Console.WriteLine($"Editing {args[0]} profile");
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