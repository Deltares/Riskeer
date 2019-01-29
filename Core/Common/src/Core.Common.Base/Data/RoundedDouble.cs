// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
    public struct RoundedDouble : IEquatable<RoundedDouble>, IEquatable<double>, IFormattable, IComparable,
                                  IComparable<RoundedDouble>, IComparable<double>
    {
        /// <summary>
        /// The maximum number of decimal places supported by this class.
        /// </summary>
        public const int MaximumNumberOfDecimalPlaces = 15;

        /// <summary>
        /// Represents a value that is not a number (NaN). This field is constant.
        /// </summary>
        /// <seealso cref="double.NaN"/>
        public static readonly RoundedDouble NaN = new RoundedDouble(MaximumNumberOfDecimalPlaces, double.NaN);

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedDouble"/> class with a 
        /// given value.
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The number of decimal places.</param>
        /// <param name="value">The value to initialize the instance with.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, <see cref="MaximumNumberOfDecimalPlaces"/>].
        /// </exception>
        public RoundedDouble(int numberOfDecimalPlaces, double value = 0.0)
        {
            ValidateNumberOfDecimalPlaces(numberOfDecimalPlaces);

            NumberOfDecimalPlaces = numberOfDecimalPlaces;
            Value = RoundDouble(value, numberOfDecimalPlaces);
        }

        /// <summary>
        /// Gets the number of decimal places use to round <see cref="Value"/> to.
        /// </summary>
        public int NumberOfDecimalPlaces { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value { get; }

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
            int smallestNumberOfDecimalPlaces = Math.Min(left.NumberOfDecimalPlaces, right.NumberOfDecimalPlaces);
            return new RoundedDouble(smallestNumberOfDecimalPlaces,
                                     left.Value - right.Value);
        }

        public static RoundedDouble operator +(RoundedDouble left, RoundedDouble right)
        {
            int smallestNumberOfDecimalPlaces = Math.Min(left.NumberOfDecimalPlaces, right.NumberOfDecimalPlaces);
            return new RoundedDouble(smallestNumberOfDecimalPlaces,
                                     left.Value + right.Value);
        }

        public static RoundedDouble operator *(RoundedDouble left, double right)
        {
            return new RoundedDouble(left.NumberOfDecimalPlaces, left.Value * right);
        }

        public static RoundedDouble operator *(double left, RoundedDouble right)
        {
            return new RoundedDouble(right.NumberOfDecimalPlaces, left * right.Value);
        }

        public static RoundedDouble operator *(RoundedDouble left, RoundedDouble right)
        {
            int smallestNumberOfDecimalPlaces = Math.Min(left.NumberOfDecimalPlaces, right.NumberOfDecimalPlaces);
            return new RoundedDouble(smallestNumberOfDecimalPlaces, left.Value * right.Value);
        }

        public static implicit operator double(RoundedDouble d)
        {
            return d.Value;
        }

        public static explicit operator RoundedDouble(double d)
        {
            return new RoundedDouble(MaximumNumberOfDecimalPlaces, d);
        }

        /// <summary>
        /// Converts this value to another <see cref="RoundedDouble"/> but with the given
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

            return new RoundedDouble(newNumberOfDecimalPlaces, Value);
        }

        public static bool operator <(RoundedDouble left, RoundedDouble right)
        {
            return left.Value < right.Value;
        }

        public static bool operator <=(RoundedDouble left, RoundedDouble right)
        {
            return left.Value <= right.Value;
        }

        public static bool operator >(RoundedDouble left, RoundedDouble right)
        {
            return left.Value > right.Value;
        }

        public static bool operator >=(RoundedDouble left, RoundedDouble right)
        {
            return left.Value >= right.Value;
        }

        public static bool operator <(RoundedDouble left, double right)
        {
            return left.Value < right;
        }

        public static bool operator <=(RoundedDouble left, double right)
        {
            return left.Value <= right;
        }

        public static bool operator >(RoundedDouble left, double right)
        {
            return left.Value > right;
        }

        public static bool operator >=(RoundedDouble left, double right)
        {
            return left.Value >= right;
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

            return Equals((RoundedDouble) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is RoundedDouble)
            {
                return CompareTo((RoundedDouble) obj);
            }

            if (obj is double)
            {
                return CompareTo((double) obj);
            }

            throw new ArgumentException("Arg must be double or RoundedDouble");
        }

        public int CompareTo(double other)
        {
            return Value.CompareTo(other);
        }

        public int CompareTo(RoundedDouble other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(double other)
        {
            return Value.Equals(other);
        }

        public bool Equals(RoundedDouble other)
        {
            return Value.Equals(other.Value);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (double.IsPositiveInfinity(Value))
            {
                return Resources.RoundedDouble_ToString_PositiveInfinity;
            }

            if (double.IsNegativeInfinity(Value))
            {
                return Resources.RoundedDouble_ToString_NegativeInfinity;
            }

            return Value.ToString(format ?? GetFormat(), formatProvider ?? CultureInfo.CurrentCulture);
        }

        private static double RoundDouble(double value, int numberOfDecimalPlaces)
        {
            return IsSpecialDoubleValue(value) ? value : Math.Round(value, numberOfDecimalPlaces, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Validates <see cref="NumberOfDecimalPlaces"/>.
        /// </summary>
        /// <param name="numberOfDecimalPlaces">The new value for <see cref="NumberOfDecimalPlaces"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="numberOfDecimalPlaces"/> is not in range [0, 15].
        /// </exception>
        private static void ValidateNumberOfDecimalPlaces(int numberOfDecimalPlaces)
        {
            if (numberOfDecimalPlaces < 0 || numberOfDecimalPlaces > MaximumNumberOfDecimalPlaces)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               "Value must be in range [0, {0}].",
                                               MaximumNumberOfDecimalPlaces);
                throw new ArgumentOutOfRangeException(nameof(numberOfDecimalPlaces),
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