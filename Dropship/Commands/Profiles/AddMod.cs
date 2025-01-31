namespace Dropship.Commands;

[RegisterCommand]
public class AddModCommand : Command
{
    public override string Name => "add_mod";
    public override CommandCategory Category => CommandCategory.Profiles;
    public override string Description => "Adds a mod and all it's dependencies to the selected profile";
    public override string Arguments => "<mod> <version>";

    public override bool Execute(string[] args)
    {
        if (args.Count() != 2)
        {
            InvalidArguments();
            return false;
        }
        if (string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
        {
            InvalidArguments();
            return false;
        }

        ProfileManager.AddMod(args[0], args[1]);

        return true;
    }
}