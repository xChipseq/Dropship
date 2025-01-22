namespace Dropship.Commands;

[RegisterCommand]
public class CreateProfile : Command
{
    public override string Name => "create_profile";
    public override CommandCategory Category => CommandCategory.Profiles;
    public override string Description => "Creates a profile with choosen mod";
    public override string Arguments => "<mod> [mod_version]";

    public override bool Execute(string[] args)
    {
        throw new NotImplementedException();
    }
}