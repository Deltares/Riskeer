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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.UITypeEditors;
using Ringtoets.GrassCoverErosionInwards.Utils;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInputContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsInputContextProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>,
                                                                  IHasHydraulicBoundaryLocationProperty
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
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
                GrassCoverErosionInwardsHelper.Update(data.FailureMechanism.SectionResults, data.Calculation);
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "WorldReferencePoint_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WorldReferencePoint_DikeProfile_Description")]
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Orientation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Orientation_DikeProfile_Description")]
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "BreakWaterProperties_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "BreakWaterProperties_Description")]
        public UseBreakWaterProperties BreakWater
        {
            get
            {
                return data.WrappedData.DikeProfile == null ?
                           new UseBreakWaterProperties(null) :
                           new UseBreakWaterProperties(data.WrappedData);
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "ForeshoreProperties_Description")]
        public UseForeshoreProperties Foreshore
        {
            get
            {
                return new UseForeshoreProperties(data.WrappedData);
            }
        }

        [PropertyOrder(dikeGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
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
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "DikeHeightCalculationType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeHeightCalculationType_Description")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public DikeHeightCalculationType DikeHeightCalculationType
        {
            get
            {
                return data.WrappedData.DikeHeightCalculationType;
            }
            set
            {
                data.WrappedData.DikeHeightCalculationType = value;
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
                return new LogNormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.CriticalFlowRate
                };
            }
        }

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocation_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocation_Description")]
        public SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryLocation != null
                           ? new SelectableHydraulicBoundaryLocation(data.WrappedData.HydraulicBoundaryLocation, WorldReferencePoint)
                           : null;
            }
            set
            {
                data.WrappedData.HydraulicBoundaryLocation = value.HydraulicBoundaryLocation;
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return data.WrappedData.DikeProfile == null;
        }

        /// <summary>
        /// Returns the collection of available dike profiles.
        /// </summary>
        /// <returns>A collection of dike profiles.</returns>
        public IEnumerable<DikeProfile> GetAvailableDikeProfiles()
        {
            return data.AvailableDikeProfiles;
        }

        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            var calculationLocation = data.WrappedData.DikeProfile != null ? data.WrappedData.DikeProfile.WorldReferencePoint : null;

            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                data.AvailableHydraulicBoundaryLocations, calculationLocation);
        }
    }
}