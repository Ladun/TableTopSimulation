using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class PlayerProfile
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public ClientSession Session { get; set; }
        public GameRoom Room { get; set; }

        public bool isLogin { get; set; }

        public P_PlayerProfile GetPacketProfile()
        {
           P_PlayerProfile p = new P_PlayerProfile();
            p.Id = Id;
            p.Name = Name;
            return p;
        }
    }
}
