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

using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Interface that provides methods for performing Hydra-Ring calculations for hydraulic boundary locations.
    /// </summary>
    public interface IHydraulicBoundaryLocationCalculationService
    {
        /// <summary>
        /// Performs validation of the values in the given <paramref name="hydraulicBoundaryDatabaseFilePath"/>. Error information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="name">The name to use in the validation logs.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The HLCD file that should be used for performing the calculation.</param>
        /// <returns><c>False</c> if the connection to <paramref name="hydraulicBoundaryDatabaseFilePath"/> contains validation errors; <c>True</c> otherwise.</returns>
        bool Validate(string name, string hydraulicBoundaryDatabaseFilePath);

        /// <summary>
        /// Performs a design water level calculation based on the supplied <see cref="IHydraulicBoundaryLocation"/> and returns the result
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="IHydraulicBoundaryLocation"/> to perform the calculation for.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="messageProvider">The message provider for the services.</param>
        /// <returns>A <see cref="ReliabilityIndexCalculationOutput"/> on a successful calculation, <c>null</c> otherwise.</returns>
        ReliabilityIndexCalculationOutput Calculate(IHydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                    string hydraulicBoundaryDatabaseFilePath,
                                                    string ringId,
                                                    double norm,
                                                    ICalculationMessageProvider messageProvider);
    }
}