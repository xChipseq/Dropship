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
        if (args.Count() != 1)
        {
            InvalidArguments();
            return false;
        }

        if (!DataManager.ManifestVersionsList.ContainsKey(args[0]))
        {
            Console.WriteLine($"Version not found: {args[0]}\nWe might not have the version you want. Please use download_custom_among command");
            return false;
        }
        
        if (DepotDownloader.LoginUsername == null || DepotDownloader.LoginPassword == null)
        {
            Console.WriteLine("Login credentials to your steam account are required to download any depots");
            Console.WriteLine("Use username and password commands");
            return false;
        }

        string buildid = DataManager.ManifestVersionsList[args[0]];

        DepotDownloader.DownloadBuild(buildid, args[0]);
        return true;
    }
}