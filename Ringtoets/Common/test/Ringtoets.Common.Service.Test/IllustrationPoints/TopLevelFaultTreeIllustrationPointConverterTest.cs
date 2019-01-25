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
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraRingIllustrationPointTreeNode = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;
using HydraRingTestWindDirection = Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints.TestWindDirection;
using HydraRingCombinationType = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;
using HydraRingWindDirection = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirection;
using HydraRingWindDirectionClosingSituation = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.WindDirectionClosingSituation;
using HydraRingStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraRingSubMechanismIllustrationPointStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;
using HydraRingFaultTreeIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingSubMechanismIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraRingIllustrationPointResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class TopLevelFaultTreeIllustrationPointConverterTest
    {
        [Test]
        public void Convert_HydraRingWindDirectionClosingSituationNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraRingFaultTreeIllustrationPoint = new HydraRingFaultTreeIllustrationPoint("fault tree",
                                                                                              double.NaN,
                                                                                              Enumerable.Empty<HydraRingStochast>(),
                                                                                              HydraRingCombinationType.And);

            var treeNode = new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPoint);

            // Call
            TestDelegate call = () => TopLevelFaultTreeIllustrationPointConverter.Convert(null, treeNode);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraRingWindDirectionClosingSituation", exception.ParamName);
        }

        [Test]
        public void Convert_HydraRingIllustrationPointTreeNodeNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraRingWindDirectionClosingSituation = new HydraRingWindDirectionClosingSituation(new HydraRingTestWindDirection(),
                                                                                                    "random closing situation");

            // Call
            TestDelegate call = () => TopLevelFaultTreeIllustrationPointConverter.Convert(hydraRingWindDirectionClosingSituation, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraRingIllustrationPointTreeNode", exception.ParamName);
        }

        [Test]
        public void Convert_ValidHydraRingFaultTreeIllustrationPointWithoutChildren_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraRingWindDirection = new HydraRingWindDirection("random name", random.NextDouble());

            var hydraRingWindDirectionClosingSituation = new HydraRingWindDirectionClosingSituation(hydraRingWindDirection,
                                                                                                    "random closing situation");

            var hydraRingStochast = new HydraRingStochast("random stochast",
                                                          random.NextDouble(),
                                                          random.NextDouble());

            var hydraRingFaultTreeIllustrationPoint = new HydraRingFaultTreeIllustrationPoint("fault tree", random.NextDouble(),
                                                                                              new[]
                                                                                              {
                                                                                                  hydraRingStochast
                                                                                              }, HydraRingCombinationType.And);

            var treeNode = new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPoint);

            // Call
            TopLevelFaultTreeIllustrationPoint topLevelIllustrationPoint =
                TopLevelFaultTreeIllustrationPointConverter.Convert(hydraRingWindDirectionClosingSituation, treeNode);

            // Assert
            WindDirection windDirection = topLevelIllustrationPoint.WindDirection;
            Assert.AreEqual(hydraRingWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(hydraRingWindDirection.Name, windDirection.Name);

            Assert.AreEqual(hydraRingWindDirectionClosingSituation.ClosingSituation, topLevelIllustrationPoint.ClosingSituation);
            IllustrationPointNode illustrationPointNode = topLevelIllustrationPoint.FaultTreeNodeRoot;
            CollectionAssert.IsEmpty(illustrationPointNode.Children);

            var illustrationPointData = (FaultTreeIllustrationPoint) illustrationPointNode.Data;
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Name, illustrationPointData.Name);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Beta, illustrationPointData.Beta, illustrationPointData.Beta.GetAccuracy());
            Assert.AreEqual(CombinationType.And, illustrationPointData.CombinationType);

            Stochast stochast = illustrationPointData.Stochasts.Single();
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Name, stochast.Name);
        }

        [Test]
        public void Convert_ValidHydraRingFaultTreeIllustrationPointWithChildren_ExpectedProperties()
        {
            // Setup
            var random = new Random(21);
            var hydraRingWindDirection = new HydraRingWindDirection("random name", random.NextDouble());

            var hydraRingWindDirectionClosingSituation = new HydraRingWindDirectionClosingSituation(hydraRingWindDirection,
                                                                                                    "random closing situation");

            var hydraRingStochast = new HydraRingStochast("random stochast",
                                                          random.NextDouble(),
                                                          random.NextDouble());

            var hydraRingFaultTreeIllustrationPointRoot = new HydraRingFaultTreeIllustrationPoint("fault tree root", random.NextDouble(),
                                                                                                  new[]
                                                                                                  {
                                                                                                      hydraRingStochast
                                                                                                  }, HydraRingCombinationType.And);

            var hydraRingFaultTreeIllustrationPointChildOne = new HydraRingFaultTreeIllustrationPoint("fault tree child one",
                                                                                                      random.NextDouble(),
                                                                                                      new[]
                                                                                                      {
                                                                                                          hydraRingStochast
                                                                                                      },
                                                                                                      HydraRingCombinationType.Or);

            var hydraRingFaultTreeIllustrationPointChildTwo = new HydraRingSubMechanismIllustrationPoint("fault tree child two",
                                                                                                         Enumerable.Empty<HydraRingSubMechanismIllustrationPointStochast>(),
                                                                                                         Enumerable.Empty<HydraRingIllustrationPointResult>(),
                                                                                                         random.NextDouble());

            var treeNodeRoot = new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPointRoot);
            treeNodeRoot.SetChildren(new[]
            {
                new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPointChildOne),
                new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPointChildTwo)
            });

            // Call
            TopLevelFaultTreeIllustrationPoint topLevelIllustrationPoint =
                TopLevelFaultTreeIllustrationPointConverter.Convert(hydraRingWindDirectionClosingSituation, treeNodeRoot);

            // Assert
            WindDirection windDirection = topLevelIllustrationPoint.WindDirection;
            Assert.AreEqual(hydraRingWindDirection.Angle, windDirection.Angle, windDirection.Angle.GetAccuracy());
            Assert.AreEqual(hydraRingWindDirection.Name, windDirection.Name);

            Assert.AreEqual(hydraRingWindDirectionClosingSituation.ClosingSituation, topLevelIllustrationPoint.ClosingSituation);
            IllustrationPointNode illustrationPoint = topLevelIllustrationPoint.FaultTreeNodeRoot;

            var illustrationPointData = (FaultTreeIllustrationPoint) illustrationPoint.Data;
            Assert.AreEqual(hydraRingFaultTreeIllustrationPointRoot.Name, illustrationPointData.Name);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPointRoot.Beta, illustrationPointData.Beta, illustrationPointData.Beta.GetAccuracy());
            Assert.AreEqual(CombinationType.And, illustrationPointData.CombinationType);

            Stochast stochast = illustrationPointData.Stochasts.Single();
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Name, stochast.Name);

            Assert.AreEqual(treeNodeRoot.Children.Count(), illustrationPoint.Children.Count());
            IllustrationPointNode[] children = illustrationPoint.Children.ToArray();
            CollectionAssert.IsEmpty(children[0].Children);
            CollectionAssert.IsEmpty(children[1].Children);

            var childOne = (FaultTreeIllustrationPoint) children[0].Data;
            Stochast childStochast = illustrationPointData.Stochasts.Single();
            Assert.AreEqual(hydraRingStochast.Alpha, childStochast.Alpha, childStochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Duration, childStochast.Duration, childStochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Name, childStochast.Name);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPointChildOne.Name, childOne.Name);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPointChildOne.Beta, childOne.Beta, childOne.Beta.GetAccuracy());
            Assert.AreEqual(CombinationType.Or, childOne.CombinationType);

            var childTwo = (SubMechanismIllustrationPoint) children[1].Data;
            CollectionAssert.IsEmpty(childTwo.Stochasts);
            CollectionAssert.IsEmpty(childTwo.IllustrationPointResults);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPointChildTwo.Name, childTwo.Name);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPointChildTwo.Beta, childTwo.Beta, childTwo.Beta.GetAccuracy());
        }
    }
}