using Dropship.DepotDownloader;

namespace Dropship.Commands;

[RegisterCommand]
public class ModListCommand : Command
{
    public override string Name => "modlist";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Shows a list of all dropship-supported mods";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        Console.WriteLine($"{DepotDownloaderLoader.LoginUsername} :: {DepotDownloaderLoader.LoginPassword}");
        return true;
    }
}