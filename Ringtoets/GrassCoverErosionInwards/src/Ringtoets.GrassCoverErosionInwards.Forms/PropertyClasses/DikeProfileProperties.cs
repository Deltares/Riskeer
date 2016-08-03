﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionInwards.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DikeProfile"/> for properties panel.
    /// </summary>
    public class DikeProfileProperties : ObjectProperties<DikeProfile>
    {
        private const int namePropertyIndex = 1;
        private const int orientationPropertyIndex = 2;
        private const int breakWaterPropertyIndex = 3;
        private const int foreshorePropertyIndex = 4;
        private const int dikeGeometryPropertyIndex = 5;
        private const int dikeHeightPropertyIndex = 6;
        private const int worldReferencePointPropertyIndex = 7;

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonDataResources), "Categories_General")]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "DikeProfile_Name_DisplayName")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "DikeProfile_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "Orientation_DisplayName")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "Orientation_Description")]
        public RoundedDouble Orientation
        {
            get
            {
                return data.Orientation;
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "BreakWaterProperties_DisplayName")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "BreakWaterProperties_Description")]
        public DikeProfileBreakWaterProperties BreakWater
        {
            get
            {
                return new DikeProfileBreakWaterProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "ForeshoreProperties_Description")]
        public DikeProfileForeshoreProperties Foreshore
        {
            get
            {
                return new DikeProfileForeshoreProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(dikeGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "DikeGeometryProperties_DisplayName")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "DikeGeometryProperties_Description")]
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
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "DikeHeight_DisplayName")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "DikeHeight_Description")]
        public RoundedDouble DikeHeight
        {
            get
            {
                return data.DikeHeight;
            }
        }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(GrassCoverErosionInwardsFormsResources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "WorldReferencePoint_DisplayName")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "WorldReferencePoint_Description")]
        public Point2D WorldReferencePoint
        {
            get
            {
                return new Point2D(
                               new RoundedDouble(0, data.WorldReferencePoint.X),
                               new RoundedDouble(0, data.WorldReferencePoint.Y));
            }
        }
    }
}