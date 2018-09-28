// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Ringtoets.Common.IO.Configurations
{
    /// <summary>
    /// Container of general identifiers related to configuration schema definitions.
    /// </summary>
    public static class ConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The identifier for configuration elements.
        /// </summary>
        public const string ConfigurationElement = "configuratie";

        /// <summary>
        /// The identifier for calculation elements.
        /// </summary>
        public const string CalculationElement = "berekening";

        /// <summary>
        /// The identifier for folder elements.
        /// </summary>
        public const string FolderElement = "map";

        /// <summary>
        /// The identifier for name attributes.
        /// </summary>
        public const string NameAttribute = "naam";

        /// <summary>
        /// The old identifier for hydraulic boundary location elements.
        /// </summary>
        /// <remarks>This property represents the element name before the rename.</remarks>
        public const string HydraulicBoundaryLocationElementOld = "hrlocatie";

        /// <summary>
        /// The new identifier for hydraulic boundary location elements.
        /// </summary>
        /// <remarks>This property represents the element name after the rename.</remarks>
        public const string HydraulicBoundaryLocationElementNew = "hblocatie";

        /// <summary>
        /// The tag of elements containing the orientation of the profile.
        /// </summary>
        public const string Orientation = "orientatie";

        /// <summary>
        /// The tag of the element containing the value whether illustration points should be read for the calculation.
        /// </summary>
        public const string ShouldIllustrationPointsBeCalculatedElement = "illustratiepunteninlezen";

        #region Stochasts

        /// <summary>
        /// The identifier for stochasts elements.
        /// </summary>
        public const string StochastsElement = "stochasten";

        /// <summary>
        /// The identifier for stochast elements.
        /// </summary>
        public const string StochastElement = "stochast";

        /// <summary>
        /// The identifier for mean elements.
        /// </summary>
        public const string MeanElement = "verwachtingswaarde";

        /// <summary>
        /// The identifier for standard deviation elements.
        /// </summary>
        public const string StandardDeviationElement = "standaardafwijking";

        /// <summary>
        /// The identifier for variation coefficient elements.
        /// </summary>
        public const string VariationCoefficientElement = "variatiecoefficient";

        /// <summary>
        /// The identifier for the phreatic level exit stochast name.
        /// </summary>
        public const string AllowedLevelIncreaseStorageStochastName = "peilverhogingkomberging";

        /// <summary>
        /// The identifier for the critical overtopping discharge stochast name.
        /// </summary>
        public const string CriticalOvertoppingDischargeStochastName = "kritiekinstromenddebiet";

        /// <summary>
        /// The identifier for the model factor super critical flow stochast name.
        /// </summary>
        public const string ModelFactorSuperCriticalFlowStochastName = "modelfactoroverloopdebiet";

        /// <summary>
        /// The identifier for the model factor super critical flow stochast name.
        /// </summary>
        public const string FlowWidthAtBottomProtectionStochastName = "breedtebodembescherming";

        /// <summary>
        /// The identifier for the storage structure area stochast name.
        /// </summary>
        public const string StorageStructureAreaStochastName = "kombergendoppervlak";

        /// <summary>
        /// The identifier for the storm duration stochast name.
        /// </summary>
        public const string StormDurationStochastName = "stormduur";

        /// <summary>
        /// The identifier for the width flow apertures stochast name.
        /// </summary>
        public const string WidthFlowAperturesStochastName = "breedtedoorstroomopening";

        #endregion

        #region Wave reduction

        /// <summary>
        /// The tag of elements containing parameters that define wave reduction.
        /// </summary>
        public const string WaveReduction = "golfreductie";

        /// <summary>
        /// The tag of elements containing the value indicating whether to use break water.
        /// </summary>
        public const string UseBreakWater = "damgebruiken";

        /// <summary>
        /// The tag of elements containing the type of the break water.
        /// </summary>
        public const string BreakWaterType = "damtype";

        /// <summary>
        /// The tag of elements containing the height of the break water.
        /// </summary>
        public const string BreakWaterHeight = "damhoogte";

        /// <summary>
        /// The tag of elements containing the value indicating whether to use break water.
        /// </summary>
        public const string UseForeshore = "voorlandgebruiken";

        /// <summary>
        /// The possible content of the <see cref="BreakWaterType"/> element indicating a
        /// caisson type of break water.
        /// </summary>
        public const string BreakWaterCaisson = "caisson";

        /// <summary>
        /// The possible content of the <see cref="BreakWaterType"/> element indicating a
        /// dam type of break water.
        /// </summary>
        public const string BreakWaterDam = "havendam";

        /// <summary>
        /// The possible content of the <see cref="BreakWaterType"/> element indicating a
        /// wall type of break water.
        /// </summary>
        public const string BreakWaterWall = "verticalewand";

        #endregion

        #region Structure calculation

        /// <summary>
        /// The tag of elements containing the failure probability of a structure with erosion.
        /// </summary>
        public const string FailureProbabilityStructureWithErosionElement = "faalkansgegevenerosiebodem";

        /// <summary>
        /// The tag of elements containing the name of the structure.
        /// </summary>
        public const string StructureElement = "kunstwerk";

        /// <summary>
        /// The tag of elements containing the name of the foreshore profile.
        /// </summary>
        public const string ForeshoreProfileNameElement = "voorlandprofiel";

        #endregion

        #region Scenario calculation

        /// <summary>
        /// The identifier for scenario elements.
        /// </summary>
        public const string ScenarioElement = "scenario";

        /// <summary>
        /// The tag of the element containing the value indicating the contribution to the scenario.
        /// </summary>
        public const string ScenarioContribution = "bijdrage";

        /// <summary>
        /// The tag of the element containing the value whether the scenario calculation 
        /// is relevant for the scenario.
        /// </summary>
        public const string IsRelevantForScenario = "gebruik";

        #endregion
    }
}