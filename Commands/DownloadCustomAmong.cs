using Dropship.DepotDownloader;

namespace Dropship.Commands;

[RegisterCommand]
public class DownloadCustomAmongCommand : Command
{
    public override string Name => "download_custom_among";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Downloads an among us version from a build id. Get one from https://steamdb.info/app/945360/patchnotes/";
    public override string Arguments => "<buildid>";

    public override bool Execute(string[] args)
    {
        if (args.Count() != 1)
        {
            InvalidArguments();
            return false;
        }
        
        if (DepotDownloaderLoader.LoginUsername == null || DepotDownloaderLoader.LoginPassword == null)
        {
            Console.WriteLine("Login credentials to your steam account are required to download any depots");
            Console.WriteLine("Use username and password commands");
            return false;
        }

        DepotDownloaderLoader.DownloadBuild(args[0]);
        return true;
    }
}