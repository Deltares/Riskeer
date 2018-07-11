﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Forms.Merge
{
    /// <summary>
    /// Interface for providing the data to merge.
    /// </summary>
    public interface IMergeDataProvider
    {
        /// <summary>
        /// Gets the selected <see cref="AssessmentSection"/>.
        /// </summary>
        AssessmentSection SelectedAssessmentSection { get; }

        /// <summary>
        /// Gets the collection of selected <see cref="IFailureMechanism"/>.
        /// </summary>
        IEnumerable<IFailureMechanism> SelectedFailureMechanisms { get; }

        /// <summary>
        /// Select the data to merge.
        /// </summary>
        /// <param name="assessmentSections">The collection of assessment sections to select from.</param>
        /// <returns>Indicator whether selection succeeded.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSections"/>
        /// is <c>null</c>.</exception>
         bool SelectData(IEnumerable<AssessmentSection> assessmentSections);
    }
}