// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Storage;

using Ringtoets.Common.Data.Contribution;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// Base implementation of assessment sections.
    /// </summary>
    public abstract class AssessmentSectionBase : Observable, IStorable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSectionBase"/> class.
        /// </summary>
        protected AssessmentSectionBase()
        {
            Name = "";
        }

        /// <summary>
        /// Gets or sets the name of the assessment section.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the reference line defining the geometry of the dike assessment section.
        /// </summary>
        public ReferenceLine ReferenceLine { get; set; }

        /// <summary>
        /// Gets or sets the contribution of each failure mechanism available in this assessment section.
        /// </summary>
        public FailureMechanismContribution FailureMechanismContribution { get; protected set; }

        /// <summary>
        /// Gets or sets the hydraulic boundary database.
        /// </summary>
        public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }

        /// <summary>
        /// Gets the failure mechanisms corresponding to the assessment section.
        /// </summary>
        public abstract IEnumerable<IFailureMechanism> GetFailureMechanisms();

        /// <summary>
        /// Gets or sets the unique identifier for the storage of the class.
        /// </summary>
        public long StorageId { get; set; }
    }
}