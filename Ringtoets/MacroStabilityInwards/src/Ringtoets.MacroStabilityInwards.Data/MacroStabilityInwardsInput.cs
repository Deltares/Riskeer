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
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Class that holds all macro stability inwards calculation specific input parameters, i.e.. the values
    /// that can differ across various calculations.
    /// </summary>
    public class MacroStabilityInwardsInput : Observable, ICalculationInput
    {
        private readonly GeneralMacroStabilityInwardsInput generalInputParameters;
        private RoundedDouble assessmentLevel;
        private bool useAssessmentLevelManualInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsInput"/> class.
        /// </summary>
        /// <param name="generalInputParameters">General macro stability inwards calculation parameters that
        /// are the same across all macro stability inwards calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalInputParameters"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsInput(GeneralMacroStabilityInwardsInput generalInputParameters)
        {
            if (generalInputParameters == null)
            {
                throw new ArgumentNullException(nameof(generalInputParameters));
            }

            this.generalInputParameters = generalInputParameters;

            assessmentLevel = new RoundedDouble(2, double.NaN);
            useAssessmentLevelManualInput = false;
        }

        /// <summary>
        /// Gets or sets the surface line.
        /// </summary>
        public RingtoetsMacroStabilityInwardsSurfaceLine SurfaceLine { get; set; }

        /// <summary>
        /// Gets or sets the stochastic soil model which is linked to the <see cref="StochasticSoilProfile"/>.
        /// </summary>
        public StochasticSoilModel StochasticSoilModel { get; set; }

        /// <summary>
        /// Gets or sets the profile which contains a 1 dimensional definition of soil layers with properties.
        /// </summary>
        public StochasticSoilProfile StochasticSoilProfile { get; set; }

        /// <summary>
        /// Gets or sets the hydraulic boundary location from which to use the assessment level.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; set; }

        /// <summary>
        /// Gets or sets whether the assessment level is manual input for the calculation.
        /// </summary>
        public bool UseAssessmentLevelManualInput
        {
            get
            {
                return useAssessmentLevelManualInput;
            }
            set
            {
                useAssessmentLevelManualInput = value;

                if (useAssessmentLevelManualInput)
                {
                    HydraulicBoundaryLocation = null;
                }
            }
        }

        #region Derived input

        /// <summary>
        /// Gets or sets the outside high water level.
        /// [m]
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the user attempts to set the 
        /// assessment level while <see cref="UseAssessmentLevelManualInput"/> is <c>false</c></exception>
        public RoundedDouble AssessmentLevel
        {
            get
            {
                if (!UseAssessmentLevelManualInput)
                {
                    return HydraulicBoundaryLocation?.DesignWaterLevel ?? new RoundedDouble(2, double.NaN);
                }

                return assessmentLevel;
            }
            set
            {
                if (!UseAssessmentLevelManualInput)
                {
                    throw new InvalidOperationException("UseAssessmentLevelManualInput is false");
                }

                assessmentLevel = value.ToPrecision(assessmentLevel.NumberOfDecimalPlaces);
            }
        }

        #endregion
    }
}