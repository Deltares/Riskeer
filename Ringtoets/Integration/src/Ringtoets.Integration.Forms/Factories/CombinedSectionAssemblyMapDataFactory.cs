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

using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="FeatureBasedMapData"/> for combined section assembly results.
    /// </summary>
    public static class CombinedSectionAssemblyMapDataFactory
    {
        /// <summary>
        /// Creates a <see cref="MapLineData"/> with default styling for a combined section assembly. 
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateCombinedSectionAssemblyResultMapData()
        {
            return new MapLineData(Resources.AssemblyResultPerSection_DisplayName,
                                   new LineStyle
                                   {
                                       Width = 6,
                                       DashStyle = LineDashStyle.Solid
                                   })
            {
                ShowLabels = true,
                SelectedMetaDataAttribute = Resources.SectionNumber_DisplayName,
                Theme = MapThemeFactory.CreateDisplayFailureMechanismAssemblyCategoryGroupMapTheme()
            };
        }
    }
}