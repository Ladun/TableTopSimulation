using System.Net;
using ServerCore;

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

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Logger.Instance.Print("Listening...");

            JobTimer.Instance.Push(FlushRoom);
            
            while (true)
            {
                JobTimer.Instance.Flush();
            }
        }
    } 
}
