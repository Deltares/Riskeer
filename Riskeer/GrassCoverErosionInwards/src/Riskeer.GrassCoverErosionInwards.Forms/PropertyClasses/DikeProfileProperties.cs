﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionInwards.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DikeProfile"/> for properties panel.
    /// </summary>
    public class DikeProfileProperties : ObjectProperties<DikeProfile>
    {
        private const int idPropertyIndex = 1;
        private const int namePropertyIndex = 2;
        private const int worldReferencePointPropertyIndex = 3;
        private const int orientationPropertyIndex = 4;
        private const int breakWaterPropertyIndex = 5;
        private const int foreshorePropertyIndex = 6;
        private const int dikeGeometryPropertyIndex = 7;
        private const int dikeHeightPropertyIndex = 8;

        [PropertyOrder(idPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Id_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeProfile_Id_Description))]
        public string Id
        {
            get
            {
                return data.Id;
            }
        }

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Profile_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeProfile_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.WorldReferencePoint_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WorldReferencePoint_DikeProfile_Description))]
        public Point2D WorldReferencePoint
        {
            get
            {
                return new Point2D(
                    new RoundedDouble(0, data.WorldReferencePoint.X),
                    new RoundedDouble(0, data.WorldReferencePoint.Y));
            }
        }

        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Orientation_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Orientation_DikeProfile_Description))]
        public RoundedDouble Orientation
        {
            get
            {
                return data.Orientation;
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.BreakWaterProperties_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.BreakWaterProperties_Description))]
        public BreakWaterProperties BreakWater
        {
            get
            {
                return new BreakWaterProperties
                {
                    Data = data.ForeshoreProfile
                };
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ForeshoreProperties_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.ForeshoreProperties_Description))]
        public ForeshoreGeometryProperties Foreshore
        {
            get
            {
                return new ForeshoreGeometryProperties
                {
                    Data = data.ForeshoreProfile
                };
            }
        }

        [PropertyOrder(dikeGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DikeGeometryProperties_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeGeometryProperties_Description))]
        public DikeProfileDikeGeometryProperties DikeGeometry
        {
            get
            {
                return new DikeProfileDikeGeometryProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(dikeHeightPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.DikeHeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.DikeHeight_Description))]
        public RoundedDouble DikeHeight
        {
            get
            {
                return data.DikeHeight;
            }
        }
    }
}