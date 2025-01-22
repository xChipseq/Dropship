namespace Dropship.Commands;

[RegisterCommand]
public class AmongListCommand : Command
{
    public override string Name => "amonglist";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Shows a list of all dropship-supported among us versions";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        throw new NotImplementedException();
    }
}