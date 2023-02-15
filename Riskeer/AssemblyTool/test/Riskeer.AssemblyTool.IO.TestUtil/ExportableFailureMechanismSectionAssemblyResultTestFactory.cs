// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using Core.Common.TestUtil;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.TestUtil
{
    /// <summary>
    /// Factory for creating simple <see cref="ExportableFailureMechanismSectionAssemblyResult"/> instances
    /// that can be used in tests. 
    /// </summary>
    public static class ExportableFailureMechanismSectionAssemblyResultTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="section">The <see cref="ExportableFailureMechanismSection"/>.</param>
        /// <param name="seed">The seed to use.</param>
        /// <returns>The created <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.</returns>
        public static ExportableFailureMechanismSectionAssemblyResult Create(
            ExportableFailureMechanismSection section, int seed)
        {
            var random = new Random(seed);

            return new ExportableFailureMechanismSectionAssemblyResult(
                "id", section,
                random.NextDouble(),
                random.NextEnumValue(new[]
                {
                    ExportableFailureMechanismSectionAssemblyGroup.NotDominant,
                    ExportableFailureMechanismSectionAssemblyGroup.III,
                    ExportableFailureMechanismSectionAssemblyGroup.II,
                    ExportableFailureMechanismSectionAssemblyGroup.I,
                    ExportableFailureMechanismSectionAssemblyGroup.Zero,
                    ExportableFailureMechanismSectionAssemblyGroup.IMin,
                    ExportableFailureMechanismSectionAssemblyGroup.IIMin,
                    ExportableFailureMechanismSectionAssemblyGroup.IIIMin,
                    ExportableFailureMechanismSectionAssemblyGroup.NotRelevant
                }),
                random.NextEnumValue<ExportableAssemblyMethod>(),
                random.NextEnumValue<ExportableAssemblyMethod>());
        }
    }
}