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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.UITypeEditors;
using Ringtoets.GrassCoverErosionInwards.Utils;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInputContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsInputContextProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>
    {
        private const int dikeProfilePropertyIndex = 1;
        private const int worldReferencePointPropertyIndex = 2;
        private const int orientationPropertyIndex = 3;
        private const int breakWaterPropertyIndex = 4;
        private const int foreshorePropertyIndex = 5;
        private const int dikeGeometryPropertyIndex = 6;
        private const int dikeHeightPropertyIndex = 7;
        private const int criticalFlowRatePropertyIndex = 8;
        private const int hydraulicBoundaryLocationPropertyIndex = 9;
        private const int calculateDikeHeightPropertyIndex = 10;

        [PropertyOrder(dikeProfilePropertyIndex)]
        [Editor(typeof(GrassCoverErosionInwardsInputContextDikeProfileEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "DikeProfile_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeProfile_Description")]
        public DikeProfile DikeProfile
        {
            get
            {
                return data.WrappedData.DikeProfile;
            }
            set
            {
                data.WrappedData.DikeProfile = value;
                AssignUnassignCalculations.Update(data.FailureMechanism.SectionResults, data.Calculation);
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "WorldReferencePoint_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WorldReferencePoint_Description")]
        public Point2D WorldReferencePoint
        {
            get
            {
                return data.WrappedData.DikeProfile == null ? null :
                           new Point2D(
                               new RoundedDouble(0, data.WrappedData.DikeProfile.WorldReferencePoint.X),
                               new RoundedDouble(0, data.WrappedData.DikeProfile.WorldReferencePoint.Y));
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "Orientation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Orientation_Description")]
        public RoundedDouble Orientation
        {
            get
            {
                return data.WrappedData.Orientation;
            }
            set
            {
                data.WrappedData.Orientation = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterProperties_Description")]
        public GrassCoverErosionInwardsInputContextBreakWaterProperties BreakWater
        {
            get
            {
                return new GrassCoverErosionInwardsInputContextBreakWaterProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ForeshoreProperties_Description")]
        public GrassCoverErosionInwardsInputContextForeshoreProperties Foreshore
        {
            get
            {
                return new GrassCoverErosionInwardsInputContextForeshoreProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(dikeGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "DikeGeometryProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeGeometryProperties_Description")]
        public GrassCoverErosionInwardsInputContextDikeGeometryProperties DikeGeometry
        {
            get
            {
                return new GrassCoverErosionInwardsInputContextDikeGeometryProperties
                {
                    Data = data
                };
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(dikeHeightPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "DikeHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeHeight_Description")]
        public RoundedDouble DikeHeight
        {
            get
            {
                return data.WrappedData.DikeHeight;
            }
            set
            {
                data.WrappedData.DikeHeight = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(calculateDikeHeightPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "CalculateDikeHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "CalculateDikeHeight_Description")]
        public bool CalculateDikeHeight
        {
            get
            {
                return data.WrappedData.CalculateDikeHeight;
            }
            set
            {
                data.WrappedData.CalculateDikeHeight = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(criticalFlowRatePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_CriticalValues")]
        [ResourcesDisplayName(typeof(Resources), "CriticalFlowRate_DisplayName")]
        [ResourcesDescription(typeof(Resources), "CriticalFlowRate_Description")]
        public LogNormalDistributionProperties CriticalFlowRate
        {
            get
            {
                return new LogNormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.CriticalFlowRate
                };
            }
        }

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(GrassCoverErosionInwardsInputContextHydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "HydraulicBoundaryLocation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "HydraulicBoundaryLocation_Description")]
        public HydraulicBoundaryLocation HydraulicBoundaryLocation
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryLocation;
            }
            set
            {
                data.WrappedData.HydraulicBoundaryLocation = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return data.WrappedData.DikeProfile == null;
        }

        public IEnumerable<DikeProfile> GetAvailableDikeProfiles()
        {
            return data.AvailableDikeProfiles;
        }

        public IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations()
        {
            return data.AvailableHydraulicBoundaryLocations;
        }
    }
}