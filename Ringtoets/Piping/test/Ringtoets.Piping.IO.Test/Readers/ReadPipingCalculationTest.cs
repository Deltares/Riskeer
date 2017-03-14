// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.IO.Readers;
using Ringtoets.Piping.IO.Readers;

namespace Ringtoets.Piping.IO.Test.Readers
{
    [TestFixture]
    public class ReadPipingCalculationTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperties_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ReadPipingCalculation(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("constructionProperties", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValues_PropertiesAreDefault()
        {
            // Call
            var readPipingCalculation = new ReadPipingCalculation(new ReadPipingCalculation.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<IReadConfigurationItem>(readPipingCalculation);
            Assert.IsNull(readPipingCalculation.Name);
            Assert.IsNull(readPipingCalculation.AssessmentLevel);
            Assert.IsNull(readPipingCalculation.HydraulicBoundaryLocation);
            Assert.IsNull(readPipingCalculation.SurfaceLine);
            Assert.IsNull(readPipingCalculation.EntryPointL);
            Assert.IsNull(readPipingCalculation.ExitPointL);
            Assert.IsNull(readPipingCalculation.StochasticSoilModel);
            Assert.IsNull(readPipingCalculation.StochasticSoilProfile);
            Assert.IsNull(readPipingCalculation.PhreaticLevelExitMean);
            Assert.IsNull(readPipingCalculation.PhreaticLevelExitStandardDeviation);
            Assert.IsNull(readPipingCalculation.DampingFactorExitMean);
            Assert.IsNull(readPipingCalculation.DampingFactorExitStandardDeviation);
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
            var readPipingCalculation = new ReadPipingCalculation(new ReadPipingCalculation.ConstructionProperties
            {
                Name = calculationName,
                AssessmentLevel = assessmentLevel,
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                SurfaceLine = surfaceLine,
                EntryPointL = entryPointL,
                ExitPointL = exitPointL,
                StochasticSoilModel = stochasticSoilModel,
                StochasticSoilProfile = stochasticSoilProfile,
                PhreaticLevelExitMean = phreaticLevelExitMean,
                PhreaticLevelExitStandardDeviation = phreaticLevelExitStandardDeviation,
                DampingFactorExitMean = dampingFactorExitMean,
                DampingFactorExitStandardDeviation = dampingFactorExitStandardDeviation
            });

            // Assert
            Assert.AreEqual(calculationName, readPipingCalculation.Name);
            Assert.AreEqual(assessmentLevel, readPipingCalculation.AssessmentLevel);
            Assert.AreEqual(hydraulicBoundaryLocation, readPipingCalculation.HydraulicBoundaryLocation);
            Assert.AreEqual(surfaceLine, readPipingCalculation.SurfaceLine);
            Assert.AreEqual(entryPointL, readPipingCalculation.EntryPointL);
            Assert.AreEqual(exitPointL, readPipingCalculation.ExitPointL);
            Assert.AreEqual(stochasticSoilModel, readPipingCalculation.StochasticSoilModel);
            Assert.AreEqual(stochasticSoilProfile, readPipingCalculation.StochasticSoilProfile);
            Assert.AreEqual(phreaticLevelExitMean, readPipingCalculation.PhreaticLevelExitMean);
            Assert.AreEqual(phreaticLevelExitStandardDeviation, readPipingCalculation.PhreaticLevelExitStandardDeviation);
            Assert.AreEqual(dampingFactorExitMean, readPipingCalculation.DampingFactorExitMean);
            Assert.AreEqual(dampingFactorExitStandardDeviation, readPipingCalculation.DampingFactorExitStandardDeviation);
        }
    }
}