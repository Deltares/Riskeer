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

using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Variables;

namespace Ringtoets.HydraRing.Calculation.TestUtil
{
    /// <summary>
    /// Test class for Hydra-Ring variables.
    /// </summary>
    public class TestHydraRingVariable : HydraRingVariable
    {
        private readonly HydraRingDistributionType distributionType;
        private readonly HydraRingDeviationType deviationType;
        private readonly double value;
        private readonly double parameter1;
        private readonly double parameter2;
        private readonly double parameter3;
        private readonly double parameter4;
        private readonly double coefficientOfVariation;

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
            this.distributionType = distributionType;
            this.deviationType = deviationType;
            this.value = value;
            this.parameter1 = parameter1;
            this.parameter2 = parameter2;
            this.parameter3 = parameter3;
            this.parameter4 = parameter4;
            this.coefficientOfVariation = coefficientOfVariation;
        }

        public override HydraRingDistributionType DistributionType
        {
            get
            {
                return distributionType;
            }
        }

        public override HydraRingDeviationType DeviationType
        {
            get
            {
                return deviationType;
            }
        }

        public override double Value
        {
            get
            {
                return value;
            }
        }

        public override double Parameter1
        {
            get
            {
                return parameter1;
            }
        }

        public override double Parameter2
        {
            get
            {
                return parameter2;
            }
        }

        public override double Parameter3
        {
            get
            {
                return parameter3;
            }
        }

        public override double Parameter4
        {
            get
            {
                return parameter4;
            }
        }

        public override double CoefficientOfVariation
        {
            get
            {
                return coefficientOfVariation;
            }
        }
    }
}