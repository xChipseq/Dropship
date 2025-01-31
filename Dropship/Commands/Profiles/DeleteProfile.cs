namespace Dropship.Commands;

[RegisterCommand]
public class DeleteProfileCommand : Command
{
    public override string Name => "delete_profile";
    public override CommandCategory Category => CommandCategory.Profiles;
    public override string Description => "Deletes the selected profile";
    public override string Arguments => "<profile>";

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

        ProfileManager.DeleteProfile(args[0]);
        ProfileManager.LoadProfiles();

        return true;
    }
}