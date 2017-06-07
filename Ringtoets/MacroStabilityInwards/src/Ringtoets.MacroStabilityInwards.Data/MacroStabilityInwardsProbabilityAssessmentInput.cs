// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.MacroStabilityInwards.Data.Properties;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// This class holds parameters which influence the probability estimate for a macro stability inwards assessment.
    /// </summary>
    public class MacroStabilityInwardsProbabilityAssessmentInput
    {
        private static readonly Range<double> validityRangeA = new Range<double>(0, 1);
        private double a;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsProbabilityAssessmentInput"/>.
        /// </summary>
        public MacroStabilityInwardsProbabilityAssessmentInput()
        {
            A = 0.4;
            B = 300.0;
            SectionLength = double.NaN;
        }

        /// <summary>
        /// Gets or sets 'a' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when value is not in the range [0, 1].</exception>
        public double A
        {
            get
            {
                return a;
            }
            set
            {
                if (!validityRangeA.InRange(value))
                {
                    throw new ArgumentException(string.Format(Resources.MacroStabilityInwardsProbabilityAssessmentInput_A_Value_must_be_in_Range_0_,
                                                              validityRangeA.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture)));
                }

                a = value;
            }
        }

        /// <summary>
        /// Gets 'b' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        public double B { get; private set; }

        /// <summary>
        /// Gets or sets the length of the assessment section.
        /// </summary>
        public double SectionLength { get; set; }
    }
}