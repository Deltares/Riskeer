﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraRingGeneralResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;
using HydraRingWindDirection = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;
using HydraRingStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraRingWindDirectionClosingSituation = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraRingIllustrationPointTreeNode = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;
using HydraRingCombinationType = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;
using HydraRingIllustrationPointResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using HydraRingFaultTreeIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingSubMechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;
using HydraRingSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultConverterTest
    {
        [Test]
        public void CreateGeneralResultTopLevelSubMechanismIllustrationPoint_HydraRingGeneralResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GeneralResultConverter.CreateGeneralResultTopLevelSubMechanismIllustrationPoint(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingGeneralResult", paramName);
        }

        [Test]
        public void CreateGeneralResultTopLevelSubMechanismIllustrationPoint_HydraGeneralResultWithoutIllustrationPoints_ExpectedProperties()
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
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResultSubMechanismIllustrationPoint =
                GeneralResultConverter.CreateGeneralResultTopLevelSubMechanismIllustrationPoint(hydraRingGeneralResult);

            // Assert
            AssertWindDirection(hydraGoverningWindDirection, generalResultSubMechanismIllustrationPoint.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResultSubMechanismIllustrationPoint.TopLevelIllustrationPoints);
            CollectionAssert.IsEmpty(generalResultSubMechanismIllustrationPoint.Stochasts);
        }

        [Test]
        public void CreateGeneralResultTopLevelSubMechanismIllustrationPoint_HydraGeneralResultWithSubMechanismIllustrationPointsOnly_ExpectedProperties()
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
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResultSubMechanismIllustrationPoint =
                GeneralResultConverter.CreateGeneralResultTopLevelSubMechanismIllustrationPoint(hydraGeneralResult);

            // Assert
            WindDirection generalResultGoverningWindDirection = generalResultSubMechanismIllustrationPoint.GoverningWindDirection;
            AssertWindDirection(governingHydraWindDirection, generalResultGoverningWindDirection);

            CollectionAssert.IsEmpty(generalResultSubMechanismIllustrationPoint.Stochasts);

            TopLevelSubMechanismIllustrationPoint combination = generalResultSubMechanismIllustrationPoint.TopLevelIllustrationPoints.Single();
            AssertWindDirection(hydraRingWindDirection, combination.WindDirection);
            Assert.AreEqual(closingSituation, combination.ClosingSituation);

            SubMechanismIllustrationPoint subMechanismIllustrationPoint = combination.SubMechanismIllustrationPoint;
            Assert.AreEqual(hydraRingIllustrationPoint.Name, subMechanismIllustrationPoint.Name);
            Assert.AreEqual(hydraRingIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta, subMechanismIllustrationPoint.Beta.GetAccuracy());
            CollectionAssert.IsEmpty(subMechanismIllustrationPoint.Stochasts);
            CollectionAssert.IsEmpty(subMechanismIllustrationPoint.IllustrationPointResults);
        }

        [Test]
        public void CreateGeneralResultTopLevelSubMechanismIllustrationPoint_HydraRingGeneralResultWithFaultTreeIllustrationPointsOnly_ThrowsIllustrationPointConversionException()
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

            double governingWindDirectionAngle = random.NextDouble();
            var governingHydraRingWindDirection = new HydraRingWindDirection(
                "Name",
                governingWindDirectionAngle);
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
            TestDelegate call = () => GeneralResultConverter.CreateGeneralResultTopLevelSubMechanismIllustrationPoint(hydraRingGeneralResult);

            // Assert
            var exception = Assert.Throws<IllustrationPointConversionException>(call);
            string expectedMessage = $"Expected a fault tree node with data of type {typeof(HydraRingSubMechanismIllustrationPoint)} as root, " +
                                     $"but got {hydraRingIllustrationPoint.GetType()}";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void CreateGeneralResultTopLevelFaultTreeIllustrationPoint_HydraRingGeneralResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GeneralResultConverter.CreateGeneralResultTopLevelFaultTreeIllustrationPoint(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraRingGeneralResult", paramName);
        }

        [Test]
        public void CreateGeneralResultTopLevelFaultTreeIllustrationPoint_HydraGeneralResultWithoutIllustrationPoints_ExpectedProperties()
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
                GeneralResultConverter.CreateGeneralResultTopLevelFaultTreeIllustrationPoint(hydraRingGeneralResult);

            // Assert
            AssertWindDirection(hydraGoverningWindDirection, generalResult.GoverningWindDirection);
            CollectionAssert.IsEmpty(generalResult.TopLevelIllustrationPoints);
            CollectionAssert.IsEmpty(generalResult.Stochasts);
        }

        [Test]
        public void CreateGeneralResultTopLevelFaultTreeIllustrationPoint_HydraGeneralResultWithSubMechanismIllustrationPointsOnly_ThrowsIllustrationPointConversionException()
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
            TestDelegate call = () => GeneralResultConverter.CreateGeneralResultTopLevelFaultTreeIllustrationPoint(hydraGeneralResult);

            // Assert
            var exception = Assert.Throws<IllustrationPointConversionException>(call);
            string expectedMessage = $"Expected a fault tree node with data of type {typeof(HydraRingFaultTreeIllustrationPoint)} as root, but got {hydraRingIllustrationPoint.GetType()}";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void CreateGeneralResultTopLevelFaultTreeIllustrationPoint_HydraRingGeneralResultWithFaultTreeIllustrationPointsOnly_ExpectedProperties()
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

            double governingWindDirectionAngle = random.NextDouble();
            var governingHydraRingWindDirection = new HydraRingWindDirection(
                "Name",
                governingWindDirectionAngle);
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
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResultSubMechanismIllustrationPoint =
                GeneralResultConverter.CreateGeneralResultTopLevelFaultTreeIllustrationPoint(hydraRingGeneralResult);

            // Assert
            WindDirection generalResultGoverningWindDirection =
                generalResultSubMechanismIllustrationPoint.GoverningWindDirection;
            AssertWindDirection(governingHydraRingWindDirection, generalResultGoverningWindDirection);

            TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint =
                generalResultSubMechanismIllustrationPoint.TopLevelIllustrationPoints.Single();
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
            Assert.AreEqual((CombinationType) hydraRingIllustrationPoint.CombinationType, faultTreeIllustrationPointData.CombinationType);
        }

        private static void AssertWindDirection(HydraRingWindDirection hydraRingWindDirection, WindDirection windDirection)
        {
            Assert.AreEqual(hydraRingWindDirection.Name, windDirection.Name);
            Assert.AreEqual(hydraRingWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
        }
    }
}