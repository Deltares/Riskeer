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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Gui.Attributes;
using Core.Common.Util.Attributes;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.HeightStructures.Util;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.HeightStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HeightStructuresInputContext"/> for properties panel.
    /// </summary>
    public class HeightStructuresInputContextProperties : StructuresInputBaseProperties<
        HeightStructure,
        HeightStructuresInput,
        StructuresCalculation<HeightStructuresInput>,
        HeightStructuresFailureMechanism>
    {
        private const int structurePropertyIndex = 0;
        private const int structureLocationPropertyIndex = 1;
        private const int hydraulicBoundaryLocationPropertyIndex = 2;

        private const int modelFactorSuperCriticalFlowPropertyIndex = 3;

        private const int structureNormalOrientationPropertyIndex = 4;
        private const int levelCrestStructurePropertyIndex = 5;
        private const int widthFlowAperturesPropertyIndex = 6;
        private const int stormDurationPropertyIndex = 7;

        private const int criticalOvertoppingDischargePropertyIndex = 8;
        private const int flowWidthAtBottomProtectionPropertyIndex = 9;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 10;

        private const int storageStructureAreaPropertyIndex = 11;
        private const int allowedLevelIncreaseStoragePropertyIndex = 12;

        private const int foreshoreProfilePropertyIndex = 13;
        private const int useBreakWaterPropertyIndex = 14;
        private const int useForeshorePropertyIndex = 15;

        /// <summary>
        /// Creates a new instance of the <see cref="HeightStructuresInputContextProperties"/> class.
        /// </summary>
        /// <param name="data">The instance to show the properties of.</param>
        /// <param name="propertyChangeHandler">The handler responsible for handling effects of a property change.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HeightStructuresInputContextProperties(HeightStructuresInputContext data, IObservablePropertyChangeHandler propertyChangeHandler)
            : base(data, new ConstructionProperties
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
                HydraulicBoundaryLocationPropertyIndex = hydraulicBoundaryLocationPropertyIndex,
                StormDurationPropertyIndex = stormDurationPropertyIndex
            }, propertyChangeHandler) {}

        #region Model factors

        [PropertyOrder(modelFactorSuperCriticalFlowPropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_ModelFactors))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_Description))]
        public NormalDistributionProperties ModelFactorSuperCriticalFlow
        {
            get
            {
                return new NormalDistributionProperties(
                    DistributionPropertiesReadOnly.StandardDeviation,
                    data.WrappedData.ModelFactorSuperCriticalFlow,
                    PropertyChangeHandler);
            }
        }

        #endregion

        #region Schematization

        [PropertyOrder(levelCrestStructurePropertyIndex)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_Schematization_Incoming_flow))]
        [ResourcesDisplayName(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_LevelCrestStructure_DisplayName))]
        [ResourcesDescription(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Structure_LevelCrestStructure_Description))]
        public NormalDistributionProperties LevelCrestStructure
        {
            get
            {
                return new NormalDistributionProperties(
                    HasStructure()
                        ? DistributionPropertiesReadOnly.None
                        : DistributionPropertiesReadOnly.All,
                    data.WrappedData.LevelCrestStructure,
                    PropertyChangeHandler);
            }
        }

        #endregion

        public override IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            return data.FailureMechanism.ForeshoreProfiles;
        }

        public override IEnumerable<HeightStructure> GetAvailableStructures()
        {
            return data.FailureMechanism.HeightStructures;
        }

        protected override bool ShouldPropertyBeReadOnlyInAbsenseOfStructure(string property)
        {
            return nameof(FailureProbabilityStructureWithErosion).Equals(property)
                   || base.ShouldPropertyBeReadOnlyInAbsenseOfStructure(property);
        }

        protected override void AfterSettingStructure()
        {
            HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(data.FailureMechanism);
        }
    }
}