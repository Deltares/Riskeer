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
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.Properties;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonUtilResources = Riskeer.Common.Util.Properties.Resources;

namespace Riskeer.Common.Forms.Factories
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
            return new MapLineData(RiskeerCommonDataResources.ReferenceLine_DisplayName,
                                   new LineStyle
                                   {
                                       Color = Color.FromArgb(0, 128, 255),
                                       Width = thickLineWidth,
                                       DashStyle = LineDashStyle.Solid
                                   })
            {
                SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for a collection of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateHydraulicBoundaryLocationsMapData()
        {
            Color color = Color.DarkBlue;
            return new MapPointData(RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                                    new PointStyle
                                    {
                                        Color = color,
                                        Size = smallPointSize,
                                        Symbol = PointSymbol.Circle,
                                        StrokeColor = color,
                                        StrokeThickness = 1
                                    })
            {
                ShowLabels = true,
                SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateFailureMechanismSectionsMapData()
        {
            return new MapLineData(Resources.FailureMechanismSections_DisplayName,
                                   new LineStyle
                                   {
                                       Color = Color.Khaki,
                                       Width = thickLineWidth,
                                       DashStyle = LineDashStyle.Dot
                                   })
            {
                SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for the start points in collections of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateFailureMechanismSectionsStartPointMapData()
        {
            string mapDataName = $"{Resources.FailureMechanismSections_DisplayName} ({Resources.FailureMechanismSections_StartPoints_DisplayName})";

            Color color = Color.DarkKhaki;
            return new MapPointData(mapDataName,
                                    new PointStyle
                                    {
                                        Color = color,
                                        Size = largePointSize,
                                        Symbol = PointSymbol.Triangle,
                                        StrokeColor = color,
                                        StrokeThickness = 1
                                    });
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for the end points in collections of <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateFailureMechanismSectionsEndPointMapData()
        {
            string mapDataName = $"{Resources.FailureMechanismSections_DisplayName} ({Resources.FailureMechanismSections_EndPoints_DisplayName})";

            Color color = Color.DarkKhaki;
            return new MapPointData(mapDataName,
                                    new PointStyle
                                    {
                                        Color = color,
                                        Size = largePointSize,
                                        Symbol = PointSymbol.Triangle,
                                        StrokeColor = color,
                                        StrokeThickness = 1
                                    });
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="DikeProfile"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateDikeProfileMapData()
        {
            return new MapLineData(Resources.DikeProfiles_DisplayName,
                                   new LineStyle
                                   {
                                       Color = Color.SaddleBrown,
                                       Width = thinLineWidth,
                                       DashStyle = LineDashStyle.Solid
                                   })
            {
                SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="ForeshoreProfile"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateForeshoreProfileMapData()
        {
            return new MapLineData(Resources.ForeshoreProfiles_DisplayName,
                                   new LineStyle
                                   {
                                       Color = Color.DarkOrange,
                                       Width = thinLineWidth,
                                       DashStyle = LineDashStyle.Solid
                                   })
            {
                SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapPointData"/> with default styling for collections of <see cref="StructureBase"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapPointData CreateStructuresMapData()
        {
            Color color = Color.DarkSeaGreen;
            return new MapPointData(Resources.StructuresCollection_DisplayName,
                                    new PointStyle
                                    {
                                        Color = color,
                                        Size = largePointSize,
                                        Symbol = PointSymbol.Square,
                                        StrokeColor = color,
                                        StrokeThickness = 1
                                    })
            {
                SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="ICalculation"/>.
        /// </summary>
        /// <returns>The created <see cref="MapPointData"/>.</returns>
        public static MapLineData CreateCalculationsMapData()
        {
            return new MapLineData(RiskeerCommonDataResources.FailureMechanism_Calculations_DisplayName,
                                   new LineStyle
                                   {
                                       Color = Color.MediumPurple,
                                       Width = thinLineWidth,
                                       DashStyle = LineDashStyle.Dash
                                   })
            {
                ShowLabels = true,
                SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="IMechanismSurfaceLine"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateSurfaceLinesMapData()
        {
            return new MapLineData(RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                                   new LineStyle
                                   {
                                       Color = Color.DarkSeaGreen,
                                       Width = 2,
                                       DashStyle = LineDashStyle.Solid
                                   })
            {
                SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create <see cref="MapLineData"/> with default styling for collections of <see cref="IMechanismStochasticSoilModel"/>.
        /// </summary>
        /// <returns>The created <see cref="MapLineData"/>.</returns>
        public static MapLineData CreateStochasticSoilModelsMapData()
        {
            return new MapLineData(RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                                   new LineStyle
                                   {
                                       Color = Color.FromArgb(70, Color.SaddleBrown),
                                       Width = 5,
                                       DashStyle = LineDashStyle.Solid
                                   })
            {
                SelectedMetaDataAttribute = RiskeerCommonUtilResources.MetaData_Name
            };
        }

        /// <summary>
        /// Create a <see cref="MapDataCollection"/> for sections.
        /// </summary>
        /// <returns>The created <see cref="MapDataCollection"/>.</returns>
        public static MapDataCollection CreateSectionsMapDataCollection()
        {
            return new MapDataCollection(Resources.FailureMechanismSections_DisplayName);
        }
    }
}