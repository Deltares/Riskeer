using System;
using Core.Common.Utils;

namespace Core.Common.Controls.Swf.Table
{
    public class TableViewComboBoxItem : IConvertible
    {
        public object Value { get; set; }

        public string DisplayText
        {
            get
            {
                if (CustomFormatter != null)
                {
                    return CustomFormatter.Format(null, Value, null);
                }

                if (Value.GetType().IsEnum)
                {
                    return EnumDescriptionAttributeTypeConverter.GetEnumDescription((Enum) Value);
                }

                return Value.ToString();
            }
        }

        public ICustomFormatter CustomFormatter { get; set; }

        public override string ToString()
        {
            return Value != null ? Value.ToString() : "null";
        }

        #region Implementation of IConvertible

        public TypeCode GetTypeCode()
        {
            return TypeCode.String;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Value;
        }

        #endregion
    }
}