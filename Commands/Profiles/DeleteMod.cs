namespace Dropship.Commands;

[RegisterCommand]
public class DeleteModCommand : Command
{
    public override string Name => "delete_mod";
    public override CommandCategory Category => CommandCategory.Profiles;
    public override string Description => "Removes a mod from a profile";
    public override string Arguments => "<mod>";

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

        ProfileManager.RemoveMod(args[0]);

        return true;
    }
}