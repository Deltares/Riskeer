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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Service;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Service;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.HydraRing.Calculation.Exceptions;

namespace Riskeer.WaveImpactAsphaltCover.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations for the wave impact on asphalt failure mechanism.
    /// </summary>
    public class WaveImpactAsphaltCoverWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        /// <summary>
        /// Performs a wave conditions calculation for the wave impact on asphalt failure mechanism based on the supplied 
        /// <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/>  and sets 
        /// <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="generalWaveConditionsInput">Calculation input parameters that apply to all <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/> instances.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>, <paramref name="assessmentSection"/>
        /// or <paramref name="generalWaveConditionsInput"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of hydraulic boundary database file path
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the target probability or 
        /// calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        /// <exception cref="HydraRingFileParserException">Thrown when an error occurs during parsing of the Hydra-Ring output.</exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during the calculation.</exception>
        public void Calculate(WaveImpactAsphaltCoverWaveConditionsCalculation calculation,
                              IAssessmentSection assessmentSection,
                              GeneralWaveConditionsInput generalWaveConditionsInput)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (generalWaveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(generalWaveConditionsInput));
            }

            CalculationServiceHelper.LogCalculationBegin();

            RoundedDouble a = generalWaveConditionsInput.A;
            RoundedDouble b = generalWaveConditionsInput.B;
            RoundedDouble c = generalWaveConditionsInput.C;

            double norm = assessmentSection.GetNorm(calculation.InputParameters.CategoryType);
            RoundedDouble assessmentLevel = assessmentSection.GetAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                 calculation.InputParameters.CategoryType);

            TotalWaterLevelCalculations = calculation.InputParameters.GetWaterLevels(assessmentLevel).Count();

            try
            {
                IEnumerable<WaveConditionsOutput> outputs = CalculateWaveConditions(calculation.InputParameters,
                                                                                    assessmentLevel,
                                                                                    a,
                                                                                    b,
                                                                                    c,
                                                                                    norm,
                                                                                    assessmentSection.HydraulicBoundaryDatabase);

                if (!Canceled)
                {
                    calculation.Output = new WaveImpactAsphaltCoverWaveConditionsOutput(outputs);
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();
            }
        }
    }
}