// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.IO.Configurations;
using Riskeer.Piping.IO.Configurations;

namespace Riskeer.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationTest
    {
        [Test]
        public void Constructor_WithoutName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingCalculationConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithName_PropertiesAreDefault()
        {
            // Setup 
            const string name = "some name";

            // Call
            var readPipingCalculation = new PipingCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readPipingCalculation);
            Assert.AreEqual(name, readPipingCalculation.Name);
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
        }

        [Test]
        public void Name_Null_ThrowsArgumentNullException()
        {
            // Setup
            var calculationConfiguration = new PipingCalculationConfiguration("valid name");

            // Call
            TestDelegate test = () => calculationConfiguration.Name = null;

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}