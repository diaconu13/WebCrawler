using System;
using System.Linq;
using WebCrawler.Common;

namespace WebCrawler.Helpers
{
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
            if (!argument.Contains(":"))
            {
                return commands;
            }

            string[] segments = argument.Split(CommandNames.Separator);

            if (segments.Length <= 1)
            {
                return commands;
            }

            string name = segments[0];
            // this os one way of getting the value ... but we can do better :)
            //var startIndex = argument.IndexOf(CommandNames.Separator)+1;
            //string value = argument.Substring(startIndex,argument.Length-startIndex);
            //like this 
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

                    if (Uri.TryCreate(value, UriKind.Absolute, out var address))
                    {
                        commands.Address = address;
                    }
                            
                    break;
            }

            return commands;
        }
    }
}
