using Unity.Netcode;

public abstract class ControllerBusMessage : INetworkSerializable
{
    public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
}
