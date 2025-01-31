using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Dropship.Commands;

[RegisterCommand]
public class LaunchVanillaCommand : Command
{
    public override string Name => "launch_vanilla";
    public override CommandCategory Category => CommandCategory.Play;
    public override string Description => "Launches the vanilla game";
    public override string Arguments => "<version>";

    public override bool Execute(string[] args)
    {
        if (args.Count() != 1)
        {
            InvalidArguments();
            return false;
        }
        if (string.IsNullOrWhiteSpace(args[0]))
        {
            InvalidArguments();
            return false;
        }

        bool validVersion = Regex.IsMatch(args[1], @"^\d{4}\.\d{1,2}\.\d{1,2}$");
        if (!validVersion)
        {
            Console.WriteLine($"The version \"{args[1]}\" is not in a correct format");
        }

        string amongPath = Path.Combine(Directories.VersionsFolder, args[0]);
        if (!Directory.Exists(amongPath))
        {
            Console.WriteLine($"The version \"{args[0]}\" is not installed");
            return false;
        }
        string amongExecutable = Path.Combine(amongPath, "Among Us.exe");

        File.WriteAllText(Path.Combine(amongPath, "steam_appid.txt"), "945360"); // steam_appid.txt because we are not using steam and this *should* work

        // Launch Among Us with steam
        var gameProcess = Process.Start(new ProcessStartInfo()
        {
            FileName = amongExecutable,
        });

        Console.WriteLine($"{args[0]} launched");

        return true;
    }
}