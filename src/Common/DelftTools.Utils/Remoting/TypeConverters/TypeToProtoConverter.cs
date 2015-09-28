using System;
using ProtoBuf;
using ProtoBufRemote;

namespace DelftTools.Utils.Remoting.TypeConverters
{
    public class TypeToProtoConverter : ITypeToProtoConverter
    {
        public object ToProtoObject(object original)
        {
            return new SerializableType((Type)original);
        }

        public object FromProtoObject(object protoObject)
        {
            return ((SerializableType)protoObject).ToType();
        }

        public Type GetProtoType()
        {
            return typeof(SerializableType);
        }

        public Type GetSourceType()
        {
            return typeof(Type);
        }

        [ProtoContract]
        private class SerializableType
        {
            [ProtoMember(1)]
            private string fullName;

            protected SerializableType() { }

            public SerializableType(Type type)
            {
                fullName = type.FullName;
            }

            public Type ToType()
            {
                return Type.GetType(fullName);
            }
        }
    }
}