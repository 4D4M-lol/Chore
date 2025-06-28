using System.Text.Json;

namespace Chore
{
    public static class Chore
    {
        public static Dictionary<string, string> Commands = new()
        {
            { "add \"[CHORE_NAME]\"", "Add a chore to your chores list." },
            { "help", "Shows a list of available commands." },
            { "list", "Shows your chores list." },
            { "remove [CHORE_INDEX]", "Remove a chore from your chores list." },
            { "reset", "Reset your chores list." },
        };
        public static List<string> Chores = new();
        public static readonly string ChoresFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ChoreApp",
            "chores.json"
        );
        public static readonly int CommandPadding = Commands.Keys.Select(k => k.Length).Max() + 5;

        private static void LoadChores()
        {
            try
            {
                if (File.Exists(ChoresFilePath))
                {
                    string json = File.ReadAllText(ChoresFilePath);
                    Chores = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                string dot = ex.Message.EndsWith('.') ? "." : "";
                
                Console.WriteLine($"Error loading chores: {ex.Message}{dot}");
                Console.ResetColor();
                
                Chores = new List<string>();
            }
        }

        private static void SaveChores()
        {
            try
            {
                string? directory = Path.GetDirectoryName(ChoresFilePath);
                
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(Chores, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                File.WriteAllText(ChoresFilePath, json);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                string dot = ex.Message.EndsWith('.') ? "." : "";
                
                Console.WriteLine($"Error saving chores: {ex.Message}{dot}");
                Console.ResetColor();
            }
        }
        
        public static void Main(string[] args)
        {
            LoadChores();
            
            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                
                Console.WriteLine("Chore by @4D4M_lol on GitHub.\nType \"chore help\" to get a list of commands and their description.");
            }
            else if (args.Length == 1)
            {
                if (args[0] == "help")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;

                    foreach ((string command, string description) in Commands)
                    {
                        Console.WriteLine($"{command.PadRight(CommandPadding)}:     {description}");
                    }
                }
                else if (args[0] == "list")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.WriteLine($"===== {Environment.UserName}'s Chore =====");

                    if (Chores.Count == 0)
                    {
                        Console.WriteLine("You do not have any chores.");
                        Console.ResetColor();

                        return;
                    }

                    for (int index = 0; index < Chores.Count; index++)
                    {
                        Console.WriteLine($"{index + 1}. {Chores[index]}");
                    }
                }
                else if (args[0] == "reset")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    
                    Chores.Clear();
                    SaveChores();
                    Console.WriteLine("Chores list has been reset.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($"Invalid command: \"{args[0]}\", type \"chore help\" to get a list of commands and their description.");
                }
            }
            else if (args.Length == 2)
            {
                if (args[0] == "add")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    
                    Chores.Add(args[1]);
                    SaveChores();
                    Console.WriteLine($"Chore \"{args[1]}\" at index {Chores.Count}.");
                }
                else if (args[0] == "remove")
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        int index = int.Parse(args[1]);

                        if (index < 1 || index > Chores.Count)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        
                            Console.WriteLine($"Index out of range: {index}, index should be between 1 and {Chores.Count}.");
                            Console.ResetColor();
                            
                            return;
                        }
                        
                        Console.WriteLine($"Chore \"{Chores[index]}\" removed at index {index}.");
                        Chores.RemoveAt(index - 1);
                        SaveChores();
                    }
                    catch (FormatException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        
                        Console.WriteLine($"Invalid command: [{String.Join(", ", args)}], type \"chore help\" to get a list of commands and their description.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    
                    Console.WriteLine($"Invalid command: [{String.Join(", ", args)}], type \"chore help\" to get a list of commands and their description.");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                    
                Console.WriteLine($"Invalid command: [{String.Join(", ", args)}], type \"chore help\" to get a list of commands and their description.");
            }
            
            Console.ResetColor();
        }
    }
}