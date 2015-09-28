using System;
using System.Runtime.InteropServices;
using ProtoBuf;
using ProtoBufRemote;

namespace DelftTools.Utils.Remoting.TypeConverters
{
    //TODO: Make this work! Excluded now because it interferes with 2d-arrays, but eventually we want to remote the BMI...
    public class ArrayToProtoConverter: ITypeToProtoConverter
    {
        public object ToProtoObject(object original)
        {
            return new SerializableArray((Array) original);
        }

        public object FromProtoObject(object protoObject)
        {
            return ((SerializableArray) protoObject).ToArray();
        }

        public Type GetProtoType()
        {
            return typeof (SerializableArray);
        }

        public Type GetSourceType()
        {
            return typeof (Array);
        }

        [ProtoContract]
        private class SerializableArray
        {
            [ProtoMember(1)] private readonly byte[] data;

            [ProtoMember(2)] private readonly string valueType;

            protected SerializableArray()
            {
            }

            public SerializableArray(Array array)
            {
                var elemType = array.GetType().GetElementType();
                valueType = elemType.FullName;
                data = new byte[Marshal.SizeOf(elemType)*array.Length];
                Buffer.BlockCopy(array, 0, data, 0, data.Length);
            }

            public Array ToArray()
            {
                var type = Type.GetType(valueType);
                if (type == typeof (double))
                {
                    var result = new double[data.Length/sizeof (double)];
                    Buffer.BlockCopy(data, 0, result, 0, data.Length);
                    return result;
                }
                if (type == typeof(int))
                {
                    var result = new int[data.Length / sizeof(int)];
                    Buffer.BlockCopy(data, 0, result, 0, data.Length);
                    return result;
                }
                if (type == typeof(float))
                {
                    var result = new float[data.Length / sizeof(float)];
                    Buffer.BlockCopy(data, 0, result, 0, data.Length);
                    return result;
                }
                throw new NotImplementedException("Arrays of type " + type + " can not be converted from protocol buffer");
            }
        }
    }
}
