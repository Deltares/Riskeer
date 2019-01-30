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
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Common.IO.ReferenceLines
{
    /// <summary>
    /// Interface for an object that can properly update a <see cref="ReferenceLine"/>.
    /// </summary>
    public interface IReferenceLineUpdateHandler
    {
        /// <summary>
        /// Checks to see if the update of the <see cref="ReferenceLine"/> should occur
        /// or not.
        /// </summary>
        /// <returns><c>true</c> if the update should occur, <c>false</c> otherwise.</returns>
        bool ConfirmUpdate();

        /// <summary>
        /// Updates the <paramref name="originalReferenceLine"/> with the <paramref name="newReferenceLine"/>
        /// and its dependent data.
        /// </summary>
        /// <param name="originalReferenceLine">The reference line to be updated.</param>
        /// <param name="newReferenceLine">The new reference line.</param>
        /// <returns>All objects that have been affected by the update.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        IEnumerable<IObservable> Update(ReferenceLine originalReferenceLine, ReferenceLine newReferenceLine);

        /// <summary>
        /// Perform post-update actions.
        /// </summary>
        void DoPostUpdateActions();
    }
}