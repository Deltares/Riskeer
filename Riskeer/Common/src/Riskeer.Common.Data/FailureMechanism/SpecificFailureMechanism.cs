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
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class is the implementation for a specific failure mechanism.
    /// </summary>
    public class SpecificFailureMechanism : Observable, IFailureMechanism<NonAdoptableFailureMechanismSectionResult>
    {
        private readonly FailureMechanismSectionCollection sectionCollection;
        private readonly ObservableList<NonAdoptableFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Creates a new instance of <see cref="SpecificFailureMechanism"/>.
        /// </summary>
        public SpecificFailureMechanism()
        {
            Name = Resources.SpecificFailureMechanism_DefaultName;
            Code = Resources.SpecificFailureMechanism_DefaultCode;

            sectionCollection = new FailureMechanismSectionCollection();
            InAssembly = true;
            InAssemblyInputComments = new Comment();
            InAssemblyOutputComments = new Comment();
            NotInAssemblyComments = new Comment();

            AssemblyResult = new FailureMechanismAssemblyResult();
            sectionResults = new ObservableList<NonAdoptableFailureMechanismSectionResult>();
        }

        public string Name { get; set; }

        public string Code { get; set; }

        public IEnumerable<FailureMechanismSection> Sections => sectionCollection;

        public FailureMechanismAssemblyResult AssemblyResult { get; }

        public string FailureMechanismSectionSourcePath => sectionCollection.SourcePath;

        public Comment InAssemblyInputComments { get; }

        public Comment InAssemblyOutputComments { get; }

        public Comment NotInAssemblyComments { get; }

        public bool InAssembly { get; set; }

        public IObservableEnumerable<NonAdoptableFailureMechanismSectionResult> SectionResults => sectionResults;

        public void SetSections(IEnumerable<FailureMechanismSection> sections, string sourcePath)
        {
            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            if (sourcePath == null)
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            sectionResults.Clear();
            sectionCollection.SetSections(sections, sourcePath);

            foreach (FailureMechanismSection section in sections)
            {
                sectionResults.Add(new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section));
            }
        }

        public void ClearAllSections()
        {
            sectionResults.Clear();
            sectionCollection.Clear();
        }
    }
}