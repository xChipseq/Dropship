namespace Dropship.Commands;

[RegisterCommand]
public class RememberLoginCommand : Command
{
    public override string Name => "remember_login";
    public override CommandCategory Category => CommandCategory.AccountStuff;
    public override string Description => "Remembers your password";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        DepotDownloader.RememberLoginData = true;
        Console.WriteLine("Your password will be saved for later");
        return true;
    }
}