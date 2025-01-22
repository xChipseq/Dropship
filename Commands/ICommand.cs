namespace Dropship.Commands;

public enum CommandCategory
{
    Profiles,
    Downloads,
    Play,
    AccountStuff,
    Other,
}

public abstract class Command
{
    public abstract string Name { get; }
    public abstract CommandCategory Category { get; }
    public abstract string Description { get; }
    public abstract string Arguments { get; }

    public abstract bool Execute(string[] args);

    public void InvalidArguments()
    {
        Console.WriteLine($"Invalid Arguments.\nUsage: {Name} {Arguments}");
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RegisterCommand : Attribute;