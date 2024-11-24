using System;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;

namespace ProtoBuf.Extension
{
    public static class ProtobufHelper
    {
        public static object FromBytes(Type type, byte[] bytes, int index, int count)
        {
            using (MemoryStream stream = new MemoryStream(bytes, index, count))
            {
                object o = RuntimeTypeModel.Default.Deserialize(stream, null, type);
                if (o is ISupportInitialize supportInitialize)
                {
                    supportInitialize.EndInit();
                }
                return o;
            }
        }

        public static byte[] ToBytes(object message)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, message);
                return stream.ToArray();
            }
        }

        public static void ToStream(object message, MemoryStream stream)
        {
            Serializer.Serialize(stream, message);
        }

        public static object FromStream(Type type, MemoryStream stream)
        {
            object o = RuntimeTypeModel.Default.Deserialize(stream, null, type);
            if (o is ISupportInitialize supportInitialize)
            {
                supportInitialize.EndInit();
            }
            return o;
        }
    }
}