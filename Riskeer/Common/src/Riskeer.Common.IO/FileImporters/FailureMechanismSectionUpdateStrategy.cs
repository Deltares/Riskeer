﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.IO.FileImporters
{
    /// <summary>
    /// An <see cref="IFailureMechanismSectionUpdateStrategy"/> that can be used to update failure mechanism sections with
    /// imported failure mechanism sections.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="FailureMechanismSectionResult"/> that will be updated.</typeparam>
    public class FailureMechanismSectionUpdateStrategy<T> : IFailureMechanismSectionUpdateStrategy
        where T : FailureMechanismSectionResult
    {
        private readonly IHasSectionResults<T> failureMechanism;
        private readonly IFailureMechanismSectionResultUpdateStrategy<T> sectionResultUpdateStrategy;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionUpdateStrategy{T}"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to update the sections for.</param>
        /// <param name="sectionResultUpdateStrategy">The <see cref="IFailureMechanismSectionResultUpdateStrategy{T}"/> to use when updating
        /// the section results.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismSectionUpdateStrategy(IHasSectionResults<T> failureMechanism,
                                                     IFailureMechanismSectionResultUpdateStrategy<T> sectionResultUpdateStrategy)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (sectionResultUpdateStrategy == null)
            {
                throw new ArgumentNullException(nameof(sectionResultUpdateStrategy));
            }

            this.failureMechanism = failureMechanism;
            this.sectionResultUpdateStrategy = sectionResultUpdateStrategy;
        }

        public void UpdateSectionsWithImportedData(IEnumerable<FailureMechanismSection> importedFailureMechanismSections,
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

            T[] oldSectionResults = failureMechanism.SectionResults.ToArray();

            try
            {
                failureMechanism.SetSections(importedFailureMechanismSections, sourcePath);
            }
            catch (ArgumentException e)
            {
                throw new UpdateDataException(e.Message, e);
            }

            foreach (T sectionResult in failureMechanism.SectionResults)
            {
                T equalSection = oldSectionResults.FirstOrDefault(item => item.Section.StartPoint.Equals(sectionResult.Section.StartPoint)
                                                                          && item.Section.EndPoint.Equals(sectionResult.Section.EndPoint));

                if (equalSection != null)
                {
                    sectionResultUpdateStrategy.UpdateSectionResult(equalSection, sectionResult);
                }
            }
        }

        public virtual void DoPostUpdateActions()
        {
            // Do nothing
        }
    }
}