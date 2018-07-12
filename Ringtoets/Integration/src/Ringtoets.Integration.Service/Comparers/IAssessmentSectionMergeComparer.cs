// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Service.Comparers
{
    /// <summary>
    /// Specifies the interface for defining classes that can be used to compare assessment sections
    /// which can then be used for merging operations.
    /// </summary>
    public interface IAssessmentSectionMergeComparer
    {
        /// <summary>
        /// Compares <see cref="AssessmentSection"/> and determines whether they are equal and thus
        /// suitable for merge operations.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to compare against.</param>
        /// <param name="otherAssessmentSection">The <see cref="AssessmentSection"/> to compare.</param>
        /// <returns><c>true</c> when <paramref name="assessmentSection"/> is equal to
        /// <paramref name="otherAssessmentSection"/> and suitable to merge, <c>false</c> if otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        bool Compare(AssessmentSection assessmentSection, AssessmentSection otherAssessmentSection);
    }
}