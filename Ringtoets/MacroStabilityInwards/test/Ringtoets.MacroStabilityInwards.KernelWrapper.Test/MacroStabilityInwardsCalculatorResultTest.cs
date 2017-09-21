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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Result;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Result;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculatorResultTest
    {
        [Test]
        public void Constructor_SlidingCurveNull_ThrowsArgumentNullException()
        {
            // Setup
            var upliftVanCalculationGrid = new MacroStabilityInwardsUpliftVanCalculationGridResult(MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   new double[0]);

            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculatorResult(null, upliftVanCalculationGrid, new MacroStabilityInwardsCalculatorResult.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("slidingCurve", exception.ParamName);
        }

        [Test]
        public void Constructor_UpliftVanCalculationGridNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSlidingCurveResult curve = MacroStabilityInwardsSlidingCurveResultTestFactory.Create();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculatorResult(curve, null, new MacroStabilityInwardsCalculatorResult.ConstructionProperties()); ;

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("upliftVanCalculationGrid", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            MacroStabilityInwardsSlidingCurveResult curve = MacroStabilityInwardsSlidingCurveResultTestFactory.Create();
            var upliftVanCalculationGrid = new MacroStabilityInwardsUpliftVanCalculationGridResult(MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   new double[0]);

            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculatorResult(curve, upliftVanCalculationGrid, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            MacroStabilityInwardsSlidingCurveResult curve = MacroStabilityInwardsSlidingCurveResultTestFactory.Create();
            var upliftVanCalculationGrid = new MacroStabilityInwardsUpliftVanCalculationGridResult(MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   new double[0]);

            // Call
            var result = new MacroStabilityInwardsCalculatorResult(curve, upliftVanCalculationGrid, new MacroStabilityInwardsCalculatorResult.ConstructionProperties());
            
            // Assert
            Assert.AreSame(curve, result.SlidingCurve);
            Assert.AreSame(upliftVanCalculationGrid, result.UpliftVanCalculationGrid);
        }

        [Test]
        public void Constructor_EmptyConstructionProperties_ExpectedValues()
        {
            // Setup
            MacroStabilityInwardsSlidingCurveResult curve = MacroStabilityInwardsSlidingCurveResultTestFactory.Create();
            var upliftVanCalculationGrid = new MacroStabilityInwardsUpliftVanCalculationGridResult(MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   new double[0]);

            // Call
            var result = new MacroStabilityInwardsCalculatorResult(curve, upliftVanCalculationGrid,
                                                                   new MacroStabilityInwardsCalculatorResult.ConstructionProperties());

            // Assert
            Assert.IsNaN(result.FactorOfStability);
            Assert.IsNaN(result.ZValue);
            Assert.IsNaN(result.ForbiddenZonesXEntryMin);
            Assert.IsNaN(result.ForbiddenZonesXEntryMax);
            Assert.IsFalse(result.ForbiddenZonesAutomaticallyCalculated);
            Assert.IsFalse(result.GridAutomaticallyCalculated);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithData_ExcpectedValues()
        {
            // Setup
            var random = new Random(21);
            double factorOfStability = random.NextDouble();
            double zValue = random.NextDouble();
            double xEntryMin = random.NextDouble();
            double xEntryMax = random.NextDouble();
            bool forbiddenZonesAutomaticallyCalculated = random.NextBoolean();
            bool gridAutomaticallyCalculated = random.NextBoolean();

            var constructionProperties = new MacroStabilityInwardsCalculatorResult.ConstructionProperties
            {
                FactorOfStability = factorOfStability,
                ZValue = zValue,
                ForbiddenZonesXEntryMin = xEntryMin,
                ForbiddenZonesXEntryMax = xEntryMax,
                ForbiddenZonesAutomaticallyCalculated = forbiddenZonesAutomaticallyCalculated,
                GridAutomaticallyCalculated = gridAutomaticallyCalculated
            };

            MacroStabilityInwardsSlidingCurveResult curve = MacroStabilityInwardsSlidingCurveResultTestFactory.Create();
            var upliftVanCalculationGrid = new MacroStabilityInwardsUpliftVanCalculationGridResult(MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   MacroStabilityInwardsGridResultTestFactory.Create(),
                                                                                                   new double[0]);

            // Call
            var result = new MacroStabilityInwardsCalculatorResult(curve, upliftVanCalculationGrid, constructionProperties);

            // Assert
            Assert.AreEqual(factorOfStability, result.FactorOfStability);
            Assert.AreEqual(zValue, result.ZValue);
            Assert.AreEqual(xEntryMin, result.ForbiddenZonesXEntryMin);
            Assert.AreEqual(xEntryMax, result.ForbiddenZonesXEntryMax);
            Assert.AreEqual(forbiddenZonesAutomaticallyCalculated, result.ForbiddenZonesAutomaticallyCalculated);
            Assert.AreEqual(gridAutomaticallyCalculated, result.GridAutomaticallyCalculated);
        }
    }
}