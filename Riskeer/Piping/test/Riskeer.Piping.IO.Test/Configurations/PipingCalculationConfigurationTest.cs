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

using System;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Piping.IO.Configurations;

namespace Riskeer.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingCalculationConfiguration(null, PipingCalculationConfigurationType.Probabilistic);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationTypeInvalid_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const PipingCalculationConfigurationType calculationType = (PipingCalculationConfigurationType) 99;

            // Call
            void Call() => new PipingCalculationConfiguration("name", calculationType);

            // Assert
            var expectedMessage = $"The value of argument '{nameof(calculationType)}' ({calculationType}) is invalid for Enum type '{nameof(PipingCalculationConfigurationType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        public void Constructor_WithValidData_PropertiesAreDefault()
        {
            // Setup 
            const string name = "some name";
            var calculationType = new Random(21).NextEnumValue<PipingCalculationConfigurationType>();

            // Call
            var readPipingCalculation = new PipingCalculationConfiguration(name, calculationType);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readPipingCalculation);
            Assert.AreEqual(name, readPipingCalculation.Name);
            Assert.AreEqual(calculationType, readPipingCalculation.CalculationType);
            Assert.IsNull(readPipingCalculation.AssessmentLevel);
            Assert.IsNull(readPipingCalculation.HydraulicBoundaryLocationName);
            Assert.IsNull(readPipingCalculation.SurfaceLineName);
            Assert.IsNull(readPipingCalculation.EntryPointL);
            Assert.IsNull(readPipingCalculation.ExitPointL);
            Assert.IsNull(readPipingCalculation.StochasticSoilModelName);
            Assert.IsNull(readPipingCalculation.StochasticSoilProfileName);
            Assert.IsNull(readPipingCalculation.PhreaticLevelExit);
            Assert.IsNull(readPipingCalculation.DampingFactorExit);
            Assert.IsNull(readPipingCalculation.Scenario);
            Assert.IsNull(readPipingCalculation.ShouldProfileSpecificIllustrationPointsBeCalculated);
            Assert.IsNull(readPipingCalculation.ShouldSectionSpecificIllustrationPointsBeCalculated);
        }
    }
}