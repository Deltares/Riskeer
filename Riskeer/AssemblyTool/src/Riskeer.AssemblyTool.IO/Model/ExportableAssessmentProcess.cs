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
using Riskeer.AssemblyTool.IO.Helpers;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class describing an assessment process.
    /// </summary>
    public class ExportableAssessmentProcess
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableAssessmentProcess"/>.
        /// </summary>
        /// <param name="id">The id of the assessment process.</param>
        /// <param name="startYear">The start year of the assessment process.</param>
        /// <param name="endYear">The end year of the assessment process.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public ExportableAssessmentProcess(string id, int startYear, int endYear)
        {
            IdValidationHelper.ThrowIfInvalid(id);

            Id = id;
            StartYear = startYear;
            EndYear = endYear;
        }

        /// <summary>
        /// Gets the id of the assessment process.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the start year of the assessment process.
        /// </summary>
        public int StartYear { get; }

        /// <summary>
        /// Gets the end year of the assessment process.
        /// </summary>
        public int EndYear { get; }
    }
}