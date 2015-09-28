using System;
using ProtoBuf;
using ProtoBufRemote;

namespace DelftTools.Utils.Remoting.TypeConverters
{
    public class DateTimeArrayToProtoConverter : ITypeToProtoConverter
    {
        public object ToProtoObject(object original)
        {
            return new SerializableDateTimeArray((DateTime[])original);
        }

        public object FromProtoObject(object protoObject)
        {
            return ((SerializableDateTimeArray)protoObject).ToDateTimeArray();
        }

        public Type GetProtoType()
        {
            return typeof(SerializableDateTimeArray);
        }

        public Type GetSourceType()
        {
            return typeof(DateTime[]);
        }

        [ProtoContract]
        private class SerializableDateTimeArray
        {
            [ProtoMember(1)] private readonly long[] dtValues;

            protected SerializableDateTimeArray() { }

            public SerializableDateTimeArray(DateTime[] dtArray)
            {
                dtValues = new long[dtArray.Length];
                for (int i = 0; i < dtArray.Length; i++)
                {
                    dtValues[i] = dtArray[i].Ticks;
                }
            }

            public DateTime[] ToDateTimeArray()
            {
                var dtArray = new DateTime[dtValues.Length];
                for (int i = 0; i < dtValues.Length; i++)
                {
                    dtArray[i] = new DateTime(dtValues[i]);
                }
                return dtArray;
            }
        }
    }
}