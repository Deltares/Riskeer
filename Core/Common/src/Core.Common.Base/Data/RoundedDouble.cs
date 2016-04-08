using System;
using System.ComponentModel;
using System.Globalization;

using Core.Common.Base.Properties;
using Core.Common.Base.TypeConverters;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// This class represents a <see cref="double"/> that is being rounded to a certain
    /// number of places.
    /// </summary>
    [TypeConverter(typeof(RoundedDoubleConverter))]
    public struct RoundedDouble : IEquatable<RoundedDouble>, IEquatable<Double>, IFormattable
    {
        /// <summary>
        /// The maximum number of decimal places supported by this class.
        /// </summary>
        public const int MaximumNumberOfDecimalPlaces = 15;

        private readonly double value;
        private readonly int numberOfDecimalPlaces;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedDouble"/> class with a 
        /// given value.
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places.</param>
        /// <param name="value">The value to initialize the instance with.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, <see cref="MaximumNumberOfDecimalPlaces"/>].
        /// </exception>
        public RoundedDouble(int numberOfDecimalPlaces, double value = 0.0)
        {
            ValidateNumberOfDecimalPlaces(numberOfDecimalPlaces);

            this.numberOfDecimalPlaces = numberOfDecimalPlaces;
            this.value = RoundDouble(value, numberOfDecimalPlaces);
        }

        /// <summary>
        /// Gets the number of decimal places use to round <see cref="Value"/> to.
        /// </summary>
        public int NumberOfDecimalPlaces
        {
            get
            {
                return numberOfDecimalPlaces;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value
        {
            get
            {
                return value;
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

        public static RoundedDouble operator -(RoundedDouble left, RoundedDouble right)
        {
            int smallestNumberOfDecimalPlaces = Math.Min(left.numberOfDecimalPlaces, right.numberOfDecimalPlaces);
            return new RoundedDouble(smallestNumberOfDecimalPlaces,
                                     left.value - right.value);
        }

        public static RoundedDouble operator +(RoundedDouble left, RoundedDouble right)
        {
            int smallestNumberOfDecimalPlaces = Math.Min(left.numberOfDecimalPlaces, right.numberOfDecimalPlaces);
            return new RoundedDouble(smallestNumberOfDecimalPlaces,
                                     left.value + right.value);
        }

        public static RoundedDouble operator *(RoundedDouble left, double right)
        {
            return new RoundedDouble(left.numberOfDecimalPlaces, left.value * right);
        }

        public static RoundedDouble operator *(double left, RoundedDouble right)
        {
            return new RoundedDouble(right.numberOfDecimalPlaces, left * right.value);
        }

        public static RoundedDouble operator *(RoundedDouble left, RoundedDouble right)
        {
            int smallestNumberOfDecimalPlaces = Math.Min(left.numberOfDecimalPlaces, right.numberOfDecimalPlaces);
            return new RoundedDouble(smallestNumberOfDecimalPlaces, left.value * right.value);
        }

        public static implicit operator double(RoundedDouble d)
        {
            return d.value;
        }

        public static explicit operator RoundedDouble(Double d)
        {
            return new RoundedDouble(MaximumNumberOfDecimalPlaces, d);
        }

        /// <summary>
        /// Converts this value to another <see cref="RoundedDouble"/> but with the givenTo the precision
        /// number of decimal places instead.
        /// </summary>
        /// <param name="newNumberOfDecimalPlaces">The new number of decimal places.</param>
        /// <returns>The converted <see cref="RoundedDouble"/>.</returns>
        public RoundedDouble ToPrecision(int newNumberOfDecimalPlaces)
        {
            if (newNumberOfDecimalPlaces == NumberOfDecimalPlaces)
            {
                return this;
            }
            return new RoundedDouble(newNumberOfDecimalPlaces, value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
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

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (double.IsPositiveInfinity(value))
            {
                return Resources.RoundedDouble_ToString_PositiveInfinity;
            }
            if (double.IsNegativeInfinity(value))
            {
                return Resources.RoundedDouble_ToString_NegativeInfinity;
            }
            
            return Value.ToString(format ?? GetFormat(), formatProvider ?? CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public bool Equals(double other)
        {
            return Value.Equals(other);
        }

        public bool Equals(RoundedDouble other)
        {
            return Value.Equals(other.Value);
        }

        private static double RoundDouble(double value, int numberOfDecimalPlaces)
        {
            return IsSpecialDoubleValue(value) ?
                       value :
                       Math.Round(value, numberOfDecimalPlaces, MidpointRounding.AwayFromZero);
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
    }
}