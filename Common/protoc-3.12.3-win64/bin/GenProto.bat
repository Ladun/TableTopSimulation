protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../../BoardGameServer/PacketGenerator/bin/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../BoardGameClient/Assets/Scripts/Packet"
XCOPY /Y Protocol.cs "../../../BoardGameServer/Server/Packet"
XCOPY /Y ClientPacketManager.cs "../../../BoardGameClient/Assets/Scripts/Packet"
XCOPY /Y ServerPacketManager.cs "../../../BoardGameServer/Server/Packet"