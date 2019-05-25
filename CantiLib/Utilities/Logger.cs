//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;

namespace Canti
{
    // Enumerator for log levels
    public enum LogLevel
    {
        // No logging happens
        NONE = -2,

        // Only important
        IMPORTANT_ONLY = -1,

        // Only important, info, and errors
        DEFAULT = 0,

        // Only important, info, errors, and warnings
        ENHANCED = 1,

        // All label types with 2 figure resolution of timestamps
        DEBUG = 2,

        // All label types with 6 figure resolution of timestamps
        MAX = 3
    }

    // TODO - queue output instead of writing it at once?
    public sealed class Logger
    {
        #region Properties and Fields

        // The log level that is displayed
        public LogLevel LogLevel { get; set; }

        // An optional file that all logger output is written to
        public string LogFile { get; set; }

        // If this is set to true, no time or label prefix is shown
        public bool ShowPrefix { get; set; }

        // An optional custom prefix
        public string CustomPrefix { get; set; }

        // The colors we will use when writing
        public ConsoleColor InfoColor { get; set; }
        public ConsoleColor ImportantColor { get; set; }
        public ConsoleColor DebugColor { get; set; }
        public ConsoleColor WarningColor { get; set; }
        public ConsoleColor ErrorColor { get; set; }
        public ConsoleColor DefaultColor { get; set; }

        #endregion

        #region Methods

        #region Public

        // Writes with the info label and an alternate color
        public void Important(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.IMPORTANT_ONLY) return;
            Console.ForegroundColor = ImportantColor;
            Write("INFO", Input, Params);
        }

        // Writes with the info label and default color
        public void WriteLine(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.DEFAULT) return;
            Console.ForegroundColor = InfoColor;
            Write("INFO", Input, Params);
        }

        // Writes with the error label and error color
        public void Error(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.DEFAULT) return;
            Console.ForegroundColor = ErrorColor;
            Write("ERROR", Input, Params);
        }

        // Writes with the warning label and warning color
        public void Warning(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.ENHANCED) return;
            Console.ForegroundColor = WarningColor;
            Write("WARNING", Input, Params);
        }

        // Writes with the debug label and debug color
        public void Debug(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.DEBUG) return;
            Console.ForegroundColor = DebugColor;
            Write("DEBUG", Input, Params);
        }

        #endregion

        #region Private

        private void Write(string Label, object Input, params object[] Params)
        {
            // Check if we are logging
            if (LogLevel <= LogLevel.NONE) return;

            // Create an output string
            string Output;

            // Add time and label prefix
            if (ShowPrefix)
            {
                if (LogLevel >= LogLevel.MAX)
                {
                    if (!string.IsNullOrEmpty(CustomPrefix))
                    {
                        Output = $"{DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.ffffff")} [{CustomPrefix} {Label}]";
                        Output = Output.PadRight(CustomPrefix.Length + 38);
                    }
                    else
                    {
                        Output = $"{DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.ffffff")} [{Label}]".PadRight(37);
                    }
                }
                else if (LogLevel == LogLevel.DEBUG)
                {
                    if (!string.IsNullOrEmpty(CustomPrefix))
                    {
                        Output = $"{DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.ff")} [{CustomPrefix} {Label}]";
                        Output = Output.PadRight(CustomPrefix.Length + 34);
                    }
                    else
                    {
                        Output = $"{DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss.ff")} [{Label}]".PadRight(33);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(CustomPrefix))
                    {
                        Output = $"{DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")} [{CustomPrefix} {Label}]";
                        Output = Output.PadRight(CustomPrefix.Length + 31);
                    }
                    else
                    {
                        Output = $"{DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")} [{Label}]".PadRight(30);
                    }
                }
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

            // Reset console color
            Console.ForegroundColor = DefaultColor;

            // Append to log file
            if (!string.IsNullOrEmpty(LogFile))
            {
                // TODO - Queue this and write it from queue to prevent rare race conditions
                File.AppendAllText(LogFile, Output + "\n");
            }
        }

        #endregion

        #endregion

        #region Constructors

        public Logger()
        {
            DefaultColor = Console.ForegroundColor;
        }

        #endregion
    }
}
