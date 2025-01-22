using Dropship.DepotDownloader;

namespace Dropship.Commands;

[RegisterCommand]
public class PasswordCommand : Command
{
    public override string Name => "password";
    public override CommandCategory Category => CommandCategory.AccountStuff;
    public override string Description => "Sets your login password for downloading depots. Required";
    public override string Arguments => "<password>";

    public override bool Execute(string[] args)
    {
        if (args.Count() != 1 || string.IsNullOrEmpty(args[0]))
        {
            InvalidArguments();
            return false;
        }
        string password = args[0];
        if (password.Length < 6)
        {
            Console.WriteLine("Password is in the wrong format: Too short");
            return false;
        }
        DepotDownloaderLoader.LoginPassword = password;
        string censored = password[..2]+"***"+password[5..];
        Console.WriteLine($"Password set to {censored}");
        return true;
    }
}