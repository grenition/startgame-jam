using Unity.Netcode;

public class ControllerBusMessage : INetworkSerializable
{
    public int[] Data;

    public ControllerBusMessage()
    {
        Data = new int[0];
    }

    public ControllerBusMessage(int[] data)
    {
        Data = data;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if(serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            int count = 0;
            reader.TryBeginReadValue(in count);
            Data = new int[count];
            for(int i = 0; i < count; i++)
            {
                reader.ReadValue(out Data[i]);
            }
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            int length = Data.Length;
            writer.TryBeginWriteValue(in length);
            for(int i = 0; i < length; i++)
            {
                writer.WriteValue(Data[i]);
            }
        }
    }
}
