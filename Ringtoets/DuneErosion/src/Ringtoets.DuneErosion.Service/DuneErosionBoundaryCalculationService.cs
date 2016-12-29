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
using System.IO;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Service;
using Ringtoets.DuneErosion.Data;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;

namespace Ringtoets.DuneErosion.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for dune erosion.
    /// </summary>
    public class DuneErosionBoundaryCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DuneErosionBoundaryCalculationService));

        private bool canceled;
        private IDunesBoundaryConditionsCalculator calculator;

        public void Calculate(DuneLocation duneLocation,
                              DuneErosionFailureMechanism failureMechanism,
                              IAssessmentSection assessmentSection,
                              string hydraulicBoundaryDatabaseFilePath)
        {
            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateDunesBoundaryConditionsCalculator(hlcdDirectory, assessmentSection.Id);

            string calculationName = duneLocation.Name;

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            var exceptionThrown = false;
            try
            {
                DunesBoundaryConditionsCalculationInput calculationInput = CreateInput(duneLocation, failureMechanism, assessmentSection, hydraulicBoundaryDatabaseFilePath);
                calculator.Calculate(calculationInput);

                if (string.IsNullOrEmpty(calculator.LastErrorFileContent))
                {
                    
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEndTime(calculationName);
            }
        }

        /// <summary>
        /// Cancels any ongoing structures stability point calculation.
        /// </summary>
        public void Cancel()
        {
            if (calculator != null)
            {
                calculator.Cancel();
            }

            canceled = true;
        }

        private static DunesBoundaryConditionsCalculationInput CreateInput(DuneLocation duneLocation,
                                                                           DuneErosionFailureMechanism failureMechanism,
                                                                           IAssessmentSection assessmentSection,
                                                                           string hydraulicBoundaryDatabaseFilePath)
        {
            double mechanismSpecificNorm;

            try
            {
                mechanismSpecificNorm = failureMechanism.GetMechanismSpecificNorm(assessmentSection);
            }
            catch (ArgumentException e)
            {
                log.Error(e.Message);
                throw;
            }

            var dunesBoundaryConditionsCalculationInput = new DunesBoundaryConditionsCalculationInput(1, duneLocation.Id, mechanismSpecificNorm, duneLocation.Orientation);
            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(dunesBoundaryConditionsCalculationInput, hydraulicBoundaryDatabaseFilePath);
            return dunesBoundaryConditionsCalculationInput;
        }
    }
}