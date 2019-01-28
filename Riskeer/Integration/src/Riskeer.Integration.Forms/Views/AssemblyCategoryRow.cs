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
using System.ComponentModel;
using System.Drawing;
using Core.Common.Util;
using Ringtoets.Common.Forms.TypeConverters;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row displaying the properties of a <see cref="AssemblyCategory"/>.
    /// </summary>
    /// <typeparam name="T">The type of the enum to display.</typeparam>
    internal class AssemblyCategoryRow<T>
        where T : struct
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyCategoryRow{T}"/>.
        /// </summary>
        /// <param name="assemblyCategory">The <see cref="AssemblyCategory"/> to use.</param>
        /// <param name="assemblyColor">The <see cref="Color"/> belonging to this category.</param>
        /// <param name="assemblyCategoryGroup">The category group of this category.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assemblyCategory"/>
        /// is <c>null</c>.</exception>
        public AssemblyCategoryRow(AssemblyCategory assemblyCategory,
                                   Color assemblyColor,
                                   T assemblyCategoryGroup)
        {
            if (assemblyCategory == null)
            {
                throw new ArgumentNullException(nameof(assemblyCategory));
            }

            Group = assemblyCategoryGroup;
            Color = assemblyColor;
            UpperBoundary = assemblyCategory.UpperBoundary;
            LowerBoundary = assemblyCategory.LowerBoundary;
        }

        /// <summary>
        /// Gets the display name of the category group.
        /// </summary>
        [TypeConverter(typeof(EnumTypeConverter))]
        public T Group { get; }

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