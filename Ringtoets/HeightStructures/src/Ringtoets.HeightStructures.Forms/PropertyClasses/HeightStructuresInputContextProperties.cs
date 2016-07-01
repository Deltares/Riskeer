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

namespace Ringtoets.HeightStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HeightStructuresInputContext"/> for properties panel.
    /// </summary>
    public class HeightStructuresInputContextProperties : ObjectProperties<HeightStructuresInputContext>
    {
        private const int orientationOfTheNormalOfTheStructurePropertyIndex = 1;
        private const int levelOfCrestOfStructurePropertyIndex = 2;
        private const int allowableIncreaseOfLevelForStoragePropertyIndex = 3;
        private const int storageStructureAreaPropertyIndex = 4;
        private const int flowWidthAtBottomProtectionPropertyIndex = 5;
        private const int widthOfFlowAperturesPropertyIndex = 6;
        private const int criticalOvertoppingDischargePropertyIndex = 7;
        private const int failureProbabilityOfStructureGivenErosionPropertyIndex = 8;
        private const int modelFactorOvertoppingSuperCriticalFlowPropertyIndex = 9;
        private const int hydraulicBoundaryLocationPropertyIndex = 10;
        private const int stormDurationPropertyIndex = 11;

        #region Model settings

        [PropertyOrder(modelFactorOvertoppingSuperCriticalFlowPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(Resources), "ModelFactorOvertoppingSuperCriticalFlow_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ModelFactorOvertoppingSuperCriticalFlow_Description")]
        public NormalDistributionProperties ModelFactorOvertoppingSuperCriticalFlow
        {
            get
            {
                return new NormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.StandardDeviation)
                {
                    Data = data.WrappedData.ModelFactorOvertoppingSuperCriticalFlow
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

        [PropertyOrder(orientationOfTheNormalOfTheStructurePropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "OrientationOfTheNormalOfTheStructure_DisplayName")]
        [ResourcesDescription(typeof(Resources), "OrientationOfTheNormalOfTheStructure_Description")]
        public RoundedDouble OrientationOfTheNormalOfTheStructure
        {
            get
            {
                return data.WrappedData.OrientationOfTheNormalOfTheStructure;
            }
            set
            {
                data.WrappedData.OrientationOfTheNormalOfTheStructure = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(levelOfCrestOfStructurePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "LevelOfCrestOfStructure_DisplayName")]
        [ResourcesDescription(typeof(Resources), "LevelOfCrestOfStructure_Description")]
        public NormalDistributionProperties LevelOfCrestOfStructure
        {
            get
            {
                return new NormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.LevelOfCrestOfStructure
                };
            }
        }

        [PropertyOrder(allowableIncreaseOfLevelForStoragePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "AllowableIncreaseOfLevelForStorage_DisplayName")]
        [ResourcesDescription(typeof(Resources), "AllowableIncreaseOfLevelForStorage_Description")]
        public LogNormalDistributionProperties AllowableIncreaseOfLevelForStorage
        {
            get
            {
                return new LogNormalDistributionProperties(data.WrappedData, DistributionPropertiesReadOnly.None)
                {
                    Data = data.WrappedData.AllowableIncreaseOfLevelForStorage
                };
            }
        }

        [PropertyOrder(storageStructureAreaPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
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
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
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
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
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
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
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

        [PropertyOrder(failureProbabilityOfStructureGivenErosionPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "FailureProbabilityOfStructureGivenErosion_DisplayName")]
        [ResourcesDescription(typeof(Resources), "FailureProbabilityOfStructureGivenErosion_Description")]
        public string FailureProbabilityOfStructureGivenErosion
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.FailureProbabilityOfStructureGivenErosion);
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", Resources.FailureProbabilityOfStructureGivenErosion_Value_cannot_be_null);
                }
                try
                {
                    data.WrappedData.FailureProbabilityOfStructureGivenErosion = (RoundedDouble) double.Parse(value);
                }
                catch (OverflowException)
                {
                    throw new ArgumentException(Resources.FailureProbabilityOfStructureGivenErosion_Value_too_large);
                }
                catch (FormatException)
                {
                    throw new ArgumentException(Resources.FailureProbabilityOfStructureGivenErosion_Could_not_parse_string_to_double_value);
                }
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion

        #region Hydraulic data

        [PropertyOrder(hydraulicBoundaryLocationPropertyIndex)]
        [Editor(typeof(HeightStructuresInputContextHydraulicBoundaryLocationEditor), typeof(UITypeEditor))]
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

        [PropertyOrder(stormDurationPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_HydraulicData")]
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