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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Utils;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="StabilityPointStructuresInputContext"/> for properties panel.
    /// </summary>
    public class StabilityPointStructuresInputContextProperties : StructuresInputBaseProperties<StabilityPointStructure,
                                                                      StabilityPointStructuresInput,
                                                                      StructuresCalculation<StabilityPointStructuresInput>,
                                                                      StabilityPointStructuresFailureMechanism>
    {
        private const int hydraulicBoundaryLocationPropertyIndex = 1;
        private const int volumicWeightWaterPropertyIndex = 2;
        private const int stormDurationPropertyIndex = 3;
        private const int insideWaterLevelPropertyIndex = 4;
        private const int insideWaterLevelFailureConstructionPropertyIndex = 5;
        private const int flowVelocityStructureClosablePropertyIndex = 6;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 7;
        private const int drainCoefficientPropertyIndex = 8;
        private const int factorStormDurationOpenStructurePropertyIndex = 9;
        private const int structurePropertyIndex = 10;
        private const int structureLocationPropertyIndex = 11;
        private const int structureNormalOrientationPropertyIndex = 12;
        private const int inflowModelTypePropertyIndex = 13;
        private const int loadSchematizationTypePropertyIndex = 14;
        private const int widthFlowAperturesPropertyIndex = 15;
        private const int areaFlowAperturesPropertyIndex = 16;
        private const int flowWidthAtBottomProtectionPropertyIndex = 17;
        private const int storageStructureAreaPropertyIndex = 18;
        private const int allowedLevelIncreaseStoragePropertyIndex = 19;
        private const int levelCrestStructurePropertyIndex = 20;
        private const int thresholdHeightOpenWeirPropertyIndex = 21;
        private const int criticalOvertoppingDischargePropertyIndex = 22;
        private const int constructiveStrengthLinearLoadModelPropertyIndex = 23;
        private const int constructiveStrengthQuadraticLoadModelPropertyIndex = 24;
        private const int bankWidthPropertyIndex = 25;
        private const int evaluationLevelPropertyIndex = 26;
        private const int verticalDistancePropertyIndex = 27;
        private const int failureProbabilityRepairClosurePropertyIndex = 28;
        private const int failureCollisionEnergyPropertyIndex = 29;
        private const int shipMassPropertyIndex = 30;
        private const int shipVelocityPropertyIndex = 31;
        private const int levellingCountPropertyIndex = 32;
        private const int probabilityCollisionSecondaryStructurePropertyIndex = 33;
        private const int stabilityLinearLoadModelPropertyIndex = 34;
        private const int stabilityQuadraticLoadModelPropertyIndex = 35;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 36;
        private const int foreshoreProfilePropertyIndex = 37;
        private const int useBreakWaterPropertyIndex = 38;
        private const int useForeshorePropertyIndex = 39;

        /// <summary>
        /// Creates a new instance of the <see cref="StabilityPointStructuresInputContextProperties"/> class.
        /// </summary>
        public StabilityPointStructuresInputContextProperties()
            : base(new ConstructionProperties
            {
                StructurePropertyIndex = structurePropertyIndex,
                StructureLocationPropertyIndex = structureLocationPropertyIndex,
                StructureNormalOrientationPropertyIndex = structureNormalOrientationPropertyIndex,
                FlowWidthAtBottomProtectionPropertyIndex = flowWidthAtBottomProtectionPropertyIndex,
                WidthFlowAperturesPropertyIndex = widthFlowAperturesPropertyIndex,
                StorageStructureAreaPropertyIndex = storageStructureAreaPropertyIndex,
                AllowedLevelIncreaseStoragePropertyIndex = allowedLevelIncreaseStoragePropertyIndex,
                CriticalOvertoppingDischargePropertyIndex = criticalOvertoppingDischargePropertyIndex,
                FailureProbabilityStructureWithErosionPropertyIndex = failureProbabilityStructureWithErosionPropertyIndex,
                ForeshoreProfilePropertyIndex = foreshoreProfilePropertyIndex,
                UseBreakWaterPropertyIndex = useBreakWaterPropertyIndex,
                UseForeshorePropertyIndex = useForeshorePropertyIndex,
                ModelFactorSuperCriticalFlowPropertyIndex = modelFactorSuperCriticalFlowPropertyIndex,
                HydraulicBoundaryLocationPropertyIndex = hydraulicBoundaryLocationPropertyIndex,
                StormDurationPropertyIndex = stormDurationPropertyIndex
            }) {}

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            // Note: A default initialized calculation doesn't have InflowModelType initialized to any of these 2 values.
            if (data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.FloodedCulvert ||
                data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.LowSill)
            {
                if (propertyName == TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ModelFactorSuperCriticalFlow))
                {
                    return data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.LowSill;
                }
                if (propertyName == TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.DrainCoefficient))
                {
                    return data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.FloodedCulvert;
                }
                if (propertyName == TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.AreaFlowApertures))
                {
                    return data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.FloodedCulvert;
                }
                if (propertyName == TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.WidthFlowApertures))
                {
                    return data.WrappedData.InflowModelType == StabilityPointStructureInflowModelType.LowSill;
                }
            }

            // Note: A default initialized calculation doesn't have LoadSchematizationType initialized to any of these 2 values.
            if (data.WrappedData.LoadSchematizationType == LoadSchematizationType.Linear ||
                data.WrappedData.LoadSchematizationType == LoadSchematizationType.Quadratic)
            {
                if (propertyName == TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthLinearLoadModel))
                {
                    return data.WrappedData.LoadSchematizationType == LoadSchematizationType.Linear;
                }
                if (propertyName == TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.ConstructiveStrengthQuadraticLoadModel))
                {
                    return data.WrappedData.LoadSchematizationType == LoadSchematizationType.Quadratic;
                }
                if (propertyName == TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityLinearLoadModel))
                {
                    return data.WrappedData.LoadSchematizationType == LoadSchematizationType.Linear;
                }
                if (propertyName == TypeUtils.GetMemberName<StabilityPointStructuresInputContextProperties>(p => p.StabilityQuadraticLoadModel))
                {
                    return data.WrappedData.LoadSchematizationType == LoadSchematizationType.Quadratic;
                }
            }

            return true;
        }

        public override IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            return data.FailureMechanism.ForeshoreProfiles;
        }

        public override IEnumerable<StabilityPointStructure> GetAvailableStructures()
        {
            return data.FailureMechanism.StabilityPointStructures;
        }

        protected override void AfterSettingStructure()
        {
            StructuresHelper.Update(data.FailureMechanism.SectionResults, data.Calculation);
        }

        #region Hydraulic data

        [PropertyOrder(volumicWeightWaterPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "Structure_VolumicWeightWater_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_VolumicWeightWater_Description")]
        public RoundedDouble VolumicWeightWater
        {
            get
            {
                return data.WrappedData.VolumicWeightWater;
            }
            set
            {
                data.WrappedData.VolumicWeightWater = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(insideWaterLevelFailureConstructionPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(Resources), "Structure_InsideWaterLevelFailureConstruction_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_InsideWaterLevelFailureConstruction_Description")]
        public NormalDistributionProperties InsideWaterLevelFailureConstruction
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.InsideWaterLevelFailureConstruction
                };
            }
        }

        [PropertyOrder(insideWaterLevelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_InsideWaterLevel_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_InsideWaterLevel_Description")]
        public NormalDistributionProperties InsideWaterLevel
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.InsideWaterLevel
                };
            }
        }

        #endregion

        #region Model factors and critical values

        [DynamicVisible]
        public override NormalDistributionProperties ModelFactorSuperCriticalFlow
        {
            get
            {
                return base.ModelFactorSuperCriticalFlow;
            }
        }

        [DynamicVisible]
        [PropertyOrder(drainCoefficientPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_DrainCoefficient_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_DrainCoefficient_Description")]
        public NormalDistributionProperties DrainCoefficient
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.StandardDeviation, data.WrappedData)
                {
                    Data = data.WrappedData.DrainCoefficient
                };
            }
        }

        [PropertyOrder(factorStormDurationOpenStructurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_ModelSettings")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_FactorStormDurationOpenStructure_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_FactorStormDurationOpenStructure_Description")]
        public RoundedDouble FactorStormDurationOpenStructure
        {
            get
            {
                return data.WrappedData.FactorStormDurationOpenStructure;
            }
            set
            {
                data.WrappedData.FactorStormDurationOpenStructure = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(flowVelocityStructureClosablePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_CriticalValues")]
        [ResourcesDisplayName(typeof(Resources), "Structure_FlowVelocityStructureClosable_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_FlowVelocityStructureClosable_Description")]
        public NormalDistributionProperties FlowVelocityStructureClosable
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.FlowVelocityStructureClosable
                };
            }
        }

        #endregion

        #region Schematization

        [DynamicVisible]
        public override VariationCoefficientNormalDistributionProperties WidthFlowApertures
        {
            get
            {
                return base.WidthFlowApertures;
            }
        }

        [PropertyOrder(inflowModelTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_InflowModelType_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_InflowModelType_Description")]
        public StabilityPointStructureInflowModelType InflowModelType
        {
            get
            {
                return data.WrappedData.InflowModelType;
            }
            set
            {
                data.WrappedData.InflowModelType = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(loadSchematizationTypePropertyIndex)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_LoadSchematizationType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_LoadSchematizationType_Description")]
        public LoadSchematizationType LoadSchematizationType
        {
            get
            {
                return data.WrappedData.LoadSchematizationType;
            }
            set
            {
                data.WrappedData.LoadSchematizationType = value;
                data.WrappedData.NotifyObservers();
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

        [PropertyOrder(thresholdHeightOpenWeirPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_ThresholdHeightOpenWeir_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_ThresholdHeightOpenWeir_Description")]
        public NormalDistributionProperties ThresholdHeightOpenWeir
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.ThresholdHeightOpenWeir
                };
            }
        }

        [DynamicVisible]
        [PropertyOrder(areaFlowAperturesPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_AreaFlowApertures_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_AreaFlowApertures_Description")]
        public LogNormalDistributionProperties AreaFlowApertures
        {
            get
            {
                return new LogNormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.AreaFlowApertures
                };
            }
        }

        [DynamicVisible]
        [PropertyOrder(constructiveStrengthLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_ConstructiveStrengthLinearLoadModel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_ConstructiveStrengthLinearLoadModel_Description")]
        public VariationCoefficientLogNormalDistributionProperties ConstructiveStrengthLinearLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.ConstructiveStrengthLinearLoadModel
                };
            }
        }

        [DynamicVisible]
        [PropertyOrder(constructiveStrengthQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_ConstructiveStrengthQuadraticLoadModel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_ConstructiveStrengthQuadraticLoadModel_Description")]
        public VariationCoefficientLogNormalDistributionProperties ConstructiveStrengthQuadraticLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.ConstructiveStrengthQuadraticLoadModel
                };
            }
        }

        [DynamicVisible]
        [PropertyOrder(stabilityLinearLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_StabilityLinearLoadModel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_StabilityLinearLoadModel_Description")]
        public VariationCoefficientLogNormalDistributionProperties StabilityLinearLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.StabilityLinearLoadModel
                };
            }
        }

        [DynamicVisible]
        [PropertyOrder(stabilityQuadraticLoadModelPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_StabilityQuadraticLoadModel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_StabilityQuadraticLoadModel_Description")]
        public VariationCoefficientLogNormalDistributionProperties StabilityQuadraticLoadModel
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.StabilityQuadraticLoadModel
                };
            }
        }

        [PropertyOrder(failureProbabilityRepairClosurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_FailureProbabilityRepairClosure_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_FailureProbabilityRepairClosure_Description")]
        public string FailureProbabilityRepairClosure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.FailureProbabilityRepairClosure);
            }
            set
            {
                SetProbabilityValue(value, data.WrappedData, (wrappedData, parsedValue) => wrappedData.FailureProbabilityRepairClosure = parsedValue);
            }
        }

        [PropertyOrder(failureCollisionEnergyPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_FailureCollisionEnergy_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_FailureCollisionEnergy_Description")]
        public VariationCoefficientLogNormalDistributionProperties FailureCollisionEnergy
        {
            get
            {
                return new VariationCoefficientLogNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.FailureCollisionEnergy
                };
            }
        }

        [PropertyOrder(shipMassPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_ShipMass_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_ShipMass_Description")]
        public VariationCoefficientNormalDistributionProperties ShipMass
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.ShipMass
                };
            }
        }

        [PropertyOrder(shipVelocityPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_ShipVelocity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_ShipVelocity_Description")]
        public VariationCoefficientNormalDistributionProperties ShipVelocity
        {
            get
            {
                return new VariationCoefficientNormalDistributionProperties(VariationCoefficientDistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.ShipVelocity
                };
            }
        }

        [PropertyOrder(levellingCountPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_LevellingCount_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_LevellingCount_Description")]
        public int LevellingCount
        {
            get
            {
                return data.WrappedData.LevellingCount;
            }
            set
            {
                data.WrappedData.LevellingCount = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(probabilityCollisionSecondaryStructurePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_ProbabilityCollisionSecondaryStructure_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_ProbabilityCollisionSecondaryStructure_Description")]
        public string ProbabilityCollisionSecondaryStructure
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.WrappedData.ProbabilityCollisionSecondaryStructure);
            }
            set
            {
                SetProbabilityValue(value, data.WrappedData, (wrappedData, parsedValue) => wrappedData.ProbabilityCollisionSecondaryStructure = parsedValue);
            }
        }

        [PropertyOrder(bankWidthPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_BankWidth_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_BankWidth_Description")]
        public NormalDistributionProperties BankWidth
        {
            get
            {
                return new NormalDistributionProperties(DistributionPropertiesReadOnly.None, data.WrappedData)
                {
                    Data = data.WrappedData.BankWidth
                };
            }
        }

        [PropertyOrder(evaluationLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_EvaluationLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_EvaluationLevel_Description")]
        public RoundedDouble EvaluationLevel
        {
            get
            {
                return data.WrappedData.EvaluationLevel;
            }
            set
            {
                data.WrappedData.EvaluationLevel = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(verticalDistancePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_Schematization")]
        [ResourcesDisplayName(typeof(Resources), "Structure_VerticalDistance_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Structure_VerticalDistance_Description")]
        public RoundedDouble VerticalDistance
        {
            get
            {
                return data.WrappedData.VerticalDistance;
            }
            set
            {
                data.WrappedData.VerticalDistance = value;
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion
    }
}