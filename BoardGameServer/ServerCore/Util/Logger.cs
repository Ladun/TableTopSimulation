using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public class Logger : Singleton<Logger>
    {
        object _lock = new object();

        public enum LogLevel { INFO = 0x001, DEBUG = 0x002 };


        private struct LogInfo
        {
            public int time;
            public string log;
        }

        private List<LogInfo> logs;

        private int logLevel;

        // Print position
        private int cx, cy;
        // Command position
        private int ccx, ccy;

        private StringBuilder sb;
        private string recentCommand;

        public Logger()
        {
            logs = new List<LogInfo>();

            foreach (LogLevel _logLevel in Enum.GetValues(typeof(LogLevel)))
            {
                logLevel = logLevel | (int)_logLevel;
            }

            cx = Console.CursorLeft;
            cy = Console.CursorTop;
            ccy = Console.WindowTop + Console.WindowHeight - 2;
            Console.SetCursorPosition(ccx, ccy);

            sb = new StringBuilder();
            recentCommand = "";
        }

        public void Print(string str, LogLevel level = LogLevel.INFO)
        {
            int time = System.Environment.TickCount;
            string printStr = GetPrefix(time, level) + str;

            lock (_lock)
            {
                logs.Add(new LogInfo { time = time, log = printStr });
                if ((logLevel & (int)level) != 0)
                {
                    ConsolePrint(printStr);
                }
            }
        }
        public void PrintCommand(string command)
        {
            lock (_lock)
            {
                ClearLineForCommand(command.Length + 1);
                Console.SetCursorPosition(0, ccy);
                Console.Write(">>" + command);
                recentCommand = command;
            }
        }

        public void ClearCommand(int size)
        {
            lock (_lock)
            {
                ClearLineForCommand(size);
                recentCommand = "";
            }
        }

        public void ClearLineForCommand(int size)
        {
            lock (_lock)
            {
                sb.Clear();
                for (int i = 0; i < size + 2; i++)
                    sb.Append(" ");
                Console.SetCursorPosition(0, ccy);
                Console.Write(sb.ToString());
            }
        }

        private void ConsolePrint(string str)
        {
            ClearLineForCommand(recentCommand.Length);
            Console.SetCursorPosition(cx, cy);
            Console.WriteLine(str);

            cx = Console.CursorLeft;
            cy = Console.CursorTop;

            if (cy == ccy)
            {
                ccy += 1;
                Console.WindowTop++;
            }
            PrintCommand(recentCommand);
        }

        private string GetPrefix(int time, LogLevel level)
        {
            return $"[{time}][{level.ToString()}] ";
        }

        public bool SetLogLevel(string[] levels)
        {
            LogLevel tmp;
            lock (_lock)
            {
                logLevel = 0;
                for (int i = 0; i < levels.Length; i++)
                {
                    if (Enum.TryParse(levels[i], out tmp))
                    {
                        logLevel = logLevel | (int)tmp;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }

}