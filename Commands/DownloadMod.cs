namespace Dropship.Commands;

[RegisterCommand]
public class DownloadModCommand : Command
{
    public override string Name => "download_mod";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Downloads the mod of your choice";
    public override string Arguments => "<mod> [version]";

    public override bool Execute(string[] args)
    {
        throw new NotImplementedException();
    }
}