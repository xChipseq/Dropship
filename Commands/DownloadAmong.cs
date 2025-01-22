namespace Dropship.Commands;

[RegisterCommand]
public class DownloadAmongCommand : Command
{
    public override string Name => "download_among";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Downloads a specific among us version";
    public override string Arguments => "<version>";

    public override bool Execute(string[] args)
    {
        throw new NotImplementedException();
    }
}