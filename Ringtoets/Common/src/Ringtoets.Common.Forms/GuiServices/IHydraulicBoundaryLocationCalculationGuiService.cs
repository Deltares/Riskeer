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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service.MessageProviders;

namespace Ringtoets.Common.Forms.GuiServices
{
    /// <summary>
    /// Interface for design water level and wave height calculations.
    /// </summary>
    public interface IHydraulicBoundaryLocationCalculationGuiService
    {
        /// <summary>
        /// Performs the provided design water level calculations.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file that should be used for performing the calculation.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="calculations">The calculations to perform.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="messageProvider">The message provider for the services.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> or
        /// <paramref name="messageProvider"/> is <c>null</c>.</exception>
        void CalculateDesignWaterLevels(string hydraulicBoundaryDatabaseFilePath,
                                        string preprocessorDirectory,
                                        IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                        double norm,
                                        ICalculationMessageProvider messageProvider);

        /// <summary>
        /// Performs the provided wave height calculations.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The hydraulic boundary database file that should be used for performing the calculation.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="calculations">The calculations to perform.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="messageProvider">The message provider for the services.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> or
        /// <paramref name="messageProvider"/> is <c>null</c>.</exception>
        void CalculateWaveHeights(string hydraulicBoundaryDatabaseFilePath,
                                  string preprocessorDirectory,
                                  IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                  double norm,
                                  ICalculationMessageProvider messageProvider);
    }
}