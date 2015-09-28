using System;
using ProtoBuf;
using ProtoBufRemote;

namespace DelftTools.Utils.Remoting.TypeConverters
{
    public class DateTimeToProtoConverter : ITypeToProtoConverter
    {
        public object ToProtoObject(object original)
        {
            return new SerializableDateTime((DateTime)original);
        }

        public object FromProtoObject(object protoObject)
        {
            return ((SerializableDateTime)protoObject).ToDateTime();
        }

        public Type GetProtoType()
        {
            return typeof(SerializableDateTime);
        }

        public Type GetSourceType()
        {
            return typeof(DateTime);
        }

        [ProtoContract]
        private class SerializableDateTime
        {
            [ProtoMember(1)] private readonly long ticks;

            protected SerializableDateTime() { }

            public SerializableDateTime(DateTime type)
            {
                ticks = type.Ticks;
            }

            public DateTime ToDateTime()
            {
                return new DateTime(ticks);
            }
        }
    }
}