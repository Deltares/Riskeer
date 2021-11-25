// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.AssemblyTool.Data.Test
{
    [TestFixture]
    public class FailureMechanismSectionInterpretationCategoryTest : EnumValuesTestFixture<FailureMechanismSectionInterpretationCategory, int>
    {
        protected override IDictionary<FailureMechanismSectionInterpretationCategory, int> ExpectedValueForEnumValues =>
            new Dictionary<FailureMechanismSectionInterpretationCategory, int>
            {
                {
                    FailureMechanismSectionInterpretationCategory.ND, 1
                },
                {
                    FailureMechanismSectionInterpretationCategory.III, 2
                },
                {
                    FailureMechanismSectionInterpretationCategory.II, 3
                },
                {
                    FailureMechanismSectionInterpretationCategory.I, 4
                },
                {
                    FailureMechanismSectionInterpretationCategory.ZeroPlus, 5
                },
                {
                    FailureMechanismSectionInterpretationCategory.Zero, 6
                },
                {
                    FailureMechanismSectionInterpretationCategory.IMin, 7
                },
                {
                    FailureMechanismSectionInterpretationCategory.IIMin, 8
                },
                {
                    FailureMechanismSectionInterpretationCategory.IIIMin, 9
                },
                {
                    FailureMechanismSectionInterpretationCategory.D, 10
                },
                {
                    FailureMechanismSectionInterpretationCategory.Gr, 11
                }
            };
    }
}