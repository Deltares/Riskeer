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

using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="MapData"/> instances for assembly data.
    /// </summary>
    public static class AssemblyMapDataFactory
    {
        private const int lineWidth = 6;
        private const LineDashStyle lineDashStyle = LineDashStyle.Solid;

        /// <summary>
        /// Creates a <see cref="MapDataCollection"/> for assembly results.
        /// </summary>
        /// <returns>The created <see cref="MapDataCollection"/>.</returns>
        public static MapDataCollection CreateAssemblyMapDataCollection()
        {
            return new MapDataCollection(Resources.AssemblyResult_DisplayName);
        }

        /// <summary>
        /// Creates a <see cref="MapLineData"/> with default styling for a simple assembly.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateSimpleAssemblyMapData()
        {
            return CreateAssemblyMapLineData(Resources.AssemblyMapDataFactory_SimpleAssemblyMapData, false);
        }

        /// <summary>
        /// Creates a <see cref="MapLineData"/> with default styling for a detailed assembly.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateDetailedAssemblyMapData()
        {
            return CreateAssemblyMapLineData(Resources.AssemblyMapDataFactory_DetailedAssemblyMapData, false);
        }

        /// <summary>
        /// Creates a <see cref="MapLineData"/> with default styling for a tailor made assembly.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateTailorMadeAssemblyMapData()
        {
            return CreateAssemblyMapLineData(Resources.AssemblyMapDataFactory_TailorMadeAssemblyMapData, false);
        }

        /// <summary>
        /// Creates a <see cref="MapLineData"/> with default styling for a combined assembly.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateCombinedAssemblyMapData()
        {
            return CreateAssemblyMapLineData(Resources.CombinedAssembly_DisplayName, true);
        }

        private static MapLineData CreateAssemblyMapLineData(string name, bool isVisible)
        {
            return new MapLineData(name, new LineStyle
                                   {
                                       Width = lineWidth,
                                       DashStyle = lineDashStyle
                                   },
                                   MapThemeFactory.CreateDisplayFailureMechanismAssemblyCategoryGroupMapTheme())
            {
                SelectedMetaDataAttribute = Resources.AssemblyCategory_Group_DisplayName,
                IsVisible = isVisible
            };
        }
    }
}