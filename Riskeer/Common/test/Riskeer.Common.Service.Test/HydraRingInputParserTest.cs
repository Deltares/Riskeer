// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Data;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class HydraRingInputParserTest
    {
        [Test]
        public void ParseForeshore_Use_ReturnCollection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var foreshore = mockRepository.Stub<IUseForeshore>();
            foreshore.UseForeshore = true;

            var pointOne = new Point2D(1, 1);
            var pointTwo = new Point2D(2, 2);
            foreshore.Stub(call => call.ForeshoreGeometry).Return(new RoundedPoint2DCollection(2, new[]
            {
                pointOne,
                pointTwo
            }));
            mockRepository.ReplayAll();

            // Call
            IEnumerable<HydraRingForelandPoint> parsedForeshore = HydraRingInputParser.ParseForeshore(foreshore);

            // Assert 
            HydraRingForelandPoint[] actualForelandPoints = parsedForeshore.ToArray();
            Assert.AreEqual(pointOne.X, actualForelandPoints[0].X);
            Assert.AreEqual(pointOne.Y, actualForelandPoints[0].Z);
            Assert.AreEqual(pointTwo.X, actualForelandPoints[1].X);
            Assert.AreEqual(pointTwo.Y, actualForelandPoints[1].Z);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ParseForeshore_DoesNotUse_ReturnEmptyCollection()
        {
            // Setup
            var mockRepository = new MockRepository();
            var foreshore = mockRepository.Stub<IUseForeshore>();
            foreshore.UseForeshore = false;
            mockRepository.ReplayAll();

            // Call
            IEnumerable<HydraRingForelandPoint> parsedForeshore = HydraRingInputParser.ParseForeshore(foreshore);

            // Assert 
            CollectionAssert.IsEmpty(parsedForeshore);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void ParseBreakWater_Use_ReturnHydraRingBreakWater(BreakWaterType breakWaterType)
        {
            // Setup
            var random = new Random(22);
            double breakWaterHeight = random.NextDouble();

            var mockRepository = new MockRepository();
            var breakWater = mockRepository.Stub<IUseBreakWater>();
            breakWater.UseBreakWater = true;
            var expectedBreakWater = new BreakWater(breakWaterType, breakWaterHeight);
            breakWater.Stub(call => call.BreakWater).Return(expectedBreakWater);
            mockRepository.ReplayAll();

            // Call
            HydraRingBreakWater parsedBreakWater = HydraRingInputParser.ParseBreakWater(breakWater);

            // Assert 
            Assert.AreEqual(BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType), parsedBreakWater.Type);
            Assert.AreEqual(expectedBreakWater.Height, parsedBreakWater.Height, expectedBreakWater.Height.GetAccuracy());
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Dam)]
        public void ParseBreakWater_DoesNotUse_ReturnNull(BreakWaterType breakWaterType)
        {
            // Setup
            var mockRepository = new MockRepository();
            var breakWater = mockRepository.Stub<IUseBreakWater>();
            breakWater.UseBreakWater = false;
            mockRepository.ReplayAll();

            // Call
            HydraRingBreakWater parsedBreakWater = HydraRingInputParser.ParseBreakWater(breakWater);

            // Assert
            Assert.IsNull(parsedBreakWater);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParseBreakWater_InvalidBreakWaterType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(22);
            var mockRepository = new MockRepository();
            var breakWater = mockRepository.Stub<IUseBreakWater>();
            breakWater.UseBreakWater = true;
            var expectedBreakWater = new BreakWater((BreakWaterType) 99, random.NextDouble());
            breakWater.Stub(call => call.BreakWater).Return(expectedBreakWater);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => HydraRingInputParser.ParseBreakWater(breakWater);

            // Assert
            string message = $"The value of argument 'type' ({99}) is invalid for Enum type '{typeof(BreakWaterType).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);

            mockRepository.VerifyAll();
        }
    }
}