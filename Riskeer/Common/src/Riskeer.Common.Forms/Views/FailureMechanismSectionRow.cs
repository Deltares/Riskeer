// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="FailureMechanismSection"/>.
    /// </summary>
    internal class FailureMechanismSectionRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionRow"/>.
        /// </summary>
        /// <param name="section">The failure mechanism section to use.</param>
        /// <param name="sectionStart">The start of the section from the beginning
        /// of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of
        /// the reference line in meters.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/>
        /// is <c>null</c>.</exception>
        public FailureMechanismSectionRow(FailureMechanismSection section, double sectionStart, double sectionEnd)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            Name = section.Name;
            SectionStart = new RoundedDouble(2, sectionStart);
            SectionEnd = new RoundedDouble(2, sectionEnd);
            Length = new RoundedDouble(2, section.Length);
        }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the start of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public RoundedDouble SectionStart { get; }

        /// <summary>
        /// Gets the end of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public RoundedDouble SectionEnd { get; }

        /// <summary>
        /// Gets the length of the section.
        /// [m]
        /// </summary>
        public RoundedDouble Length { get; }
    }
}