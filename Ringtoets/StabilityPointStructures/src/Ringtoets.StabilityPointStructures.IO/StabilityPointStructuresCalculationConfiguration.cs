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

using System;
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.StabilityPointStructures.IO
{
    /// <summary>
    /// Configuration of a stability point structures calculation.
    /// </summary>
    public class StabilityPointStructuresCalculationConfiguration : StructuresCalculationConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="StabilityPointStructuresCalculationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationConfiguration(string name) : base(name) {}

        /// <summary>
        /// Gets or sets the stochast configuration for the area flow apertures of the structure.
        /// </summary>
        public MeanStandardDeviationStochastConfiguration AreaFlowApertures { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the bank width of the structure.
        /// </summary>
        public MeanStandardDeviationStochastConfiguration BankWidth { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the constructive strength of the linear load model of the structure.
        /// </summary>
        public MeanVariationCoefficientStochastConfiguration ConstructiveStrengthLinearLoadModel { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the constructive strength of the quadratic load model of the structure.
        /// </summary>
        public MeanVariationCoefficientStochastConfiguration ConstructiveStrengthQuadraticLoadModel { get; set; }

        /// <summary>
        /// Gets or sets the evaluation level of the structure.
        /// </summary>
        public double? EvaluationLevel { get; set; }

        /// <summary>
        /// Gets or sets the stochast configuration for the failure collision energy of the structure.
        /// </summary>
        public MeanVariationCoefficientStochastConfiguration FailureCollisionEnergy { get; set; }
    }
}