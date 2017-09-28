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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.TestUtil.Test.IllustrationPoints
{
    [TestFixture]
    public class GeneralResultTestFactoryTest
    {
        [Test]
        public void CreateGeneralResultWithNonDistinctStochasts_ExpectedProperties()
        {
            // Call
            GeneralResult generalResult = GeneralResultTestFactory.CreateGeneralResultWithNonDistinctStochasts();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0.5, generalResult.Beta);

            var expectedWindDirection = new TestWindDirection();
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWindDirection);
            Assert.AreEqual(2, generalResult.Stochasts.Count());
            foreach (Stochast stochast in generalResult.Stochasts)
            {
                Assert.AreEqual("Stochast A", stochast.Name);
            }
            CollectionAssert.IsEmpty(generalResult.IllustrationPoints);
        }

        [Test]
        public void CreateGeneralResultWithIncorrectTopLevelStochasts_ExpectedProperties()
        {
            // Call
            GeneralResult generalResult = GeneralResultTestFactory.CreateGeneralResultWithIncorrectTopLevelStochasts();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0.5, generalResult.Beta);

            var expectedWindDirection = new TestWindDirection();
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWindDirection);
            Assert.AreEqual(2, generalResult.Stochasts.Count());
            Stochast[] stochasts = generalResult.Stochasts.ToArray();
            Assert.AreEqual("Stochast A", stochasts[0].Name);
            Assert.AreEqual("Stochast B", stochasts[1].Name);

            KeyValuePair<WindDirectionClosingSituation, IllustrationPointTreeNode> topLevelIllustrationPoint =
                generalResult.IllustrationPoints.Single();

            WindDirectionClosingSituation actualWindDirectionClosingSituation = topLevelIllustrationPoint.Key;
            AssertWindDirection(expectedWindDirection, actualWindDirectionClosingSituation.WindDirection);
            Assert.AreEqual("closing A", actualWindDirectionClosingSituation.ClosingSituation);

            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(topLevelIllustrationPoint.Value.Data);
            Assert.AreEqual("Point A", ((FaultTreeIllustrationPoint) topLevelIllustrationPoint.Value.Data).Name);

            Stochast topLevelIllustrationPointStochast = ((FaultTreeIllustrationPoint) topLevelIllustrationPoint.Value.Data).Stochasts.Single();
            Assert.AreEqual("Stochast C", topLevelIllustrationPointStochast.Name);
        }

        [Test]
        public void CreateGeneralResultWithIncorrectStochastsInChildren_ExpectedProperties()
        {
            // Call
            GeneralResult generalResult = GeneralResultTestFactory.CreateGeneralResultWithIncorrectStochastsInChildren();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0.5, generalResult.Beta);

            var expectedWindDirection = new TestWindDirection();
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWindDirection);
            Assert.AreEqual(2, generalResult.Stochasts.Count());
            Stochast[] stochasts = generalResult.Stochasts.ToArray();
            Assert.AreEqual("Stochast A", stochasts[0].Name);
            Assert.AreEqual("Stochast D", stochasts[1].Name);

            KeyValuePair<WindDirectionClosingSituation, IllustrationPointTreeNode> topLevelIllustrationPoint =
                generalResult.IllustrationPoints.Single();

            WindDirectionClosingSituation actualWindDirectionClosingSituation = topLevelIllustrationPoint.Key;
            AssertWindDirection(expectedWindDirection, actualWindDirectionClosingSituation.WindDirection);
            Assert.AreEqual("closing A", actualWindDirectionClosingSituation.ClosingSituation);

            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(topLevelIllustrationPoint.Value.Data);
            Assert.AreEqual("Point A", ((FaultTreeIllustrationPoint) topLevelIllustrationPoint.Value.Data).Name);

            Stochast topLevelIllustrationPointStochast = ((FaultTreeIllustrationPoint) topLevelIllustrationPoint.Value.Data).Stochasts.Single();
            Assert.AreEqual("Stochast A", topLevelIllustrationPointStochast.Name);

            IllustrationPointTreeNode[] children = topLevelIllustrationPoint.Value.Children.ToArray();
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(children[0].Data);
            SubMechanismIllustrationPointStochast childStochast = ((SubMechanismIllustrationPoint) children[0].Data).Stochasts.Single();
            Assert.AreEqual("Stochast D", childStochast.Name);
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(children[1].Data);
        }

        [Test]
        public void CreateGeneralResultWithNonDistinctIllustrationPoints_ExpectedProperties()
        {
            // Call
            GeneralResult generalResult = GeneralResultTestFactory.CreateGeneralResultWithNonDistinctIllustrationPoints();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0.5, generalResult.Beta);

            AssertWindDirection(new TestWindDirection(), generalResult.GoverningWindDirection);

            KeyValuePair<WindDirectionClosingSituation, IllustrationPointTreeNode>[] topLevelIllustrationPoints =
                generalResult.IllustrationPoints.ToArray();
            Assert.AreEqual(2, topLevelIllustrationPoints.Length);
             
            AssertWindDirection(new WindDirection("N", 0.0), topLevelIllustrationPoints[0].Key.WindDirection);
            Assert.AreEqual("closing A", topLevelIllustrationPoints[0].Key.ClosingSituation);

            AssertWindDirection(new WindDirection("S", 0.0), topLevelIllustrationPoints[1].Key.WindDirection);
            Assert.AreEqual("closing A", topLevelIllustrationPoints[1].Key.ClosingSituation);

            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(topLevelIllustrationPoints[0].Value.Data);
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(topLevelIllustrationPoints[1].Value.Data);
        }

        [Test]
        public void CreateGeneralResultWithNonDistinctIllustrationPointResults_ExpectedProperties()
        {
            // Call
            GeneralResult generalResult = GeneralResultTestFactory.CreateGeneralResultWithNonDistinctIllustrationPointResults();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0.5, generalResult.Beta);

            var expectedWindDirection = new TestWindDirection();
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWindDirection);

            KeyValuePair<WindDirectionClosingSituation, IllustrationPointTreeNode> topLevelIllustrationPoint =
                generalResult.IllustrationPoints.Single();

            WindDirectionClosingSituation actualWindDirectionClosingSituation = topLevelIllustrationPoint.Key;
            AssertWindDirection(expectedWindDirection, actualWindDirectionClosingSituation.WindDirection);
            Assert.AreEqual("closing A", actualWindDirectionClosingSituation.ClosingSituation);

            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(topLevelIllustrationPoint.Value.Data);
            Assert.AreEqual("Point A", ((FaultTreeIllustrationPoint)topLevelIllustrationPoint.Value.Data).Name);

            IllustrationPointTreeNode[] children = topLevelIllustrationPoint.Value.Children.ToArray();
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(children[0].Data);
            IllustrationPointResult[] illustrationPointResults = ((SubMechanismIllustrationPoint)children[0].Data).Results.ToArray();
            Assert.AreEqual("Result A", illustrationPointResults[0].Description);
            Assert.AreEqual(0.0, illustrationPointResults[0].Value);
            Assert.AreEqual("Result A", illustrationPointResults[1].Description);
            Assert.AreEqual(1.0, illustrationPointResults[1].Value);
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(children[1].Data);
        }

        [Test]
        public void CreateGeneralResultWithNonDistinctNamesInChildren_ExpectedProperties()
        {
            // Call
            GeneralResult generalResult = GeneralResultTestFactory.CreateGeneralResultWithNonDistinctNamesInChildren();

            // Assert
            Assert.IsInstanceOf<GeneralResult>(generalResult);
            Assert.AreEqual(0.5, generalResult.Beta);

            var expectedWindDirection = new TestWindDirection();
            AssertWindDirection(expectedWindDirection, generalResult.GoverningWindDirection);

            KeyValuePair<WindDirectionClosingSituation, IllustrationPointTreeNode> topLevelIllustrationPoint =
                generalResult.IllustrationPoints.Single();
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(topLevelIllustrationPoint.Value.Data);

            var children = topLevelIllustrationPoint.Value.Children.ToArray();
            Assert.AreEqual("Point B", ((SubMechanismIllustrationPoint)children[0].Data).Name);
            Assert.AreEqual("Point B", ((SubMechanismIllustrationPoint)children[1].Data).Name);

            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(children[0].Data);
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(children[1].Data);
        }

        private static void AssertWindDirection(WindDirection expected, WindDirection actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Angle, actual.Angle);
        }
    }
}