﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;

namespace Riskeer.Integration.Forms.Merge
{
    /// <summary>
    /// Interface for providing the data to merge.
    /// </summary>
    public interface IAssessmentSectionMergeDataProvider
    {
        /// <summary>
        /// Gets the data to merge.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to get the merge data from.</param>
        /// <returns>The selected merge data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        AssessmentSectionMergeData GetMergeData(AssessmentSection assessmentSection);
    }
}