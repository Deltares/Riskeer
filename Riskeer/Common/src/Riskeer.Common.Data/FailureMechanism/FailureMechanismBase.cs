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

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class is the base implementation for a specific failure mechanism. Classes which want
    /// to implement <see cref="IFailureMechanism"/> can and should most likely inherit
    /// from this class.
    /// </summary>
    /// <typeparam name="T">The type of section results.</typeparam>
    public abstract class FailureMechanismBase<T> : Observable, IFailureMechanism<T> where T : FailureMechanismSectionResult
    {
        private readonly FailureMechanismSectionCollection sectionCollection;
        private readonly ObservableList<T> sectionResults;

        /// <summary>
        /// Creates a new instance of the <see cref="FailureMechanismBase{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the failure mechanism.</param>
        /// <param name="failureMechanismCode">The code of the failure mechanism.</param>
        /// <exception cref="ArgumentException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="name"/> is <c>null</c> or empty.</item>
        /// <item><paramref name="failureMechanismCode"/> is <c>null</c> or empty.</item>
        /// </list>
        /// </exception>
        protected FailureMechanismBase(string name, string failureMechanismCode)
        {
            ValidateParameters(name, failureMechanismCode);

            Name = name;
            Code = failureMechanismCode;
            sectionCollection = new FailureMechanismSectionCollection();
            InAssembly = true;
            InAssemblyInputComments = new Comment();
            InAssemblyOutputComments = new Comment();
            NotInAssemblyComments = new Comment();

            AssemblyResult = new FailureMechanismAssemblyResult();
            sectionResults = new ObservableList<T>();
        }

        public string Name { get; }

        public string Code { get; }

        public IEnumerable<FailureMechanismSection> Sections => sectionCollection;

        public FailureMechanismAssemblyResult AssemblyResult { get; }

        public string FailureMechanismSectionSourcePath => sectionCollection.SourcePath;

        public Comment InAssemblyInputComments { get; }

        public Comment InAssemblyOutputComments { get; }

        public Comment NotInAssemblyComments { get; }

        public bool InAssembly { get; set; }
        
        public bool IsCorrelated { get; set; }

        public IObservableEnumerable<T> SectionResults => sectionResults;

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

            ClearSectionDependentData();
            sectionCollection.SetSections(sections, sourcePath);

            foreach (FailureMechanismSection failureMechanismSection in Sections)
            {
                AddSectionDependentData(failureMechanismSection);
            }
        }

        public void ClearAllSections()
        {
            sectionCollection.Clear();
            ClearSectionDependentData();
        }

        protected virtual void AddSectionDependentData(FailureMechanismSection section)
        {
            sectionResults.Add(FailureMechanismSectionResultFactory.Create<T>(section));
        }

        protected virtual void ClearSectionDependentData()
        {
            sectionResults.Clear();
        }

        private static void ValidateParameters(string failureMechanismName, string failureMechanismCode)
        {
            const string parameterIsRequired = "Parameter is required.";
            if (string.IsNullOrEmpty(failureMechanismName))
            {
                throw new ArgumentException(parameterIsRequired, nameof(failureMechanismName));
            }

            if (string.IsNullOrEmpty(failureMechanismCode))
            {
                throw new ArgumentException(parameterIsRequired, nameof(failureMechanismCode));
            }
        }
    }
}