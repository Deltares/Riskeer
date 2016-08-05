﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Storage;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class holds parameters which influence the probability estimate for a piping assessment.
    /// </summary>
    public class PipingProbabilityAssessmentInput : IStorable
    {
        private double a;

        /// <summary>
        /// Creates a new instance of <see cref="PipingProbabilityAssessmentInput"/>.
        /// </summary>
        public PipingProbabilityAssessmentInput()
        {
            A = 0.4;
            B = 300.0;
            SectionLength = double.NaN;
            UpliftCriticalSafetyFactor = 1.2;
        }

        /// <summary>
        /// Gets 'a' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        public double A
        {
            get
            {
                return a;
            }
            set
            {
                if (!(value >= 0) || !(value <= 1))
                {
                    throw new ArgumentException(Resources.PipingProbabilityAssessmentInput_A_Value_must_be_between_zero_and_one);
                }

                a = value;
            }
        }

        /// <summary>
        /// Gets the critical safety factor to which the calculated uplift stability factor is compared.
        /// </summary>
        public double UpliftCriticalSafetyFactor { get; set; }

        /// <summary>
        /// Gets 'b' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        public double B { get; private set; }

        /// <summary>
        /// Gets or sets the length of the assessment section.
        /// </summary>
        public double SectionLength { get; set; }

        public long StorageId { get; set; }
    }
}