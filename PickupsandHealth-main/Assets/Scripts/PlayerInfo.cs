using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;
using Unity.Collections;
using UnityEngine.UI;

public struct PlayerInfo : INetworkSerializable, System.IEquatable<PlayerInfo> {
    public ulong clientId;
    public Color color;
    public bool isReady;
    public int playerSprite;

    public PlayerInfo(ulong id, Color c, int i, bool ready=false ) {
        clientId = id;
        color = c;
        playerSprite = i;
        isReady = ready;

    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref color);
        serializer.SerializeValue(ref isReady);
        serializer.SerializeValue(ref playerSprite);
    }

    public bool Equals(PlayerInfo other) {
        return other.clientId == clientId;
    }
}