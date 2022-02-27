using System;
using System.Net;
using System.Threading;
using ServerCore;
using System.IO;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        static void FlushRoom()
        {
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            IPAddress ipAddr = IPAddress.Any;
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            // Command
            CustomCommand cmd = new CustomCommand();

            PackageManager.Instance.GetHashCode();

            // Server Setting
            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Logger.Instance.Print("Listening...");
            JobTimer.Instance.Push(FlushRoom);

            while (true)
            {
                JobTimer.Instance.Flush();

                if (!cmd.Update())
                {
                    break;
                }
            }

            Logger.Instance.Print("Server Process Terminated");
        }
    } 
}
