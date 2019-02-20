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
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Service.Properties;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Service;
using RevetmentServiceResources = Riskeer.Revetment.Service.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations for the grass cover erosion outwards failure mechanism.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        private readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionOutwardsWaveConditionsCalculationService));

        /// <summary>
        /// Performs a wave conditions calculation for the grass cover erosion outwards failure mechanism based on the supplied 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>  and sets 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="failureMechanism">The grass cover erosion outwards failure mechanism, which contains general parameters that apply to all 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> instances.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>, <paramref name="failureMechanism"/>
        /// or <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>the hydraulic boundary database file path contains invalid characters.</item>
        /// <item><paramref name="failureMechanism"/> has no (0) contribution.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database file path
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
                              IAssessmentSection assessmentSection)
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

            GrassCoverErosionOutwardsWaveConditionsInput calculationInput = calculation.InputParameters;
            RoundedDouble assessmentLevel = failureMechanism.GetAssessmentLevel(assessmentSection,
                                                                                calculationInput.HydraulicBoundaryLocation,
                                                                                calculationInput.CategoryType);
            double norm = failureMechanism.GetNorm(assessmentSection, calculation.InputParameters.CategoryType);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType = calculationInput.CalculationType;
            int waterLevelCount = calculationInput.GetWaterLevels(assessmentLevel).Count();
            TotalWaterLevelCalculations = calculationInput.CalculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.Both
                                              ? waterLevelCount * 2
                                              : waterLevelCount;

            try
            {
                IEnumerable<WaveConditionsOutput> waveRunUpOutput = null;
                if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.Both
                    || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp)
                {
                    CurrentCalculationType = Resources.GrassCoverErosionOutwardsWaveConditions_WaveRunUp_DisplayName;
                    waveRunUpOutput = CalculateWaveRunUp(calculation, failureMechanism, hydraulicBoundaryDatabase, assessmentLevel, norm);
                }

                if (Canceled)
                {
                    return;
                }

                IEnumerable<WaveConditionsOutput> waveImpactOutput = null;
                if (calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.Both
                    || calculationType == GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact)
                {
                    CurrentCalculationType = Resources.GrassCoverErosionOutwardsWaveConditions_WaveImpact_DisplayName;
                    waveImpactOutput = CalculateWaveImpact(calculation, failureMechanism, hydraulicBoundaryDatabase, assessmentLevel, norm);
                }

                if (!Canceled)
                {
                    calculation.Output = CreateOutput(calculationType, waveRunUpOutput, waveImpactOutput);
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();
            }
        }

        private static GrassCoverErosionOutwardsWaveConditionsOutput CreateOutput(GrassCoverErosionOutwardsWaveConditionsCalculationType calculationType,
                                                                                  IEnumerable<WaveConditionsOutput> waveRunUpOutput,
                                                                                  IEnumerable<WaveConditionsOutput> waveImpactOutput)
        {
            switch (calculationType)
            {
                case GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp:
                    return GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUp(waveRunUpOutput);
                case GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact:
                    return GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveImpact(waveImpactOutput);
                default:
                    return GrassCoverErosionOutwardsWaveConditionsOutputFactory.CreateOutputWithWaveRunUpAndWaveImpact(waveRunUpOutput, waveImpactOutput);
            }
        }

        private IEnumerable<WaveConditionsOutput> CalculateWaveRunUp(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                     GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                     HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                                     RoundedDouble assessmentLevel,
                                                                     double norm)
        {
            return Calculate(calculation, hydraulicBoundaryDatabase, norm, assessmentLevel,
                             failureMechanism.GeneralInput.GeneralWaveRunUpWaveConditionsInput,
                             Resources.GrassCoverErosionOutwardsWaveConditions_WaveRunUp_DisplayName);
        }

        private IEnumerable<WaveConditionsOutput> CalculateWaveImpact(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                      GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                      HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                                      RoundedDouble assessmentLevel,
                                                                      double norm)
        {
            return Calculate(calculation, hydraulicBoundaryDatabase, norm, assessmentLevel,
                             failureMechanism.GeneralInput.GeneralWaveImpactWaveConditionsInput,
                             Resources.GrassCoverErosionOutwardsWaveConditions_WaveImpact_DisplayName);
        }

        private IEnumerable<WaveConditionsOutput> Calculate(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                            HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                            double norm,
                                                            RoundedDouble assessmentLevel,
                                                            GeneralWaveConditionsInput generalInput,
                                                            string calculationType)
        {
            log.InfoFormat(RevetmentServiceResources.WaveConditionsCalculationService_Calculate_calculationType_0_started, calculationType);

            IEnumerable<WaveConditionsOutput> outputs = CalculateWaveConditions(calculation.InputParameters,
                                                                                assessmentLevel,
                                                                                generalInput.A, generalInput.B, generalInput.C, norm,
                                                                                hydraulicBoundaryDatabase);
            log.InfoFormat(RevetmentServiceResources.WaveConditionsCalculationService_Calculate_calculationType_0_ended, calculationType);
            return outputs;
        }
    }
}