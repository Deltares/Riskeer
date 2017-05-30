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
using Ringtoets.MacroStabilityInwards.IO.Readers;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Readers
{
    [TestFixture]
    public class ReadMacroStabilityInwardsCalculationTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperties_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ReadMacroStabilityInwardsCalculation(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("constructionProperties", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValues_PropertiesAreDefault()
        {
            // Call
            var readCalculation = new ReadMacroStabilityInwardsCalculation(new ReadMacroStabilityInwardsCalculation.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<IConfigurationItem>(readCalculation);
            Assert.IsNull(readCalculation.Name);
            Assert.IsNull(readCalculation.AssessmentLevel);
            Assert.IsNull(readCalculation.HydraulicBoundaryLocation);
            Assert.IsNull(readCalculation.SurfaceLine);
            Assert.IsNull(readCalculation.StochasticSoilModel);
            Assert.IsNull(readCalculation.StochasticSoilProfile);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithValuesSet_PropertiesAsExpected()
        {
            // Setup
            const string calculationName = "Name of the calculation";
            const double assessmentLevel = 1.1;
            const string hydraulicBoundaryLocation = "Name of the hydraulic boundary location";
            const string surfaceLine = "Name of the surface line";
            const double entryPointL = 2.2;
            const double exitPointL = 3.3;
            const string stochasticSoilModel = "Name of the stochastic soil model";
            const string stochasticSoilProfile = "Name of the stochastic soil profile";
            const double phreaticLevelExitMean = 4.4;
            const double phreaticLevelExitStandardDeviation = 5.5;
            const double dampingFactorExitMean = 6.6;
            const double dampingFactorExitStandardDeviation = 7.7;

            // Call
            var readCalculation = new ReadMacroStabilityInwardsCalculation(new ReadMacroStabilityInwardsCalculation.ConstructionProperties
            {
                Name = calculationName,
                AssessmentLevel = assessmentLevel,
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                SurfaceLine = surfaceLine,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile
            });

            // Assert
            Assert.AreEqual(calculationName, readCalculation.Name);
            Assert.AreEqual(assessmentLevel, readCalculation.AssessmentLevel);
            Assert.AreEqual(hydraulicBoundaryLocation, readCalculation.HydraulicBoundaryLocation);
            Assert.AreEqual(surfaceLine, readCalculation.SurfaceLine);
            Assert.AreEqual(stochasticSoilModel, readCalculation.StochasticSoilModel);
            Assert.AreEqual(stochasticSoilProfile, readCalculation.StochasticSoilProfile);
        }
    }
}