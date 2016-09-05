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

using System.IO;
using log4net;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.Properties;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for marginal wave statistics.
    /// </summary>
    public class WaveHeightCalculationService : IWaveHeightCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveHeightCalculationService));
        private static IWaveHeightCalculationService instance;

        /// <summary>
        /// Gets or sets an instance of <see cref="IWaveHeightCalculationService"/>.
        /// </summary>
        public static IWaveHeightCalculationService Instance
        {
            get
            {
                return instance ?? (instance = new WaveHeightCalculationService());
            }
            set
            {
                instance = value;
            }
        }

        public bool Validate(string name, string hydraulicBoundaryDatabaseFilePath)
        {
            CalculationServiceHelper.LogValidationBeginTime(name);

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabaseFilePath);
            var isValid = string.IsNullOrEmpty(validationProblem);

            if (!isValid)
            {
                CalculationServiceHelper.LogMessagesAsError(Resources.Hydraulic_boundary_database_connection_failed_0_,
                                                            validationProblem);
            }

            CalculationServiceHelper.LogValidationEndTime(name);

            return isValid;
        }

        public ReliabilityIndexCalculationOutput Calculate(ICalculationMessageProvider messageProvider,
                                                           IHydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                           string hydraulicBoundaryDatabaseFilePath,
                                                           string ringId, double norm)
        {
            var hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            var input = CreateInput(hydraulicBoundaryLocation, norm);
            var targetProbabilityCalculationParser = new ReliabilityIndexCalculationParser();
            var calculationName = messageProvider.GetCalculationName(hydraulicBoundaryLocation.Name);

            CalculationServiceHelper.PerformCalculation(
                calculationName,
                () =>
                {
                    HydraRingCalculationService.Instance.PerformCalculation(
                        hlcdDirectory,
                        ringId,
                        HydraRingUncertaintiesType.All,
                        input,
                        new[]
                        {
                            targetProbabilityCalculationParser
                        });

                    VerifyOutput(targetProbabilityCalculationParser.Output, messageProvider, hydraulicBoundaryLocation.Name);
                });

            return targetProbabilityCalculationParser.Output;
        }

        private static void VerifyOutput(ReliabilityIndexCalculationOutput output, ICalculationMessageProvider messageProvider, string locationName)
        {
            if (output == null)
            {
                log.Error(messageProvider.GetCalculationFailedMessage(locationName));
            }
        }

        private static WaveHeightCalculationInput CreateInput(IHydraulicBoundaryLocation hydraulicBoundaryLocation, double norm)
        {
            return new WaveHeightCalculationInput(1, hydraulicBoundaryLocation.Id, norm);
        }
    }
}