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
using Core.Common.Base;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.IO.FileImporters
{
    /// <summary>
    /// An <see cref="IFailureMechanismSectionUpdateStrategy"/> that can be used to update failure mechanism sections with
    /// imported failure mechanism sections.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="FailureMechanismSectionResultOld"/> that will be updated.</typeparam>
    public class FailureMechanismSectionUpdateStrategy<T> : IFailureMechanismSectionUpdateStrategy
        where T : FailureMechanismSectionResultOld
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

            T[] oldSectionResults = failureMechanism.SectionResultsOld.ToArray();

            try
            {
                failureMechanism.SetSections(importedFailureMechanismSections, sourcePath);
            }
            catch (ArgumentException e)
            {
                throw new UpdateDataException(e.Message, e);
            }

            foreach (T sectionResult in failureMechanism.SectionResultsOld)
            {
                T equalSection = oldSectionResults.FirstOrDefault(item => item.Section.StartPoint.Equals(sectionResult.Section.StartPoint)
                                                                          && item.Section.EndPoint.Equals(sectionResult.Section.EndPoint));

                if (equalSection != null)
                {
                    sectionResultUpdateStrategy.UpdateSectionResultOld(equalSection, sectionResult);
                }
            }

            return new IObservable[]
            {
                failureMechanism,
                failureMechanism.SectionResultsOld
            };
        }

        public virtual IEnumerable<IObservable> DoPostUpdateActions()
        {
            // Do nothing
            yield break;
        }
    }
    
    /// <summary>
    /// An <see cref="IFailureMechanismSectionUpdateStrategy"/> that can be used to update failure mechanism sections with
    /// imported failure mechanism sections.
    /// </summary>
    /// <typeparam name="TOld">The type of <see cref="FailureMechanismSectionResultOld"/> that will be updated.</typeparam>
    /// <typeparam name="TNew">The type of <see cref="FailureMechanismSectionResult"/> that will be updated.</typeparam>
    public class FailureMechanismSectionUpdateStrategy<TOld, TNew> : FailureMechanismSectionUpdateStrategy<TOld>
        where TOld : FailureMechanismSectionResultOld
        where TNew : FailureMechanismSectionResult
    {
        private readonly IHasSectionResults<TOld, TNew> failureMechanism;
        private readonly IFailureMechanismSectionResultUpdateStrategy<TOld, TNew> sectionResultUpdateStrategy;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionUpdateStrategy{TOld, TNew}"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to update the sections for.</param>
        /// <param name="sectionResultUpdateStrategy">The <see cref="IFailureMechanismSectionResultUpdateStrategy{T}"/> to use when updating
        /// the section results.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismSectionUpdateStrategy(IHasSectionResults<TOld, TNew> failureMechanism,
                                                     IFailureMechanismSectionResultUpdateStrategy<TOld, TNew> sectionResultUpdateStrategy) 
            : base(failureMechanism, sectionResultUpdateStrategy)
        {
            this.failureMechanism = failureMechanism;
            this.sectionResultUpdateStrategy = sectionResultUpdateStrategy;
        }

        public override IEnumerable<IObservable> UpdateSectionsWithImportedData(IEnumerable<FailureMechanismSection> importedFailureMechanismSections, string sourcePath)
        {
            TNew[] oldSectionResults = failureMechanism.SectionResults.ToArray();
            List<IObservable> affectedObjects = base.UpdateSectionsWithImportedData(importedFailureMechanismSections, sourcePath).ToList();
            
            foreach (TNew sectionResult in failureMechanism.SectionResults)
            {
                TNew equalSection = oldSectionResults.FirstOrDefault(item => item.Section.StartPoint.Equals(sectionResult.Section.StartPoint)
                                                                             && item.Section.EndPoint.Equals(sectionResult.Section.EndPoint));

                if (equalSection != null)
                {
                    sectionResultUpdateStrategy.UpdateSectionResult(equalSection, sectionResult);
                }
            }
            
            affectedObjects.Add(failureMechanism.SectionResults);
            return affectedObjects;
        }
    }
}