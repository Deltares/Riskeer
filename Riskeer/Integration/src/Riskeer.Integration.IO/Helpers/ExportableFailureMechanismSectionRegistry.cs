// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Integration.IO.Helpers
{
    /// <summary>
    /// Class that keeps track of the the created <see cref="ExportableFailureMechanismSection"/>.
    /// </summary>
    public class ExportableFailureMechanismSectionRegistry
    {
        private readonly Dictionary<FailureMechanismSection, ExportableFailureMechanismSection> failureMechanismSections;

        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismSectionRegistry"/>.
        /// </summary>
        public ExportableFailureMechanismSectionRegistry()
        {
            failureMechanismSections = new Dictionary<FailureMechanismSection, ExportableFailureMechanismSection>();
        }

        /// <summary>
        /// Obtains the <see cref="ExportableFailureMechanismSection"/> which was registered for the
        /// given <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> that has been registered.</param>
        /// <returns>The associated <see cref="ExportableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no <see cref="ExportableFailureMechanismSection"/>
        /// has been registered for <paramref name="section"/>.</exception>
        /// <remarks>Use <see cref="Contains(FailureMechanismSection)"/> to find out whether a create
        /// operation has been registered for <paramref name="section"/>.</remarks>
        public ExportableFailureMechanismSection Get(FailureMechanismSection section)
        {
            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            try
            {
                return failureMechanismSections[section];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Registers a <paramref name="section"/> and the <paramref name="exportableSection"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to be registered with.</param>
        /// <param name="exportableSection">The <see cref="ExportableFailureMechanismSection"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(FailureMechanismSection section, ExportableFailureMechanismSection exportableSection)
        {
            if (exportableSection == null)
            {
                throw new ArgumentNullException(nameof(exportableSection));
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            failureMechanismSections[section] = exportableSection;
        }

        /// <summary>
        /// Checks whether the given <paramref name="section" /> has been registered.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref name="section"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        internal bool Contains(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            return failureMechanismSections.ContainsKey(section);
        }
    }
}