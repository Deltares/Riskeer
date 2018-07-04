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
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path of the hydraulic boundary database file.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="calculations">The calculations to perform.</param>
        /// <param name="norm">The norm to use during the calculations.</param>
        /// <param name="categoryBoundaryName">The category boundary name of the calculations.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        void CalculateDesignWaterLevels(string hydraulicBoundaryDatabaseFilePath,
                                        string preprocessorDirectory,
                                        IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                        double norm,
                                        string categoryBoundaryName);

        /// <summary>
        /// Performs the provided wave height calculations.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path of the hydraulic boundary database file.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <param name="calculations">The calculations to perform.</param>
        /// <param name="norm">The norm to use during the calculations.</param>
        /// <param name="categoryBoundaryName">The category boundary name of the calculations.</param>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="categoryBoundaryName"/> is <c>null</c> or empty.</exception>
        void CalculateWaveHeights(string hydraulicBoundaryDatabaseFilePath,
                                  string preprocessorDirectory,
                                  IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                  double norm,
                                  string categoryBoundaryName);
    }
}