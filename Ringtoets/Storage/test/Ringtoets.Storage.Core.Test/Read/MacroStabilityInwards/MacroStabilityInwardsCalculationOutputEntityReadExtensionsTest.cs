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
using System.Collections.Generic;
using Application.Ringtoets.Storage.TestUtil.MacroStabilityInwards;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;
using Ringtoets.Storage.Core.Read.MacroStabilityInwards;

namespace Ringtoets.Storage.Core.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((MacroStabilityInwardsCalculationOutputEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_EntityWithValues_ReturnExpectedOutput()
        {
            // Setup
            var random = new Random(21);
            IEnumerable<MacroStabilityInwardsSlice> slices = new[]
            {
                MacroStabilityInwardsSliceTestFactory.CreateSlice()
            };
            IEnumerable<RoundedDouble> tangentLines = new[]
            {
                new RoundedDouble(2, random.NextDouble())
            };

            var entity = new MacroStabilityInwardsCalculationOutputEntity
            {
                FactorOfStability = random.NextDouble(),
                ZValue = random.NextDouble(),
                ForbiddenZonesXEntryMin = random.NextDouble(),
                ForbiddenZonesXEntryMax = random.NextDouble(),
                SlidingCurveSliceXML = new MacroStabilityInwardsSliceXmlSerializer().ToXml(slices),
                SlidingCurveNonIteratedHorizontalForce = random.NextDouble(),
                SlidingCurveIteratedHorizontalForce = random.NextDouble(),
                SlidingCurveLeftSlidingCircleCenterX = random.NextDouble(),
                SlidingCurveLeftSlidingCircleCenterY = random.NextDouble(),
                SlidingCurveLeftSlidingCircleRadius = random.NextDouble(),
                SlidingCurveLeftSlidingCircleIsActive = Convert.ToByte(random.NextBoolean()),
                SlidingCurveLeftSlidingCircleNonIteratedForce = random.NextDouble(),
                SlidingCurveLeftSlidingCircleIteratedForce = random.NextDouble(),
                SlidingCurveLeftSlidingCircleDrivingMoment = random.NextDouble(),
                SlidingCurveLeftSlidingCircleResistingMoment = random.NextDouble(),
                SlidingCurveRightSlidingCircleCenterX = random.NextDouble(),
                SlidingCurveRightSlidingCircleCenterY = random.NextDouble(),
                SlidingCurveRightSlidingCircleRadius = random.NextDouble(),
                SlidingCurveRightSlidingCircleIsActive = Convert.ToByte(random.NextBoolean()),
                SlidingCurveRightSlidingCircleNonIteratedForce = random.NextDouble(),
                SlidingCurveRightSlidingCircleIteratedForce = random.NextDouble(),
                SlidingCurveRightSlidingCircleDrivingMoment = random.NextDouble(),
                SlidingCurveRightSlidingCircleResistingMoment = random.NextDouble(),
                SlipPlaneTangentLinesXml = new TangentLinesXmlSerializer().ToXml(tangentLines),
                SlipPlaneLeftGridXLeft = random.NextDouble(0.0, 1.0),
                SlipPlaneLeftGridXRight = random.NextDouble(2.0, 3.0),
                SlipPlaneLeftGridNrOfHorizontalPoints = random.Next(1, 100),
                SlipPlaneLeftGridZTop = random.NextDouble(2.0, 3.0),
                SlipPlaneLeftGridZBottom = random.NextDouble(0.0, 1.0),
                SlipPlaneLeftGridNrOfVerticalPoints = random.Next(1, 100),
                SlipPlaneRightGridXLeft = random.NextDouble(0.0, 1.0),
                SlipPlaneRightGridXRight = random.NextDouble(2.0, 3.0),
                SlipPlaneRightGridNrOfHorizontalPoints = random.Next(1, 100),
                SlipPlaneRightGridZTop = random.NextDouble(2.0, 3.0),
                SlipPlaneRightGridZBottom = random.NextDouble(0.0, 1.0),
                SlipPlaneRightGridNrOfVerticalPoints = random.Next(1, 100)
            };

            // Call
            MacroStabilityInwardsOutput output = entity.Read();

            // Assert
            MacroStabilityInwardsCalculationOutputEntityTestHelper.AssertOutputPropertyValues(output, entity);
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnExpectedOutputWithNaNValues()
        {
            // Setup
            MacroStabilityInwardsCalculationOutputEntity entity = CreateValidCalculationOutputEntity();

            // Call
            MacroStabilityInwardsOutput output = entity.Read();

            // Assert
            MacroStabilityInwardsCalculationOutputEntityTestHelper.AssertOutputPropertyValues(output, entity);
        }

        [Test]
        public void Read_SlipPlaneTangentLineXMLEmpty_ThrowsArgumentException()
        {
            // Setup
            MacroStabilityInwardsCalculationOutputEntity entity = CreateValidCalculationOutputEntity();
            entity.SlipPlaneTangentLinesXml = string.Empty;

            // Call
            TestDelegate call = () => entity.Read();

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("xml", exception.ParamName);
        }

        [Test]
        public void Read_SlidingCurveSliceXMLEmpty_ThrowsArgumentException()
        {
            // Setup
            MacroStabilityInwardsCalculationOutputEntity entity = CreateValidCalculationOutputEntity();
            entity.SlidingCurveSliceXML = string.Empty;

            // Call
            TestDelegate call = () => entity.Read();

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("xml", exception.ParamName);
        }

        private static MacroStabilityInwardsCalculationOutputEntity CreateValidCalculationOutputEntity()
        {
            var random = new Random(31);
            return new MacroStabilityInwardsCalculationOutputEntity
            {
                SlidingCurveSliceXML = new MacroStabilityInwardsSliceXmlSerializer().ToXml(new MacroStabilityInwardsSlice[0]),
                SlipPlaneTangentLinesXml = new TangentLinesXmlSerializer().ToXml(new RoundedDouble[0]),
                SlipPlaneLeftGridNrOfHorizontalPoints = random.Next(1, 100),
                SlipPlaneLeftGridNrOfVerticalPoints = random.Next(1, 100),
                SlipPlaneRightGridNrOfHorizontalPoints = random.Next(1, 100),
                SlipPlaneRightGridNrOfVerticalPoints = random.Next(1, 100)
            };
        }
    }
}