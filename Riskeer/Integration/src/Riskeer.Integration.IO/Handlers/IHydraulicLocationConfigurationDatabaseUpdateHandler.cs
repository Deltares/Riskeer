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
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Riskeer.Integration.IO.Handlers
{
    /// <summary>
    /// Interface for an object that can properly update <see cref="HydraulicLocationConfigurationDatabase"/> instances.
    /// </summary>
    public interface IHydraulicLocationConfigurationDatabaseUpdateHandler
    {
        /// <summary>
        /// Gets confirmation for updating the <see cref="HydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <returns><c>true</c> when confirmation is given; <c>false</c> otherwise.</returns>
        bool InquireConfirmation();

        /// <summary>
        /// Updates the <see cref="HydraulicBoundaryData.HydraulicLocationConfigurationDatabase"/> and its dependent data with the
        /// <paramref name="readHydraulicLocationConfigurationDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The hydraulic boundary data to update the hydraulic location configuration
        /// database for.</param>
        /// <param name="readHydraulicLocationConfigurationDatabase">The read hydraulic location configuration database to
        /// update with.</param>
        /// <param name="hlcdFilePath">The file path of the hydraulic location configuration database.</param>
        /// <returns>All objects that have been affected by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        IEnumerable<IObservable> Update(HydraulicBoundaryData hydraulicBoundaryData,
                                        ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                        string hlcdFilePath);
    }
}