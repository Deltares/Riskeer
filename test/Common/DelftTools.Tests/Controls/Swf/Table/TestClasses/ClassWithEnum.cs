using System;
using System.ComponentModel;
using System.Globalization;

namespace DelftTools.Tests.Controls.Swf.Table.TestClasses
{
    internal class ClassWithEnum
    {
        public FruitType Type { get; set; }
    }

    //Type converter is needed when the enum is used in a datatable for xtragrid
    [TypeConverter(typeof(EnumToInt32TypeConverter<FruitType>))]
    internal enum FruitType
    {
        [Description("Een Appel")]
        Appel,

        [Description("Een Peer")]
        Peer,

        [Description("Een Banaan")]
        Banaan
    }

    public class EnumToInt32TypeConverter<TEnum> : EnumConverter
    {
        public EnumToInt32TypeConverter(Type type)
            : base(type)
        {
            // since generic type parameters can't be constrained to enums,
            //  this asset is required to perform the logic check
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(int))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(int))
            {
                return (TEnum) value;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}