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
using Riskeer.Piping.Data;
using Riskeer.Piping.Service;

namespace Riskeer.Piping.Plugin.FileImporter
{
    /// <summary>
    /// A <see cref="FailureMechanismSectionReplaceStrategy"/> that can be used to replace
    /// piping failure mechanism sections with imported failure mechanism sections.
    /// </summary>
    public class PipingFailureMechanismSectionReplaceStrategy : FailureMechanismSectionReplaceStrategy
    {
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionReplaceStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> to set the sections to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public PipingFailureMechanismSectionReplaceStrategy(PipingFailureMechanism failureMechanism)
            : base(failureMechanism)
        {
            this.failureMechanism = failureMechanism;
        }

        public override IEnumerable<IObservable> UpdateSectionsWithImportedData(IEnumerable<FailureMechanismSection> importedFailureMechanismSections, string sourcePath)
        {
            List<IObservable> affectedObjects = base.UpdateSectionsWithImportedData(importedFailureMechanismSections, sourcePath).ToList();
            affectedObjects.Add(failureMechanism.SectionConfigurations);
            return affectedObjects;
        }

        public override IEnumerable<IObservable> DoPostUpdateActions()
        {
            return PipingDataSynchronizationService.ClearAllProbabilisticCalculationOutput(failureMechanism);
        }
    }
}