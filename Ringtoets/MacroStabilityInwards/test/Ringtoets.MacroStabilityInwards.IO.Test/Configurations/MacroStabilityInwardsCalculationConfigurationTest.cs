// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using NUnit.Framework;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.MacroStabilityInwards.IO.Configurations;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperties_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculationConfiguration(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValues_PropertiesAreDefault()
        {
            // Setup
            const string name = "some name";

            // Call
            var readCalculation = new MacroStabilityInwardsCalculationConfiguration(name);

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readCalculation);
            Assert.AreEqual(name, readCalculation.Name);
            Assert.IsNull(readCalculation.AssessmentLevel);
            Assert.IsNull(readCalculation.HydraulicBoundaryLocationName);
            Assert.IsNull(readCalculation.SurfaceLineName);
            Assert.IsNull(readCalculation.StochasticSoilModelName);
            Assert.IsNull(readCalculation.StochasticSoilProfileName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithValuesSet_PropertiesAsExpected()
        {
            // Setup
            const string calculationName = "Name of the calculation";
            const double assessmentLevel = 1.1;
            const string hydraulicBoundaryLocation = "Name of the hydraulic boundary location";
            const string surfaceLine = "Name of the surface line";
            const string stochasticSoilModel = "Name of the stochastic soil model";
            const string stochasticSoilProfile = "Name of the stochastic soil profile";

            // Call
            var readCalculation = new MacroStabilityInwardsCalculationConfiguration(calculationName)
            {
                AssessmentLevel = assessmentLevel,
                HydraulicBoundaryLocationName = hydraulicBoundaryLocation,
                SurfaceLineName = surfaceLine,
                StochasticSoilModelName = stochasticSoilModel,
                StochasticSoilProfileName = stochasticSoilProfile
            };

            // Assert
            Assert.AreEqual(calculationName, readCalculation.Name);
            Assert.AreEqual(assessmentLevel, readCalculation.AssessmentLevel);
            Assert.AreEqual(hydraulicBoundaryLocation, readCalculation.HydraulicBoundaryLocationName);
            Assert.AreEqual(surfaceLine, readCalculation.SurfaceLineName);
            Assert.AreEqual(stochasticSoilModel, readCalculation.StochasticSoilModelName);
            Assert.AreEqual(stochasticSoilProfile, readCalculation.StochasticSoilProfileName);
        }
    }
}