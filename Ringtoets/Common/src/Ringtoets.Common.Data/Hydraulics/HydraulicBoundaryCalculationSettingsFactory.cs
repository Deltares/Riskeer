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
using System.IO;

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Factory to create instances of <see cref="HydraulicBoundaryCalculationSettings"/>.
    /// </summary>
    public static class HydraulicBoundaryCalculationSettingsFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/>
        /// based on a <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/>
        /// to create a <see cref="HydraulicBoundaryCalculationSettings"/> for.</param>
        /// <returns>A <see cref="HydraulicBoundaryCalculationSettings"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <see cref="HydraulicBoundaryDatabase.FilePath"/>
        /// is <c>null</c>, is empty or consists of whitespace.</exception>
        public static HydraulicBoundaryCalculationSettings CreateSettings(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            string hydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabase.FilePath;
            string directory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            string hlcdFilePath = Path.Combine(directory, "HLCD.sqlite");
            return new HydraulicBoundaryCalculationSettings(hydraulicBoundaryDatabaseFilePath,
                                                            hlcdFilePath,
                                                            hydraulicBoundaryDatabase.EffectivePreprocessorDirectory());
        }
    }
}