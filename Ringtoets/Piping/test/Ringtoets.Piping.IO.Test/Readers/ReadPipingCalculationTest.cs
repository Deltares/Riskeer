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
            Assert.IsNull(readPipingCalculation.Name);
            Assert.AreEqual(0.0, readPipingCalculation.AssessmentLevel);
            Assert.IsNull(readPipingCalculation.HydraulicBoundaryLocation);
            Assert.IsNull(readPipingCalculation.SurfaceLine);
            Assert.AreEqual(0.0, readPipingCalculation.EntryPointL);
            Assert.AreEqual(0.0, readPipingCalculation.ExitPointL);
            Assert.IsNull(readPipingCalculation.StochasticSoilModel);
            Assert.IsNull(readPipingCalculation.StochasticSoilProfile);
            Assert.AreEqual(0.0, readPipingCalculation.PhreaticLevelExitMean);
            Assert.AreEqual(0.0, readPipingCalculation.PhreaticLevelExitStandardDeviation);
            Assert.AreEqual(0.0, readPipingCalculation.DampingFactorExitMean);
            Assert.AreEqual(0.0, readPipingCalculation.DampingFactorExitStandardDeviation);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithValuesSet_PropertiesAsExpected()
        {
            // Call
            var readPipingCalculation = new ReadPipingCalculation(new ReadPipingCalculation.ConstructionProperties
            {
                Name = "Name of the calculation",
                AssessmentLevel = 1.1,
                HydraulicBoundaryLocation = "Name of the hydraulic boundary location",
                SurfaceLine = "Name of the surface line",
                EntryPointL = 2.2,
                ExitPointL = 3.3,
                StochasticSoilModel = "Name of the stochastic soil model",
                StochasticSoilProfile = "Name of the stochastic soil profile",
                PhreaticLevelExitMean = 4.4,
                PhreaticLevelExitStandardDeviation = 5.5,
                DampingFactorExitMean = 6.6,
                DampingFactorExitStandardDeviation = 7.7
            });

            // Assert
            Assert.AreEqual("Name of the calculation", readPipingCalculation.Name);
            Assert.AreEqual(1.1, readPipingCalculation.AssessmentLevel);
            Assert.AreEqual("Name of the hydraulic boundary location", readPipingCalculation.HydraulicBoundaryLocation);
            Assert.AreEqual("Name of the surface line", readPipingCalculation.SurfaceLine);
            Assert.AreEqual(2.2, readPipingCalculation.EntryPointL);
            Assert.AreEqual(3.3, readPipingCalculation.ExitPointL);
            Assert.AreEqual("Name of the stochastic soil model", readPipingCalculation.StochasticSoilModel);
            Assert.AreEqual("Name of the stochastic soil profile", readPipingCalculation.StochasticSoilProfile);
            Assert.AreEqual(4.4, readPipingCalculation.PhreaticLevelExitMean);
            Assert.AreEqual(5.5, readPipingCalculation.PhreaticLevelExitStandardDeviation);
            Assert.AreEqual(6.6, readPipingCalculation.DampingFactorExitMean);
            Assert.AreEqual(7.7, readPipingCalculation.DampingFactorExitStandardDeviation);
        }
    }
}