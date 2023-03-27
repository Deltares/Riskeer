﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Riskeer.Integration.IO.Handlers
{
    /// <summary>
    /// Interface for an object that can properly update the hydraulic boundary databases within a
    /// <see cref="HydraulicBoundaryData"/> instance.
    /// </summary>
    public interface IHydraulicBoundaryDataUpdateHandler
    {
        /// <summary>
        /// Updates the <paramref name="hydraulicBoundaryData"/> and its dependent data with the
        /// <paramref name="readHydraulicBoundaryDatabase"/> and the <paramref name="readHydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The hydraulic boundary data to update.</param>
        /// <param name="readHydraulicBoundaryDatabase">The read hydraulic boundary database to update with.</param>
        /// <param name="readHydraulicLocationConfigurationDatabase">The read hydraulic location configuration database to
        /// update with.</param>
        /// <param name="excludedLocationIds">The location ids that should be excluded.</param>
        /// <param name="hrdFilePath">The file path of the hydraulic boundary database.</param>
        /// <param name="hlcdFilePath">The file path of the hydraulic location configuration database.</param>
        /// <returns>All objects that have been affected by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryData"/> cannot be updated with
        /// <paramref name="readHydraulicLocationConfigurationDatabase"/>.</exception>
        IEnumerable<IObservable> Update(HydraulicBoundaryData hydraulicBoundaryData,
                                        ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                        ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                        IEnumerable<long> excludedLocationIds,
                                        string hrdFilePath,
                                        string hlcdFilePath);

        /// <summary>
        /// Perform post-update actions.
        /// </summary>
        void DoPostUpdateActions();
    }
}