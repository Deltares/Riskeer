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
using System.Collections.Generic;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service.MessageProviders;

namespace Ringtoets.Common.Forms.GuiServices
{
    /// <summary>
    /// Interface for <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/> and 
    /// <see cref="HydraulicBoundaryLocation.WaveHeight"/> calculations.
    /// </summary>
    public interface IHydraulicBoundaryLocationCalculationGuiService
    {
        /// <summary>
        /// Performs the <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/> calculation for all <paramref name="locations"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file that should be used for performing the calculation.</param>
        /// <param name="locations">The <see cref="HydraulicBoundaryLocation"/> objects to calculate 
        ///     the <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/> for.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="messageProvider">The message provider for the services.</param>
        /// <returns>True if the observers should be notified; false if otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageProvider"/> 
        /// or <paramref name="locations"/> is <c>null</c>.</exception>
        bool CalculateDesignWaterLevels(string hydraulicBoundaryDatabaseFilePath,
                                        IEnumerable<HydraulicBoundaryLocation> locations,
                                        double norm,
                                        ICalculationMessageProvider messageProvider);

        /// <summary>
        /// Performs the <see cref="HydraulicBoundaryLocation.WaveHeight"/> calculation for all <paramref name="locations"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file that should be used for performing the calculation.</param>
        /// <param name="locations">The <see cref="HydraulicBoundaryLocation"/> objects to calculate 
        ///     the <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/> for.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="messageProvider">The message provider for the services.</param>
        /// <returns>True if the observers should be notified; false if otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="messageProvider"/> 
        /// or <paramref name="locations"/> is <c>null</c>.</exception>
        bool CalculateWaveHeights(string hydraulicBoundaryDatabaseFilePath,
                                  IEnumerable<HydraulicBoundaryLocation> locations,
                                  double norm,
                                  ICalculationMessageProvider messageProvider);
    }
}