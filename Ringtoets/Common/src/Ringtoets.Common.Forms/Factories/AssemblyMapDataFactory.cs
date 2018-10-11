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

using System.Drawing;
using Core.Common.Util;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using Ringtoets.AssemblyTool.Forms;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="MapData"/> instances for assembly data.
    /// </summary>
    public static class AssemblyMapDataFactory
    {
        private const int lineWidth = 2;
        private const LineDashStyle lineDashStyle = LineDashStyle.Solid;

        /// <summary>
        /// Creates a <see cref="MapDataCollection"/> for assembly results.
        /// </summary>
        /// <returns>The created <see cref="MapDataCollection"/>.</returns>
        public static MapDataCollection CreateAssemblyMapDataCollection()
        {
            return new MapDataCollection(Resources.AssemblyMapDataFactory_AssemblyMapDataCollection);
        }

        /// <summary>
        /// Creates a <see cref="MapLineData"/> with default styling for a simple assembly.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateSimpleAssemblyMapData()
        {
            return new MapLineData(Resources.AssemblyMapDataFactory_SimpleAssemblyMapData, CreateLineStyle())
            {
                SelectedMetaDataAttribute = Resources.AssemblyCategory_Group_DisplayName,
                IsVisible = false,
                MapTheme = CreateMapTheme()
            };
        }

        /// <summary>
        /// Creates a <see cref="MapLineData"/> with default styling for a detailed assembly.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateDetailedAssemblyMapData()
        {
            return new MapLineData(Resources.AssemblyMapDataFactory_DetailedAssemblyMapData, CreateLineStyle())
            {
                SelectedMetaDataAttribute = Resources.AssemblyCategory_Group_DisplayName,
                IsVisible = false,
                MapTheme = CreateMapTheme()
            };
        }

        /// <summary>
        /// Creates a <see cref="MapLineData"/> with default styling for a tailor made assembly.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateTailorMadeAssemblyMapData()
        {
            return new MapLineData(Resources.AssemblyMapDataFactory_TailorMadeAssemblyMapData, CreateLineStyle())
            {
                SelectedMetaDataAttribute = Resources.AssemblyCategory_Group_DisplayName,
                IsVisible = false,
                MapTheme = CreateMapTheme()
            };
        }

        /// <summary>
        /// Creates a <see cref="MapLineData"/> with default styling for a combined assembly.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateCombinedAssemblyMapData()
        {
            return new MapLineData(Resources.AssemblyMapDataFactory_CombinedAssemblyMapData, CreateLineStyle())
            {
                SelectedMetaDataAttribute = Resources.AssemblyCategory_Group_DisplayName,
                IsVisible = true,
                MapTheme = CreateMapTheme()
            };
        }

        private static LineStyle CreateLineStyle()
        {
            return new LineStyle
            {
                Width = lineWidth,
                DashStyle = lineDashStyle
            };
        }

        private static MapTheme CreateMapTheme()
        {
            return new MapTheme(Resources.AssemblyCategory_Group_DisplayName, new[]
            {
                new CategoryTheme(Color.FromArgb(255, 0, 255, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.Iv)),
                new CategoryTheme(Color.FromArgb(255, 118, 147, 60), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.IIv)),
                new CategoryTheme(Color.FromArgb(255, 255, 255, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.IIIv)),
                new CategoryTheme(Color.FromArgb(255, 204, 192, 218), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.IVv)),
                new CategoryTheme(Color.FromArgb(255, 255, 153, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.Vv)),
                new CategoryTheme(Color.FromArgb(255, 255, 0, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.VIv)),
                new CategoryTheme(Color.FromArgb(255, 255, 255, 255), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.VIIv)),
                new CategoryTheme(Color.HotPink, CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.NotApplicable)),
                new CategoryTheme(Color.FromArgb(0, 0, 0, 0), CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup.None))
            });
        }

        private static ValueCriterion CreateCriterion(DisplayFailureMechanismSectionAssemblyCategoryGroup category)
        {
            return new ValueCriterion(ValueCriterionOperator.EqualValue,
                                      new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyCategoryGroup>(category).DisplayName);
        }
    }
}