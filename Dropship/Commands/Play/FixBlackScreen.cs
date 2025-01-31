namespace Dropship.Commands;

[RegisterCommand]
public class FixBlackScreenCommand : Command
{
    public override string Name => "fix_black_screen";
    public override CommandCategory Category => CommandCategory.Play;
    public override string Description => "If you are having problems with older among us versions, this command should fix the black screen";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        string settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "Innersloth", "Among Us", "settings.amogus");
        if (File.Exists(settingsFile))
            File.Delete(settingsFile);
        Console.WriteLine($"{settingsFile} removed, this should fix the black screen!");
        return true;
    }
}