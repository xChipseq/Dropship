namespace Dropship.Commands;

[RegisterCommand]
public class AmongListCommand : Command
{
    public override string Name => "amonglist";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Shows a list of all dropship-provided among us versions";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        Console.WriteLine("All dropship-provied among us versions:");
        foreach (var version in DataManager.ManifestVersionsList.Keys)
        {
            Console.WriteLine($"    {version}");
        }

        Console.WriteLine("\nDownloaded versions:");
        foreach (var version in DepotDownloader.GetInstalledVersions())
        {
            Console.WriteLine($"    {version}");
        }

        return true;
    }
}