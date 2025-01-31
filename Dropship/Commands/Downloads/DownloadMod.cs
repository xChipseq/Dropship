namespace Dropship.Commands;

[RegisterCommand]
public class DownloadModCommand : Command
{
    public override string Name => "download_mod";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Downloads the mod of your choice";
    public override string Arguments => "<mod> [version]";

    public override bool Execute(string[] args)
    {
        if (args.Count() == 1 && !string.IsNullOrWhiteSpace(args[0])) // No version specified
        {
            if (!DataManager.ModList.ContainsKey(args[0]))
            {
                Console.WriteLine($"No such mod as \"{args[0]}\"\nmodlist for a list of all mods");
                return false;
            }
            DataManager.DownloadMod(args[0], "latest").GetAwaiter().GetResult();
        }
        else if (args.Count() == 2 && !string.IsNullOrWhiteSpace(args[1])) // A version is specified
        {
            if (!DataManager.ModList.ContainsKey(args[0]))
            {
                Console.WriteLine($"No such mod as \"{args[0]}\"\nmodlist for a list of all mods");
                return false;
            }
            DataManager.DownloadMod(args[0], args[1]).GetAwaiter().GetResult(); // Downloads the specified version
        }
        else
        {
            InvalidArguments();
            return false;
        }
        return true;
    }
}