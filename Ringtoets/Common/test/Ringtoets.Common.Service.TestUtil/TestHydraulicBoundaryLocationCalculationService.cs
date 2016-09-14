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

using Core.Common.Utils;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service.TestUtil
{
    /// <summary>
    /// This class allows mocking an actual design water level calculation, 
    /// so that tests can be performed upon them.
    /// </summary>
    public class TestHydraulicBoundaryLocationCalculationService : IHydraulicBoundaryLocationCalculationService
    {
        private CalculationConvergence returnValidOutput = CalculationConvergence.CalculatedConverged;

        /// <summary>
        /// Set the expected output for <see cref="Calculate"/>.
        /// </summary>
        public CalculationConvergence CalculationConvergenceOutput
        {
            set
            {
                returnValidOutput = value;
            }
        }

        /// <summary>
        /// Gets the used <see cref="ICalculationMessageProvider"/>.
        /// </summary>
        public ICalculationMessageProvider MessageProvider { get; private set; }

        /// <summary>
        /// Gets the used <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; private set; }

        /// <summary>
        /// Gets the used hydraulic boundary database file path.
        /// </summary>
        public string HydraulicBoundaryDatabaseFilePath { get; private set; }

        /// <summary>
        /// Gets the used ring id.
        /// </summary>
        public string RingId { get; private set; }

        /// <summary>
        /// Gets the used norm.
        /// </summary>
        public double Norm { get; private set; }

        public bool Validate(string name, string hydraulicBoundaryDatabaseFilePath)
        {
            return true;
        }

        public ReliabilityIndexCalculationOutput Calculate(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                           string hydraulicBoundaryDatabaseFilePath,
                                                           string ringId,
                                                           double norm,
                                                           ICalculationMessageProvider messageProvider)
        {
            MessageProvider = messageProvider;
            HydraulicBoundaryLocation = hydraulicBoundaryLocation;
            HydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            RingId = ringId;
            Norm = norm;

            switch (returnValidOutput)
            {
                case CalculationConvergence.NotCalculated:
                    return null;
                case CalculationConvergence.CalculatedNotConverged:
                    return new ReliabilityIndexCalculationOutput(norm, norm);
                default:
                    return new ReliabilityIndexCalculationOutput(norm, StatisticsConverter.NormToBeta(norm));
            }
        }
    }
}