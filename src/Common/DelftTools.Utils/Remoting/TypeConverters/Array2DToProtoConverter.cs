using System;
using System.Linq;
using System.Runtime.InteropServices;
using ProtoBuf;
using ProtoBufRemote;

namespace DelftTools.Utils.Remoting.TypeConverters
{
    public class Array2DToProtoConverter<T> : ITypeToProtoConverter
    {
        public object ToProtoObject(object original)
        {
            return new SerializableArray2D((T[,])original);
        }

        public object FromProtoObject(object protoObject)
        {
            return ((SerializableArray2D)protoObject).ToArray2D();
        }

        public Type GetProtoType()
        {
            return typeof(SerializableArray2D);
        }

        public Type GetSourceType()
        {
            return typeof(T[,]);
        }

        [ProtoContract]
        private class SerializableArray2D
        {
            [ProtoMember(1)]
            private readonly T[] flatArray;

            [ProtoMember(2)]
            private readonly int stride;

            protected SerializableArray2D()
            {
            }

            public SerializableArray2D(T[,] matrix)
            {
                stride = matrix.GetLength(1);
                flatArray = matrix.Cast<T>().ToArray();
            }

            public T[,] ToArray2D()
            {
                var matrix = new T[flatArray.Length / stride, stride];
                Buffer.BlockCopy(flatArray, 0, matrix, 0, Marshal.SizeOf(flatArray[0])*flatArray.Length);
                return matrix;
            }
        }
    }


}
