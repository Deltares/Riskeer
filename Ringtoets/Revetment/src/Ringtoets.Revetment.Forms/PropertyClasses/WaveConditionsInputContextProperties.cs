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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Revetment.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveConditionsInputContext"/> for properties panel.
    /// </summary>
    public abstract class WaveConditionsInputContextProperties<T> : ObjectProperties<T>,
                                                                    IHasHydraulicBoundaryLocationProperty,
                                                                    IHasForeshoreProfileProperty
        where T : WaveConditionsInputContext
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 0;
        private const int assessmentLevelPropertyIndex = 1;
        private const int upperBoundaryDesignWaterLevelPropertyIndex = 2;
        private const int upperBoundaryRevetmentPropertyIndex = 3;
        private const int lowerBoundaryRevetmentPropertyIndex = 4;
        private const int upperBoundaryWaterLevelsPropertyIndex = 5;
        private const int lowerBoundaryWaterLevelsPropertyIndex = 6;
        private const int stepSizePropertyIndex = 7;
        private const int waterLevelsPropertyIndex = 8;

        private const int foreshoreProfilePropertyIndex = 9;
        private const int worldReferencePointPropertyIndex = 10;
        private const int orientationPropertyIndex = 11;
        private const int breakWaterPropertyIndex = 12;
        private const int foreshoreGeometryPropertyIndex = 13;
        private const int revetmentTypePropertyIndex = 14;

        [PropertyOrder(assessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "AssessmentLevel_Description")]
        public virtual RoundedDouble AssessmentLevel
        {
            get
            {
                return data.WrappedData.AssessmentLevel;
            }
        }

        [PropertyOrder(upperBoundaryDesignWaterLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsInput_UpperBoundaryDesignWaterLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WaveConditionsInput_UpperBoundaryDesignWaterLevel_Description")]
        public virtual RoundedDouble UpperBoundaryDesignWaterLevel
        {
            get
            {
                return data.WrappedData.UpperBoundaryDesignWaterLevel;
            }
        }

        [PropertyOrder(upperBoundaryRevetmentPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsInput_UpperBoundaryRevetment_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WaveConditionsInput_UpperBoundaryRevetment_Description")]
        public RoundedDouble UpperBoundaryRevetment
        {
            get
            {
                return data.WrappedData.UpperBoundaryRevetment;
            }
            set
            {
                data.WrappedData.UpperBoundaryRevetment = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(lowerBoundaryRevetmentPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsInput_LowerBoundaryRevetment_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WaveConditionsInput_LowerBoundaryRevetment_Description")]
        public RoundedDouble LowerBoundaryRevetment
        {
            get
            {
                return data.WrappedData.LowerBoundaryRevetment;
            }
            set
            {
                data.WrappedData.LowerBoundaryRevetment = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(upperBoundaryWaterLevelsPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsInput_UpperBoundaryWaterLevels_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WaveConditionsInput_UpperBoundaryWaterLevels_Description")]
        public RoundedDouble UpperBoundaryWaterLevels
        {
            get
            {
                return data.WrappedData.UpperBoundaryWaterLevels;
            }
            set
            {
                data.WrappedData.UpperBoundaryWaterLevels = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(lowerBoundaryWaterLevelsPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsInput_LowerBoundaryWaterLevels_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WaveConditionsInput_LowerBoundaryWaterLevels_Description")]
        public RoundedDouble LowerBoundaryWaterLevels
        {
            get
            {
                return data.WrappedData.LowerBoundaryWaterLevels;
            }
            set
            {
                data.WrappedData.LowerBoundaryWaterLevels = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(stepSizePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsInput_StepSize_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WaveConditionsInput_StepSize_Description")]
        public WaveConditionsInputStepSize StepSize
        {
            get
            {
                return data.WrappedData.StepSize;
            }
            set
            {
                data.WrappedData.StepSize = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(waterLevelsPropertyIndex)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsInput_WaterLevels_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WaveConditionsInput_WaterLevels_Description")]
        public RoundedDouble[] WaterLevels
        {
            get
            {
                return data.WrappedData.WaterLevels.ToArray();
            }
        }

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "WorldReferencePoint_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "WorldReferencePoint_ForeshoreProfile_Description")]
        public Point2D WorldReferencePoint
        {
            get
            {
                return data.WrappedData.ForeshoreProfile == null ? null :
                           new Point2D(
                               new RoundedDouble(0, data.WrappedData.ForeshoreProfile.WorldReferencePoint.X),
                               new RoundedDouble(0, data.WrappedData.ForeshoreProfile.WorldReferencePoint.Y));
            }
        }

        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Orientation_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Orientation_ForeshoreProfile_Description")]
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
                return data.WrappedData.ForeshoreProfile == null ?
                           new UseBreakWaterProperties() :
                           new UseBreakWaterProperties(data.WrappedData, data.Calculation);
            }
        }

        [PropertyOrder(foreshoreGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "ForeshoreProperties_Description")]
        public UseForeshoreProperties ForeshoreGeometry
        {
            get
            {
                return new UseForeshoreProperties(data.WrappedData);
            }
        }

        [PropertyOrder(revetmentTypePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "WaveConditionsInput_RevetmentType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WaveConditionsInput_RevetmentType_Description")]
        public abstract string RevetmentType { get; }

        [PropertyOrder(foreshoreProfilePropertyIndex)]
        [Editor(typeof(ForeshoreProfileEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "ForeshoreProfile_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ForeshoreProfile_Description")]
        public ForeshoreProfile ForeshoreProfile
        {
            get
            {
                return data.WrappedData.ForeshoreProfile;
            }
            set
            {
                data.WrappedData.ForeshoreProfile = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocation_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocation_Description")]
        public virtual SelectableHydraulicBoundaryLocation SelectedHydraulicBoundaryLocation
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryLocation != null
                           ? new SelectableHydraulicBoundaryLocation(data.WrappedData.HydraulicBoundaryLocation,
                                                                     WorldReferencePoint)
                           : null;
            }
            set
            {
                data.WrappedData.HydraulicBoundaryLocation = value.HydraulicBoundaryLocation;
                data.WrappedData.NotifyObservers();
            }
        }

        public virtual IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            return data.ForeshoreProfiles;
        }

        public IEnumerable<SelectableHydraulicBoundaryLocation> GetSelectableHydraulicBoundaryLocations()
        {
            return SelectableHydraulicBoundaryLocationHelper.GetSortedSelectableHydraulicBoundaryLocations(
                data.HydraulicBoundaryLocations, WorldReferencePoint);
        }
    }
}