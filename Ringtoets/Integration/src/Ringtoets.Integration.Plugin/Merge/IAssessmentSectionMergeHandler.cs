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
using Ringtoets.Integration.Data.Merge;

namespace Ringtoets.Integration.Plugin.Merge
{
    /// <summary>
    /// Interface for handling the merge of <see cref="AssessmentSection"/> data.
    /// </summary>
    public interface IAssessmentSectionMergeHandler
    {
        /// <summary>
        /// Performs the merge.
        /// </summary>
        /// <param name="targetAssessmentSection">The assessment section to merge to.</param>
        /// <param name="mergeData">The data to merge.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        void PerformMerge(AssessmentSection targetAssessmentSection, AssessmentSectionMergeData mergeData);
    }
}