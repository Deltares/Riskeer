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
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ExportableFailureMechanismAssemblyResult"/> instances
    /// which can be used for testing.
    /// </summary>
    public static class ExportableFailureMechanismAssemblyResultTestFactory
    {
        /// <summary>
        /// Creates a default instance of <see cref="ExportableFailureMechanismAssemblyResult"/>.
        /// </summary>
        /// <returns>A default instance of <see cref="ExportableFailureMechanismAssemblyResult"/>.</returns>
        public static ExportableFailureMechanismAssemblyResult CreateResultWithoutProbability()
        {
            return new ExportableFailureMechanismAssemblyResult(ExportableAssemblyMethod.WBI1A1,
                                                                FailureMechanismAssemblyCategoryGroup.IVt);
        }

        /// <summary>
        /// Creates a default instance of <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/>.
        /// </summary>
        /// <returns>A default instance of <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/>.</returns>
        public static ExportableFailureMechanismAssemblyResultWithProbability CreateResultWithProbability()
        {
            return new ExportableFailureMechanismAssemblyResultWithProbability(ExportableAssemblyMethod.WBI1B1,
                                                                               FailureMechanismAssemblyCategoryGroup.IIt,
                                                                               0.75);
        }
    }
}