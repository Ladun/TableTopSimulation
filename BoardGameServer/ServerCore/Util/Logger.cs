using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public class Logger : Singleton<Logger>
    {
        object _lock = new object();

        public enum LogLevel { Info };
        private struct LogInfo
        {
            public int time;
            public string log;
        }

        private List<LogInfo> logs;

        public Logger()
        {
            logs = new List<LogInfo>();
        }

        public void Print(string str, LogLevel level = LogLevel.Info)
        {
            int time = System.Environment.TickCount;
            string printStr = GetPrefix(time, level) + str;

            lock (_lock)
            {
                logs.Add(new LogInfo { time = time, log = printStr });
                Console.WriteLine(printStr);
            }
        }

        private string GetPrefix(int time, LogLevel level)
        {
            return $"[{time}][{level.ToString()}] ";
        }
    }
}