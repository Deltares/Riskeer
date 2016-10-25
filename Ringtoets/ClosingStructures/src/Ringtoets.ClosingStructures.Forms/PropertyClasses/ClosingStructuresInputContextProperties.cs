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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Utils.Attributes;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PropertyClasses;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ClosingStructuresInputContext"/> for properties panel.
    /// </summary>
    public class ClosingStructuresInputContextProperties : StructuresInputBaseProperties<ClosingStructure, ClosingStructuresInput, ClosingStructuresCalculation, ClosingStructuresFailureMechanism>
    {
        private const int structurePropertyIndex = 1;
        private const int structureLocationPropertyIndex = 2;
        private const int structureNormalOrientationPropertyIndex = 3;
        private const int flowWidthAtBottomProtectionPropertyIndex = 4;
        private const int widthFlowAperturesPropertyIndex = 5;
        private const int storageStructureAreaPropertyIndex = 6;
        private const int allowedLevelIncreaseStoragePropertyIndex = 7;
        private const int criticalOvertoppingDischargePropertyIndex = 9;
        private const int failureProbabilityStructureWithErosionPropertyIndex = 10;
        private const int foreshoreProfilePropertyIndex = 11;
        private const int useBreakWaterPropertyIndex = 12;
        private const int useForeshorePropertyIndex = 13;
        private const int modelFactorSuperCriticalFlowPropertyIndex = 14;
        private const int hydraulicBoundaryLocationPropertyIndex = 15;
        private const int stormDurationPropertyIndex = 16;
        private const int deviationWaveDirectionPropertyIndex = 17;

        /// <summary>
        /// Creates a new instance of the <see cref="ClosingStructuresInputContextProperties"/> class.
        /// </summary>
        public ClosingStructuresInputContextProperties() : base(new ConstructionProperties
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

        #region Hydraulic data

        [PropertyOrder(deviationWaveDirectionPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), "Categories_HydraulicData")]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "Structure_DeviationWaveDirection_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "Structure_DeviationWaveDirection_Description")]
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

        public override IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
        {
            return data.FailureMechanism.ForeshoreProfiles;
        }

        public override IEnumerable<ClosingStructure> GetAvailableStructures()
        {
            return data.FailureMechanism.ClosingStructures;
        }

        protected override void AfterSettingStructure() {}

        #region Schematization

        #endregion
    }
}