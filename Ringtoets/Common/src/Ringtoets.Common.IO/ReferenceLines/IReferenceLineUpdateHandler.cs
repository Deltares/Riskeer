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
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.IO.ReferenceLines
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
        bool ConfirmReplace();

        /// <summary>
        /// Replaces the <see cref="ReferenceLine"/> of a <see cref="IAssessmentSection"/>
        /// and propagates the changes to underlying data structure.
        /// </summary>
        /// <param name="originalReferenceLine">The reference line to be updated.</param>
        /// <param name="newReferenceLine">The new reference line.</param>
        /// <returns>All objects that have been affected by the replacement.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        IEnumerable<IObservable> Replace(ReferenceLine originalReferenceLine, ReferenceLine newReferenceLine);

        /// <summary>
        /// Perform post-replacement updates.
        /// </summary>
        void DoPostReplacementUpdates();
    }
}