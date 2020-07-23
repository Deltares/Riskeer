// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityPointStructure"/> for properties panel.
    /// </summary>
    public class StabilityPointStructureProperties : ObjectProperties<StabilityPointStructure>
    {
        private const int idPropertyIndex = 1;
        private const int namePropertyIndex = 2;
        private const int locationPropertyIndex = 3;

        private const int insideWaterLevelFailureConstructionPropertyIndex = 4;
        private const int bankWidthPropertyIndex = 5;
        private const int verticalDistancePropertyIndex = 6;

        private const int constructiveStrengthQuadraticLoadModelPropertyIndex = 7;
        private const int constructiveStrengthLinearLoadModelPropertyIndex = 8;
        private const int evaluationLevelPropertyIndex = 9;
        private const int failureProbabilityRepairClosurePropertyIndex = 10;

        private const int stabilityPointStructureInflowModelTypePropertyIndex = 11;
        private const int structureNormalOrientationPropertyIndex = 12;
        private const int levelCrestStructurePropertyIndex = 13;
        private const int insideWaterLevelPropertyIndex = 14;
        private const int thresholdHeightOpenWeirPropertyIndex = 15;
        private const int widthFlowAperturesPropertyIndex = 16;
        private const int areaFlowAperturesPropertyIndex = 17;

        private const int criticalOvertoppingDischargePropertyIndex = 18;
        private const int flowWidthAtBottomProtectionPropertyIndex = 19;

        private const int storageStructureAreaPropertyIndex = 20;
        private const int allowedLevelIncreaseStoragePropertyIndex = 21;

        private const int stabilityQuadraticLoadModelPropertyIndex = 22;
        private const int stabilityLinearLoadModelPropertyIndex = 23;

        private const int failureCollisionEnergyPropertyIndex = 24;
        private const int shipMassPropertyIndex = 25;
        private const int shipVelocityPropertyIndex = 26;
        private const int levellingCountPropertyIndex = 27;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 28;
        private const int flowVelocityStructureClosablePropertyIndex = 29;

        #region General

        [PropertyOrder(idPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Id_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_Id_Description))]
        public string Id
        {
            get
            {
                return data.Id;
            }
        }

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_Name_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(locationPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_Location_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_Location_Description))]
        public Point2D Location
        {
            get
            {
                return new Point2D(new RoundedDouble(0, data.Location.X),
                                   new RoundedDouble(0, data.Location.Y));
            }
        }

        #endregion

        #region Schematization

        [PropertyOrder(insideWaterLevelFailureConstructionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_InsideWaterLevelFailureConstruction_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_InsideWaterLevelFailureConstruction_Description))]
        public NormalDistributionProperties InsideWaterLevelFailureConstruction
        {
            get
            {
                return new NormalDistributionProperties(data.InsideWaterLevelFailureConstruction);
            }
        }

        [PropertyOrder(bankWidthPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_BankWidth_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_BankWidth_Description))]
        public NormalDistributionProperties BankWidth
        {
            get
            {
                return new NormalDistributionProperties(data.BankWidth);
            }
        }

        [PropertyOrder(verticalDistancePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_VerticalDistance_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_VerticalDistance_Description))]
        public RoundedDouble VerticalDistance
        {
            get
            {
                return data.VerticalDistance;
            }
        }

        [PropertyOrder(constructiveStrengthQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthQuadraticLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthQuadraticLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties ConstructiveStrengthQuadraticLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(data.ConstructiveStrengthQuadraticLoadModel);
            }
        }

        [PropertyOrder(constructiveStrengthLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthLinearLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ConstructiveStrengthLinearLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties ConstructiveStrengthLinearLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(data.ConstructiveStrengthLinearLoadModel);
            }
        }

        [PropertyOrder(evaluationLevelPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Evaluation_Level_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_EvaluationLevel_Description))]
        public RoundedDouble EvaluationLevel
        {
            get
            {
                return data.EvaluationLevel;
            }
        }

        [PropertyOrder(failureProbabilityRepairClosurePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FailureProbabilityRepairClosure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FailureProbabilityRepairClosure_Description))]
        public string FailureProbabilityRepairClosure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.FailureProbabilityRepairClosure);
            }
        }

        [PropertyOrder(stabilityPointStructureInflowModelTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_InflowModelType_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_InflowModelType_Description))]
        public StabilityPointStructureInflowModelType InflowModelType
        {
            get
            {
                return data.InflowModelType;
            }
        }

        [PropertyOrder(structureNormalOrientationPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_StructureNormalOrientation_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_StructureNormalOrientation_Description))]
        public RoundedDouble StructureNormalOrientation
        {
            get
            {
                return data.StructureNormalOrientation;
            }
        }

        [PropertyOrder(levelCrestStructurePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_LevelCrestStructure_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_LevelCrestStructure_Description))]
        public NormalDistributionProperties LevelCrestStructure
        {
            get
            {
                return new NormalDistributionProperties(data.LevelCrestStructure);
            }
        }

        [PropertyOrder(insideWaterLevelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_InsideWaterLevel_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_InsideWaterLevel_Description))]
        public NormalDistributionProperties InsideWaterLevel
        {
            get
            {
                return new NormalDistributionProperties(data.InsideWaterLevel);
            }
        }

        [PropertyOrder(thresholdHeightOpenWeirPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_ThresholdHeightOpenWeir_Description))]
        public NormalDistributionProperties ThresholdHeightOpenWeir
        {
            get
            {
                return new NormalDistributionProperties(data.ThresholdHeightOpenWeir);
            }
        }

        [PropertyOrder(widthFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_WidthFlowApertures_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_WidthFlowApertures_Description))]
        public NormalDistributionProperties WidthFlowApertures
        {
            get
            {
                return new NormalDistributionProperties(data.WidthFlowApertures);
            }
        }

        [PropertyOrder(areaFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_AreaFlowApertures_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_AreaFlowApertures_Description))]
        public LogNormalDistributionProperties AreaFlowApertures
        {
            get
            {
                return new LogNormalDistributionProperties(data.AreaFlowApertures);
            }
        }

        [PropertyOrder(criticalOvertoppingDischargePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_CriticalOvertoppingDischarge_Description))]
        public VariationCoefficientLogNormalDistributionProperties CriticalOvertoppingDischarge
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(data.CriticalOvertoppingDischarge);
            }
        }

        [PropertyOrder(flowWidthAtBottomProtectionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_FlowWidthAtBottomProtection_Description))]
        public LogNormalDistributionProperties FlowWidthAtBottomProtection
        {
            get
            {
                return new LogNormalDistributionProperties(data.FlowWidthAtBottomProtection);
            }
        }

        [PropertyOrder(storageStructureAreaPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_StorageStructureArea_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_StorageStructureArea_Description))]
        public VariationCoefficientLogNormalDistributionProperties StorageStructureArea
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(data.StorageStructureArea);
            }
        }

        [PropertyOrder(allowedLevelIncreaseStoragePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_AllowedLevelIncreaseStorage_Description))]
        public LogNormalDistributionProperties AllowedLevelIncreaseStorage
        {
            get
            {
                return new LogNormalDistributionProperties(data.AllowedLevelIncreaseStorage);
            }
        }

        [PropertyOrder(stabilityQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StabilityQuadraticLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StabilityQuadraticLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties StabilityQuadraticLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(data.StabilityQuadraticLoadModel);
            }
        }

        [PropertyOrder(stabilityLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_StabilityLinearLoadModel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_StabilityLinearLoadModel_Description))]
        public VariationCoefficientLogNormalDistributionProperties StabilityLinearLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(data.StabilityLinearLoadModel);
            }
        }

        [PropertyOrder(failureCollisionEnergyPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FailureCollisionEnergy_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FailureCollisionEnergy_Description))]
        public VariationCoefficientLogNormalDistributionProperties FailureCollisionEnergy
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(data.FailureCollisionEnergy);
            }
        }

        [PropertyOrder(shipMassPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ShipMass_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ShipMass_Description))]
        public VariationCoefficientNormalDistributionProperties ShipMass
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(data.ShipMass);
            }
        }

        [PropertyOrder(shipVelocityPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ShipVelocity_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ShipVelocity_Description))]
        public VariationCoefficientNormalDistributionProperties ShipVelocity
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(data.ShipVelocity);
            }
        }

        [PropertyOrder(levellingCountPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_LevellingCount_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_LevellingCount_Description))]
        public int LevellingCount
        {
            get
            {
                return data.LevellingCount;
            }
        }

        [PropertyOrder(probabilityCollisionSecondaryStructurePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_ProbabilityCollisionSecondaryStructure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_ProbabilityCollisionSecondaryStructure_Description))]
        public string ProbabilityCollisionSecondaryStructure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.ProbabilityCollisionSecondaryStructure);
            }
        }

        [PropertyOrder(flowVelocityStructureClosablePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Structure_FlowVelocityStructureClosable_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Structure_FlowVelocityStructureClosable_Description))]
        public VariationCoefficientNormalDistributionProperties FlowVelocityStructureClosable
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(data.FlowVelocityStructureClosable);
            }
        }

        #endregion
    }
}