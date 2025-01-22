namespace Dropship.Commands;

[RegisterCommand]
public class DownloadCustomModCommand : Command
{
    public override string Name => "download_custom_mod";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Downloads a mod from a specific repo";
    public override string Arguments => "<link>";

    public override bool Execute(string[] args)
    {
        throw new NotImplementedException();
    }
}