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

namespace Ringtoets.AssemblyTool.Data.Input
{
    /// <summary>
    /// This class contains all the parameters that are required to perform an assembly categories calculation.
    /// </summary>
    public class AssemblyCategoriesCalculatorInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyCategoriesCalculatorInput"/>.
        /// </summary>
        /// <param name="signalingNorm">The signaling norm to use in the calculation.</param>
        /// <param name="lowerBoundaryNorm">The lower boundary norm to use in the calculation.</param>
        public AssemblyCategoriesCalculatorInput(double signalingNorm, double lowerBoundaryNorm)
        {
            SignalingNorm = signalingNorm;
            LowerBoundaryNorm = lowerBoundaryNorm;
        }

        /// <summary>
        /// Gets the signaling norm to use in the calculation.
        /// </summary>
        public double SignalingNorm { get; }

        /// <summary>
        /// Gets the lower boundary norm to use in the calculation.
        /// </summary>
        public double LowerBoundaryNorm { get; }
    }
}