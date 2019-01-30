// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
    /// Interface for an object that can properly update <see cref="HydraulicLocationConfigurationSettings"/>.
    /// </summary>
    public interface IHydraulicLocationConfigurationDatabaseUpdateHandler
    {
        /// <summary>
        /// Gets confirmation for updating the <see cref="HydraulicLocationConfigurationSettings"/>.
        /// </summary>
        /// <returns><c>true</c> when confirmation is given; <c>false</c> otherwise.</returns>
        bool InquireConfirmation();

        /// <summary>
        /// Updates the <see cref="HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings"/> and its dependent data with the
        /// <paramref name="readHydraulicLocationConfigurationDatabaseSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to update the settings for.</param>
        /// <param name="readHydraulicLocationConfigurationDatabaseSettings">The read hydraulic location
        /// configuration database settings to update with.</param>
        /// <param name="hlcdFilePath">The file path of the hlcd.</param>
        /// <returns>All objects that have been affected by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabase"/>
        /// or <paramref name="hlcdFilePath"/> is <c>null</c>.</exception>
        IEnumerable<IObservable> Update(HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                        ReadHydraulicLocationConfigurationDatabaseSettings readHydraulicLocationConfigurationDatabaseSettings,
                                        string hlcdFilePath);
    }
}