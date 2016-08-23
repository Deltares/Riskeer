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

using Core.Common.Utils;

namespace Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions
{
    /// <summary>
    /// Container of all data necessary for performing a wave conditions calculation (Q-variant) via Hydra-Ring.
    /// </summary>
    public class WaveConditionsInput : HydraRingCalculationInput
    {
        private readonly double beta;
        private readonly HydraRingSection section;

        /// <summary>
        /// Creates a new instance of the <see cref="WaveConditionsInput"/> class.
        /// </summary>
        /// <param name="sectionId">The id of the section to use during the calculation.</param>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic station to use during the calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <remarks>As a part of the constructor, the <paramref name="norm"/> is automatically converted into a reliability index.</remarks>
        public WaveConditionsInput(int sectionId, long hydraulicBoundaryLocationId, double norm) : base(hydraulicBoundaryLocationId)
        {
            beta = StatisticsConverter.NormToBeta(norm);
            section = new HydraRingSection(sectionId, double.NaN, double.NaN);
        }

        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.QVariant;
            }
        }

        public override int CalculationTypeId
        {
            get
            {
                return 6;
            }
        }

        public override int VariableId
        {
            get
            {
                return 114;
            }
        }

        public override HydraRingSection Section
        {
            get
            {
                return section;
            }
        }

        public override double Beta
        {
            get
            {
                return beta;
            }
        }
    }
}