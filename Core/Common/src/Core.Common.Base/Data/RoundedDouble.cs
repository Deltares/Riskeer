using System;
using System.Globalization;

using Core.Common.Base.Properties;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// This class represents a <see cref="double"/> that is being rounded to a certain
    /// number of places.
    /// </summary>
    public sealed class RoundedDouble : IEquatable<RoundedDouble>
    {
        private double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedDouble"/> class.
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, 28].
        /// </exception>
        public RoundedDouble(int numberOfDecimalPlaces)
        {
            if (numberOfDecimalPlaces < 0 || numberOfDecimalPlaces > 28)
            {
                throw new ArgumentOutOfRangeException("numberOfDecimalPlaces",
                                                      "Value must be in range [0, 28].");
            }

            NumberOfDecimalPlaces = numberOfDecimalPlaces;
        }

        /// <summary>
        /// Gets the number of decimal places use to round <see cref="Value"/> to.
        /// </summary>
        public int NumberOfDecimalPlaces { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <exception cref="OverflowException">When <paramref name="value"/> is too big
        /// or too small to be represented as a rounded double.</exception>
        public double Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = IsSpecialDoubleValue(value) ?
                                 value :
                                 ConvertIntoRoundedDouble(value);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((RoundedDouble)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            if (double.IsPositiveInfinity(value))
            {
                return Resources.RoundedDouble_ToString_PositiveInfinity;
            }
            if (double.IsNegativeInfinity(value))
            {
                return Resources.RoundedDouble_ToString_NegativeInfinity;
            }
            return Value.ToString(GetFormat(), CultureInfo.CurrentCulture);
        }

        public bool Equals(RoundedDouble other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Value.Equals(other.Value);
        }

        public static bool operator ==(RoundedDouble left, RoundedDouble right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RoundedDouble left, RoundedDouble right)
        {
            return !Equals(left, right);
        }

        private static bool IsSpecialDoubleValue(double value)
        {
            return double.IsNaN(value) ||
                   double.IsPositiveInfinity(value) ||
                   double.IsNegativeInfinity(value);
        }

        private string GetFormat()
        {
            return "F" + NumberOfDecimalPlaces;
        }

        private double ConvertIntoRoundedDouble(double valueToConvert)
        {
            decimal decimalValue = Convert.ToDecimal(valueToConvert);
            decimal roundedDecimal = Math.Round(decimalValue, NumberOfDecimalPlaces);
            return Convert.ToDouble(roundedDecimal);
        }
    }
}