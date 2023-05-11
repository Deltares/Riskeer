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

using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismAssemblyProbabilityResultTypeTest : EnumWithResourcesDisplayNameTestFixture<FailureMechanismAssemblyProbabilityResultType>
    {
        protected override IDictionary<FailureMechanismAssemblyProbabilityResultType, int> ExpectedValueForEnumValues =>
            new Dictionary<FailureMechanismAssemblyProbabilityResultType, int>
            {
                {
                    FailureMechanismAssemblyProbabilityResultType.Default, 0
                },
                {
                    FailureMechanismAssemblyProbabilityResultType.AutomaticWorstSectionOrProfile, 1
                },
                {
                    FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections, 2
                },
                {
                    FailureMechanismAssemblyProbabilityResultType.Manual, 3
                }
            };

        protected override IDictionary<FailureMechanismAssemblyProbabilityResultType, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<FailureMechanismAssemblyProbabilityResultType, string>
            {
                {
                    FailureMechanismAssemblyProbabilityResultType.Default, "<selecteer>"
                },
                {
                    FailureMechanismAssemblyProbabilityResultType.AutomaticWorstSectionOrProfile, "Automatisch berekenen o.b.v. slechtste doorsnede of vak"
                },
                {
                    FailureMechanismAssemblyProbabilityResultType.AutomaticIndependentSections, "Automatisch berekenen o.b.v. onafhankelijke vakken"
                },
                {
                    FailureMechanismAssemblyProbabilityResultType.Manual, "Handmatig invullen"
                }
            };
    }
}