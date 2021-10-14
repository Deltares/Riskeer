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
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class is the implementation for a specific failure mechanism.
    /// </summary>
    public class SpecificFailureMechanism : Observable
    {
        private readonly FailureMechanismSectionCollection sectionCollection;

        /// <summary>
        /// Creates a new instance of the <see cref="SpecificFailureMechanism"/> class.
        /// </summary>
        public SpecificFailureMechanism()
        {
            Name = Resources.SpecificFailureMechanism_Name_DefaultName;

            sectionCollection = new FailureMechanismSectionCollection();
            IsRelevant = true;
            InputComments = new Comment();
            OutputComments = new Comment();
            NotRelevantComments = new Comment();
        }

        /// <summary>
        /// Gets or sets the name of the specific failure mechanism.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the collection of sections that define areas for which a calculation could determine
        /// a representative result.
        /// </summary>
        public IEnumerable<FailureMechanismSection> Sections
        {
            get
            {
                return sectionCollection;
            }
        }

        /// <summary>
        /// Gets the source path of the imported <see cref="Sections"/>.
        /// </summary>
        public string FailureMechanismSectionSourcePath
        {
            get
            {
                return sectionCollection.SourcePath;
            }
        }

        /// <summary>
        /// Gets the comments associated with the input of the data object.
        /// </summary>
        public Comment InputComments { get; }

        /// <summary>
        /// Gets the comments associated with the output of the data object.
        /// </summary>
        public Comment OutputComments { get; }

        /// <summary>
        /// Gets the comments associated when the failure mechanism is set to be not relevant.
        /// </summary>
        public Comment NotRelevantComments { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this specific failure mechanism is relevant.
        /// </summary>
        public bool IsRelevant { get; set; }

        /// <summary>
        /// Sets a collection of <see cref="FailureMechanismSection"/> to <see cref="Sections"/>.
        /// </summary>
        /// <param name="sections">The sections to set.</param>
        /// <param name="sourcePath">The path of the file the sections originate from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="sourcePath"/> is not a valid file path.</item>
        /// <item><paramref name="sections"/> contains sections that are not properly chained.</item>
        /// </list>
        /// </exception>
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

            sectionCollection.SetSections(sections, sourcePath);
        }

        /// <summary>
        /// Clears all sections from <see cref="Sections"/>.
        /// </summary>
        public void ClearAllSections()
        {
            sectionCollection.Clear();
        }
    }
}