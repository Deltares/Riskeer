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

namespace Ringtoets.AssemblyTool.Data
{
    /// <summary>
    /// Class that contains the data that will be sent to the assembly kernel
    /// for the determination of the assembly categories. 
    /// </summary>
    public class AssemblyCategoriesInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssemblyCategoriesInput"/>.
        /// </summary>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <param name="failureMechanismContribution">The contribution of a failure mechanism.</param>
        /// <param name="signalingNorm">The signaling norm of the assessment section.</param>
        /// <param name="lowerLimitNorm">The lower limit norm of the assessment section.</param>
        public AssemblyCategoriesInput(double n,
                                       double failureMechanismContribution,
                                       double signalingNorm,
                                       double lowerLimitNorm)
        {
            N = n;
            FailureMechanismContribution = failureMechanismContribution / 100;
            SignalingNorm = signalingNorm;
            LowerLimitNorm = lowerLimitNorm;
        }

        public double N { get; }
        public double FailureMechanismContribution { get; }
        public double SignalingNorm { get; }
        public double LowerLimitNorm { get; }
    }
}