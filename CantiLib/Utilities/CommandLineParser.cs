//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Canti
{
    // Contains information about a command line argument/option
    public sealed class CommandLineOption
    {
        #region Properties and Fields

        // The description for this option
        internal string Description { get; set; }

        // The expected type we compare against when parsing this option's value
        internal Type Type { get; set; }

        // The delegate that gets invoked when this option's value is parsed
        internal dynamic Handler { get; set; }

        #endregion

        #region Constructors

        internal CommandLineOption(string Description, Type Type, Action<dynamic> Handler)
        {
            this.Description = Description;
            this.Type = Type;
            this.Handler = Handler;
        }

        #endregion
    }

    public sealed class CommandLineArguments : Dictionary<string, CommandLineOption>
    {
        #region Methods

        // Allows us to simplify adding of values when creating an argument list
        public void Add(string Option, string Description, Type Type, Action<dynamic> Handler)
        {
            Add(Option, new CommandLineOption(Description, Type, Handler));
        }

        #endregion

        #region Constructors

        // Sets the default comparer to case-insensitive
        public CommandLineArguments() : base(StringComparer.OrdinalIgnoreCase) { }

        #endregion
    }

    // Simplifies parsing of command line arguments/options
    public sealed class CommandLineParser
    {
        #region Methods

        // Parses command line arguments
        public static bool Parse(CommandLineArguments Args, string[] Input)
        {
            // Set a read state - this could be an enum, but... *shrug*
            // 0 = option, 1 = value, 2, process
            int ReadState = 0;
            string Option = null;

            // Check for a help command
            if (Input.Contains("-help", StringComparer.OrdinalIgnoreCase))
            {
                // Show argument descriptions
                ShowDescriptions(Args);
                return false;
            }
            else
            {
                // Iterate through all strings in input array
                foreach (string Arg in Input)
                {
                    // Looking for an option
                    if (ReadState == 0 && Arg.StartsWith("-"))
                    {
                        // Get option name
                        Option = Arg.Substring(1);

                        // Ignore if this option is not in our argument list
                        if (!Args.ContainsKey(Option)) continue;

                        // Set read state to look for a value
                        ReadState = 1;
                    }

                    // Looking for a value
                    else if (ReadState == 1)
                    {
                        // Attempt to process this option's value
                        try
                        {
                            // Declare a converted value object
                            dynamic ConvertedValue;

                            // Check if the expected type has a parse method
                            var Parse = Args[Option].Type.GetMethod("Parse", new[] { typeof(string) });
                            if (Parse != null)
                            {
                                ConvertedValue = Parse.Invoke(null, new object[] { Arg });
                            }

                            // Attempt to change type from string directly
                            else
                            {
                                ConvertedValue = Convert.ChangeType(Arg, Args[Option].Type);
                            }

                            // Invoke this option's handler
                            Args[Option].Handler.Invoke(ConvertedValue);

                            // Remove this option entry
                            Args.Remove(Option);

                            // Reset read state to look for next option
                            ReadState = 0;
                        }

                        // Attempt failed, this value was of the wrong type
                        catch
                        {
                            Console.WriteLine($"Invalid value for option flag -{Option} " +
                                $"(expecting type {Args[Option].Type.Name})");
                            return false;
                        }
                    }
                }
            }

            // If read state does not end on an option state, arguments were invalid
            if (ReadState != 0)
            {
                Console.WriteLine($"Could not process command line arguments (invalid syntax)");
                return false;
            }

            // If read state ends on an option state, parsing was successful
            else
            {
                return true;
            }
        }

        // Prints out all given arguments
        public static void ShowDescriptions(CommandLineArguments Args)
        {
            // Print header
            Console.WriteLine("Arguments:");

            // Iterate through all given argument options
            foreach (var Arg in Args)
            {
                // Print a new line
                Console.WriteLine();

                // Print name
                Console.WriteLine($"-{Arg.Key}:");

                // Print description
                Console.WriteLine($"  Description: {Arg.Value.Description}");

                // Print expected type
                Console.WriteLine($"  Expected Value Type: {Arg.Value.Type.Name}");

                // Print syntax
                if (Arg.Value.Type.IsValueType)
                {
                    Console.WriteLine($"  Example: -{Arg.Key} {Activator.CreateInstance(Arg.Value.Type)}");
                }
                else if (Arg.Value.Type == typeof(string))
                {
                    Console.WriteLine($"  Example: -{Arg.Key} \"value\"");
                }
            }
        }

        #endregion
    }
}
