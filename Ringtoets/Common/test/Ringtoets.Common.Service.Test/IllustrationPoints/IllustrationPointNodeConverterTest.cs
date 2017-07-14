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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Service.IllustrationPoints;
using HydraRingIllustrationPointTreeNode = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;
using HydraRingStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraRingFaultTreeIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraRingSubMechanismIllustrationPointStochast = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;
using HydraRingIllustrationPointResult = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using TestHydraRingSubMechanismIllustrationPoint = Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints.TestSubMechanismIllustrationPoint;
using TestHydraRingFaultTreeIllustrationPoint = Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints.TestFaultTreeIllustrationPoint;
using HydraRingCombinationType = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointNodeConverterTest
    {
        [Test]
        public void Create_HydraRingIllustrationPointTreeNodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => IllustrationPointNodeConverter.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraRingIllustrationPointTreeNode", exception.ParamName);
        }

        [Test]
        public void Create_TreeNodeWithoutChildrenAndFaultTreeIllustrationPointData_ReturnIllustrationPointNode()
        {
            // Setup
            var hydraRingStochast = new HydraRingStochast("stochast", 1, 2);
            var hydraRingFaultTreeIllustrationPoint = new HydraRingFaultTreeIllustrationPoint("point", 3, new []
            {
                hydraRingStochast
            }, HydraRingCombinationType.And);
            var hydraRingIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPoint);

            // Call
            IllustrationPointNode illustrationPointNode = IllustrationPointNodeConverter.Create(hydraRingIllustrationPointTreeNode);

            // Assert
            CollectionAssert.IsEmpty(illustrationPointNode.Children);
            var faultTreeIllustrationPointTreeNodeData = (FaultTreeIllustrationPoint) illustrationPointNode.Data;

            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Name, faultTreeIllustrationPointTreeNodeData.Name);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Beta, faultTreeIllustrationPointTreeNodeData.Beta);
            Assert.AreEqual((int) hydraRingFaultTreeIllustrationPoint.CombinationType, (int) faultTreeIllustrationPointTreeNodeData.CombinationType);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Stochasts.Count(), faultTreeIllustrationPointTreeNodeData.Stochasts.Count());
            Stochast stochast = faultTreeIllustrationPointTreeNodeData.Stochasts.First();

            Assert.AreEqual(hydraRingStochast.Name, stochast.Name);
            Assert.AreEqual(hydraRingStochast.Duration, stochast.Duration);
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha);
        }

        [Test]
        public void Create_TreeNodeWithoutChildrenAndSubMechanismIllustrationPointData_ReturnIllustrationPointNode()
        {
            var hydraRingStochast = new HydraRingSubMechanismIllustrationPointStochast("stochast", 1, 2, 3);
            var hydraRingIllustrationPointResult = new HydraRingIllustrationPointResult("description", 4);
            var hydraRingSubMechanismIllustrationPoint = new HydraRingSubMechanismIllustrationPoint(
                "point",
                new[]
                {
                    hydraRingStochast
                }, new[]
                {
                    hydraRingIllustrationPointResult
                }, 5);
            var hydraRingIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingSubMechanismIllustrationPoint);

            // Call
            IllustrationPointNode illustrationPointNode = IllustrationPointNodeConverter.Create(hydraRingIllustrationPointTreeNode);

            // Assert
            CollectionAssert.IsEmpty(illustrationPointNode.Children);
            var subMechanismIllustrationPointTreeNodeData = (SubMechanismIllustrationPoint) illustrationPointNode.Data;

            Assert.AreEqual(hydraRingSubMechanismIllustrationPoint.Name, subMechanismIllustrationPointTreeNodeData.Name);
            Assert.AreEqual(hydraRingSubMechanismIllustrationPoint.Beta, subMechanismIllustrationPointTreeNodeData.Beta);
            Assert.AreEqual(hydraRingSubMechanismIllustrationPoint.Stochasts.Count(), subMechanismIllustrationPointTreeNodeData.Stochasts.Count());
            Assert.AreEqual(hydraRingSubMechanismIllustrationPoint.Results.Count(), subMechanismIllustrationPointTreeNodeData.IllustrationPointResults.Count());

            SubMechanismIllustrationPointStochast stochast = subMechanismIllustrationPointTreeNodeData.Stochasts.First();
            IllustrationPointResult result = subMechanismIllustrationPointTreeNodeData.IllustrationPointResults.First();

            Assert.AreEqual(hydraRingStochast.Name, stochast.Name);
            Assert.AreEqual(hydraRingStochast.Duration, stochast.Duration);
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha);
            Assert.AreEqual(hydraRingStochast.Realization, stochast.Realization);

            Assert.AreEqual(hydraRingIllustrationPointResult.Description, result.Description);
            Assert.AreEqual(hydraRingIllustrationPointResult.Value, result.Value);
        }

        [Test]
        public void Create_TreeNodeWithChildren_ReturnIllustrationPointNode()
        {
            // Setup
            var hydraRingFaultTreeIllustrationPoint = new TestHydraRingFaultTreeIllustrationPoint();
            var hydraRingIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPoint);

            var nestedHydraRingSubMechanismIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(new TestHydraRingSubMechanismIllustrationPoint());
            var nestedHydraRingFaultTreeIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(new TestHydraRingFaultTreeIllustrationPoint());
            nestedHydraRingFaultTreeIllustrationPointTreeNode.Children.Add(new HydraRingIllustrationPointTreeNode(new TestHydraRingSubMechanismIllustrationPoint()));
            nestedHydraRingFaultTreeIllustrationPointTreeNode.Children.Add(new HydraRingIllustrationPointTreeNode(new TestHydraRingSubMechanismIllustrationPoint()));

            hydraRingIllustrationPointTreeNode.Children.Add(nestedHydraRingSubMechanismIllustrationPointTreeNode);
            hydraRingIllustrationPointTreeNode.Children.Add(nestedHydraRingFaultTreeIllustrationPointTreeNode);

            // Call
            IllustrationPointNode illustrationPointNode = IllustrationPointNodeConverter.Create(hydraRingIllustrationPointTreeNode);

            // Assert
            IllustrationPointNode[] children = illustrationPointNode.Children.ToArray();
            Assert.AreEqual(2, children.Length);
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(children[0].Data);
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(children[1].Data);
            CollectionAssert.IsEmpty(children[0].Children);
            Assert.AreEqual(2, children[1].Children.Count());
        }

        [Test]
        public void Create_TreeNodeWithInvalidNumberOfChildren_ThrowsArgumentException()
        {
            // Setup
            var hydraRingFaultTreeIllustrationPoint = new TestHydraRingFaultTreeIllustrationPoint();
            var hydraRingIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPoint);

            var nestedHydraRingSubMechanismIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(new TestHydraRingSubMechanismIllustrationPoint());
            hydraRingIllustrationPointTreeNode.Children.Add(nestedHydraRingSubMechanismIllustrationPointTreeNode);

            // Call
            TestDelegate test = () => IllustrationPointNodeConverter.Create(hydraRingIllustrationPointTreeNode);

            // Assert
            const string expectedMessage = "Een illustratiepunt node in de foutenboom moet 0 of 2 kind nodes hebben.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
            Assert.AreEqual("children", exception.ParamName);
        }
    }
}