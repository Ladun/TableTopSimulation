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
                successMsg = sChatPacket.Chat;
                PlayerProfileManager.Instance.Broadcast(sChatPacket);
                return;
            }
            else if (components[0].Equals("players"))
            {
                List<PlayerProfile> pp = PlayerProfileManager.Instance.GetProfiles();
                Logger.Instance.Print($"[Player List]=======================================", Logger.LogLevel.DEBUG);
                for (int i = 0; i < pp.Count; i++)
                {
                    Logger.Instance.Print($"Id: {pp[i].Id},\t Name: {pp[i].Name},\t Current Room Id: {(pp[i].Room != null? pp[i].Room.RoomId: 'x')}", Logger.LogLevel.DEBUG);
                }

                return;
            }
            else if (components[0].Equals("rooms"))
            {
                if (components.Length == 1)
                {
                    List<GameRoom> roomList = RoomManager.Instance.GetRoomList();
                    Logger.Instance.Print($"[Room List]=======================================", Logger.LogLevel.DEBUG);
                    for (int i = 0; i < roomList.Count; i++)
                    {
                        Logger.Instance.Print($"Id: {roomList[i].RoomId},\t Name: {roomList[i].RoomName}", Logger.LogLevel.DEBUG);
                    }
                }
                else if(components.Length == 2)
                {
                    int roomId;
                    if(int.TryParse(components[1], out roomId))
                    {
                        GameRoom gr = RoomManager.Instance.Find(roomId);
                        Logger.Instance.Print($"[Room Info]Id: {gr.RoomId},\t Name: {gr.RoomName},\tCurrent Player Count: {gr.CurrentPlayerCount}", Logger.LogLevel.DEBUG);
                        return;
                    }
                    else
                    {
                        // TODO: set error message
                    }
                }
                return;
            }

            state = State.Failed;
        }
    }
}
