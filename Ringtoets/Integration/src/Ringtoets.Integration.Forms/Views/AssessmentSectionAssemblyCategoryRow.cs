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
using System.ComponentModel;
using System.Drawing;
using Core.Common.Util;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of a <see cref="AssessmentSectionAssemblyCategory"/>.
    /// </summary>
    public class AssessmentSectionAssemblyCategoryRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyCategoryRow"/>.
        /// </summary>
        /// <param name="assessmentSectionAssemblyCategory">The <see cref="AssessmentSectionAssemblyCategory"/> to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSectionAssemblyCategory"/>
        /// is <c>null</c>.</exception>
        public AssessmentSectionAssemblyCategoryRow(AssessmentSectionAssemblyCategory assessmentSectionAssemblyCategory)
        {
            if (assessmentSectionAssemblyCategory == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionAssemblyCategory));
            }

            Group = new EnumDisplayWrapper<AssessmentSectionAssemblyCategoryGroup>(assessmentSectionAssemblyCategory.Group).DisplayName;
            Color = AssemblyCategoryGroupHelper.GetAssessmentSectionAssemblyCategoryGroupColor(assessmentSectionAssemblyCategory.Group);
            UpperBoundary = assessmentSectionAssemblyCategory.UpperBoundary;
            LowerBoundary = assessmentSectionAssemblyCategory.LowerBoundary;
        }

        /// <summary>
        /// Gets the display name of the category group.
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// Gets the color of the category.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Gets the lower boundary of the category.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double LowerBoundary { get; }

        /// <summary>
        /// Gets the upper boundary of the category.
        /// </summary>
        [TypeConverter(typeof(NoProbabilityValueDoubleConverter))]
        public double UpperBoundary { get; }
    }
}