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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.StabilityPointStructures.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityPointStructure"/> for properties panel.
    /// </summary>
    public class StabilityPointStructureProperties : ObjectProperties<StabilityPointStructure>
    {
        private const int namePropertyIndex = 1;
        private const int locationPropertyIndex = 2;

        private const int insideWaterLevelPropertyIndex = 3;
        private const int insideWaterLevelFailureConstructionPropertyIndex = 4;

        private const int structureNormalOrientationPropertyIndex = 5;
        private const int stabilityPointStructureInflowModelTypePropertyIndex = 6;
        private const int widthFlowAperturesPropertyIndex = 7;
        private const int areaFlowAperturesPropertyIndex = 8;
        private const int flowWidthAtBottomProtectionPropertyIndex = 9;
        private const int storageStructureAreaPropertyIndex = 10;
        private const int allowedLevelIncreaseStoragePropertyIndex = 11;
        private const int levelCrestStructurePropertyIndex = 12;
        private const int thresholdHeightOpenWeirPropertyIndex = 13;
        private const int criticalOvertoppingDischargePropertyIndex = 14;
        private const int constructiveStrengthLinearLoadModelPropertyIndex = 15;
        private const int constructiveStrengthQuadraticLoadModelPropertyIndex = 16;
        private const int bankWidthPropertyIndex = 17;
        private const int evaluationLevelPropertyIndex = 18;
        private const int verticalDistancePropertyIndex = 19;
        private const int failureProbabilityRepairClosurePropertyIndex = 20;
        private const int failureCollisionEnergyPropertyIndex = 21;
        private const int shipMassPropertyIndex = 22;
        private const int shipVelocityPropertyIndex = 23;
        private const int levellingCountPropertyIndex = 24;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 25;
        private const int flowVelocityStructureClosablePropertyIndex = 26;
        private const int stabilityLinearLoadModelPropertyIndex = 27;
        private const int stabilityQuadraticLoadModelPropertyIndex = 28;
        
        #region General

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_Name_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(locationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_General")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_Location_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_Location_Description")]
        public Point2D Location
        {
            get
            {
                return new Point2D(new RoundedDouble(0, data.Location.X),
                                   new RoundedDouble(0, data.Location.Y));
            }
        }

        #endregion

        #region HydraulicData

        [PropertyOrder(insideWaterLevelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        public NormalDistributionProperties InsideWaterLevel
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.InsideWaterLevel
                };
            }
        }

        [PropertyOrder(insideWaterLevelFailureConstructionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        public NormalDistributionProperties InsideWaterLevelFailureConstruction
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.InsideWaterLevelFailureConstruction
                };
            }
        }

        #endregion

        #region Schematization

        [PropertyOrder(structureNormalOrientationPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_StructureNormalOrientation_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_StructureNormalOrientation_Description")]
        public RoundedDouble StructureNormalOrientation
        {
            get
            {
                return data.StructureNormalOrientation;
            }
        }

        [PropertyOrder(stabilityPointStructureInflowModelTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public StabilityPointStructureInflowModelType InflowModelType
        {
            get
            {
                return data.InflowModelType;
            }
        }

        [PropertyOrder(widthFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_WidthFlowApertures_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_WidthFlowApertures_Description")]
        public NormalDistributionVariationProperties WidthFlowApertures
        {
            get
            {
                return new NormalDistributionVariationProperties
                {
                    Data = data.WidthFlowApertures
                };
            }
        }

        [PropertyOrder(areaFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_AreaFlowApertures_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_AreaFlowApertures_Description")]
        public LogNormalDistributionProperties AreaFlowApertures
        {
            get
            {
                return new LogNormalDistributionProperties
                {
                    Data = data.AreaFlowApertures
                };
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
                return new LogNormalDistributionProperties
                {
                    Data = data.FlowWidthAtBottomProtection
                };
            }
        }

        [PropertyOrder(storageStructureAreaPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_StorageStructureArea_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_StorageStructureArea_Description")]
        public LogNormalDistributionVariationProperties StorageStructureArea
        {
            get
            {
                return new LogNormalDistributionVariationProperties
                {
                    Data = data.StorageStructureArea
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
                return new LogNormalDistributionProperties
                {
                    Data = data.AllowedLevelIncreaseStorage
                };
            }
        }

        [PropertyOrder(levelCrestStructurePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public NormalDistributionProperties LevelCrestStructure
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.LevelCrestStructure
                };
            }
        }

        [PropertyOrder(thresholdHeightOpenWeirPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public NormalDistributionProperties ThresholdHeightOpenWeir
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.ThresholdHeightOpenWeir
                };
            }
        }

        [PropertyOrder(criticalOvertoppingDischargePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_CriticalOvertoppingDischarge_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_CriticalOvertoppingDischarge_Description")]
        public LogNormalDistributionVariationProperties CriticalOvertoppingDischarge
        {
            get
            {
                return new LogNormalDistributionVariationProperties
                {
                    Data = data.CriticalOvertoppingDischarge
                };
            }
        }
        
        [PropertyOrder(constructiveStrengthLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public LogNormalDistributionVariationProperties ConstructiveStrengthLinearLoadModel
        {
            get
            {
                return new LogNormalDistributionVariationProperties
                {
                    Data = data.ConstructiveStrengthLinearLoadModel
                };
            }
        }

        [PropertyOrder(constructiveStrengthQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public LogNormalDistributionVariationProperties ConstructiveStrengthQuadraticLoadModel
        {
            get
            {
                return new LogNormalDistributionVariationProperties
                {
                    Data = data.ConstructiveStrengthQuadraticLoadModel
                };
            }
        }

        [PropertyOrder(bankWidthPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public NormalDistributionProperties BankWidth
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.BankWidth
                };
            }
        }

        [PropertyOrder(evaluationLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public RoundedDouble EvaluationLevel
        {
            get
            {
                return data.EvaluationLevel;
            }
        }

        [PropertyOrder(verticalDistancePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public RoundedDouble VerticalDistance
        {
            get
            {
                return data.VerticalDistance;
            }
        }

        [PropertyOrder(failureProbabilityRepairClosurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public string FailureProbabilityRepairClosure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.FailureProbabilityRepairClosure);
            }
        }

        [PropertyOrder(failureCollisionEnergyPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public LogNormalDistributionVariationProperties FailureCollisionEnergy
        {
            get
            {
                return new LogNormalDistributionVariationProperties
                {
                    Data = data.FailureCollisionEnergy
                };
            }
        }

        [PropertyOrder(shipMassPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public NormalDistributionVariationProperties ShipMass
        {
            get
            {
                return new NormalDistributionVariationProperties
                {
                    Data = data.ShipMass
                };
            }
        }

        [PropertyOrder(shipVelocityPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public NormalDistributionVariationProperties ShipVelocity
        {
            get
            {
                return new NormalDistributionVariationProperties
                {
                    Data = data.ShipVelocity
                };
            }
        }

        [PropertyOrder(levellingCountPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public string LevellingCount
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.LevellingCount);
            }
        }

        [PropertyOrder(probabilityCollisionSecondaryStructurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public string ProbabilityCollisionSecondaryStructure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.ProbabilityCollisionSecondaryStructure);
            }
        }

        [PropertyOrder(flowVelocityStructureClosablePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public NormalDistributionProperties FlowVelocityStructureClosable
        {
            get
            {
                return new NormalDistributionProperties
                {
                    Data = data.FlowVelocityStructureClosable
                };
            }
        }

        [PropertyOrder(stabilityLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public LogNormalDistributionVariationProperties StabilityLinearLoadModel
        {
            get
            {
                return new LogNormalDistributionVariationProperties
                {
                    Data = data.StabilityLinearLoadModel
                };
            }
        }

        [PropertyOrder(stabilityQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        public LogNormalDistributionVariationProperties StabilityQuadraticLoadModel
        {
            get
            {
                return new LogNormalDistributionVariationProperties
                {
                    Data = data.StabilityQuadraticLoadModel
                };
            }
        }

        #endregion
    }
}