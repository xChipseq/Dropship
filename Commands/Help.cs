namespace Dropship.Commands;

[RegisterCommand]
public class HelpCommand : Command
{
    public override string Name => "help";
    public override CommandCategory Category => CommandCategory.Other;
    public override string Description => "Shows a list of all the commands.";
    public override string Arguments => null;

    public override bool Execute(string[] args)
    {
        Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        var allCommands = CommandManager.Commands;
        Dictionary<CommandCategory, List<Command>> categories = new();
        foreach (var command in allCommands)
        {
            if (!categories.ContainsKey(command.Category))
            {
                categories.Add(command.Category, new());
            }
            categories[command.Category].Add(command);
            categories[command.Category].Sort((x, y) => x.Name.CompareTo(y.Name));
        }
        var sortedCategories = categories
            .OrderBy(kv => kv.Key)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        Console.WriteLine("Commands:");
        foreach (var category in sortedCategories)
        {
            foreach (Command command in category.Value)
            {
                int spaces = 50 - $"{command.Name}{(command.Arguments != null ? $" {command.Arguments}" : "")}".Length;
                string space = "";
                for (int i = 0; i <= spaces; i++)
                {
                    space += " ";
                }
                Console.WriteLine($"    {command.Name}{(command.Arguments != null ? $" {command.Arguments}" : "")}{space}- {command.Description}");
            }
            if (category.Key == CommandCategory.Other)
            {
                Console.WriteLine($"\n    close                                              - Exits this program");
            }

            Console.WriteLine();
        }

        return true;
    }
}