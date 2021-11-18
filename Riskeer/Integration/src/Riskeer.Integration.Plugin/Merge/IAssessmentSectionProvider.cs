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
using Riskeer.Integration.Data;

namespace Riskeer.Integration.Plugin.Merge
{
    /// <summary>
    /// Interface for providing an <see cref="AssessmentSection"/> instance.
    /// </summary>
    public interface IAssessmentSectionProvider
    {
        /// <summary>
        /// Gets the assessment section from the given <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path to read the assessment section from.</param>
        /// <returns>An <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssessmentSectionProviderException">Thrown when something went wrong
        /// while getting the assessment section.</exception>
        AssessmentSection GetAssessmentSection(string filePath);
    }
}