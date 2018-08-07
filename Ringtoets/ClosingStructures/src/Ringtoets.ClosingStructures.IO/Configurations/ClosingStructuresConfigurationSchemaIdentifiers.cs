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

namespace Ringtoets.ClosingStructures.IO.Configurations
{
    /// <summary>
    /// Container of identifiers related to closing structure calculation configuration schema definitions.
    /// </summary>
    internal static class ClosingStructuresConfigurationSchemaIdentifiers
    {
        #region Parameters

        /// <summary>
        /// The identifier for the identical apertures element.
        /// </summary>
        public const string IdenticalApertures = "nrdoorstroomopeningen";

        /// <summary>
        /// The identifier for the factor storm duration open structure element.
        /// </summary>
        public const string FactorStormDurationOpenStructure = "factorstormduur";

        /// <summary>
        /// The identifier for the failure probability open structure element.
        /// </summary>
        public const string FailureProbabilityOpenStructure = "kansmislukkensluiting";

        /// <summary>
        /// The identifier for the failure probability reparation element.
        /// </summary>
        public const string FailureProbabilityReparation = "faalkansherstel";

        /// <summary>
        /// The identifier for the probability open structure before flooding element.
        /// </summary>
        public const string ProbabilityOpenStructureBeforeFlooding = "kansopopenstaan";

        /// <summary>
        /// The identifier for the inflow model type element.
        /// </summary>
        public const string InflowModelType = "instroommodel";

        #endregion

        #region Stochasts

        /// <summary>
        /// The identifier for the level crest structure not closing stochast name.
        /// </summary>
        public const string LevelCrestStructureNotClosingStochastName = "kruinhoogte";

        /// <summary>
        /// The identifier for the inside water level stochast name.
        /// </summary>
        public const string InsideWaterLevelStochastName = "binnenwaterstand";

        /// <summary>
        /// The identifier for the drain coefficient stochast name.
        /// </summary>
        public const string DrainCoefficientStochastName = "afvoercoefficient";

        /// <summary>
        /// The identifier for the threshold height open weir stochast name.
        /// </summary>
        public const string ThresholdHeightOpenWeirStochastName = "drempelhoogte";

        /// <summary>
        /// The identifier for the area flow apertures stochast name.
        /// </summary>
        public const string AreaFlowAperturesStochastName = "doorstroomoppervlak";

        #endregion

        #region Inflow model types

        /// <summary>
        /// The identifier for the flooded culvert inflow model type.
        /// </summary>
        public const string FloodedCulvert = "verdronkenkoker";

        /// <summary>
        /// The identifier for the low will inflow model type.
        /// </summary>
        public const string LowSill = "lagedrempel";

        /// <summary>
        /// The identifier for the vertical wall inflow model type.
        /// </summary>
        public const string VerticalWall = "verticalewand";

        #endregion
    }
}