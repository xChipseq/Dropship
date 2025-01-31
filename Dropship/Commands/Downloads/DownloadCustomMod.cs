/*
namespace Dropship.Commands;

[RegisterCommand]
public class DownloadCustomModCommand : Command
{
    public override string Name => "download_custom_mod";
    public override CommandCategory Category => CommandCategory.Downloads;
    public override string Description => "Downloads a mod from a specific link. Must be a direct-download zip file";
    public override string Arguments => "<link> <modname>";

    public override bool Execute(string[] args)
    {
        if (args.Count() != 2)
        {
            InvalidArguments();
            return false;
        }

        //DataManager.DownloadModFromLink(args[0], args[1]);
        return true;
    }
}
*/