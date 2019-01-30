// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="SerializableAssessmentSectionAssemblyResult"/>
    /// </summary>
    public static class SerializableAssessmentSectionAssemblyResultCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="SerializableAssessmentSectionAssemblyResult"/> based
        /// on <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The <see cref="ExportableAssessmentSectionAssemblyResult"/> to create a
        /// a <see cref="SerializableAssessmentSectionAssemblyResult"/> for.</param>
        /// <returns>A <see cref="SerializableAssessmentSectionAssemblyResult"/>,</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        public static SerializableAssessmentSectionAssemblyResult Create(ExportableAssessmentSectionAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new SerializableAssessmentSectionAssemblyResult(SerializableAssemblyMethodCreator.Create(result.AssemblyMethod),
                                                                   SerializableAssessmentSectionCategoryGroupCreator.Create(result.AssemblyCategory));
        }
    }
}