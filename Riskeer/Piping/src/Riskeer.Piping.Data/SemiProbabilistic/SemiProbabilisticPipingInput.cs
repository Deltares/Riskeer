// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Data;

namespace Riskeer.Piping.Data.SemiProbabilistic
{
    /// <summary>
    /// Class for semi-probabilistic piping calculation specific input parameters.
    /// </summary>
    public class SemiProbabilisticPipingInput : PipingInput
    {
        private RoundedDouble assessmentLevel;

        /// <summary>
        /// Creates a new instance of <see cref="SemiProbabilisticPipingInput"/>.
        /// </summary>
        /// <param name="generalInputParameters">General piping calculation parameters that are the same across all
        /// piping calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalInputParameters"/>
        /// is <c>null</c>.</exception>
        public SemiProbabilisticPipingInput(GeneralPipingInput generalInputParameters) : base(generalInputParameters)
        {
            assessmentLevel = new RoundedDouble(2, double.NaN);
        }
        
        /// <summary>
        /// Gets or sets whether the assessment level is manual input for the calculation.
        /// </summary>
        public bool UseAssessmentLevelManualInput { get; set; }

        /// <summary>
        /// Gets or sets the outside high water level.
        /// [m+NAP]
        /// </summary>
        /// <remarks>This property is only used for calculations when <see cref="UseAssessmentLevelManualInput"/> is <c>true</c>.</remarks>
        public RoundedDouble AssessmentLevel
        {
            get => assessmentLevel;
            set => assessmentLevel = value.ToPrecision(assessmentLevel.NumberOfDecimalPlaces);
        }
    }
}