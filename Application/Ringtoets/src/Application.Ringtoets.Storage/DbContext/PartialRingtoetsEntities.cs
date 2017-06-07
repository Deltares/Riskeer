// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Data.Entity;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Partial implementation of <see cref="RingtoetsEntities"/> that support a connection string 
    /// and does not read the connection string from the configuration.
    /// </summary>
    public partial class RingtoetsEntities
    {
        /// <summary>
        /// A new instance of <see cref="RingtoetsEntities"/>.
        /// </summary>
        /// <param name="connString">A connection string.</param>
        public RingtoetsEntities(string connString) : base(connString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Loads all tables into the context.
        /// </summary>
        public void LoadTablesIntoContext()
        {
            AssessmentSectionEntities.Load();
            BackgroundDataEntities.Load();
            BackgroundDataMetaEntities.Load();
            CalculationGroupEntities.Load();
            CharacteristicPointEntities.Load();
            ClosingStructureEntities.Load();
            ClosingStructuresCalculationEntities.Load();
            ClosingStructuresFailureMechanismMetaEntities.Load();
            ClosingStructuresOutputEntities.Load();
            ClosingStructuresSectionResultEntities.Load();
            DikeProfileEntities.Load();
            DuneErosionFailureMechanismMetaEntities.Load();
            DuneErosionSectionResultEntities.Load();
            DuneLocationEntities.Load();
            DuneLocationOutputEntities.Load();
            DuneLocationOutputEntities.Load();
            FailureMechanismEntities.Load();
            FailureMechanismSectionEntities.Load();
            ForeshoreProfileEntities.Load();
            GrassCoverErosionInwardsCalculationEntities.Load();
            GrassCoverErosionInwardsDikeHeightOutputEntities.Load();
            GrassCoverErosionInwardsFailureMechanismMetaEntities.Load();
            GrassCoverErosionInwardsOutputEntities.Load();
            GrassCoverErosionInwardsOvertoppingRateOutputEntities.Load();
            GrassCoverErosionInwardsSectionResultEntities.Load();
            GrassCoverErosionOutwardsFailureMechanismMetaEntities.Load();
            GrassCoverErosionOutwardsHydraulicLocationEntities.Load();
            GrassCoverErosionOutwardsHydraulicLocationOutputEntities.Load();
            GrassCoverErosionOutwardsSectionResultEntities.Load();
            GrassCoverErosionOutwardsWaveConditionsCalculationEntities.Load();
            GrassCoverErosionOutwardsWaveConditionsOutputEntities.Load();
            GrassCoverSlipOffInwardsSectionResultEntities.Load();
            GrassCoverSlipOffOutwardsSectionResultEntities.Load();
            HeightStructureEntities.Load();
            HeightStructuresCalculationEntities.Load();
            HeightStructuresFailureMechanismMetaEntities.Load();
            HeightStructuresOutputEntities.Load();
            HeightStructuresSectionResultEntities.Load();
            HydraulicLocationEntities.Load();
            HydraulicLocationOutputEntities.Load();
            MacroStabilityInwardsSectionResultEntities.Load();
            MacrostabilityOutwardsSectionResultEntities.Load();
            MicrostabilitySectionResultEntities.Load();
            ProjectEntities.Load();
            PipingCalculationEntities.Load();
            PipingCalculationOutputEntities.Load();
            PipingFailureMechanismMetaEntities.Load();
            PipingSectionResultEntities.Load();
            PipingSemiProbabilisticOutputEntities.Load();
            PipingStructureSectionResultEntities.Load();
            SoilLayerEntities.Load();
            SoilProfileEntities.Load();
            StabilityPointStructureEntities.Load();
            StabilityPointStructuresCalculationEntities.Load();
            StabilityPointStructuresFailureMechanismMetaEntities.Load();
            StabilityPointStructuresOutputEntities.Load();
            StabilityPointStructuresSectionResultEntities.Load();
            StabilityStoneCoverFailureMechanismMetaEntities.Load();
            StabilityStoneCoverSectionResultEntities.Load();
            StabilityStoneCoverWaveConditionsCalculationEntities.Load();
            StabilityStoneCoverWaveConditionsOutputEntities.Load();
            StochasticSoilModelEntities.Load();
            StochasticSoilProfileEntities.Load();
            StrengthStabilityLengthwiseConstructionSectionResultEntities.Load();
            SurfaceLineEntities.Load();
            TechnicalInnovationSectionResultEntities.Load();
            WaterPressureAsphaltCoverSectionResultEntities.Load();
            WaveImpactAsphaltCoverFailureMechanismMetaEntities.Load();
            WaveImpactAsphaltCoverSectionResultEntities.Load();
            WaveImpactAsphaltCoverWaveConditionsCalculationEntities.Load();
            WaveImpactAsphaltCoverWaveConditionsOutputEntities.Load();
        }
    }
}