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

using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.FileImporters;

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
        /// <param name="readHydraulicBoundaryDatabase">The read hydraulic boundary database.</param>
        /// <returns><c>true</c> when confirmation is required; <c>false</c> otherwise.</returns>
        bool IsConfirmationRequired(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase);

        /// <summary>
        /// Gets confirmation for updating the <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <returns><c>true</c> when confirmation is given; <c>false</c> otherwise.</returns>
        bool InquireConfirmation();

        /// <summary>
        /// Updates the <see cref="HydraulicBoundaryDatabase"/> and its dependent data with the
        /// <paramref name="readHydraulicBoundaryDatabase"/>
        /// </summary>
        /// <param name="readHydraulicBoundaryDatabase">The read data to update with.</param>
        /// <returns>All objects that have been affected by the update.</returns>
        IEnumerable<IObservable> Update(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase);

        /// <summary>
        /// Perform post-update actions.
        /// </summary>
        void DoPostUpdateActions();
    }
}