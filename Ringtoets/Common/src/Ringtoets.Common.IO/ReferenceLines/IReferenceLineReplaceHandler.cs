﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.IO.ReferenceLines
{
    /// <summary>
    /// Interface for an object that can properly replace a <see cref="ReferenceLine"/>
    /// from an <see cref="IAssessmentSection"/>.
    /// </summary>
    public interface IReferenceLineReplaceHandler
    {
        /// <summary>
        /// Checks to see if the replacement of the <see cref="ReferenceLine"/> should occur
        /// or not.
        /// </summary>
        /// <returns><c>true</c> if the replacement should occur, <c>false</c> otherwise.</returns>
        bool ConfirmReplace();

        /// <summary>
        /// Replaces the <see cref="ReferenceLine"/> of a <see cref="IAssessmentSection"/>
        /// and propagates the changes to underlying data structure.
        /// </summary>
        /// <param name="section">The section to be updated.</param>
        /// <param name="newReferenceLine">The new reference line.</param>
        /// <returns>All objects that have been affected by the replacement.</returns>
        IEnumerable<IObservable> Replace(IAssessmentSection section, ReferenceLine newReferenceLine);

        /// <summary>
        /// Perform post-replacement updates.
        /// </summary>
        void DoPostReplacementUpdates();
    }
}