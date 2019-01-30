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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.MacroStabilityInwards.Data.TestUtil;

namespace Riskeer.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsOutputTest
    {
        [Test]
        public void Constructor_SlidingCurveNull_ThrowsArgumentNullException()
        {
            // Setup
            var slipPlane = new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                                        MacroStabilityInwardsGridTestFactory.Create(),
                                                                        new RoundedDouble[0]);

            // Call
            TestDelegate call = () => new MacroStabilityInwardsOutput(null, slipPlane, new MacroStabilityInwardsOutput.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("slidingCurve", exception.ParamName);
        }

        [Test]
        public void Constructor_SlipPlaneNull_ThrowsArgumentNullException()
        {
            // Setup
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     new MacroStabilityInwardsSlice[0], 0, 0);
            // Call
            TestDelegate call = () => new MacroStabilityInwardsOutput(slidingCurve, null,
                                                                      new MacroStabilityInwardsOutput.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("slipPlane", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     new MacroStabilityInwardsSlice[0], 0, 0);

            var slipPlane = new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                                        MacroStabilityInwardsGridTestFactory.Create(),
                                                                        new RoundedDouble[0]);

            // Call
            TestDelegate call = () => new MacroStabilityInwardsOutput(slidingCurve, slipPlane, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValuesSet_PropertiesAreDefault()
        {
            // Setup
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     new MacroStabilityInwardsSlice[0], 0, 0);

            var slipPlane = new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                                        MacroStabilityInwardsGridTestFactory.Create(),
                                                                        new RoundedDouble[0]);

            // Call
            var output = new MacroStabilityInwardsOutput(slidingCurve, slipPlane, new MacroStabilityInwardsOutput.ConstructionProperties());

            // Assert
            Assert.IsNaN(output.FactorOfStability);
            Assert.IsNaN(output.ZValue);
            Assert.IsNaN(output.ForbiddenZonesXEntryMin);
            Assert.IsNaN(output.ForbiddenZonesXEntryMax);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     new MacroStabilityInwardsSlice[0], 0, 0);

            var slipPlane = new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                                        MacroStabilityInwardsGridTestFactory.Create(),
                                                                        new RoundedDouble[0]);

            var random = new Random(21);
            double factorOfStability = random.NextDouble();
            double zValue = random.NextDouble();
            double xEntryMin = random.NextDouble();
            double xEntryMax = random.NextDouble();

            var properties = new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = factorOfStability,
                ZValue = zValue,
                ForbiddenZonesXEntryMin = xEntryMin,
                ForbiddenZonesXEntryMax = xEntryMax
            };

            // Call
            var output = new MacroStabilityInwardsOutput(slidingCurve, slipPlane, properties);

            // Assert
            Assert.IsInstanceOf<CloneableObservable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);

            Assert.AreSame(slidingCurve, output.SlidingCurve);
            Assert.AreSame(slipPlane, output.SlipPlane);

            Assert.AreEqual(factorOfStability, output.FactorOfStability);
            Assert.AreEqual(zValue, output.ZValue);
            Assert.AreEqual(xEntryMin, output.ForbiddenZonesXEntryMin);
            Assert.AreEqual(xEntryMax, output.ForbiddenZonesXEntryMax);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                     new[]
                                                                     {
                                                                         MacroStabilityInwardsSliceTestFactory.CreateSlice()
                                                                     },
                                                                     random.NextDouble(),
                                                                     random.NextDouble());

            var slipPlane = new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                                        MacroStabilityInwardsGridTestFactory.Create(),
                                                                        new[]
                                                                        {
                                                                            random.NextRoundedDouble()
                                                                        });

            var properties = new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = random.NextDouble(),
                ZValue = random.NextDouble(),
                ForbiddenZonesXEntryMin = random.NextDouble(),
                ForbiddenZonesXEntryMax = random.NextDouble()
            };

            var original = new MacroStabilityInwardsOutput(slidingCurve, slipPlane, properties);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }
    }
}