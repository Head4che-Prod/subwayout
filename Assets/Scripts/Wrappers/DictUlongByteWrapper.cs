using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.Netcode;

namespace Wrappers
{
    public struct DictUlongByteWrapper : INetworkSerializable
    {
        public Dictionary<ulong, byte> Dictionary;

        public DictUlongByteWrapper(Dictionary<ulong, byte> dictionary)
        {
            Dictionary = dictionary;
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = Dictionary?.Count * 2 ?? 0;      // Number of individual elements to serialize
            
            serializer.SerializeValue(ref count);   // Serialize element count in write mode, reads it in read mode

            if (serializer.IsWriter) // If serializing
            {
                if (Dictionary == null)
                    return;

                foreach (KeyValuePair<ulong, byte> pair in Dictionary)
                {
                    ulong key = pair.Key;
                    serializer.SerializeValue(ref key);
                    byte value = pair.Value;
                    serializer.SerializeValue(ref value);
                }
            }
            else // if reading
            {
                Dictionary = new Dictionary<ulong, byte>();
                for (int i = 0; i < count; i += 2)
                {
                    ulong key = 0;
                    serializer.SerializeValue(ref key);
                    byte value = 0;
                    serializer.SerializeValue(ref value);
                    Dictionary.Add(key, value);
                }
            }
        }
    }
}