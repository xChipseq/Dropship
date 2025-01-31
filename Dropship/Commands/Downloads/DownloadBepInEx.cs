namespace Dropship.Commands;

[RegisterCommand]
public class DownloadBepInExCommand : Command
{
    public override string Name => "download_bepinex";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Downloads BepInEx, the modloader needed for mods to work";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        DataManager.DownloadBepInEx().GetAwaiter().GetResult();
        return true;
    }
}