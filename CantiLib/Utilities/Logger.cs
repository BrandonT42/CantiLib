using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Canti.Utilities
{
    public enum Level
    {
        DEBUG, INFO, WARNING, ERROR, FATAL
    }

    public struct Entry
    {
        public DateTime Timestamp { get; internal set; }
        public Level Level { get; internal set; }
        public string Content { get; internal set; }
        public Entry(DateTime Timestamp, Level Level, string Content)
        {
            this.Timestamp = Timestamp;
            this.Level = Level;
            this.Content = Content;
        }
    }

    public class Logger
    {
        public string LogFile { get; private set; }
        private Queue<Entry> Entries { get; set; }
        private bool Running { get; set; }

        public Logger(string LogFile = null)
        {
            this.LogFile = LogFile;
            Entries = new Queue<Entry>();
            Running = true;
        }

        public void Start()
        {
            Thread Thread = new Thread(delegate ()
            {
                while (true)
                {
                    if (Entries.Count == 0)
                    {
                        if (!Running) break;
                    }
                    else
                    {
                        Entry Entry = Entries.Dequeue();
                        string Output = string.Format("{0} [{1}] {2}", Entry.Timestamp, Entry.Level, Entry.Content);
                        Console.WriteLine(Output);
                        if (LogFile != null) File.AppendAllText(LogFile, Output + Environment.NewLine);
                    }
                    Thread.Sleep(10);
                }
            });
            Thread.Start();
        }

        public void Stop()
        {
            Running = false;
        }

        public void Log(Level Level, string Content)
        {
            Entries.Enqueue(new Entry(DateTime.Now.ToUniversalTime(), Level, Content));
        }
    }
}
