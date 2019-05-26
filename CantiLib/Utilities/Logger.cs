//
// Copyright (c) 2018-2019 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.IO;

namespace Canti
{
    /// <summary>
    /// An enumerator that specifies which level of output a logger will utilize
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// No logging takes place
        /// </summary>
        NONE = -2,

        /// <summary>
        /// Only messages with the "important" label are shown
        /// </summary>
        IMPORTANT_ONLY = -1,

        /// <summary>
        /// Only messages with the "important", "info", and "error" labels are shown
        /// </summary>
        DEFAULT = 0,

        /// <summary>
        /// Only messages with the "important", "info", "error", and "warning" labels are shown
        /// </summary>
        ENHANCED = 1,

        /// <summary>
        /// All message types are shown, including "debug", and time labels show TWO decimal places
        /// </summary>
        DEBUG = 2,

        /// <summary>
        /// All message types are shown, including "debug", and time labels show SIX decimal places
        /// </summary>
        MAX = 3
    }

    /// <summary>
    /// Logger utility to facilitate logging at different levels and saving to a log file
    /// </summary>
    public sealed class Logger
    {
        #region Properties and Fields

        /// <summary>
        /// What level of logging will be shown
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// (OPTIONAL) A file where all logger output is also written to
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// If set to true, a time and label prefix is shown alongside each log message
        /// </summary>
        public bool ShowPrefix { get; set; }

        /// <summary>
        /// (OPTIONAL) A custom prefix that will be shown before a label name, if showing prefixes
        /// </summary>
        public string CustomPrefix { get; set; }

        /// <summary>
        /// The default color for logger output
        /// </summary>
        public ConsoleColor InfoColor { get; set; }

        /// <summary>
        /// The color important messages will be shown in when logging
        /// </summary>
        public ConsoleColor ImportantColor { get; set; }

        /// <summary>
        /// The color debug messages will be shown in when logging
        /// </summary>
        public ConsoleColor DebugColor { get; set; }

        /// <summary>
        /// The color warning messages will be shown in when logging
        /// </summary>
        public ConsoleColor WarningColor { get; set; }

        /// <summary>
        /// The color error messages will be shown in when logging
        /// </summary>
        public ConsoleColor ErrorColor { get; set; }

        // Stores the current console color at the time of initialization
        private ConsoleColor DefaultColor { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Logs an "info" message with the specified "important" message color
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void Important(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.IMPORTANT_ONLY) return;
            Console.ForegroundColor = ImportantColor;
            Write("INFO", Input, Params);
        }

        /// <summary>
        /// Logs an "info" message
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void WriteLine(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.DEFAULT) return;
            Console.ForegroundColor = InfoColor;
            Write("INFO", Input, Params);
        }

        /// <summary>
        /// Logs an "error" message
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void Error(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.DEFAULT) return;
            Console.ForegroundColor = ErrorColor;
            Write("ERROR", Input, Params);
        }

        /// <summary>
        /// Logs a "warning" message
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void Warning(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.ENHANCED) return;
            Console.ForegroundColor = WarningColor;
            Write("WARNING", Input, Params);
        }

        /// <summary>
        /// Logs a "debug" message
        /// </summary>
        /// <param name="Input">The value to write</param>
        /// <param name="Params">An array of objects to write using format</param>
        public void Debug(object Input, params object[] Params)
        {
            if (LogLevel < LogLevel.DEBUG) return;
            Console.ForegroundColor = DebugColor;
            Write("DEBUG", Input, Params);
        }

        #endregion

        #region Private

        // Writes a log message to the console, as well as to an optional log file
        private void Write(string Label, object Input, params object[] Params)
        {
            // Check if we are logging
            if (LogLevel <= LogLevel.NONE) return;

            // Create an output string
            string Output;

            // Add time and label prefix
            if (ShowPrefix)
            {
                // Max log level, 6 figures of decimal resolution
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

                // Debug log level, 2 figures of decimal resolution
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

                // Default, no decimal places in time prefix
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

        /// <summary>
        /// Initialized a new logger instance
        /// </summary>
        public Logger()
        {
            // Store the current console color so we can reset it after writing a message
            DefaultColor = Console.ForegroundColor;
        }

        #endregion
    }
}
