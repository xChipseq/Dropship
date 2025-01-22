using Dropship.DepotDownloader;

namespace Dropship.Commands;

[RegisterCommand]
public class UsernameCommand : Command
{
    public override string Name => "username";
    public override CommandCategory Category => CommandCategory.AccountStuff;
    public override string Description => "Sets your login username for downloading depots. Required";
    public override string Arguments => "<name>";

    public override bool Execute(string[] args)
    {
        if (args.Count() != 1 || string.IsNullOrEmpty(args[0]))
        {
            InvalidArguments();
            return false;
        }
        string username = args[0];
        DepotDownloaderLoader.LoginUsername = username;
        Console.WriteLine($"Username set to {username}");
        return true;
    }
}