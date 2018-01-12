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

using AssemblyTool.Kernel;
using AssemblyTool.Kernel.CategoriesOutput;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels.CategoryBoundaries;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.CategoryBoundaries
{
    /// <summary>
    /// Assembly category boundaries kernel stub for testing purposes.
    /// </summary>
    public class AssemblyCategoryBoundariesKernelStub : IAssemblyCategoryBoundariesKernel
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="Calculate"/> was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets the lower boundary norm.
        /// </summary>
        public double LowerBoundaryNorm { get; private set; }

        /// <summary>
        /// Gets the upper boundary norm.
        /// </summary>
        public double SignalingNorm { get; private set; }
        
        /// <summary>
        /// Gets the assessment section categories output.
        /// </summary>
        public CalculationOutput<AssessmentSectionCategoriesOutput[]> AssessmentSectionCategoriesOutput { get; set; }

        public CalculationOutput<AssessmentSectionCategoriesOutput[]> Calculate(double signalingNorm, double lowerBoundaryNorm)
        {
            LowerBoundaryNorm = lowerBoundaryNorm;
            SignalingNorm = signalingNorm;

            Calculated = true;

            return AssessmentSectionCategoriesOutput;
        }
    }
}