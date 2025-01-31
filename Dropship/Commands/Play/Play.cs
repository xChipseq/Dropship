namespace Dropship.Commands;

[RegisterCommand]
public class PlayCommand : Command
{
    public override string Name => "play";
    public override CommandCategory Category => CommandCategory.Play;
    public override string Description => "Runs the choosen profile";
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

        ProfileManager.LaunchProfile(args[0]);

        return true;
    }
}