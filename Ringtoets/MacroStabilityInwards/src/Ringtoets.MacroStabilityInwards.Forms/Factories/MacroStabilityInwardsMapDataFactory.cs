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

using System.Drawing;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Primitives;
using DataResources = Ringtoets.MacroStabilityInwards.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="FeatureBasedMapData"/> for data used as input in the macro stability inwards failure mechanism.
    /// </summary>
    internal static class MacroStabilityInwardsMapDataFactory
    {
        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateSurfaceLinesMapData()
        {
            return new MapLineData(DataResources.MacroStabilityInwardsSurfaceLineCollection_TypeDescriptor,
                                   new LineStyle
                                   {
                                       Color = Color.DarkSeaGreen,
                                       Width = 2,
                                       DashStyle = LineDashStyle.Solid
                                   })
            {
                SelectedMetaDataAttribute = RingtoetsCommonFormsResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateStochasticSoilModelsMapData()
        {
            return new MapLineData(DataResources.StochasticSoilModelCollection_TypeDescriptor,
                                   new LineStyle
                                   {
                                       Color = Color.FromArgb(70, Color.SaddleBrown),
                                       Width = 5,
                                       DashStyle = LineDashStyle.Solid
                                   })
            {
                SelectedMetaDataAttribute = RingtoetsCommonFormsResources.MetaData_Name
            };
        }
    }
}