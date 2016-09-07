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

using System.Collections;
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
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.UITypeEditors;
using Ringtoets.StabilityStoneCover.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.PropertyClasses
{
    public class StabilityStoneCoverWaveConditionsCalculationInputContextProperties : ObjectProperties<StabilityStoneCoverWaveConditionsCalculationInputContext>
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

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(StabilityStoneCoverWaveConditionsCalculationInputContextHydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocation_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "HydraulicBoundaryLocation_Description")]
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

        [PropertyOrder(assessmentLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "AssessmentLevel_Description")]
        public RoundedDouble AssessmentLevel
        {
            get
            {
                return data.WrappedData.AssessmentLevel;
            }
        }

        [PropertyOrder(upperBoundaryDesignWaterLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_UpperBoundaryDesignWaterLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_UpperBoundaryDesignWaterLevel_Description")]
        public RoundedDouble UpperBoundaryDesignWaterLevel
        {
            get
            {
                return data.WrappedData.UpperBoundaryDesignWaterLevel;
            }
        }

        [PropertyOrder(upperBoundaryRevetmentPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_UpperBoundaryRevetment_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_UpperBoundaryRevetment_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_LowerBoundaryRevetment_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_LowerBoundaryRevetment_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_UpperBoundaryWaterLevels_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_UpperBoundaryWaterLevels_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_LowerBoundaryWaterLevels_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_LowerBoundaryWaterLevels_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_StepSize_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_StepSize_Description")]
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
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_WaterLevels_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_WaterLevels_Description")]
        public RoundedDouble[] WaterLevels
        {
            get
            {
                return data.WrappedData.WaterLevels.ToArray();
            }
        }

        [PropertyOrder(foreshoreProfilePropertyIndex)]
        [Editor(typeof(StabilityStoneCoverWaveConditionsCalculationInputContextForeshoreProfileEditor), typeof(UITypeEditor))]
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

        [PropertyOrder(worldReferencePointPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "WorldReferencePoint_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "WorldReferencePoint_ForeshoreProfile_Description")]
        public Point2D WorldReferencePoint
        {
            get
            {
                if (data.WrappedData.ForeshoreProfile != null && data.WrappedData.ForeshoreProfile.WorldReferencePoint != null)
                {
                    return new Point2D(
                        new RoundedDouble(0, data.WrappedData.ForeshoreProfile.WorldReferencePoint.X),
                        new RoundedDouble(0, data.WrappedData.ForeshoreProfile.WorldReferencePoint.Y));
                }
                return null;
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
                if (data.WrappedData.ForeshoreProfile != null)
                {
                    return data.WrappedData.ForeshoreProfile.Orientation;
                }
                return new RoundedDouble(2, double.NaN);
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(breakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "BreakWaterProperties_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "BreakWaterProperties_Description")]
        public WaveConditionsInputBreakWaterProperties BreakWater
        {
            get
            {
                return new WaveConditionsInputBreakWaterProperties
                {
                    Data = data.WrappedData
                };
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(foreshoreGeometryPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "ForeshoreProperties_Description")]
        public WaveConditionsInputForeshoreProfileProperties ForeshoreGeometry
        {
            get
            {
                return new WaveConditionsInputForeshoreProfileProperties
                {
                    Data = data.WrappedData
                };
            }
        }

        [PropertyOrder(revetmentTypePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_RevetmentType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StabilityStoneCoverWaveConditionsCalculationInput_RevetmentType_Description")]
        public string RevetmentType
        {
            get
            {
                return Resources.StabilityStoneCover_StoneRevetment_DisplayName;
            }
        }

        public IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations()
        {
            return data.AvailableHydraulicBoundaryLocations;
        }

        public IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            return data.AvailableForeshoreProfiles;
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (data.WrappedData.ForeshoreProfile == null &&
                propertyName.Equals(TypeUtils.GetMemberName<StabilityStoneCoverWaveConditionsCalculationInputContextProperties>(i => i.ForeshoreGeometry)))
            {
                return true;
            }

            return false;
        }
    }
}