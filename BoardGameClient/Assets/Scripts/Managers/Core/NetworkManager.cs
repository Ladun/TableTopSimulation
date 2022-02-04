using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.Protocol;

public class NetworkManager
{
	ServerSession _session = new ServerSession();
	List<IMessage> delaySendQueue = new List<IMessage>();

	public void Send(IMessage packet)
	{
		if (_session.Connected)
			_session.Send(packet);
        else
        {
			delaySendQueue.Add(packet);
		}
	}

	public void Init()
	{
	}

	public bool Connect(string ip)
	{
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress cipAddr = ipHost.AddressList[0];
		Debug.Log(cipAddr.ToString());

        IPAddress ipAddr = null;
		if (!IPAddress.TryParse(ip, out ipAddr))
			return false;

		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		if (_session.Connected)
			Disconnect();

		_session = new ServerSession();

		Connector connector = new Connector();
		connector.Connect(endPoint,
			() => { return _session; },
			1);

		return true;
	}

	public void Disconnect()
    {
		_session.Disconnect();
    }

	public void Update()
	{
		List<PacketMessage> list = PacketQueue.Instance.PopAll(Managers.Instance.Scene.GetCurrentSceneBuildIndex());
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}

		if (_session.Connected && delaySendQueue.Count > 0)
		{
			foreach(IMessage packet in delaySendQueue)
            {
				_session.Send(packet);
            }
			delaySendQueue.Clear();
		}
	}

}
