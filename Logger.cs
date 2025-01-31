using System.Reflection;
using Dropship.Commands;

namespace Dropship;

public static class Logger
{
    public static bool Debug = false;

    public static void Log(string text, bool logOnly = false)
    {
        File.AppendAllText(Directories.LogPath, $"({Program.RuntimeTimer.ElapsedMilliseconds}mm) [Debug/Log] {text}{Environment.NewLine}");
        if (Debug && !logOnly)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"[Debug] {text}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    public static void Warn(string text)
    {
        File.AppendAllText(Directories.LogPath, $"({Program.RuntimeTimer.ElapsedMilliseconds}mm) [Debug/Warn] {text}{Environment.NewLine}");
        if (Debug)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[Debug] {text}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    public static void Error(string text)
    {
        File.AppendAllText(Directories.LogPath, $"({Program.RuntimeTimer.ElapsedMilliseconds}mm) [Debug/Error] {text}{Environment.NewLine}");
        
        // Errors are always logged
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"[Error] {text}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void Title()
    {
        Console.Title = "Dropship Mod Manager";
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
      ___                            _      _        
     |   \   _ _   ___   _ __   ___ | |_   (_)  _ __ 
     | |) | | '_| / _ \ | '_ \ (_-< | ' \  | | | '_ \                   by Chipseq
     |___/  |_|   \___/ | .__/ /__/ |_||_| |_| | .__/                   version 1.0
                        |_|                    |_|   
        ");

        Console.ForegroundColor = ConsoleColor.White;
    }
}