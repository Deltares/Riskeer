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
using System.Collections.Generic;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Output
{
    [TestFixture]
    public class UpliftVanCalculatorResultTest
    {
        [Test]
        public void Constructor_SlidingCurveResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new UpliftVanCalculatorResult(null, CreateGridResult(), new MacroStabilityInwardsKernelMessage[0],
                                                         new UpliftVanCalculatorResult.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("slidingCurveResult", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationGridResultNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanSlidingCurveResult slidingCurveResult = UpliftVanSlidingCurveResultTestFactory.Create();

            // Call
            void Call() => new UpliftVanCalculatorResult(slidingCurveResult, null, new MacroStabilityInwardsKernelMessage[0],
                                                         new UpliftVanCalculatorResult.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGridResult", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationMessagesNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanSlidingCurveResult slidingCurveResult = UpliftVanSlidingCurveResultTestFactory.Create();

            // Call
            void Call() => new UpliftVanCalculatorResult(slidingCurveResult, CreateGridResult(), null,
                                                         new UpliftVanCalculatorResult.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationMessages", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanSlidingCurveResult slidingCurveResult = UpliftVanSlidingCurveResultTestFactory.Create();

            // Call
            void Call() => new UpliftVanCalculatorResult(slidingCurveResult, CreateGridResult(), new MacroStabilityInwardsKernelMessage[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            UpliftVanSlidingCurveResult slidingCurveResult = UpliftVanSlidingCurveResultTestFactory.Create();
            UpliftVanCalculationGridResult calculationGridResult = CreateGridResult();

            IEnumerable<MacroStabilityInwardsKernelMessage> calculationMessages = new List<MacroStabilityInwardsKernelMessage>();

            // Call
            var result = new UpliftVanCalculatorResult(slidingCurveResult,
                                                       calculationGridResult,
                                                       calculationMessages,
                                                       new UpliftVanCalculatorResult.ConstructionProperties());

            // Assert
            Assert.AreSame(slidingCurveResult, result.SlidingCurveResult);
            Assert.AreSame(calculationGridResult, result.CalculationGridResult);
            Assert.AreSame(calculationMessages, result.CalculationMessages);
        }

        [Test]
        public void Constructor_EmptyConstructionProperties_ExpectedValues()
        {
            // Setup
            UpliftVanSlidingCurveResult slidingCurveResult = UpliftVanSlidingCurveResultTestFactory.Create();

            // Call
            var result = new UpliftVanCalculatorResult(slidingCurveResult,
                                                       CreateGridResult(),
                                                       new MacroStabilityInwardsKernelMessage[0],
                                                       new UpliftVanCalculatorResult.ConstructionProperties());

            // Assert
            Assert.IsNaN(result.FactorOfStability);
            Assert.IsNaN(result.ForbiddenZonesXEntryMin);
            Assert.IsNaN(result.ForbiddenZonesXEntryMax);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithData_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double factorOfStability = random.NextDouble();
            double xEntryMin = random.NextDouble();
            double xEntryMax = random.NextDouble();

            var constructionProperties = new UpliftVanCalculatorResult.ConstructionProperties
            {
                FactorOfStability = factorOfStability,
                ForbiddenZonesXEntryMin = xEntryMin,
                ForbiddenZonesXEntryMax = xEntryMax
            };

            UpliftVanSlidingCurveResult slidingCurveResult = UpliftVanSlidingCurveResultTestFactory.Create();

            // Call
            var result = new UpliftVanCalculatorResult(slidingCurveResult,
                                                       CreateGridResult(),
                                                       new MacroStabilityInwardsKernelMessage[0],
                                                       constructionProperties);

            // Assert
            Assert.AreEqual(factorOfStability, result.FactorOfStability);
            Assert.AreEqual(xEntryMin, result.ForbiddenZonesXEntryMin);
            Assert.AreEqual(xEntryMax, result.ForbiddenZonesXEntryMax);
        }

        private static UpliftVanCalculationGridResult CreateGridResult()
        {
            return new UpliftVanCalculationGridResult(UpliftVanGridTestFactory.Create(),
                                                      UpliftVanGridTestFactory.Create(),
                                                      new double[0]);
        }
    }
}