﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.IO
{
    /// <summary>
    /// Container of identifiers related to stability point structures calculation configuration schema definitions.
    /// </summary>
    public static class StabilityPointStructuresConfigurationSchemaIdentifiers
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
        /// The identifier for evaluation level elements.
        /// </summary>
        public const string EvaluationLevelElement = "analysehoogte";

        /// <summary>
        /// The identifier for the failure collision energy stochast name.
        /// </summary>
        public const string FailureCollisionEnergyStochastName = "aanvaarenergie";

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
        /// The possible content of the <see cref="StabilityPointStructureInflowModelType"/> element indicating a
        /// low sill structure.
        /// </summary>
        public const string InflowModelLowSillStructure = "lagedrempel";

        /// <summary>
        /// The possible content of the <see cref="StabilityPointStructureInflowModelType"/> element indicating a
        /// flooded culvert structure.
        /// </summary>
        public const string InflowModelFloodedCulvertStructure = "verdronkenkoker";

        /// <summary>
        /// The identifier for the inside water level stochast name.
        /// </summary>
        public const string InsideWaterLevelStochastName = "binnenwaterstand";
    }
}