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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.MacroStabilityInwards;
using Application.Ringtoets.Storage.Serializers;
using Application.Ringtoets.Storage.TestUtil.MacroStabilityInwards;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Application.Ringtoets.Storage.Test.Read.MacroStabilityInwards
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
            IEnumerable<double> tangentLines = new[]
            {
                random.NextDouble()
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
                SlipPlaneLeftGridXLeft = random.NextDouble(),
                SlipPlaneLeftGridXRight = random.NextDouble(),
                SlipPlaneLeftGridNrOfHorizontalPoints = random.Next(),
                SlipPlaneLeftGridZTop = random.NextDouble(),
                SlipPlaneLeftGridZBottom = random.NextDouble(),
                SlipPlaneLeftGridNrOfVerticalPoints = random.Next(),
                SlipPlaneRightGridXLeft = random.NextDouble(),
                SlipPlaneRightGridXRight = random.NextDouble(),
                SlipPlaneRightGridNrOfHorizontalPoints = random.Next(),
                SlipPlaneRightGridZTop = random.NextDouble(),
                SlipPlaneRightGridZBottom = random.NextDouble(),
                SlipPlaneRightGridNrOfVerticalPoints = random.Next()
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
            var entity = new MacroStabilityInwardsCalculationOutputEntity
            {
                SlidingCurveSliceXML = new MacroStabilityInwardsSliceXmlSerializer().ToXml(new MacroStabilityInwardsSlice[0]),
                SlipPlaneTangentLinesXml = new TangentLinesXmlSerializer().ToXml(new double[0])
            };

            // Call
            MacroStabilityInwardsOutput output = entity.Read();

            // Assert
            MacroStabilityInwardsCalculationOutputEntityTestHelper.AssertOutputPropertyValues(output, entity);
        }

        [Test]
        public void Read_SlipPlaneTangentLineXMLEmpty_ThrowsArgumentException()
        {
            // Setup
            var entity = new MacroStabilityInwardsCalculationOutputEntity
            {
                SlidingCurveSliceXML = new MacroStabilityInwardsSliceXmlSerializer().ToXml(new MacroStabilityInwardsSlice[0]),
                SlipPlaneTangentLinesXml = string.Empty
            };

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
            var entity = new MacroStabilityInwardsCalculationOutputEntity
            {
                SlidingCurveSliceXML = string.Empty,
                SlipPlaneTangentLinesXml = new TangentLinesXmlSerializer().ToXml(new double[0])
            };

            // Call
            TestDelegate call = () => entity.Read();

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("xml", exception.ParamName);
        }
    }
}