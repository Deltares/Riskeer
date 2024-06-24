﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismScenarioConfigurationTypeTest : EnumWithResourcesDisplayNameTestFixture<PipingFailureMechanismScenarioConfigurationType>
    {
        protected override IDictionary<PipingFailureMechanismScenarioConfigurationType, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<PipingFailureMechanismScenarioConfigurationType, string>
            {
                {
                    PipingFailureMechanismScenarioConfigurationType.SemiProbabilistic, "Semi-probabilistisch"
                },
                {
                    PipingFailureMechanismScenarioConfigurationType.Probabilistic, "Probabilistisch"
                },
                {
                    PipingFailureMechanismScenarioConfigurationType.PerFailureMechanismSection, "Per vak instelbaar"
                }
            };

        protected override IDictionary<PipingFailureMechanismScenarioConfigurationType, int> ExpectedValueForEnumValues =>
            new Dictionary<PipingFailureMechanismScenarioConfigurationType, int>
            {
                {
                    PipingFailureMechanismScenarioConfigurationType.SemiProbabilistic, 1
                },
                {
                    PipingFailureMechanismScenarioConfigurationType.Probabilistic, 2
                },
                {
                    PipingFailureMechanismScenarioConfigurationType.PerFailureMechanismSection, 3
                }
            };
    }
}