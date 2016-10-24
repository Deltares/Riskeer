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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.Properties;
using Ringtoets.HeightStructures.Utils;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HeightStructuresInputContext"/> for properties panel.
    /// </summary>
    public class HeightStructuresInputContextProperties : ObjectProperties<HeightStructuresInputContext>,
                                                          IHasStructureProperty<HeightStructure>,
                                                          IHasForeshoreProfileProperty,
                                                          IHasHydraulicBoundaryLocationProperty
    {
        private const int heightStructurePropertyIndex = 1;
        private const int heightStructureLocationPropertyIndex = 2;
        private const int structureNormalOrientationPropertyIndex = 3;
        private const int flowWidthAtBottomProtectionPropertyIndex = 4;
        private const int widthFlowAperturesPropertyIndex = 5;
        private const int storageStructureAreaPropertyIndex = 6;
        private const int allowedLevelIncreaseStoragePropertyIndex = 7;
        private const int levelCrestStructurePropertyIndex = 8;
        private const int criticalOvertoppingDischargePropertyIndex = 9;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 10;
        private const int foreshoreProfilePropertyIndex = 11;
        private const int useBreakWaterPropertyIndex = 12;
        private const int useForeshorePropertyIndex = 13;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 14;
        private const int hydraulicBoundaryLocationPropertyIndex = 15;
        private const int stormDurationPropertyIndex = 16;
        private const int deviationWaveDirectionPropertyIndex = 17;

        #region Model factors

        [PropertyOrder(modelFactorSuperCriticalFlowPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_ModelFactorSuperCriticalFlow_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_ModelFactorSuperCriticalFlow_Description")]
        public NormalDistributionProperties ModelFactorSuperCriticalFlow
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.StandardDeviation, data.WrappedData)
                {
                    Data = data.WrappedData.ModelFactorSuperCriticalFlow
                };
            }
        }

        #endregion

        public IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            return data.FailureMechanism.ForeshoreProfiles;
        }

        public IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations()
        {
            return data.AvailableHydraulicBoundaryLocations;
        }

        public IEnumerable<HeightStructure> GetAvailableStructures()
        {
            return data.FailureMechanism.HeightStructures;
        }

        #region Schematization

        [PropertyOrder(heightStructurePropertyIndex)]
        [Editor(typeof(StructureEditor<HeightStructure>), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_Description")]
        public HeightStructure Structure
        {
            get
            {
                return data.WrappedData.Structure;
            }
            set
            {
                data.WrappedData.Structure = value;
                HeightStructuresHelper.Update(data.FailureMechanism.SectionResults, data.Calculation);
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(heightStructureLocationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_Location_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_Location_Description")]
        public Point2D HeightStructureLocation
        {
            get
            {
                return data.WrappedData.Structure == null ? null :
                           new Point2D(
                               new RoundedDouble(0, data.WrappedData.Structure.Location.X),
                               new RoundedDouble(0, data.WrappedData.Structure.Location.Y));
            }
        }

        [PropertyOrder(structureNormalOrientationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_StructureNormalOrientation_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_StructureNormalOrientation_Description")]
        public RoundedDouble StructureNormalOrientation
        {
            get
            {
                return data.WrappedData.StructureNormalOrientation;
            }
            set
            {
                data.WrappedData.StructureNormalOrientation = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(flowWidthAtBottomProtectionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_FlowWidthAtBottomProtection_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_FlowWidthAtBottomProtection_Description")]
        public LogNormalDistributionProperties FlowWidthAtBottomProtection
        {
            get
            {
                return new LogNormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.FlowWidthAtBottomProtection
                };
            }
        }

        [PropertyOrder(widthFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_WidthFlowApertures_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_WidthFlowApertures_Description")]
        public VariationCoefficientNormalDistributionProperties WidthFlowApertures
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.WidthFlowApertures
                };
            }
        }

        [PropertyOrder(storageStructureAreaPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_StorageStructureArea_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_StorageStructureArea_Description")]
        public VariationCoefficientLogNormalDistributionProperties StorageStructureArea
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.StorageStructureArea
                };
            }
        }

        [PropertyOrder(allowedLevelIncreaseStoragePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_AllowedLevelIncreaseStorage_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_AllowedLevelIncreaseStorage_Description")]
        public LogNormalDistributionProperties AllowedLevelIncreaseStorage
        {
            get
            {
                return new LogNormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.AllowedLevelIncreaseStorage
                };
            }
        }

        [PropertyOrder(levelCrestStructurePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_LevelCrestStructure_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_LevelCrestStructure_Description")]
        public NormalDistributionProperties LevelCrestStructure
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.LevelCrestStructure
                };
            }
        }

        [PropertyOrder(criticalOvertoppingDischargePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_CriticalOvertoppingDischarge_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_CriticalOvertoppingDischarge_Description")]
        public VariationCoefficientLogNormalDistributionProperties CriticalOvertoppingDischarge
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.CriticalOvertoppingDischarge
                };
            }
        }

        [PropertyOrder(failureProbabilityStructureWithErosionPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_FailureProbabilityStructureWithErosion_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_FailureProbabilityStructureWithErosion_Description")]
        public string FailureProbabilityStructureWithErosion
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.FailureProbabilityStructureWithErosion);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", RingtoetsCommonFormsResources.FailureProbabilityStructureWithErosion_Value_cannot_be_null);
                }
                try
                {
                    data.WrappedData.FailureProbabilityStructureWithErosion = (RoundedDouble) double.Parse(value);
                }
                catch (OverflowException)
                {
                    throw new ArgumentException(RingtoetsCommonFormsResources.FailureProbabilityStructureWithErosion_Value_too_large);
                }
                catch (FormatException)
                {
                    throw new ArgumentException(RingtoetsCommonFormsResources.FailureProbabilityStructureWithErosion_Could_not_parse_string_to_double_value);
                }
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(foreshoreProfilePropertyIndex)]
        [Editor(typeof(ForeshoreProfileEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_ForeshoreProfile_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_ForeshoreProfile_Description")]
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

        [PropertyOrder(useBreakWaterPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "BreakWaterProperties_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "BreakWaterProperties_Description")]
        public UseBreakWaterProperties UseBreakWater
        {
            get
            {
                return data.WrappedData.ForeshoreProfile == null ?
                           new UseBreakWaterProperties(null) :
                           new UseBreakWaterProperties(data.WrappedData);
            }
        }

        [PropertyOrder(useForeshorePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "ForeshoreProperties_Description")]
        public UseForeshoreProperties UseForeshore
        {
            get
            {
                return new UseForeshoreProperties(data.WrappedData);
            }
        }

        #endregion

        #region Hydraulic data

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
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

        [PropertyOrder(stormDurationPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_StormDuration_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_StormDuration_Description")]
        public VariationCoefficientLogNormalDistributionProperties StormDuration
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.CoefficientOfVariation, data.WrappedData)
                {
                    Data = data.WrappedData.StormDuration
                };
            }
        }

        [PropertyOrder(deviationWaveDirectionPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "DeviationWaveDirection_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DeviationWaveDirection_Description")]
        public RoundedDouble DeviationWaveDirection
        {
            get
            {
                return data.WrappedData.DeviationWaveDirection;
            }
            set
            {
                data.WrappedData.DeviationWaveDirection = value;
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion
    }
}