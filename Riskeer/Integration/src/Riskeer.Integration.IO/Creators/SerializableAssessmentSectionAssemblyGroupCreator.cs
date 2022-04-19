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
using System.ComponentModel;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create <see cref="SerializableAssessmentSectionAssemblyGroup"/>.
    /// </summary>
    public static class SerializableAssessmentSectionAssemblyGroupCreator
    {
        /// <summary>
        /// Creates a <see cref="SerializableAssessmentSectionAssemblyGroup"/> based on <paramref name="group"/>.
        /// </summary>
        /// <param name="group">The <see cref="AssessmentSectionAssemblyGroup"/> to
        /// create a <see cref="SerializableAssessmentSectionAssemblyGroup"/> for.</param>
        /// <returns>A <see cref="SerializableAssessmentSectionAssemblyGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="group"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="group"/>
        /// is a valid value, but unsupported.</exception>
        public static SerializableAssessmentSectionAssemblyGroup Create(AssessmentSectionAssemblyGroup group)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionAssemblyGroup), group))
            {
                throw new InvalidEnumArgumentException(nameof(group),
                                                       (int) group,
                                                       typeof(AssessmentSectionAssemblyGroup));
            }

            switch (group)
            {
                case AssessmentSectionAssemblyGroup.APlus:
                    return SerializableAssessmentSectionAssemblyGroup.APlus;
                case AssessmentSectionAssemblyGroup.A:
                    return SerializableAssessmentSectionAssemblyGroup.A;
                case AssessmentSectionAssemblyGroup.B:
                    return SerializableAssessmentSectionAssemblyGroup.B;
                case AssessmentSectionAssemblyGroup.C:
                    return SerializableAssessmentSectionAssemblyGroup.C;
                case AssessmentSectionAssemblyGroup.D:
                    return SerializableAssessmentSectionAssemblyGroup.D;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}