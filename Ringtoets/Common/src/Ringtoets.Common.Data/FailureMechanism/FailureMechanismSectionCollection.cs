// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Util;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.FailureMechanism
{
    public class FailureMechanismSectionCollection : Observable, IEnumerable<FailureMechanismSection>
    {
        private readonly List<FailureMechanismSection> sections = new List<FailureMechanismSection>();

        /// <summary>
        /// Gets the last known file path from which the <see cref="FailureMechanismSection"/> were imported.
        /// </summary>
        /// <returns>The path where the elements originate
        /// from, or <c>null</c> if the collection is cleared.</returns>
        public string SourcePath { get; private set; }

        /// <summary>
        /// Clears the imported items in the collection and the <see cref="SourcePath"/>.
        /// </summary>
        public void Clear()
        {
            SourcePath = null;
            sections.Clear();
        }

        /// <summary>
        /// Adds all <see cref="FailureMechanismSection"/> originating from a source file.
        /// </summary>
        /// <param name="failureMechanismSections">The collection of <see cref="FailureMechanismSection"/> to add</param>
        /// <param name="sourcePath">The path to the source file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sourcePath"/> is not a valid file path.</exception>
        public void SetSections(IEnumerable<FailureMechanismSection> failureMechanismSections,
                                string sourcePath)
        {
            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            if (sourcePath == null)
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            if (!IOUtils.IsValidFilePath(sourcePath) && sourcePath.Length > 0)
            {
                throw new ArgumentException($@"'{sourcePath}' is not a valid file path.", nameof(sourcePath));
            }

            Clear();

            if (failureMechanismSections.Any())
            {
                FailureMechanismSection firstSection = failureMechanismSections.First();
                var newSections = new List<FailureMechanismSection>
                {
                    firstSection
                };

                FailureMechanismSection previousSection = firstSection;
                foreach (FailureMechanismSection section in failureMechanismSections.Skip(1))
                {
                    ValidateSection(section, previousSection);
                    newSections.Add(section);
                    previousSection = section;
                }

                sections.AddRange(newSections);
            }

            SourcePath = sourcePath;
        }

        public IEnumerator<FailureMechanismSection> GetEnumerator()
        {
            return sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Validates the section on its connectivity order (neighboring
        /// <see cref="FailureMechanismSection"/> must have same end points).
        /// </summary>
        /// <param name="section">The new section.</param>
        /// <param name="previousSection">The previous section.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="section"/> cannot
        /// be connected to the previous section.</exception>
        private static void ValidateSection(FailureMechanismSection section, FailureMechanismSection previousSection)
        {
            if (!previousSection.EndPoint.Equals(section.StartPoint))
            {
                string message = string.Format(Resources.FailureMechanismSectionCollection_ValidateSection_Section_0_must_connect_to_existing_sections,
                                               section.Name);
                throw new ArgumentException(message, nameof(section));
            }
        }
    }
}