// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Data.AssemblyTool
{
    /// <summary>
    /// Factory for creating results of a failure mechanism assembly.
    /// </summary>
    public static class FailureMechanismAssemblyResultFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="FailureMechanismAssembly"/> which
        /// represents a not applicable result.
        /// </summary>
        public static FailureMechanismAssembly CreateNotApplicableAssembly()
        {
            return new FailureMechanismAssembly(double.NaN, FailureMechanismAssemblyCategoryGroup.NotApplicable);
        }

        /// <summary>
        /// Creates an instance of <see cref="FailureMechanismAssemblyCategoryGroup"/> which
        /// represents a not applicable result.
        /// </summary>
        public static FailureMechanismAssemblyCategoryGroup CreateNotApplicableCategory()
        {
            return FailureMechanismAssemblyCategoryGroup.NotApplicable;
        }
    }
}