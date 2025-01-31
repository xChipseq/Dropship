namespace Dropship.Commands;

[RegisterCommand]
public class DebugCommand : Command
{
    public override string Name => "debug";
    public override CommandCategory Category => CommandCategory.Other;
    public override string Description => "Toggles debugging mode. All logs will be visible";
    public override string Arguments => "<enabled: true | false>";

    public override bool Execute(string[] args)
    {
        if (args.Count() != 1 || (args[0].ToLower() != bool.FalseString.ToLower() && args[0].ToLower() != bool.TrueString.ToLower()))
        {
            InvalidArguments();
            return false;
        }

        bool.TryParse(args[0].ToLower(), out bool toggle);
        Logger.Debug = toggle;
        return true;
    }
}