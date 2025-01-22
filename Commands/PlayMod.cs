namespace Dropship.Commands;

[RegisterCommand]
public class PlayModCommand : Command
{
    public override string Name => "play_mod";
    public override CommandCategory Category => CommandCategory.Play;
    public override string Description => "Runs the choosen mod";
    public override string Arguments => "<mod> [version]";

    public override bool Execute(string[] args)
    {
        throw new NotImplementedException();
    }
}