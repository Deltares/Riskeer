// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Integration.Forms.Merge
{
    /// <summary>
    /// Row representing the information of a <see cref="IFailurePath"/> to be
    /// used for merging.
    /// </summary>
    internal class FailurePathMergeDataRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailurePathMergeDataRow"/>.
        /// </summary>
        /// <param name="failurePath">The wrapped <see cref="IFailureMechanism"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failurePath"/>
        /// is <c>null</c>.</exception>
        public FailurePathMergeDataRow(IFailurePath failurePath)
        {
            if (failurePath == null)
            {
                throw new ArgumentNullException(nameof(failurePath));
            }

            FailurePath = failurePath;
        }

        /// <summary>
        /// Gets the wrapped failure path of the row.
        /// </summary>
        public IFailurePath FailurePath { get; }

        /// <summary>
        /// Gets and sets whether the failure mechanism is selected to be merged.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets the name of the failure mechanism.
        /// </summary>
        public string Name
        {
            get
            {
                return FailurePath.Name;
            }
        }

        /// <summary>
        /// Gets indicator whether the failure mechanism is marked relevant.
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return FailurePath.IsRelevant;
            }
        }

        /// <summary>
        /// Gets indicator whether the failure mechanism has sections.
        /// </summary>
        public bool HasSections
        {
            get
            {
                return FailurePath.Sections.Any();
            }
        }

        /// <summary>
        /// Gets the amount of calculations that are contained by the failure mechanism.
        /// </summary>
        public virtual int NumberOfCalculations
        {
            get
            {
                return 0;
            }
        }
    }
}