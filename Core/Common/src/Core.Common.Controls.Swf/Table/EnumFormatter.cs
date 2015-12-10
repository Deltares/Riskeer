using System;

namespace Core.Common.Controls.Swf.Table
{
    public class EnumFormatter : ICustomFormatter
    {
        private readonly Type enumType;

        public EnumFormatter(Type enumType = null)
        {
            this.enumType = enumType;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            Enum enumValue = null;
            if (arg.GetType().IsEnum)
            {
                enumValue = (Enum) arg;
            }
            else if (enumType != null && arg is Int32)
            {
                enumValue = (Enum) Enum.ToObject(enumType, arg);
            }

            return enumValue != null
                       ? enumValue.ToString()
                       : arg.ToString();
        }
    }
}