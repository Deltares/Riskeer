// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Probability
{
    /// <summary>
    /// Class that holds all probability assessment calculation specific input parameters.
    /// </summary>
    public class ProbabilityAssessmentInput
    {
        private double contribution;
        private int n;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProbabilityAssessmentInput"/> class.
        /// </summary>
        public ProbabilityAssessmentInput()
        {
            N = 2;
            Norm = 0;
            Contribution = double.NaN;
        }

        /// <summary>
        /// Gets or sets the 'N' parameter used to factor in the 'length effect'.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="value"/> is not in interval 
        /// [1-20].</exception>
        public int N
        {
            get
            {
                return n;
            }
            set
            {
                if (value < 1 || value > 20)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.N_Value_should_be_in_interval_1_20);
                }
                n = value;
            }
        }

        /// <summary>
        /// Gets or sets the contribution of the failure mechanism as a percentage (0-100) to the total of the 
        /// failure probability of the assessment section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/> is not 
        /// in interval [1-100].</exception>
        public double Contribution
        {
            get
            {
                return contribution;
            }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.Contribution_Value_should_be_in_interval_0_100);
                }
                contribution = value;
            }
        }

        /// <summary>
        /// Gets or sets the return period to assess for.
        /// </summary>
        public int Norm { get; set; }
    }
}