﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Service.Properties;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service;

namespace Ringtoets.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations for the grass cover erosion outwards failure mechanism.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        /// <summary>
        /// Performs validation over the input parameters. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> for which to validate the values.</param>
        /// <param name="assessmentLevel">The assessment level to use for determining water levels.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database file which to validate.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory to validate.</param>
        /// <param name="targetProbability">The target probability to validate.</param>
        /// <returns><c>true</c> if there were no validation errors; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static bool Validate(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                    RoundedDouble assessmentLevel,
                                    string hydraulicBoundaryDatabaseFilePath,
                                    string preprocessorDirectory,
                                    double targetProbability)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            return ValidateWaveConditionsInput(calculation.InputParameters,
                                               assessmentLevel,
                                               hydraulicBoundaryDatabaseFilePath,
                                               preprocessorDirectory,
                                               targetProbability,
                                               Resources.GrassCoverErosionOutwardsWaveConditionsCalculationService_LogMessage_DesignWaterLevel_name);
        }

        /// <summary>
        /// Performs a wave conditions calculation for the grass cover erosion outwards failure mechanism based on the supplied 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>  and sets 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="failureMechanism">The grass cover erosion outwards failure mechanism, which contains general parameters that apply to all 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> instances.</param>
        /// <param name="hlcdFilePath">The path of the HLCD file that should be used for performing the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>, <paramref name="failureMechanism"/>
        /// or <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="hlcdFilePath"/> contains invalid characters.</item>
        /// <item><paramref name="failureMechanism"/> has no (0) contribution.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hlcdFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the target probability or 
        /// calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        /// <exception cref="HydraRingFileParserException">Thrown when an error occurs during parsing of the Hydra-Ring output.</exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during the calculation.</exception>
        public void Calculate(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                              GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                              IAssessmentSection assessmentSection,
                              string hlcdFilePath)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            CalculationServiceHelper.LogCalculationBegin();

            RoundedDouble a = failureMechanism.GeneralInput.GeneralWaveConditionsInput.A;
            RoundedDouble b = failureMechanism.GeneralInput.GeneralWaveConditionsInput.B;
            RoundedDouble c = failureMechanism.GeneralInput.GeneralWaveConditionsInput.C;

            double norm = failureMechanism.GetNorm(assessmentSection, calculation.InputParameters.CategoryType);
            string preprocessorDirectory = assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory();

            RoundedDouble assessmentLevel = failureMechanism.GetAssessmentLevel(assessmentSection,
                                                                                calculation.InputParameters.HydraulicBoundaryLocation,
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
                                                                                    hlcdFilePath,
                                                                                    preprocessorDirectory);

                if (!Canceled)
                {
                    calculation.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(outputs);
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();
            }
        }
    }
}