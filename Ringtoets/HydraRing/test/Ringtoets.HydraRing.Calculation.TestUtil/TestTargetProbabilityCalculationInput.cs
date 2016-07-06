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
using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.TestUtil
{
    /// <summary>
    /// Implementation of <see cref="TargetProbabilityCalculationInput"/> which can be used for test purposes.
    /// </summary>
    public class TestTargetProbabilityCalculationInput : TargetProbabilityCalculationInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestTargetProbabilityCalculationInput"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationId">The id of the hydraulic boundary location.</param>
        /// <param name="norm">The norm to use in the calculation.</param>
        public TestTargetProbabilityCalculationInput(int hydraulicBoundaryLocationId, double norm) : base(hydraulicBoundaryLocationId, norm) { }

        /// <summary>
        /// Gets an arbitrary <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        public override HydraRingFailureMechanismType FailureMechanismType
        {
            get
            {
                return HydraRingFailureMechanismType.DikesPiping;
            }
        }

        /// <summary>
        /// Gets an arbitrary variable id.
        /// </summary>
        public override int VariableId
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// Gets a new <see cref="HydraRingSection"/> with arbitrary default values.
        /// </summary>
        public override HydraRingSection Section
        {
            get
            {
                return new HydraRingSection(1, 2.2, 3.3);
            }
        }
    }
}