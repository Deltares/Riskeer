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
using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Hydraulics;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Ringtoets.Integration.IO.Handlers
{
    /// <summary>
    /// Interface for an object that can properly update a <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public interface IHydraulicBoundaryDatabaseUpdateHandler
    {
        /// <summary>
        /// Checks whether confirmation is required before updating the <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database.</param>
        /// <param name="readHydraulicBoundaryDatabase">The read hydraulic boundary database.</param>
        /// <returns><c>true</c> when confirmation is required; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        bool IsConfirmationRequired(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase);

        /// <summary>
        /// Gets confirmation for updating the <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <returns><c>true</c> when confirmation is given; <c>false</c> otherwise.</returns>
        bool InquireConfirmation();

        /// <summary>
        /// Updates the <paramref name="hydraulicBoundaryDatabase"/> and its dependent data with the
        /// <paramref name="readHydraulicBoundaryDatabase"/> and the <paramref name="readHydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to update.</param>
        /// <param name="readHydraulicBoundaryDatabase">The read hydraulic boundary database to update with.</param>
        /// <param name="readHydraulicLocationConfigurationDatabase">The read hydraulic location configuration database to update with.</param>
        /// <param name="excludedLocationIds">The location ids that should be excluded.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the read hydraulic boundary database.</param>
        /// <param name="hlcdFilePath">The file path of the hlcd.</param>
        /// <returns>All objects that have been affected by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabase"/>
        /// cannot be updated with <paramref name="readHydraulicLocationConfigurationDatabase"/>.</exception>
        IEnumerable<IObservable> Update(HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                        ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                        ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                        IEnumerable<long> excludedLocationIds,
                                        string hydraulicBoundaryDatabaseFilePath, 
                                        string hlcdFilePath);

        /// <summary>
        /// Perform post-update actions.
        /// </summary>
        void DoPostUpdateActions();
    }
}