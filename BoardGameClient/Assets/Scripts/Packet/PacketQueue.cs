using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketMessage
{
	public ushort Id { get; set; }
	public IMessage Message { get; set; }
}

public class PacketQueue
{
	public static PacketQueue Instance { get; } = new PacketQueue();

	Queue<PacketMessage> _packetQueue = new Queue<PacketMessage>();
	object _lock = new object();

	public void Push(ushort id, IMessage packet)
	{
		lock (_lock)
		{
			_packetQueue.Enqueue(new PacketMessage() { Id = id, Message = packet });
		}
	}

	public PacketMessage Pop()
	{
		lock (_lock)
		{
			if (_packetQueue.Count == 0)
				return null;

			return _packetQueue.Dequeue();
		}
	}

	public List<PacketMessage> PopAll(int sceneBuildIndex)
	{
		List<PacketMessage> list = new List<PacketMessage>();

		lock (_lock)
		{
			int queueCount = _packetQueue.Count;
			while (queueCount > 0)
			{
				PacketMessage packet = _packetQueue.Dequeue();
				string msgName = packet.Message.Descriptor.Name.Replace("_", string.Empty);
				int packetCode = (((int)(MsgId)Enum.Parse(typeof(MsgId), msgName)) & 0xF000) >> 12;


				if ((packetCode == 0 || packetCode - 1 == sceneBuildIndex) && Managers.Instance.GetScene<BaseScene>().SettingFinish)
					list.Add(packet);
				else
					_packetQueue.Enqueue(packet);

				queueCount--;
			}
		}

		return list;
	}
}