using Google.Protobuf.Protocol;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class CustomCommand
    {
        private enum State { Success, Failed, Terminated}

        private State state;

        StringBuilder sb = new StringBuilder();

        private string successMsg = "";

        public bool Update()
        {

            sb.Clear();
            while (true)
            {
                Logger.Instance.PrintCommand(sb.ToString());
                char ch = Console.ReadKey().KeyChar;
                if (ch == (char)13)
                {
                    break;
                }
                else if (ch == (char)8)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                else
                {
                    sb.Append(ch);
                }
            }
            string line = sb.ToString();
            Logger.Instance.ClearCommand(line.Length);

            InstructionProcess(line);

            switch (state)
            {
                case State.Success:
                    Logger.Instance.Print(successMsg, Logger.LogLevel.DEBUG);
                    break;
                case State.Failed:
                    Logger.Instance.Print($"'{line}' is Wrong command", Logger.LogLevel.DEBUG);
                    break;
                case State.Terminated:
                    Logger.Instance.Print("Terminate Server", Logger.LogLevel.DEBUG);
                    return false;
            }

            return true;
        }

        private void InstructionProcess(string command)
        {
            command = command.Trim();
            string[] components = command.Split(" ");

            state = State.Success;
            if (components[0].Equals("save"))
            {
                // TODO: save something
            }
            else if (components[0].Equals("quit") || components[0].Equals("exit"))
            {
                state = State.Terminated;
                return;
            }
            // log LOGLEVEL1,LOGLEVEL2,...
            // ex) log INFO,DEBUG
            else if (components[0].Equals("log"))
            {
                string[] param = components[1].Split(",");
                if (Logger.Instance.SetLogLevel(param))
                {
                    return;
                }
            }
            else if (components[0].Equals("echo"))
            {

                S_Chat sChatPacket = new S_Chat();
                sb.Clear();
                for (int i = 1; i < components.Length; i++)
                {
                    sb.Append(components[i]);
                    sb.Append(" ");
                }

                sChatPacket.Chat = "[Server] " + sb.ToString();
                PlayerProfileManager.Instance.Broadcast(sChatPacket);
                return;
            }

            state = State.Failed;
        }
    }
}
