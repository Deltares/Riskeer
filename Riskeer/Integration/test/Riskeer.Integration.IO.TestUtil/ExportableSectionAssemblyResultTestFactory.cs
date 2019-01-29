﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
    /// Factory that creates simple exportable failure mechanism section result instances
    /// which can be used for testing.
    /// </summary>
    public static class ExportableSectionAssemblyResultTestFactory
    {
        /// <summary>
        /// Creates a default <see cref="ExportableSectionAssemblyResult"/>.
        /// </summary>
        /// <returns>A default instance of <see cref="ExportableSectionAssemblyResult"/>.</returns>
        public static ExportableSectionAssemblyResult CreateSectionAssemblyResult()
        {
            return new ExportableSectionAssemblyResult(ExportableAssemblyMethod.WBI0T1,
                                                       FailureMechanismSectionAssemblyCategoryGroup.IIIv);
        }

        /// <summary>
        /// Creates a default <see cref="ExportableSectionAssemblyResultWithProbability"/>.
        /// </summary>
        /// <returns>A default instance of <see cref="ExportableSectionAssemblyResultWithProbability"/>.</returns>
        public static ExportableSectionAssemblyResultWithProbability CreateSectionAssemblyResultWithProbability()
        {
            return new ExportableSectionAssemblyResultWithProbability(ExportableAssemblyMethod.WBI0T1,
                                                                      FailureMechanismSectionAssemblyCategoryGroup.IIIv,
                                                                      0.75);
        }
    }
}