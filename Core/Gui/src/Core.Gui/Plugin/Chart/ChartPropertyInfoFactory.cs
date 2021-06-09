// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using Core.Components.Chart.Data;
using Core.Gui.PropertyClasses.Chart;

namespace Core.Gui.Plugin.Chart
{
    /// <summary>
    /// Factory for creating <see cref="PropertyInfo"/> objects for <see cref="ChartData"/>.
    /// </summary>
    public static class ChartPropertyInfoFactory
    {
        /// <summary>
        /// Creates the <see cref="PropertyInfo"/> objects.
        /// </summary>
        /// <returns>The created <see cref="PropertyInfo"/> objects.</returns>
        public static IEnumerable<PropertyInfo> Create()
        {
            yield return new PropertyInfo<ChartDataCollection, ChartDataCollectionProperties>();
            yield return new PropertyInfo<ChartLineData, ChartLineDataProperties>();
            yield return new PropertyInfo<ChartAreaData, ChartAreaDataProperties>();
            yield return new PropertyInfo<ChartMultipleAreaData, ChartMultipleAreaDataProperties>();
            yield return new PropertyInfo<ChartMultipleLineData, ChartMultipleLineDataProperties>();
            yield return new PropertyInfo<ChartPointData, ChartPointDataProperties>();
        }
    }
}