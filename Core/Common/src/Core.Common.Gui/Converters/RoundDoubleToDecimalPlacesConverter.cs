using System;
using System.ComponentModel;
using System.Globalization;

namespace Core.Common.Gui.Converters
{
    /// <summary>
    /// This class can convert doubles (and their string counterparts) to doubles rounded
    /// to a given number of decimal places.
    /// </summary>
    public class RoundDoubleToDecimalPlacesConverter : TypeConverter
    {
        private readonly int numberOfDecimalPlaces;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundDoubleToDecimalPlacesConverter"/> class.
        /// </summary>
        /// <param name="numberOfPlaces">The number of decimal places.</param>
        public RoundDoubleToDecimalPlacesConverter(int numberOfPlaces)
        {
            if (numberOfPlaces < 0 || numberOfPlaces > 28)
            {
                throw new ArgumentOutOfRangeException("numberOfPlaces", "Value must be in range [0, 28].");
            }
            numberOfDecimalPlaces = numberOfPlaces;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(double))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string) || destinationType == typeof(double))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(double))
            {
                decimal numberAsDecimal = ConvertValueToDecimal(value);
                decimal roundedDecimal = Math.Round(numberAsDecimal, numberOfDecimalPlaces);
                return Convert.ToDouble(roundedDecimal);
            }
            if (destinationType == typeof(string))
            {
                decimal numberAsDecimal = ConvertValueToDecimal(value);
                decimal roundedDecimal = Math.Round(numberAsDecimal, numberOfDecimalPlaces);
                return roundedDecimal.ToString(GetStringFormat());
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private string GetStringFormat()
        {
            return "F"+numberOfDecimalPlaces;
        }

        private static decimal ConvertValueToDecimal(object value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch (FormatException formatException)
            {
                string message = string.Format("De waarde '{0}' is geen getal.", value);
                throw new NotSupportedException(message, formatException);
            }
            catch (OverflowException overflowException)
            {
                string message = string.Format("De waarde '{0}' is te groot of te klein om te kunnen verwerken.", value);
                throw new NotSupportedException(message, overflowException);
            }
        }
    }

    #region Specializations of RoundDoubleToDecimalPlacesConverter to be used for TypeConverterAttribute

    /// <summary>
    /// This class can convert doubles (and their string counterparts) to doubles rounded
    /// to two decimal places.
    /// </summary>
    public class RoundDoubleToTwoDecimalPlacesConverter : RoundDoubleToDecimalPlacesConverter
    {
        public RoundDoubleToTwoDecimalPlacesConverter() : base(2) {}
    }

    /// <summary>
    /// This class can convert doubles (and their string counterparts) to doubles rounded
    /// to three decimal places.
    /// </summary>
    public class RoundDoubleToThreeDecimalPlacesConverter : RoundDoubleToDecimalPlacesConverter
    {
        public RoundDoubleToThreeDecimalPlacesConverter() : base(3) {}
    }

    #endregion
}