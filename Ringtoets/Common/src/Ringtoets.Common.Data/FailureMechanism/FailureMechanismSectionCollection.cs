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
    public class FailureMechanismSectionCollection : Observable, IObservableEnumerable<FailureMechanismSection>
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
        /// <param name="filePath">The path to the source file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="failureMechanismSections"/> contains <c>null</c>.</item>
        /// <item><paramref name="filePath"/> is not a valid file path.</item>
        /// </list>
        /// </exception>
        public void SetSections(IEnumerable<FailureMechanismSection> failureMechanismSections,
                                string filePath)
        {
            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!IOUtils.IsValidFilePath(filePath) && filePath.Length > 0)
            {
                throw new ArgumentException($@"'{filePath}' is not a valid file path.", nameof(filePath));
            }

            sections.Clear();

            List<FailureMechanismSection> sourceCollection = failureMechanismSections.ToList();
            if (!sourceCollection.Any())
            {
                return;
            }

            FailureMechanismSection firstSection = sourceCollection.First();
            sections.Add(firstSection);
            sourceCollection.Remove(firstSection);

            foreach (FailureMechanismSection section in sourceCollection)
            {
                InsertSectionWhileMaintainingConnectivityOrder(section);
            }

            SourcePath = filePath;
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
        /// Inserts the section to the collection while maintaining connectivity
        /// order (neighboring <see cref="FailureMechanismSection"/> have same end points).
        /// </summary>
        /// <param name="sectionToInsert">The new section.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sectionToInsert"/> cannot
        /// be connected to elements already defined in this collection.</exception>
        private void InsertSectionWhileMaintainingConnectivityOrder(FailureMechanismSection sectionToInsert)
        {
            if (sections.Last().EndPoint.Equals(sectionToInsert.StartPoint))
            {
                sections.Add(sectionToInsert);
            }
            else
            {
                string message = string.Format(Resources.BaseFailureMechanism_AddSection_Section_0_must_connect_to_existing_sections,
                                               sectionToInsert.Name);
                throw new ArgumentException(message, nameof(sectionToInsert));
            }
        }
    }
}