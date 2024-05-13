﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.Data.Entity;

namespace Riskeer.Storage.Core.DbContext
{
    /// <summary>
    /// Partial implementation of <see cref="RiskeerEntities"/> that support a connection string 
    /// and does not read the connection string from the configuration.
    /// </summary>
    public partial class RiskeerEntities
    {
        /// <summary>
        /// A new instance of <see cref="RiskeerEntities"/>.
        /// </summary>
        /// <param name="connString">A connection string.</param>
        public RiskeerEntities(string connString) : base(connString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Loads all tables into the context.
        /// </summary>
        public void LoadTablesIntoContext()
        {
            AssessmentSectionEntities.Load();
            AdoptableFailureMechanismSectionResultEntities.Load();
            BackgroundDataEntities.Load();
            BackgroundDataMetaEntities.Load();
            CalculationGroupEntities.Load();
            ClosingStructureEntities.Load();
            ClosingStructuresCalculationEntities.Load();
            ClosingStructuresFailureMechanismMetaEntities.Load();
            ClosingStructuresOutputEntities.Load();
            DikeProfileEntities.Load();
            DuneErosionFailureMechanismMetaEntities.Load();
            DuneLocationEntities.Load();
            DuneLocationCalculationForTargetProbabilityCollectionEntities.Include(e => e.DuneLocationCalculationEntities)
                                                                         .Load();
            DuneLocationCalculationEntities.Load();
            DuneLocationCalculationOutputEntities.Load();
            FailureMechanismEntities.Include(e => e.FailureMechanismSectionEntities)
                                    .Load();
            FailureMechanismSectionEntities.Load();
            ForeshoreProfileEntities.Load();
            GrassCoverErosionInwardsCalculationEntities.Load();
            GrassCoverErosionInwardsDikeHeightOutputEntities.Load();
            GrassCoverErosionInwardsFailureMechanismMetaEntities.Load();
            GrassCoverErosionInwardsOutputEntities.Load();
            GrassCoverErosionInwardsOvertoppingRateOutputEntities.Load();
            GrassCoverErosionOutwardsFailureMechanismMetaEntities.Load();
            GrassCoverErosionOutwardsWaveConditionsCalculationEntities.Load();
            GrassCoverErosionOutwardsWaveConditionsOutputEntities.Load();
            HeightStructureEntities.Load();
            HeightStructuresCalculationEntities.Load();
            HeightStructuresFailureMechanismMetaEntities.Load();
            HeightStructuresOutputEntities.Load();
            HydraulicBoundaryDataEntities.Load();
            HydraulicBoundaryDatabaseEntities.Load();
            HydraulicLocationEntities.Load();
            HydraulicLocationCalculationCollectionEntities.Include(e => e.HydraulicLocationCalculationEntities)
                                                          .Load();
            HydraulicLocationCalculationForTargetProbabilityCollectionEntities.Include(e => e.HydraulicLocationCalculationEntities)
                                                                              .Load();
            HydraulicLocationCalculationEntities.Load();
            HydraulicLocationOutputEntities.Load();
            MacroStabilityInwardsCharacteristicPointEntities.Load();
            MacroStabilityInwardsFailureMechanismMetaEntities.Load();
            MacroStabilityInwardsSoilProfileOneDEntities.Load();
            MacroStabilityInwardsSoilLayerOneDEntities.Load();
            MacroStabilityInwardsSoilProfileTwoDEntities.Include(e => e.MacroStabilityInwardsSoilLayerTwoDEntities)
                                                        .Load();
            MacroStabilityInwardsSoilLayerTwoDEntities.Load();
            MacroStabilityInwardsStochasticSoilProfileEntities.Load();
            MacroStabilityInwardsPreconsolidationStressEntities.Load();
            MacroStabilityInwardsCalculationEntities.Load();
            MacroStabilityInwardsCalculationOutputEntities.Load();
            NonAdoptableFailureMechanismSectionResultEntities.Load();
            ProjectEntities.Load();
            SemiProbabilisticPipingCalculationEntities.Load();
            SemiProbabilisticPipingCalculationOutputEntities.Load();
            ProbabilisticPipingCalculationEntities.Load();
            ProbabilisticPipingCalculationOutputEntities.Load();
            PipingCharacteristicPointEntities.Load();
            PipingFailureMechanismMetaEntities.Load();
            PipingScenarioConfigurationPerFailureMechanismSectionEntities.Load();
            PipingSoilLayerEntities.Load();
            PipingSoilProfileEntities.Load();
            PipingStochasticSoilProfileEntities.Load();
            StabilityPointStructureEntities.Load();
            StabilityPointStructuresCalculationEntities.Load();
            StabilityPointStructuresFailureMechanismMetaEntities.Load();
            StabilityPointStructuresOutputEntities.Load();
            StabilityStoneCoverFailureMechanismMetaEntities.Load();
            StabilityStoneCoverWaveConditionsCalculationEntities.Load();
            StabilityStoneCoverWaveConditionsOutputEntities.Load();
            StochasticSoilModelEntities.Load();
            SurfaceLineEntities.Load();
            WaveImpactAsphaltCoverFailureMechanismMetaEntities.Load();
            WaveImpactAsphaltCoverWaveConditionsCalculationEntities.Load();
            WaveImpactAsphaltCoverWaveConditionsOutputEntities.Load();

            SpecificFailureMechanismEntities.Include(e => e.FailureMechanismSectionEntities)
                                            .Load();

            LoadIllustrationPointsIntoContext();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            SQLiteConnectionHelper.ForcefullyDisposeSQLiteConnection();
        }

        private void LoadIllustrationPointsIntoContext()
        {
            FaultTreeIllustrationPointEntities.Include(ip => ip.SubMechanismIllustrationPointEntities)
                                              .Include(ip => ip.StochastEntities)
                                              .Load();
            GeneralResultFaultTreeIllustrationPointEntities.Include(gr => gr.StochastEntities)
                                                           .Load();
            GeneralResultSubMechanismIllustrationPointEntities.Include(grsm => grsm.StochastEntities)
                                                              .Load();
            StochastEntities.Load();
            TopLevelFaultTreeIllustrationPointEntities.Load();
            TopLevelSubMechanismIllustrationPointEntities.Load();
            SubMechanismIllustrationPointEntities.Load();
            SubMechanismIllustrationPointStochastEntities.Load();
            IllustrationPointResultEntities.Load();
        }
    }
}