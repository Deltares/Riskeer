// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// This class represents a definition of some structure-parameter that has been defined
    /// in a *.csv file.
    /// </summary>
    public class StructuresParameterRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StructuresParameterRow"/> class.
        /// </summary>
        public StructuresParameterRow()
        {
            NumericalValue = double.NaN;
            VarianceValue = double.NaN;
            LineNumber = -1;
        }

        /// <summary>
        /// Gets or sets the location identifier to which this parameter belongs.
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Gets or sets the ID of this parameter.
        /// </summary>
        public string ParameterId { get; set; }

        /// <summary>
        /// Gets or sets the alphanumerical value for the parameter.
        /// </summary>
        public string AlphanumericValue { get; set; }

        /// <summary>
        /// Gets or sets the numerical value (interpreted as the mean of a random variable
        /// or as deterministic value) for the parameter.
        /// </summary>
        public double NumericalValue { get; set; }

        /// <summary>
        /// Gets or sets the variance value (interpreted as the standard deviation of a
        /// random variable or a the coefficient of variation of a random variable).
        /// </summary>
        public double VarianceValue { get; set; }

        /// <summary>
        /// Gets or sets the type that defines how <see cref="VarianceValue"/> should be interpreted.
        /// </summary>
        public VarianceType VarianceType { get; set; }

        /// <summary>
        /// Gets or sets the line number of where this parameter was defined.
        /// </summary>
        public int LineNumber { get; set; }
    }
}