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

namespace Ringtoets.StabilityPointStructures.IO.Configurations
{
    /// <summary>
    /// Container of identifiers related to stability point structures calculation configuration schema definitions.
    /// </summary>
    internal static class StabilityPointStructuresConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The identifier for the area flow apertures stochast name.
        /// </summary>
        public const string AreaFlowAperturesStochastName = "doorstroomoppervlak";

        /// <summary>
        /// The identifier for the bank width stochast name.
        /// </summary>
        public const string BankWidthStochastName = "bermbreedte";

        /// <summary>
        /// The identifier for the constructive strength linear load model stochast name.
        /// </summary>
        public const string ConstructiveStrengthLinearLoadModelStochastName = "lineairebelastingschematiseringsterkte";

        /// <summary>
        /// The identifier for the constructive strength quadratic load model stochast name.
        /// </summary>
        public const string ConstructiveStrengthQuadraticLoadModelStochastName = "kwadratischebelastingschematiseringsterkte";

        /// <summary>
        /// The identifier for the drain coefficient stochast name.
        /// </summary>
        public const string DrainCoefficientStochastName = "afvoercoefficient";

        /// <summary>
        /// The identifier for evaluation level elements.
        /// </summary>
        public const string EvaluationLevelElement = "analysehoogte";

        /// <summary>
        /// The identifier for the failure collision energy stochast name.
        /// </summary>
        public const string FailureCollisionEnergyStochastName = "bezwijkwaardeaanvaarenergie";

        /// <summary>
        /// The identifier for the failure probability of repairing a closure elements.
        /// </summary>
        public const string FailureProbabilityRepairClosureElement = "faalkansherstel";

        /// <summary>
        /// The identifier for the flow velocity structure closable stochast name.
        /// </summary>
        public const string FlowVelocityStructureClosableStochastName = "kritiekestroomsnelheid";

        /// <summary>
        /// The identifier for the type of stability point structure inflow model elements.
        /// </summary>
        public const string InflowModelTypeElement = "instroommodel";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationStabilityPointStructuresInflowModelType"/> element indicating a
        /// low sill structure.
        /// </summary>
        public const string InflowModelLowSillStructure = "lagedrempel";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationStabilityPointStructuresInflowModelType"/> element indicating a
        /// flooded culvert structure.
        /// </summary>
        public const string InflowModelFloodedCulvertStructure = "verdronkenkoker";

        /// <summary>
        /// The identifier for the inside water level stochast name.
        /// </summary>
        public const string InsideWaterLevelStochastName = "binnenwaterstand";

        /// <summary>
        /// The identifier for the inside water level failure construction stochast name.
        /// </summary>
        public const string InsideWaterLevelFailureConstructionStochastName = "binnenwaterstandbijfalen";

        /// <summary>
        /// The identifier for the level crest structure stochast name.
        /// </summary>
        public const string LevelCrestStructureStochastName = "kerendehoogte";

        /// <summary>
        /// The identifier for the leveling count elements.
        /// </summary>
        public const string LevellingCountElement = "nrnivelleringen";

        /// <summary>
        /// The identifier for the load schematization type elements.
        /// </summary>
        public const string LoadSchematizationTypeElement = "belastingschematisering";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationStabilityPointStructuresLoadSchematizationType"/>
        /// element indicating a linear schematization.
        /// </summary>
        public const string LoadSchematizationLinearStructure = "lineair";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationStabilityPointStructuresLoadSchematizationType"/>
        /// element indicating a quadratic schematization.
        /// </summary>
        public const string LoadSchematizationQuadraticStructure = "kwadratisch";

        /// <summary>
        /// The identifier for the probability of a secondary collision on the structure per leveling elements.
        /// </summary>
        public const string ProbabilityCollisionSecondaryStructureElement = "kansaanvaringtweedekeermiddel";

        /// <summary>
        /// The identifier for the mass of the ship stochast name.
        /// </summary>
        public const string ShipMassStochastName = "massaschip";

        /// <summary>
        /// The identifier for the velocity of the ship stochast name.
        /// </summary>
        public const string ShipVelocityStochastName = "aanvaarsnelheid";

        /// <summary>
        /// The identifier for the stability properties of the linear load model stochast name.
        /// </summary>
        public const string StabilityLinearLoadModelStochastName = "lineairebelastingschematiseringstabiliteit";

        /// <summary>
        /// The identifier for the stability properties of the quadratic load model stochast name.
        /// </summary>
        public const string StabilityQuadraticLoadModelStochastName = "kwadratischebelastingschematiseringstabiliteit";

        /// <summary>
        /// The identifier for the threshold height of the open weir stochast name.
        /// </summary>
        public const string ThresholdHeightOpenWeirStochastName = "drempelhoogte";

        /// <summary>
        /// The identifier for the vertical distance of the structure elements.
        /// </summary>
        public const string VerticalDistanceElement = "afstandonderkantwandteendijk";

        /// <summary>
        /// The identifier for the factor for the storm duration for an open structure elements.
        /// </summary>
        public const string FactorStormDurationOpenStructureElement = "factorstormduur";

        /// <summary>
        /// The identifier for the volumic weight of water elements.
        /// </summary>
        public const string VolumicWeightWaterElement = "volumiekgewichtwater";
    }
}