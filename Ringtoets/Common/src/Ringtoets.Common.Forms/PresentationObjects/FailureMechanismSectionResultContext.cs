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

using System;
using System.Collections.Generic;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for a collection of <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    public class FailureMechanismSectionResultContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionResultContext"/>.
        /// </summary>
        /// <param name="sectionResults">The <see cref="IEnumerable{T}"/> of <see cref="FailureMechanismSectionResult"/> to wrap.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> <paramref name="sectionResults"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResults"/> or <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public FailureMechanismSectionResultContext(IEnumerable<FailureMechanismSectionResult> sectionResults, IFailureMechanism failureMechanism)
        {
            if (sectionResults == null)
            {
                throw new ArgumentNullException("sectionResults");
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            SectionResults = sectionResults;
            FailureMechanism = failureMechanism;
        }

        /// <summary>
        /// Gets the wrapped <see cref="IEnumerable{T}"/> of <see cref="FailureMechanismSectionResult"/>.
        /// </summary>
        public IEnumerable<FailureMechanismSectionResult> SectionResults { get; private set; }

        /// <summary>
        /// Gets the <see cref="IFailureMechanism"/>.
        /// </summary>
        public IFailureMechanism FailureMechanism { get; private set; }
    }
}