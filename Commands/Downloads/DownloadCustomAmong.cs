using System.Text.RegularExpressions;

namespace Dropship.Commands;

[RegisterCommand]
public class DownloadCustomAmongCommand : Command
{
    public override string Name => "download_custom_among";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Downloads an among us version from a build id. Get one from https://steamdb.info/app/945360/patchnotes/";
    public override string Arguments => "<buildid> <version>";

    public override bool Execute(string[] args)
    {
        if (args.Count() != 2)
        {
            InvalidArguments();
            return false;
        }

        bool validVersion = Regex.IsMatch(args[1], @"^\d{4}\.\d{1,2}\.\d{1,2}$");
        if (!validVersion)
        {
            Console.WriteLine($"The version \"{args[1]}\" is not in a correct format");
        }

        if (DepotDownloader.LoginUsername == null || DepotDownloader.LoginPassword == null)
        {
            Console.WriteLine("Login credentials to your steam account are required to download any depots");
            Console.WriteLine("Use username and password commands");
            return false;
        }

        DepotDownloader.DownloadBuild(args[0], args[1]);
        return true;
    }
}