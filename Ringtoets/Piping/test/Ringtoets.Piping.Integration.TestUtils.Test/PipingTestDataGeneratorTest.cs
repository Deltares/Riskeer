﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Integration.TestUtils.Test
{
    [TestFixture]
    public class PipingTestDataGeneratorTest
    {
        [Test]
        public void GetPipingFailureMechanismWithAllCalculationConfigurations_ReturnsFailureMechanismWithAllConfigurations()
        {
            // Call
            PipingFailureMechanism failureMechanism = PipingTestDataGenerator.GetPipingFailureMechanismWithAllCalculationConfigurations();

            // Assert
            PipingTestDataGeneratorHelper.AssertHasStochasticSoilModels(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasSurfaceLines(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(failureMechanism);
        }

        [Test]
        public void SetPipingFailureMechanismWithAllCalculationConfigurations_ReturnsFailureMechanismWithAllConfigurations()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateFullyCalculated();

            // Call
            PipingTestDataGenerator.ConfigureFailureMechanismWithAllCalculationConfigurations(failureMechanism, hydraulicBoundaryLocation);

            // Assert
            PipingTestDataGeneratorHelper.AssertHasStochasticSoilModels(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasSurfaceLines(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithOutputs(failureMechanism);
            PipingTestDataGeneratorHelper.AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(failureMechanism);
        }
    }
}