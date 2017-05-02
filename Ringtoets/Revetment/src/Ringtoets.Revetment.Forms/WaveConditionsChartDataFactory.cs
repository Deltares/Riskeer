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

using Core.Components.Charting.Data;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Revetment.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Revetment.Forms
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as
    /// wave conditions input.
    /// </summary>
    internal static class WaveConditionsChartDataFactory
    {
        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="input"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartLineData"/> to update the name for.</param>
        /// <param name="input">The <see cref="WaveConditionsInput"/> used for obtaining the name.</param>
        /// <remarks>A default name is set (the same as in <see cref="RingtoetsChartDataFactory.CreateForeshoreGeometryChartData"/>) when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore profile in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore profile should not be used.</item>
        /// </list>
        /// </remarks>
        public static void UpdateForeshoreGeometryChartDataName(ChartLineData chartData, WaveConditionsInput input)
        {
            chartData.Name = input?.ForeshoreProfile != null && input.UseForeshore
                                 ? string.Format(RingtoetsCommonFormsResources.ChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                 input.ForeshoreProfile.Name,
                                                 RingtoetsCommonFormsResources.Foreshore_DisplayName)
                                 : RingtoetsCommonFormsResources.Foreshore_DisplayName;
        }
    }
}