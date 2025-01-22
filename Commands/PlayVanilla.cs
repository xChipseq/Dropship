namespace Dropship.Commands;

[RegisterCommand]
public class PlayVanillaCommand : Command
{
    public override string Name => "play_vanilla";
    public override CommandCategory Category => CommandCategory.Play;
    public override string Description => "Runs the vanilla game";
    public override string Arguments => "[version]";

    public override bool Execute(string[] args)
    {
        throw new NotImplementedException();
    }
}