namespace Dropship.Commands;

[RegisterCommand]
public class RememberLoginCommand : Command
{
    public override string Name => "remember_login";
    public override CommandCategory Category => CommandCategory.AccountStuff;
    public override string Description => "Saves your login credentials for later";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        DepotDownloader.RememberLoginData = true;
        DepotDownloader.EncryptLogin();
        Console.WriteLine("Login credentials saved");
        return true;
    }
}