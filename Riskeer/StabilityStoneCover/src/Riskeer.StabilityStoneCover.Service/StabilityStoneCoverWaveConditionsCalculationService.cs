// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Service;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Service;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Service.Properties;
using Riskeer.HydraRing.Calculation.Exceptions;

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

            CalculationServiceHelper.LogCalculationBegin();

            RoundedDouble aBlocks = generalWaveConditionsInput.GeneralBlocksWaveConditionsInput.A;
            RoundedDouble bBlocks = generalWaveConditionsInput.GeneralBlocksWaveConditionsInput.B;
            RoundedDouble cBlocks = generalWaveConditionsInput.GeneralBlocksWaveConditionsInput.C;

            RoundedDouble aColumns = generalWaveConditionsInput.GeneralColumnsWaveConditionsInput.A;
            RoundedDouble bColumns = generalWaveConditionsInput.GeneralColumnsWaveConditionsInput.B;
            RoundedDouble cColumns = generalWaveConditionsInput.GeneralColumnsWaveConditionsInput.C;

            double norm = assessmentSection.GetNorm(calculation.InputParameters.CategoryType);

            RoundedDouble assessmentLevel = assessmentSection.GetAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                 calculation.InputParameters.CategoryType);

            TotalWaterLevelCalculations = calculation.InputParameters.GetWaterLevels(assessmentLevel).Count() * 2;

            try
            {
                log.InfoFormat(Resources.StabilityStoneCoverWaveConditionsCalculationService_Calculate_Calculation_for_blocks_started);
                IEnumerable<WaveConditionsOutput> blocksOutputs = CalculateWaveConditions(calculation.InputParameters,
                                                                                          assessmentLevel,
                                                                                          aBlocks,
                                                                                          bBlocks,
                                                                                          cBlocks,
                                                                                          norm,
                                                                                          assessmentSection.HydraulicBoundaryDatabase);
                log.InfoFormat(Resources.StabilityStoneCoverWaveConditionsCalculationService_Calculate_Calculation_for_blocks_finished);

                IEnumerable<WaveConditionsOutput> columnsOutputs = null;
                if (!Canceled)
                {
                    log.InfoFormat(Resources.StabilityStoneCoverWaveConditionsCalculationService_Calculate_Calculation_for_columns_started);
                    columnsOutputs = CalculateWaveConditions(calculation.InputParameters,
                                                             assessmentLevel,
                                                             aColumns,
                                                             bColumns,
                                                             cColumns,
                                                             norm,
                                                             assessmentSection.HydraulicBoundaryDatabase);
                    log.InfoFormat(Resources.StabilityStoneCoverWaveConditionsCalculationService_Calculate_Calculation_for_columns_finished);
                }

                if (!Canceled)
                {
                    calculation.Output = new StabilityStoneCoverWaveConditionsOutput(columnsOutputs, blocksOutputs);
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();
            }
        }
    }
}