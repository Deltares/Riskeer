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
using System.Collections.Generic;
using Core.Common.Base;
using Riskeer.Common.Data.AssessmentSection;
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
        private readonly IFailurePath failurePath;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionReplaceStrategy"/>.
        /// </summary>
        /// <param name="failurePath">The <see cref="IFailurePath"/> to set the sections to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failurePath"/>
        /// is <c>null</c>.</exception>
        public FailureMechanismSectionReplaceStrategy(IFailurePath failurePath)
        {
            if (failurePath == null)
            {
                throw new ArgumentNullException(nameof(failurePath));
            }

            this.failurePath = failurePath;
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
                failurePath.SetSections(importedFailureMechanismSections, sourcePath);
                affectedObjects.Add(failurePath);
            }
            catch (ArgumentException e)
            {
                throw new UpdateDataException(e.Message, e);
            }

            if (failurePath is IHasSectionResults<FailureMechanismSectionResultOld> hasSectionResultsOld)
            {
                affectedObjects.Add(hasSectionResultsOld.SectionResultsOld);
            }

            if (failurePath is IHasSectionResults<FailureMechanismSectionResultOld, FailureMechanismSectionResult> hasSectionResults)
            {
                affectedObjects.Add(hasSectionResults.SectionResults);
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