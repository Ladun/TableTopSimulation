using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game;
using ServerCore;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public PlayerProfile Profile { get; set; }
        public int SessionId { get; set; }

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Logger.Instance.Print($"OnConnected: {endPoint}/{SessionId}");
            //MyPlayer = PlayerProfileManager.Instance.Add();
            //{
            //    MyPlayer.Name = $"Player_{MyPlayer.Id}";
            //    MyPlayer.Session = this;
            //}
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            if(Profile.Room != null)
            {
                Profile.Room.LeaveGame(Profile.Id);
            }

            SessionManager.Instance.Remove(this);
            PlayerProfileManager.Instance.Remove(Profile.Id);


            Logger.Instance.Print($"OnDisconnected: {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            //Logger.Instance.Print($"Transferred bytes: {numOfBytes}");
        }
    }
}
