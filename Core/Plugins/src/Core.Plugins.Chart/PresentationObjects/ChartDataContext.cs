// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System;
using Core.Common.Controls.PresentationObjects;
using Core.Components.Chart.Data;

namespace Core.Plugins.Chart.PresentationObjects
{
    /// <summary>
    /// Presentation object for <see cref="ChartData"/>.
    /// </summary>
    public class ChartDataContext : ObservableWrappedObjectContextBase<ChartData>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartDataContext"/>.
        /// </summary>
        /// <param name="wrappedData">The <see cref="ChartData"/> to wrap.</param>
        /// <param name="parentChartData">The parent <see cref="ChartDataCollection"/> 
        /// the <paramref name="wrappedData"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parentChartData"/> is <c>null</c>.</exception>
        public ChartDataContext(ChartData wrappedData, ChartDataCollection parentChartData) : base(wrappedData)
        {
            if (parentChartData == null)
            {
                throw new ArgumentNullException(nameof(parentChartData));
            }

            ParentChartData = parentChartData;
        }

        /// <summary>
        /// Gets the parent <see cref="ChartDataCollection"/>
        /// the <see cref="WrappedObjectContextBase{T}.WrappedData"/> belongs to.
        /// </summary>
        public ChartDataCollection ParentChartData { get; }
    }
}