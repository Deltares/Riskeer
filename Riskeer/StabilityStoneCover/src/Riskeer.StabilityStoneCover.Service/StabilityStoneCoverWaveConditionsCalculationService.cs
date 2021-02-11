﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Service;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Service;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Service.Properties;
using RevetmentServiceResources = Riskeer.Revetment.Service.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring wave conditions calculations for the stability of stone revetment failure mechanism.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationService : WaveConditionsCalculationServiceBase
    {
        private readonly ILog log = LogManager.GetLogger(typeof(StabilityStoneCoverWaveConditionsCalculationService));

        /// <summary>
        /// Performs a wave conditions calculation for the stability of stone revetment failure mechanism based on the supplied 
        /// <see cref="StabilityStoneCoverWaveConditionsCalculation"/>  and sets 
        /// <see cref="StabilityStoneCoverWaveConditionsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StabilityStoneCoverWaveConditionsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="generalWaveConditionsInput">Calculation input parameters that apply to all <see cref="StabilityStoneCoverWaveConditionsCalculation"/> instances.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>, <paramref name="assessmentSection"/>
        /// or <paramref name="generalWaveConditionsInput"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path contains invalid characters.</exception>
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
        public void Calculate(StabilityStoneCoverWaveConditionsCalculation calculation,
                              IAssessmentSection assessmentSection,
                              GeneralStabilityStoneCoverWaveConditionsInput generalWaveConditionsInput)
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

            StabilityStoneCoverWaveConditionsInput calculationInput = calculation.InputParameters;
            StabilityStoneCoverWaveConditionsCalculationType calculationType = calculationInput.CalculationType;
            if (!Enum.IsDefined(typeof(StabilityStoneCoverWaveConditionsCalculationType), calculationType))
            {
                throw new InvalidEnumArgumentException(nameof(calculationType), (int) calculationType,
                                                       typeof(StabilityStoneCoverWaveConditionsCalculationType));
            }

            CalculationServiceHelper.LogCalculationBegin();

            double norm = assessmentSection.GetNorm(calculationInput.CategoryType);

            RoundedDouble assessmentLevel = assessmentSection.GetAssessmentLevel(calculationInput.HydraulicBoundaryLocation,
                                                                                 calculationInput.CategoryType);

            int waterLevelCount = calculationInput.GetWaterLevels(assessmentLevel).Count();
            TotalWaterLevelCalculations = calculationType == StabilityStoneCoverWaveConditionsCalculationType.Both
                                              ? waterLevelCount * 2
                                              : waterLevelCount;

            try
            {
                IEnumerable<WaveConditionsOutput> blocksOutputs = null;
                if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Both
                    || calculationType == StabilityStoneCoverWaveConditionsCalculationType.Blocks)
                {
                    CurrentCalculationType = Resources.StabilityStoneCoverWaveConditions_Blocks_DisplayName;
                    blocksOutputs = CalculateBlocks(calculation, assessmentSection, assessmentLevel,
                                                    generalWaveConditionsInput.GeneralBlocksWaveConditionsInput, norm);
                }

                if (Canceled)
                {
                    return;
                }

                IEnumerable<WaveConditionsOutput> columnsOutputs = null;
                if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Both
                    || calculationType == StabilityStoneCoverWaveConditionsCalculationType.Columns)
                {
                    CurrentCalculationType = Resources.StabilityStoneCoverWaveConditions_Columns_DisplayName;
                    columnsOutputs = CalculateColumns(calculation, assessmentSection, assessmentLevel,
                                                      generalWaveConditionsInput.GeneralColumnsWaveConditionsInput, norm);
                }

                if (!Canceled)
                {
                    calculation.Output = CreateOutput(calculationType, blocksOutputs, columnsOutputs);
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();
            }
        }

        private static StabilityStoneCoverWaveConditionsOutput CreateOutput(StabilityStoneCoverWaveConditionsCalculationType calculationType,
                                                                            IEnumerable<WaveConditionsOutput> blocksOutputs,
                                                                            IEnumerable<WaveConditionsOutput> columnsOutputs)
        {
            if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Blocks)
            {
                return StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithBlocks(blocksOutputs);
            }

            if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Columns)
            {
                return StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithColumns(columnsOutputs);
            }

            return StabilityStoneCoverWaveConditionsOutputFactory.CreateOutputWithColumnsAndBlocks(columnsOutputs, blocksOutputs);
        }

        private IEnumerable<WaveConditionsOutput> CalculateColumns(StabilityStoneCoverWaveConditionsCalculation calculation,
                                                                   IAssessmentSection assessmentSection, RoundedDouble assessmentLevel,
                                                                   GeneralWaveConditionsInput generalInput, double norm)
        {
            return Calculate(calculation, assessmentSection, assessmentLevel, generalInput, norm,
                             Resources.StabilityStoneCoverWaveConditions_Columns_DisplayName);
        }

        private IEnumerable<WaveConditionsOutput> CalculateBlocks(StabilityStoneCoverWaveConditionsCalculation calculation,
                                                                  IAssessmentSection assessmentSection, RoundedDouble assessmentLevel,
                                                                  GeneralWaveConditionsInput generalInput, double norm)
        {
            return Calculate(calculation, assessmentSection, assessmentLevel, generalInput, norm,
                             Resources.StabilityStoneCoverWaveConditions_Blocks_DisplayName);
        }

        private IEnumerable<WaveConditionsOutput> Calculate(StabilityStoneCoverWaveConditionsCalculation calculation,
                                                            IAssessmentSection assessmentSection, RoundedDouble assessmentLevel,
                                                            GeneralWaveConditionsInput generalInput, double norm,
                                                            string calculationType)
        {
            log.InfoFormat(RevetmentServiceResources.WaveConditionsCalculationService_Calculate_calculationType_0_started, calculationType);
            IEnumerable<WaveConditionsOutput> outputs = CalculateWaveConditions(calculation.InputParameters,
                                                                                assessmentLevel, generalInput.A,
                                                                                generalInput.B, generalInput.C, norm,
                                                                                assessmentSection.HydraulicBoundaryDatabase);
            log.InfoFormat(RevetmentServiceResources.WaveConditionsCalculationService_Calculate_calculationType_0_ended, calculationType);
            return outputs;
        }
    }
}