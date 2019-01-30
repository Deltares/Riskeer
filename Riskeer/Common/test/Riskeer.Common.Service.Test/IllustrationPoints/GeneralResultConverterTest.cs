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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.IllustrationPoints;
using HydraRingGeneralResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;
using HydraRingWindDirection = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;
using HydraRingStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraRingWindDirectionClosingSituation = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraRingIllustrationPointTreeNode = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;
using HydraRingCombinationType = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;
using HydraRingIllustrationPointResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using HydraRingFaultTreeIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingSubMechanismIllustrationPointStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;
using HydraRingSubMechanismIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;

namespace Riskeer.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultConverterTest
    {
        [Test]
        public void ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint_HydraRingGeneralResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GeneralResultConverter.ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingGeneralResult", paramName);
        }

        [Test]
        public void ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint_HydraGeneralResultWithoutIllustrationPoints_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraGoverningWindDirection = new HydraRingWindDirection("Name", random.NextDouble());

            var hydraRingGeneralResult = new HydraRingGeneralResult(
                random.NextDouble(),
                hydraGoverningWindDirection,
                Enumerable.Empty<HydraRingStochast>(),
                new Dictionary<
                    HydraRingWindDirectionClosingSituation,
                    HydraRingIllustrationPointTreeNode>());

            // Call
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult =
                GeneralResultConverter.ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint(hydraRingGeneralResult);

            // Assert
            AssertWindDirection(hydraGoverningWindDirection, generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.TopLevelIllustrationPoints);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
        }

        [Test]
        public void ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint_HydraGeneralResultWithSubMechanismIllustrationPointsOnly_ExpectedProperties()
        {
            // Setup
            const string closingSituation = "Closing situation";

            var random = new Random(21);
            var hydraRingWindDirection = new HydraRingWindDirection("SSE", random.NextDouble());
            var hydraRingWindDirectionClosingSituation = new HydraRingWindDirectionClosingSituation(
                hydraRingWindDirection, closingSituation);

            var hydraRingIllustrationPoint = new HydraRingSubMechanismIllustrationPoint(
                "Illustration Point",
                Enumerable.Empty<HydraRingSubMechanismIllustrationPointStochast>(),
                Enumerable.Empty<HydraRingIllustrationPointResult>(),
                random.NextDouble());
            var hydraRingIllustrationTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingIllustrationPoint);

            var governingHydraWindDirection = new HydraRingWindDirection("Name", random.NextDouble());
            var hydraGeneralResult = new HydraRingGeneralResult(
                random.NextDouble(),
                governingHydraWindDirection,
                Enumerable.Empty<HydraRingStochast>(),
                new Dictionary<HydraRingWindDirectionClosingSituation, HydraRingIllustrationPointTreeNode>
                {
                    {
                        hydraRingWindDirectionClosingSituation, hydraRingIllustrationTreeNode
                    }
                });

            // Call
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult =
                GeneralResultConverter.ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint(hydraGeneralResult);

            // Assert
            WindDirection generalResultGoverningWindDirection = generalResult.GoverningWindDirection;
            AssertWindDirection(governingHydraWindDirection, generalResultGoverningWindDirection);

            CollectionAssert.IsEmpty(generalResult.Stochasts);

            TopLevelSubMechanismIllustrationPoint combination = generalResult.TopLevelIllustrationPoints.Single();
            AssertWindDirection(hydraRingWindDirection, combination.WindDirection);
            Assert.AreEqual(closingSituation, combination.ClosingSituation);

            SubMechanismIllustrationPoint subMechanismIllustrationPoint = combination.SubMechanismIllustrationPoint;
            Assert.AreEqual(hydraRingIllustrationPoint.Name, subMechanismIllustrationPoint.Name);
            Assert.AreEqual(hydraRingIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(subMechanismIllustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(subMechanismIllustrationPoint.IllustrationPointResults);
        }

        [Test]
        public void ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint_HydraRingGeneralResultWithFaultTreeIllustrationPointsOnly_ThrowsIllustrationPointConversionException()
        {
            // Setup
            var random = new Random(21);

            const string closingSituation = "Closing situation";
            var hydraRingWindDirection = new HydraRingWindDirection("SSE", random.NextDouble());
            var hydraRingWindDirectionClosingSituation = new HydraRingWindDirectionClosingSituation(
                hydraRingWindDirection,
                closingSituation);

            var hydraRingIllustrationPoint = new HydraRingFaultTreeIllustrationPoint(
                "IllustrationPoint",
                random.NextDouble(),
                Enumerable.Empty<HydraRingStochast>(),
                random.NextEnumValue<HydraRingCombinationType>());
            var hydraRingIllustrationTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingIllustrationPoint);

            var governingHydraRingWindDirection = new HydraRingWindDirection("Name", random.NextDouble());
            var hydraRingGeneralResult = new HydraRingGeneralResult(
                random.NextDouble(),
                governingHydraRingWindDirection,
                Enumerable.Empty<HydraRingStochast>(),
                new Dictionary<HydraRingWindDirectionClosingSituation, HydraRingIllustrationPointTreeNode>
                {
                    {
                        hydraRingWindDirectionClosingSituation, hydraRingIllustrationTreeNode
                    }
                });

            // Call
            TestDelegate call = () => GeneralResultConverter.ConvertToGeneralResultTopLevelSubMechanismIllustrationPoint(hydraRingGeneralResult);

            // Assert
            var exception = Assert.Throws<IllustrationPointConversionException>(call);
            string expectedMessage = $"Expected a fault tree node with data of type {typeof(HydraRingSubMechanismIllustrationPoint)} as root, " +
                                     $"but got {hydraRingIllustrationPoint.GetType()}";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint_HydraRingGeneralResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GeneralResultConverter.ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingGeneralResult", paramName);
        }

        [Test]
        public void ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint_HydraGeneralResultWithoutIllustrationPoints_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraGoverningWindDirection = new HydraRingWindDirection("Name", random.NextDouble());

            var hydraRingGeneralResult = new HydraRingGeneralResult(
                random.NextDouble(),
                hydraGoverningWindDirection,
                Enumerable.Empty<HydraRingStochast>(),
                new Dictionary<
                    HydraRingWindDirectionClosingSituation,
                    HydraRingIllustrationPointTreeNode>());

            // Call
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult =
                GeneralResultConverter.ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint(hydraRingGeneralResult);

            // Assert
            AssertWindDirection(hydraGoverningWindDirection, generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.TopLevelIllustrationPoints);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
        }

        [Test]
        public void ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint_HydraGeneralResultWithSubMechanismIllustrationPointsOnly_ThrowsIllustrationPointConversionException()
        {
            // Setup
            const string closingSituation = "Closing situation";

            var random = new Random(21);
            var hydraRingWindDirection = new HydraRingWindDirection("SSE", random.NextDouble());
            var hydraRingWindDirectionClosingSituation = new HydraRingWindDirectionClosingSituation(
                hydraRingWindDirection, closingSituation);

            var hydraRingIllustrationPoint = new HydraRingSubMechanismIllustrationPoint(
                "Illustration Point",
                Enumerable.Empty<HydraRingSubMechanismIllustrationPointStochast>(),
                Enumerable.Empty<HydraRingIllustrationPointResult>(),
                random.NextDouble());
            var hydraRingIllustrationTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingIllustrationPoint);

            double governingWindDirectionAngle = random.NextDouble();
            var governingHydraWindDirection = new HydraRingWindDirection("Name", governingWindDirectionAngle);
            var hydraGeneralResult = new HydraRingGeneralResult(
                random.NextDouble(),
                governingHydraWindDirection,
                Enumerable.Empty<HydraRingStochast>(),
                new Dictionary<HydraRingWindDirectionClosingSituation, HydraRingIllustrationPointTreeNode>
                {
                    {
                        hydraRingWindDirectionClosingSituation, hydraRingIllustrationTreeNode
                    }
                });

            // Call
            TestDelegate call = () => GeneralResultConverter.ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint(hydraGeneralResult);

            // Assert
            var exception = Assert.Throws<IllustrationPointConversionException>(call);
            string expectedMessage = $"Expected a fault tree node with data of type {typeof(HydraRingFaultTreeIllustrationPoint)} as root, but got {hydraRingIllustrationPoint.GetType()}";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint_HydraRingGeneralResultWithFaultTreeIllustrationPointsOnly_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);

            const string closingSituation = "Closing situation";
            var hydraRingWindDirection = new HydraRingWindDirection("SSE", random.NextDouble());
            var hydraRingWindDirectionClosingSituation = new HydraRingWindDirectionClosingSituation(
                hydraRingWindDirection,
                closingSituation);

            var hydraRingIllustrationPoint = new HydraRingFaultTreeIllustrationPoint(
                "IllustrationPoint",
                random.NextDouble(),
                Enumerable.Empty<HydraRingStochast>(),
                HydraRingCombinationType.Or);
            var hydraRingIllustrationTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingIllustrationPoint);

            var governingHydraRingWindDirection = new HydraRingWindDirection("Name", random.NextDouble());
            var hydraRingGeneralResult = new HydraRingGeneralResult(
                random.NextDouble(),
                governingHydraRingWindDirection,
                Enumerable.Empty<HydraRingStochast>(),
                new Dictionary<HydraRingWindDirectionClosingSituation, HydraRingIllustrationPointTreeNode>
                {
                    {
                        hydraRingWindDirectionClosingSituation, hydraRingIllustrationTreeNode
                    }
                });

            // Call
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult =
                GeneralResultConverter.ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint(hydraRingGeneralResult);

            // Assert
            WindDirection generalResultGoverningWindDirection =
                generalResult.GoverningWindDirection;
            AssertWindDirection(governingHydraRingWindDirection, generalResultGoverningWindDirection);

            TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint =
                generalResult.TopLevelIllustrationPoints.Single();
            AssertWindDirection(hydraRingWindDirection, topLevelFaultTreeIllustrationPoint.WindDirection);
            Assert.AreEqual(closingSituation, topLevelFaultTreeIllustrationPoint.ClosingSituation);

            IllustrationPointNode faultTreeIllustrationPoint =
                topLevelFaultTreeIllustrationPoint.FaultTreeNodeRoot;
            CollectionAssert.IsEmpty(faultTreeIllustrationPoint.Children);

            var faultTreeIllustrationPointData = (FaultTreeIllustrationPoint) faultTreeIllustrationPoint.Data;
            CollectionAssert.IsEmpty(faultTreeIllustrationPointData.Stochasts);
            Assert.AreEqual(hydraRingIllustrationPoint.Name, faultTreeIllustrationPointData.Name);
            Assert.AreEqual(hydraRingIllustrationPoint.Beta, faultTreeIllustrationPointData.Beta,
                            faultTreeIllustrationPointData.Beta.GetAccuracy());
            Assert.AreEqual(CombinationType.Or, faultTreeIllustrationPointData.CombinationType);
        }

        private static void AssertWindDirection(HydraRingWindDirection hydraRingWindDirection, WindDirection windDirection)
        {
            Assert.AreEqual(hydraRingWindDirection.Name, windDirection.Name);
            Assert.AreEqual(hydraRingWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
        }
    }
}