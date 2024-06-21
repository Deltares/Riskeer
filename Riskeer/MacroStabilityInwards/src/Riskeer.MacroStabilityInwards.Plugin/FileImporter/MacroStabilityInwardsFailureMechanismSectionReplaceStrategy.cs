// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.IO.FileImporters;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.Plugin.FileImporter
{
    /// <summary>
    /// An <see cref="FailureMechanismSectionReplaceStrategy"/> that can be used to replace
    /// macro stability inwards failure mechanism sections with imported failure mechanism sections.
    /// </summary>
    public class MacroStabilityInwardsFailureMechanismSectionReplaceStrategy : FailureMechanismSectionReplaceStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsFailureMechanismSectionReplaceStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="MacroStabilityInwardsFailureMechanism"/> to set the sections to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsFailureMechanismSectionReplaceStrategy(MacroStabilityInwardsFailureMechanism failureMechanism)
            : base(failureMechanism) {}

        public override IEnumerable<IObservable> UpdateSectionsWithImportedData(IEnumerable<FailureMechanismSection> importedFailureMechanismSections, string sourcePath)
        {
            List<IObservable> affectedObjects = base.UpdateSectionsWithImportedData(importedFailureMechanismSections, sourcePath).ToList();

            var macroStabilityInwardsFailureMechanism = (MacroStabilityInwardsFailureMechanism) FailureMechanism;
            affectedObjects.Add(macroStabilityInwardsFailureMechanism.SectionConfigurations);

            return affectedObjects;
        }
    }
}