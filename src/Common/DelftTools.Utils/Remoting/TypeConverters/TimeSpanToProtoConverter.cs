using System;
using ProtoBuf;
using ProtoBufRemote;

namespace DelftTools.Utils.Remoting.TypeConverters
{
    public class TimeSpanToProtoConverter : ITypeToProtoConverter
    {
        public object ToProtoObject(object original)
        {
            return new SerializableTimeSpan((TimeSpan)original);
        }

        public object FromProtoObject(object protoObject)
        {
            return ((SerializableTimeSpan)protoObject).ToTimeSpan();
        }

        public Type GetProtoType()
        {
            return typeof(SerializableTimeSpan);
        }

        public Type GetSourceType()
        {
            return typeof(TimeSpan);
        }

        [ProtoContract]
        private class SerializableTimeSpan
        {
            [ProtoMember(1)]
            private readonly long ticks;

            protected SerializableTimeSpan() { }

            public SerializableTimeSpan(TimeSpan type)
            {
                ticks = type.Ticks;
            }

            public TimeSpan ToTimeSpan()
            {
                return new TimeSpan(ticks);
            }
        }
    }
}