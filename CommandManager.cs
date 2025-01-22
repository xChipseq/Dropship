using System.Reflection;
using Dropship.Commands;

namespace Dropship;

public static class CommandManager
{
    public static readonly List<Command> Commands = new();
    public static void RegisterCommands()
    {
        Logger.Log("registering commands...");
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.GetCustomAttribute<RegisterCommand>() != null);
        
        foreach (var type in types)
        {
            var command = (Command)Activator.CreateInstance(type);
            Commands.Add(command);
            Logger.Log($"registered \"{command.Name}\" command");
        }
    }

    public static void ExecuteCommand(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        var commandName = parts[0].ToLower();
        var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        if (CommandExists(commandName))
        {
            Command command = Commands.FirstOrDefault(c => c.Name.ToLower() == commandName.ToLower());
            try
            {
                command.Execute(args);
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }
        else
        {
            Console.WriteLine($"Unknown command: {commandName}\nhelp for a list of commands");
        }
    }

    public static bool CommandExists(string text)
    {
        return Commands.Where(c => c.Name.ToLower() == text.ToLower()).ToList().Count > 0;
    }
}