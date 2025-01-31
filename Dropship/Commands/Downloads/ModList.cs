namespace Dropship.Commands;

[RegisterCommand]
public class ModListCommand : Command
{
    public override string Name => "modlist";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Shows a list of all dropship-supported and downloaded mods";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        Console.WriteLine("All available mods:");
        foreach (var mod in DataManager.ModList)
        {
            Console.WriteLine(mod.Key);
            Console.WriteLine($"    Name: {mod.Value.Name}");
            Console.WriteLine($"    Author: {mod.Value.Author}");
        }
        var mods = DataManager.GetDownloadedMods();
        if (mods.Count != 0)
        {
            Console.WriteLine("\nDownloaded mods:");
            foreach (var mod in mods)
            {
                Console.WriteLine($"{mod.Key}:");
                foreach (var version in mod.Value)
                {
                    Console.WriteLine($"    - {version}");
                }
            }
        }

        return true;
    }
}