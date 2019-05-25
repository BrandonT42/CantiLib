//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;

namespace Canti
{
    // TODO - queue output instead of writing it at once?
    // TODO - log level, check in each method
    public sealed class Logger
    {
        #region Properties and Fields

        // An optional file that all logger output is written to
        public static string LogFile { get; set; }

        // If this is set to true, no time or label prefix is shown
        public static bool ShowPrefix { get; set; }

        #endregion

        #region Methods

        #region Public

        // Writes with the info label and default color
        public static void WriteLine(object Input, params object[] Params)
        {
            Console.ForegroundColor = Globals.InfoColor;
            Write("INFO", Input, Params);
        }

        // Writes with the info label and an alternate color
        public static void Important(object Input, params object[] Params)
        {
            Console.ForegroundColor = Globals.ImportantColor;
            Write("INFO", Input, Params);
        }

        // Writes with the debug label and debug color
        public static void Debug(object Input, params object[] Params)
        {
            Console.ForegroundColor = Globals.DebugColor;
            Write("DEBUG", Input, Params);
        }

        // Writes with the warning label and warning color
        public static void Warning(object Input, params object[] Params)
        {
            Console.ForegroundColor = Globals.WarningColor;
            Write("WARNING", Input, Params);
        }

        // Writes with the error label and error color
        public static void Error(object Input, params object[] Params)
        {
            Console.ForegroundColor = Globals.ErrorColor;
            Write("ERROR", Input, Params);
        }

        #endregion

        #region Private

        private static void Write(string Label, object Input, params object[] Params)
        {
            // Create an output string
            string Output;

            // Add time and label prefix
            if (ShowPrefix)
            {
                Output = $"{DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.ff")} [{Label}] ".PadRight(33);
                Output += $"{Input}";
            }
            else
            {
                Output = $"{Input}";
            }

            // Write to console
            if (Params.Length > 0)
            {
                Console.WriteLine(Output, Params);
            }
            else
            {
                Console.WriteLine(Output);
            }

            // Append to log file
            if (!string.IsNullOrEmpty(LogFile))
            {
                // TODO - Queue this and write it from queue to prevent rare race conditions
                File.AppendAllText(LogFile, Output + "\n");
            }
        }

        #endregion

        #endregion
    }
}
