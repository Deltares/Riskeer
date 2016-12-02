// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="FeatureBasedMapData"/> for data used as input in the assessment section.
    /// </summary>
    public static class RingtoetsMapDataFactory
    {
        private const int thinLineWidth = 2;
        private const int thickLineWidth = 3;
        private const int smallPointSize = 6;
        private const int largePointSize = 15;

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for a <see cref="ReferenceLine"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateReferenceLineMapData()
        {
            return new MapLineData(RingtoetsCommonDataResources.ReferenceLine_DisplayName)
            {
                Style = new LineStyle(Color.Red, thickLineWidth, DashStyle.Solid),
                SelectedMetaDataAttribute = Resources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for a collection of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateHydraulicBoundaryLocationsMapData()
        {
            return new MapPointData(RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName)
            {
                Style = new PointStyle(Color.DarkBlue, smallPointSize, PointSymbol.Circle),
                ShowLabels = true,
                SelectedMetaDataAttribute = Resources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateFailureMechanismSectionsMapData()
        {
            return new MapLineData(Resources.FailureMechanism_Sections_DisplayName)
            {
                Style = new LineStyle(Color.Khaki, thickLineWidth, DashStyle.Dot),
                SelectedMetaDataAttribute = Resources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for the start points in collections of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateFailureMechanismSectionsStartPointMapData()
        {
            var mapDataName = String.Format("{0} ({1})",
                                            Resources.FailureMechanism_Sections_DisplayName,
                                            Resources.FailureMechanismSections_StartPoints_DisplayName);

            return new MapPointData(mapDataName)
            {
                Style = new PointStyle(Color.DarkKhaki, largePointSize, PointSymbol.Triangle)
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for the end points in collections of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateFailureMechanismSectionsEndPointMapData()
        {
            var mapDataName = String.Format("{0} ({1})",
                                            Resources.FailureMechanism_Sections_DisplayName,
                                            Resources.FailureMechanismSections_EndPoints_DisplayName);

            return new MapPointData(mapDataName)
            {
                Style = new PointStyle(Color.DarkKhaki, largePointSize, PointSymbol.Triangle)
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="DikeProfile"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateDikeProfileMapData()
        {
            return new MapLineData(Resources.DikeProfiles_DisplayName)
            {
                Style = new LineStyle(Color.SaddleBrown, thinLineWidth, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="ForeshoreProfile"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateForeshoreProfileMapData()
        {
            return new MapLineData(Resources.ForeshoreProfiles_DisplayName)
            {
                Style = new LineStyle(Color.DarkOrange, thinLineWidth, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for collections of <see cref="StructureBase"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateStructuresMapData()
        {
            return new MapPointData(Resources.StructuresCollection_DisplayName)
            {
                Style = new PointStyle(Color.DarkSeaGreen, largePointSize, PointSymbol.Square)
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="ICalculation"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapLineData CreateCalculationsMapData()
        {
            return new MapLineData(RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName)
            {
                Style = new LineStyle(Color.MediumPurple, thinLineWidth, DashStyle.Dash),
                ShowLabels = true,
                SelectedMetaDataAttribute = Resources.MetaData_Name
            };
        }
    }
}