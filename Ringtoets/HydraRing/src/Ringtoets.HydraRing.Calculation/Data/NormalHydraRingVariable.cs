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

namespace Ringtoets.HydraRing.Calculation.Data
{
    /// <summary>
    /// Class for Normal Hydra-Ring variable related data.
    /// </summary>
    public class NormalHydraRingVariable : HydraRingVariable2
    {
        private readonly double parameter1;
        private readonly double parameter2;

        /// <summary>
        /// Creates a new instance of <see cref="NormalHydraRingVariable"/>.
        /// </summary>
        /// <param name="variableId">The Hydra-Ring id corresponding to the variable that is considered.</param>
        /// <param name="deviationType">The deviation type in case the variable is random.</param>
        /// <param name="parameter1">The parameter1 value of the variable.</param>
        /// <param name="parameter2">The parameter2 value of the variable.</param>
        public NormalHydraRingVariable(int variableId, HydraRingDeviationType deviationType,
                                       double parameter1, double parameter2)
            : base(variableId, deviationType)
        {
            this.parameter1 = parameter1;
            this.parameter2 = parameter2;
        }

        public override double Parameter1
        {
            get
            {
                return parameter1;
            }
        }

        public override double? Parameter2
        {
            get
            {
                return DeviationType == HydraRingDeviationType.Standard
                           ? parameter2
                           : base.Parameter2;
            }
        }

        public override double CoefficientOfVariation
        {
            get
            {
                return DeviationType == HydraRingDeviationType.Variation
                           ? parameter2
                           : base.CoefficientOfVariation;
            }
        }

        public override HydraRingDistributionType DistributionType
        {
            get
            {
                return HydraRingDistributionType.Normal;
            }
        }
    }
}