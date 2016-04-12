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

namespace Ringtoets.Piping.Data
{
    public class SemiProbabilisticPipingInput
    {
        private double contribution;

        public SemiProbabilisticPipingInput()
        {
            A = 1.0;
            B = 350.0;
            SectionLength = double.NaN;
            Norm = 0;
            Contribution = double.NaN;
        }

        /// <summary>
        /// Gets 'a' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        public double A { get; private set; }

        /// <summary>
        /// Gets 'b' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        public double B { get; private set; }

        /// <summary>
        /// Gets or sets the length of the assessment section.
        /// </summary>
        public double SectionLength { get; set; }

        /// <summary>
        /// Gets or sets the contribution of piping as a percentage (0-100) to the total of the failure 
        /// probability of the assessment section.
        /// </summary>
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