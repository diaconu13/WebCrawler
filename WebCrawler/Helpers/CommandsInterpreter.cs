using System;
using System.Collections.Generic;
using System.Linq;

namespace WebCrawler.Helpers
{
    public class Commands
    {
        public bool AllowExternal { get; set; }
        public int Depth { get; set; }
        public string Destination { get; set; }
        public string Address { get; set; }
    }
    public class CommandsInterpreter
    {
        public Commands ParseCommand(string[] args)
        {
            Commands commands = new Commands();
            foreach (string argument in args)
            {
                if (argument.StartsWith(CommandNames.Identifier))
                {
                    if (argument == CommandNames.AllowExternal)
                    {
                        commands.AllowExternal = true;
                    }

                    commands = ParseTwoSegmentsCommands(argument, commands);
                }
            }

            return commands;
        }

        private static Commands ParseTwoSegmentsCommands(string argument, Commands commands)
        {
            if (argument.Contains(":"))
            {
                string[] segments = argument.Split(CommandNames.Separator);
                if (segments.Length > 1)
                {
                    string name = segments[0];
                    // this os one way of getting the value ... but we can do better :)
                    //var startIndex = argument.IndexOf(CommandNames.Separator)+1;
                    //string value = argument.Substring(startIndex,argument.Length-startIndex);

                    string value = string.Join(":", segments.Skip(1));

                    switch (name)
                    {
                        case CommandNames.Depth:

                            if (int.TryParse(value, out var number))
                            {
                                commands.Depth = number;
                            }
                            else
                            {
                                Console.WriteLine($"Parameter named {name} value is not a int number");
                            }

                            break;
                        case CommandNames.Destination:
                            commands.Destination = value;
                            break;
                        case CommandNames.Address:
                            commands.Address = value;
                            break;
                    }
                }
            }

            return commands;
        }
    }

    public struct CommandNames
    {
        public const char Separator = ':';
        public const string Identifier = "--";
        public const string Depth = "--depth";
        public const string Destination = "--destination";
        public const string Address = "--address";
        public const string AllowExternal = "--allowExternal";

    }
}
