using System;
using System.Globalization;

using Core.Common.Base.Properties;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// This class represents a <see cref="double"/> that is being rounded to a certain
    /// number of places.
    /// </summary>
    public sealed class RoundedDouble : IEquatable<RoundedDouble>, IEquatable<Double>
    {
        /// <summary>
        /// The maximum number of decimal places supported by this class.
        /// </summary>
        public const int MaximumNumberOfDecimalPlaces = 15;

        private double value;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedDouble"/> class with a 
        /// given value.
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places.</param>
        /// <param name="value">The value to initalize the instance with.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, 15].
        /// </exception>
        public RoundedDouble(int numberOfDecimalPlaces, double value = 0.0)
        {
            ValidateNumberOfDecimalPlaces(numberOfDecimalPlaces);

            NumberOfDecimalPlaces = numberOfDecimalPlaces;
            Value = value;
        }

        /// <summary>
        /// Gets the number of decimal places use to round <see cref="Value"/> to.
        /// </summary>
        public int NumberOfDecimalPlaces { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
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

        public static bool operator ==(RoundedDouble left, RoundedDouble right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RoundedDouble left, RoundedDouble right)
        {
            return !Equals(left, right);
        }

        public static implicit operator double(RoundedDouble d)
        {
            return d.Value;
        }

        public static explicit operator RoundedDouble(Double d)
        {
            return new RoundedDouble(MaximumNumberOfDecimalPlaces, d);
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

        public bool Equals(double other)
        {
            return Value.Equals(other);
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

        /// <summary>
        /// Validates <see cref="NumberOfDecimalPlaces"/>.
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The new value for <see cref="NumberOfDecimalPlaces"/>.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, 15].
        /// </exception>
        private static void ValidateNumberOfDecimalPlaces(int numberOfDecimalPlaces)
        {
            if (numberOfDecimalPlaces < 0 || numberOfDecimalPlaces > MaximumNumberOfDecimalPlaces)
            {
                string message = string.Format("Value must be in range [0, {0}].",
                                               MaximumNumberOfDecimalPlaces);
                throw new ArgumentOutOfRangeException("numberOfDecimalPlaces",
                                                      message);
            }
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
            return Math.Round(valueToConvert, NumberOfDecimalPlaces, MidpointRounding.AwayFromZero);
        }
    }
}