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

using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.TestUtil
{
    /// <summary>
    /// Test class for Hydra-Ring variables.
    /// </summary>
    public class TestHydraRingVariable : HydraRingVariable
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestHydraRingVariable"/>.
        /// </summary>
        /// <param name="variableId">The id of the variable.</param>
        /// <param name="distributionType">The distribution type of the variable.</param>
        /// <param name="deviationType">The deviation type of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        /// <param name="parameter1">The parameter1 of the variable.</param>
        /// <param name="parameter2">The parameter2 of the variable.</param>
        /// <param name="parameter3">The parameter3 of the variable.</param>
        /// <param name="parameter4">The parameter4 of the variable.</param>
        /// <param name="coefficientOfVariation">The coefficient of variation of the variable.</param>
        public TestHydraRingVariable(int variableId,
                                     HydraRingDistributionType distributionType,
                                     HydraRingDeviationType deviationType,
                                     double value,
                                     double parameter1,
                                     double parameter2,
                                     double parameter3,
                                     double parameter4,
                                     double coefficientOfVariation)
            : base(variableId)
        {
            DistributionType = distributionType;
            DeviationType = deviationType;
            Value = value;
            Parameter1 = parameter1;
            Parameter2 = parameter2;
            Parameter3 = parameter3;
            Parameter4 = parameter4;
            CoefficientOfVariation = coefficientOfVariation;
        }

        public override HydraRingDistributionType DistributionType { get; }

        public override HydraRingDeviationType DeviationType { get; }

        public override double Value { get; }

        public override double Parameter1 { get; }

        public override double Parameter2 { get; }

        public override double Parameter3 { get; }

        public override double Parameter4 { get; }

        public override double CoefficientOfVariation { get; }
    }
}