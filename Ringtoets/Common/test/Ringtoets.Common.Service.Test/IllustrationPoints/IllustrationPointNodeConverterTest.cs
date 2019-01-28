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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;
using CombinationType = Ringtoets.Common.Data.IllustrationPoints.CombinationType;
using FaultTreeIllustrationPoint = Ringtoets.Common.Data.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingIllustrationPointTreeNode = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointTreeNode;
using HydraRingStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.Stochast;
using HydraRingFaultTreeIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.FaultTreeIllustrationPoint;
using HydraRingSubMechanismIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPoint;
using HydraRingSubMechanismIllustrationPointStochast = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.SubMechanismIllustrationPointStochast;
using HydraRingIllustrationPointResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IllustrationPointResult;
using TestHydraRingSubMechanismIllustrationPoint = Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints.TestSubMechanismIllustrationPoint;
using TestHydraRingFaultTreeIllustrationPoint = Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints.TestFaultTreeIllustrationPoint;
using HydraRingCombinationType = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;
using HydraRingIIllustrationPoint = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.IIllustrationPoint;
using IllustrationPointResult = Ringtoets.Common.Data.IllustrationPoints.IllustrationPointResult;
using Stochast = Ringtoets.Common.Data.IllustrationPoints.Stochast;
using SubMechanismIllustrationPoint = Ringtoets.Common.Data.IllustrationPoints.SubMechanismIllustrationPoint;
using SubMechanismIllustrationPointStochast = Ringtoets.Common.Data.IllustrationPoints.SubMechanismIllustrationPointStochast;

namespace Ringtoets.Common.Service.Test.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointNodeConverterTest
    {
        [Test]
        public void Convert_HydraRingIllustrationPointTreeNodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => IllustrationPointNodeConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraRingIllustrationPointTreeNode", exception.ParamName);
        }

        [Test]
        public void Convert_TreeNodeWithoutChildrenAndFaultTreeIllustrationPointData_ReturnIllustrationPointNode()
        {
            // Setup
            var hydraRingStochast = new HydraRingStochast("stochast", 1, 2);
            var hydraRingFaultTreeIllustrationPoint = new HydraRingFaultTreeIllustrationPoint("point", 3, new[]
            {
                hydraRingStochast
            }, HydraRingCombinationType.And);
            var hydraRingIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPoint);

            // Call
            IllustrationPointNode illustrationPointNode = IllustrationPointNodeConverter.Convert(hydraRingIllustrationPointTreeNode);

            // Assert
            CollectionAssert.IsEmpty(illustrationPointNode.Children);
            var faultTreeIllustrationPointTreeNodeData = (FaultTreeIllustrationPoint) illustrationPointNode.Data;

            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Name, faultTreeIllustrationPointTreeNodeData.Name);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Beta, faultTreeIllustrationPointTreeNodeData.Beta);
            Assert.AreEqual(CombinationType.And, faultTreeIllustrationPointTreeNodeData.CombinationType);
            Assert.AreEqual(hydraRingFaultTreeIllustrationPoint.Stochasts.Count(), faultTreeIllustrationPointTreeNodeData.Stochasts.Count());
            Stochast stochast = faultTreeIllustrationPointTreeNodeData.Stochasts.First();

            Assert.AreEqual(hydraRingStochast.Name, stochast.Name);
            Assert.AreEqual(hydraRingStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
        }

        [Test]
        public void Convert_TreeNodeWithoutChildrenAndSubMechanismIllustrationPointData_ReturnIllustrationPointNode()
        {
            // Setup
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
            IllustrationPointNode illustrationPointNode = IllustrationPointNodeConverter.Convert(hydraRingIllustrationPointTreeNode);

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
            Assert.AreEqual(hydraRingStochast.Duration, stochast.Duration, stochast.Duration.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Alpha, stochast.Alpha, stochast.Alpha.GetAccuracy());
            Assert.AreEqual(hydraRingStochast.Realization, stochast.Realization, stochast.Realization.GetAccuracy());

            Assert.AreEqual(hydraRingIllustrationPointResult.Description, result.Description);
            Assert.AreEqual(hydraRingIllustrationPointResult.Value, result.Value, result.Value.GetAccuracy());
        }

        [Test]
        public void Convert_TreeNodeWithChildren_ReturnIllustrationPointNode()
        {
            // Setup
            var nestedHydraRingIllustrationPointTreeNodes = new HydraRingIllustrationPointTreeNode(new TestHydraRingFaultTreeIllustrationPoint());
            nestedHydraRingIllustrationPointTreeNodes.SetChildren(new[]
            {
                new HydraRingIllustrationPointTreeNode(new TestHydraRingSubMechanismIllustrationPoint("Point A")),
                new HydraRingIllustrationPointTreeNode(new TestHydraRingSubMechanismIllustrationPoint("Point B"))
            });

            var hydraRingIllustrationPointRootTreeNode = new HydraRingIllustrationPointTreeNode(new TestHydraRingFaultTreeIllustrationPoint());
            hydraRingIllustrationPointRootTreeNode.SetChildren(new[]
            {
                new HydraRingIllustrationPointTreeNode(new TestHydraRingSubMechanismIllustrationPoint("Point C")),
                nestedHydraRingIllustrationPointTreeNodes
            });

            // Call
            IllustrationPointNode illustrationPointNode = IllustrationPointNodeConverter.Convert(hydraRingIllustrationPointRootTreeNode);

            // Assert
            IllustrationPointNode[] children = illustrationPointNode.Children.ToArray();
            Assert.AreEqual(2, children.Length);
            Assert.IsInstanceOf<SubMechanismIllustrationPoint>(children[0].Data);
            Assert.IsInstanceOf<FaultTreeIllustrationPoint>(children[1].Data);
            CollectionAssert.IsEmpty(children[0].Children);
            Assert.AreEqual(2, children[1].Children.Count());
        }

        [Test]
        public void Convert_UnsupportedTreeNodeDataWithoutChildren_ThrowsIllustrationPointConversionException()
        {
            // Setup
            var hydraRingIllustrationPointData = new TestHydraRingIllustrationPointData();
            var hydraRingIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingIllustrationPointData);

            // Call
            TestDelegate call = () => IllustrationPointNodeConverter.Convert(hydraRingIllustrationPointTreeNode);

            // Assert
            var exception = Assert.Throws<IllustrationPointConversionException>(call);
            string expectedMessage = $"An illustration point containing a Hydra ring data type of {hydraRingIllustrationPointData.GetType()} is not supported.";
            Assert.AreEqual(expectedMessage, exception.Message);

            string expectedMessageInnerException = $"Cannot convert {hydraRingIllustrationPointData.GetType()}.";
            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<NotSupportedException>(innerException);
            Assert.AreEqual(expectedMessageInnerException, innerException.Message);
        }

        [Test]
        public void Convert_ValidTreeNodeRootWithUnsupportedTreeNodeDataChildren_ThrowsIllustrationPointConversionException()
        {
            // Setup
            var hydraRingFaultTreeIllustrationPoint = new TestHydraRingFaultTreeIllustrationPoint();
            var hydraRingIllustrationPointTreeNode = new HydraRingIllustrationPointTreeNode(hydraRingFaultTreeIllustrationPoint);

            var nestedUnsupportedIllustrationPointData = new HydraRingIllustrationPointTreeNode(new TestHydraRingIllustrationPointData());
            var nestedUnsupportedIllustrationPointData2 = new HydraRingIllustrationPointTreeNode(new TestHydraRingIllustrationPointData());
            hydraRingIllustrationPointTreeNode.SetChildren(new[]
            {
                nestedUnsupportedIllustrationPointData,
                nestedUnsupportedIllustrationPointData2
            });

            IllustrationPointNode illustrationPointNode = null;

            // Call
            TestDelegate call = () => illustrationPointNode = IllustrationPointNodeConverter.Convert(hydraRingIllustrationPointTreeNode);

            // Assert
            Assert.IsNull(illustrationPointNode);

            var exception = Assert.Throws<IllustrationPointConversionException>(call);
            string expectedMessage = $"An illustration point containing a Hydra ring data type of {typeof(TestHydraRingIllustrationPointData)} is not supported.";
            Assert.AreEqual(expectedMessage, exception.Message);

            string expectedMessageInnerException = $"Cannot convert {typeof(TestHydraRingIllustrationPointData)}.";
            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<NotSupportedException>(innerException);
            Assert.AreEqual(expectedMessageInnerException, innerException.Message);
        }

        private class TestHydraRingIllustrationPointData : IIllustrationPoint {}
    }
}