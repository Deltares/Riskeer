// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Helpers;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class that holds all the information to export the assembly data.
    /// </summary>
    public class ExportableAssembly
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableAssembly"/>.
        /// </summary>
        /// <param name="id">The id of the assembly.</param>
        /// <param name="assessmentSection">The assessment section of the assembly.</param>
        /// <param name="assessmentProcess">The assessment process of the assembly.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter except <paramref name="id"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public ExportableAssembly(string id, ExportableAssessmentSection assessmentSection, ExportableAssessmentProcess assessmentProcess)
        {
            IdValidationHelper.ThrowIfInvalid(id);

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (assessmentProcess == null)
            {
                throw new ArgumentNullException(nameof(assessmentProcess));
            }

            Id = id;
            AssessmentSection = assessmentSection;
            AssessmentProcess = assessmentProcess;
        }

        /// <summary>
        /// Gets the id of the assembly.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the assessment section of the assembly.
        /// </summary>
        public ExportableAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the assessment process of the assembly.
        /// </summary>
        public ExportableAssessmentProcess AssessmentProcess { get; }
    }
}