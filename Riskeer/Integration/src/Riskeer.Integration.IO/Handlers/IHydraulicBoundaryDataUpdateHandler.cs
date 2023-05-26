// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
        /// Adds the <paramref name="readHydraulicBoundaryDatabase"/> and the <paramref name="readHydraulicLocationConfigurationDatabase"/>
        /// to <see cref="HydraulicBoundaryData"/> and its dependent data.
        /// </summary>
        /// <param name="readHydraulicBoundaryDatabase">The read hydraulic boundary database to update with.</param>
        /// <param name="readHydraulicLocationConfigurationDatabase">The read hydraulic location configuration database to
        /// update with.</param>
        /// <param name="excludedLocationIds">The location ids that should be excluded.</param>
        /// <param name="hrdFilePath">The file path of the hydraulic boundary database.</param>
        /// <returns>All objects that have been affected by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        IEnumerable<IObservable> AddHydraulicBoundaryDatabase(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                                              ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                                              IEnumerable<long> excludedLocationIds,
                                                              string hrdFilePath);

        /// <summary>
        /// Adds the <paramref name="hydraulicBoundaryDatabase"/> to <see cref="HydraulicBoundaryData"/> and its dependent data.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to add.</param>
        /// <returns>All objects that have been affected by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        IEnumerable<IObservable> AddHydraulicBoundaryDatabase(HydraulicBoundaryDatabase hydraulicBoundaryDatabase);

        /// <summary>
        /// Removes the hydraulic boundary database.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to remove.</param>
        /// <returns>All objects that have been affected by the remove.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabase"/>
        /// is <c>null</c>.</exception>
        IEnumerable<IObservable> RemoveHydraulicBoundaryDatabase(HydraulicBoundaryDatabase hydraulicBoundaryDatabase);

        /// <summary>
        /// Perform post-update actions.
        /// </summary>
        void DoPostUpdateActions();
    }
}