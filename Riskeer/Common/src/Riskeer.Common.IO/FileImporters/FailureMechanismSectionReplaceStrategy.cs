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
using Core.Common.Base;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.IO.FileImporters
{
    /// <summary>
    /// An <see cref="IFailureMechanismSectionUpdateStrategy"/> that can be used to replace failure mechanism sections with
    /// imported failure mechanism sections.
    /// </summary>
    public class FailureMechanismSectionReplaceStrategy : IFailureMechanismSectionUpdateStrategy
    {
        protected readonly IFailureMechanism<FailureMechanismSectionResult> FailureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionReplaceStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to set the sections to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>
        /// is <c>null</c>.</exception>
        public FailureMechanismSectionReplaceStrategy(IFailureMechanism<FailureMechanismSectionResult> failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            FailureMechanism = failureMechanism;
        }

        public virtual IEnumerable<IObservable> UpdateSectionsWithImportedData(IEnumerable<FailureMechanismSection> importedFailureMechanismSections,
                                                                               string sourcePath)
        {
            if (importedFailureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(importedFailureMechanismSections));
            }

            if (sourcePath == null)
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            var affectedObjects = new List<IObservable>();

            try
            {
                FailureMechanism.SetSections(importedFailureMechanismSections, sourcePath);
                affectedObjects.Add(FailureMechanism);
                affectedObjects.Add(FailureMechanism.SectionResults);
            }
            catch (ArgumentException e)
            {
                throw new UpdateDataException(e.Message, e);
            }

            return affectedObjects;
        }

        public virtual IEnumerable<IObservable> DoPostUpdateActions()
        {
            // Do nothing
            yield break;
        }
    }
}