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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.Properties;
using Ringtoets.HeightStructures.Forms.UITypeEditors;
using Ringtoets.HydraRing.Data;
using CoreCommonBasePropertiesResources = Core.Common.Base.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HeightStructuresInputContext"/> for properties panel.
    /// </summary>
    public class HeightStructuresInputContextProperties : ObjectProperties<HeightStructuresInputContext>
    {
        private const int structureNormalOrientationPropertyIndex = 1;
        private const int levelCrestStructurePropertyIndex = 2;
        private const int allowedLevelIncreaseStoragePropertyIndex = 3;
        private const int storageStructureAreaPropertyIndex = 4;
        private const int flowWidthAtBottomProtectionPropertyIndex = 5;
        private const int widthOfFlowAperturesPropertyIndex = 6;
        private const int criticalOvertoppingDischargePropertyIndex = 7;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 8;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 9;
        private const int hydraulicBoundaryLocationPropertyIndex = 10;
        private const int stormDurationPropertyIndex = 11;

        #region Model settings

        [PropertyOrder(modelFactorSuperCriticalFlowPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "ModelFactorSuperCriticalFlow_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ModelFactorSuperCriticalFlow_Description")]
        public NormalDistributionProperties ModelFactorSuperCriticalFlow
        {
            get
            {
                return new NormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.StandardDeviation)
                {
                    Data = data.WrappedData.ModelFactorSuperCriticalFlow
                };
            }
        }

        #endregion

        /// <summary>
        /// Returns the available hydraulic boundary locations in order for the user to select one to 
        /// set <see cref="HeightStructuresInput.HydraulicBoundaryLocation"/>.</summary>
        /// <returns>The available hydraulic boundary locations.</returns>
        public IEnumerable<HydraulicBoundaryLocation> GetAvailableHydraulicBoundaryLocations()
        {
            return data.AvailableHydraulicBoundaryLocations;
        }

        #region Schematisation

        [PropertyOrder(structureNormalOrientationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "StructureNormalOrientation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StructureNormalOrientation_Description")]
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

        [PropertyOrder(levelCrestStructurePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "LevelCrestStructure_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LevelCrestStructure_Description")]
        public NormalDistributionProperties LevelCrestStructure
        {
            get
            {
                return new NormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.LevelCrestStructure
                };
            }
        }

        [PropertyOrder(allowedLevelIncreaseStoragePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "AllowedLevelIncreaseStorage_DisplayName")]
        [ResourcesDescription(typeof(Resources), "AllowedLevelIncreaseStorage_Description")]
        public LogNormalDistributionProperties AllowedLevelIncreaseStorage
        {
            get
            {
                return new LogNormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.AllowedLevelIncreaseStorage
                };
            }
        }

        [PropertyOrder(storageStructureAreaPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "StorageStructureArea_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StorageStructureArea_Description")]
        public LogNormalDistributionVariationProperties StorageStructureArea
        {
            get
            {
                return new LogNormalDistributionVariationProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.StorageStructureArea
                };
            }
        }

        [PropertyOrder(flowWidthAtBottomProtectionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "FlowWidthAtBottomProtection_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FlowWidthAtBottomProtection_Description")]
        public LogNormalDistributionProperties FlowWidthAtBottomProtection
        {
            get
            {
                return new LogNormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.FlowWidthAtBottomProtection
                };
            }
        }

        [PropertyOrder(widthOfFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "WidthOfFlowApertures_DisplayName")]
        [ResourcesDescription(typeof(Resources), "WidthOfFlowApertures_Description")]
        public NormalDistributionProperties WidthOfFlowApertures
        {
            get
            {
                return new NormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.WidthOfFlowApertures
                };
            }
        }

        [PropertyOrder(criticalOvertoppingDischargePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "CriticalOvertoppingDischarge_DisplayName")]
        [ResourcesDescription(typeof(Resources), "CriticalOvertoppingDischarge_Description")]
        public LogNormalDistributionVariationProperties CriticalOvertoppingDischarge
        {
            get
            {
                return new LogNormalDistributionVariationProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.CriticalOvertoppingDischarge
                };
            }
        }

        [PropertyOrder(failureProbabilityStructureWithErosionPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "FailureProbabilityStructureWithErosion_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FailureProbabilityStructureWithErosion_Description")]
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
                    throw new ArgumentNullException("value", Resources.FailureProbabilityStructureWithErosion_Value_cannot_be_null);
                }
                try
                {
                    data.WrappedData.FailureProbabilityStructureWithErosion = (RoundedDouble) double.Parse(value);
                }
                catch (OverflowException)
                {
                    throw new ArgumentException(Resources.FailureProbabilityStructureWithErosion_Value_too_large);
                }
                catch (FormatException)
                {
                    throw new ArgumentException(Resources.FailureProbabilityStructureWithErosion_Could_not_parse_string_to_double_value);
                }
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion

        #region Hydraulic data

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HeightStructuresInputContextHydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
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
        [ResourcesDisplayName(typeof(Resources), "StormDuration_DisplayName")]
        [ResourcesDescription(typeof(Resources), "StormDuration_Description")]
        public LogNormalDistributionVariationProperties StormDuration
        {
            get
            {
                return new LogNormalDistributionVariationProperties(data.WrappedData, DistributionPropertiesReadOnly.VariationCoefficient)
                {
                    Data = data.WrappedData.StormDuration
                };
            }
        }

        #endregion
    }
}