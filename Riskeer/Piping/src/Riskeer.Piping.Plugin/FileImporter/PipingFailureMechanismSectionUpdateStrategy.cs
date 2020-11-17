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
using Riskeer.Common.IO.FileImporters;
using Riskeer.Piping.Data;

namespace Riskeer.Piping.Plugin.FileImporter
{
    /// <summary>
    /// An <see cref="IFailureMechanismSectionUpdateStrategy"/> that can be used to update
    /// piping failure mechanism sections with imported failure mechanism sections.
    /// </summary>
    public class PipingFailureMechanismSectionUpdateStrategy : FailureMechanismSectionUpdateStrategy<PipingFailureMechanismSectionResult>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionUpdateStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> to update the sections for.</param>
        /// <param name="sectionResultUpdateStrategy">The <see cref="PipingFailureMechanismSectionResultUpdateStrategy"/>
        /// to use when updating the section results.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismSectionUpdateStrategy(
            PipingFailureMechanism failureMechanism,
            PipingFailureMechanismSectionResultUpdateStrategy sectionResultUpdateStrategy)
            : base(failureMechanism, sectionResultUpdateStrategy) {}
    }
}