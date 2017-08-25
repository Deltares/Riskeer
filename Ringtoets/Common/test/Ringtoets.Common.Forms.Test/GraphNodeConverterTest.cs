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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Components.PointedTree.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Common.Forms.Test
{
    [TestFixture]
    public class GraphNodeConverterTest
    {
        [Test]
        public void Convert_NodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GraphNodeConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("node", exception.ParamName);
        }

        [Test]
        public void Convert_InvalidTypeNodeData_ThrowsNotSupportedException()
        {
            // Setup
            var node = new IllustrationPointNode(new TestIllustrationPoint());

            // Call
            TestDelegate test = () => GraphNodeConverter.Convert(node);

            // Assert
            var exception = Assert.Throws<NotSupportedException>(test);
            Assert.AreEqual($"Cannot convert {node.Data.GetType()}.", exception.Message);
        }

        [Test]
        public void Convert_SubMechanismIllustrationPointNodeData_ReturnsExpected()
        {
            // Setup
            const string name = "Illustration Point";
            RoundedDouble beta = new Random(7).NextRoundedDouble();
            var node = new IllustrationPointNode(new SubMechanismIllustrationPoint(
                                                     name,
                                                     beta,
                                                     Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                     Enumerable.Empty<IllustrationPointResult>()));

            // Call
            GraphNode graphNode = GraphNodeConverter.Convert(node);

            // Assert
            Assert.AreEqual(CreateExpectedGraphNodeContent(name, beta), graphNode.Content);
            Assert.IsTrue(graphNode.IsSelectable);
            CollectionAssert.IsEmpty(graphNode.ChildNodes);
        }

        [Test]
        [TestCase(CombinationType.And)]
        [TestCase(CombinationType.Or)]
        public void Convert_FaultTreeIllustrationPointNodeDataWithoutChildren_ReturnsExpected(CombinationType combinationType)
        {
            // Setup
            const string name = "Illustration Point";
            RoundedDouble beta = new Random(7).NextRoundedDouble();
            var node = new IllustrationPointNode(new FaultTreeIllustrationPoint(
                                                     name,
                                                     beta,
                                                     Enumerable.Empty<Stochast>(),
                                                     combinationType));

            // Call
            GraphNode graphNode = GraphNodeConverter.Convert(node);

            // Assert
            Assert.AreEqual(CreateExpectedGraphNodeContent(name, beta), graphNode.Content);
            Assert.IsTrue(graphNode.IsSelectable);

            Assert.AreEqual(1, graphNode.ChildNodes.Count());
            GraphNode childNode = graphNode.ChildNodes.First();
            Assert.AreEqual(CreateExpectedGraphConnectingNodeContent(combinationType), childNode.Content);
            Assert.IsFalse(childNode.IsSelectable);
            CollectionAssert.IsEmpty(childNode.ChildNodes);
        }

        [Test]
        public void Convert_FaultTreeIllustrationPointNodeDataWithChildren_ReturnsExpected()
        {
            // Setup
            var random = new Random(70);

            var childFaultTreeNode = new IllustrationPointNode(new FaultTreeIllustrationPoint(
                                                                   "ChildFaultTreeIllustrationPoint",
                                                                   random.NextRoundedDouble(),
                                                                   Enumerable.Empty<Stochast>(),
                                                                   CombinationType.And));
            childFaultTreeNode.SetChildren(new[]
            {
                new IllustrationPointNode(new SubMechanismIllustrationPoint(
                                              "ChildChildSubMechanismIllustrationPoint1",
                                              random.NextRoundedDouble(),
                                              Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                              Enumerable.Empty<IllustrationPointResult>())),
                new IllustrationPointNode(new SubMechanismIllustrationPoint(
                                              "ChildChildSubMechanismIllustrationPoint2",
                                              random.NextRoundedDouble(),
                                              Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                              Enumerable.Empty<IllustrationPointResult>()))
            });

            var node = new IllustrationPointNode(new FaultTreeIllustrationPoint(
                                                     "FaultTreeIllustrationPoint",
                                                     random.NextRoundedDouble(),
                                                     Enumerable.Empty<Stochast>(),
                                                     CombinationType.Or));
            node.SetChildren(new[]
            {
                new IllustrationPointNode(new SubMechanismIllustrationPoint(
                                              "SubMechanismIllustrationPoint",
                                              random.NextRoundedDouble(),
                                              Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                              Enumerable.Empty<IllustrationPointResult>())),
                childFaultTreeNode
            });

            // Call
            GraphNode graphNode = GraphNodeConverter.Convert(node);

            // Assert
            Assert.AreEqual(CreateExpectedGraphNodeContent(node.Data.Name, node.Data.Beta), graphNode.Content);

            Assert.AreEqual(1, graphNode.ChildNodes.Count());
            GraphNode connectingNode1 = graphNode.ChildNodes.First();
            Assert.AreEqual(CreateExpectedGraphConnectingNodeContent(CombinationType.Or), connectingNode1.Content);

            Assert.AreEqual(2, connectingNode1.ChildNodes.Count());

            IllustrationPointNode childIllustrationPointNode1 = node.Children.ElementAt(0);
            GraphNode childGraphNode1 = connectingNode1.ChildNodes.ElementAt(0);
            Assert.AreEqual(CreateExpectedGraphNodeContent(childIllustrationPointNode1.Data.Name, childIllustrationPointNode1.Data.Beta),
                            childGraphNode1.Content);
            CollectionAssert.IsEmpty(childGraphNode1.ChildNodes);

            IllustrationPointNode childIllustrationPointNode2 = node.Children.ElementAt(1);
            GraphNode childGraphNode2 = connectingNode1.ChildNodes.ElementAt(1);
            Assert.AreEqual(CreateExpectedGraphNodeContent(childIllustrationPointNode2.Data.Name, childIllustrationPointNode2.Data.Beta),
                            childGraphNode2.Content);

            Assert.AreEqual(1, childGraphNode2.ChildNodes.Count());
            GraphNode connectingNode2 = childGraphNode2.ChildNodes.First();
            Assert.AreEqual(CreateExpectedGraphConnectingNodeContent(CombinationType.And), connectingNode2.Content);

            Assert.AreEqual(2, connectingNode2.ChildNodes.Count());

            IllustrationPointNode childIllustrationPointNode21 = childIllustrationPointNode2.Children.ElementAt(0);
            GraphNode childGraphNode11 = connectingNode2.ChildNodes.ElementAt(0);
            Assert.AreEqual(CreateExpectedGraphNodeContent(childIllustrationPointNode21.Data.Name, childIllustrationPointNode21.Data.Beta),
                            childGraphNode11.Content);
            CollectionAssert.IsEmpty(childGraphNode11.ChildNodes);

            IllustrationPointNode childIllustrationPointNode22 = childIllustrationPointNode2.Children.ElementAt(0);
            GraphNode childGraphNode12 = connectingNode2.ChildNodes.ElementAt(0);
            Assert.AreEqual(CreateExpectedGraphNodeContent(childIllustrationPointNode22.Data.Name, childIllustrationPointNode22.Data.Beta),
                            childGraphNode12.Content);
            CollectionAssert.IsEmpty(childGraphNode12.ChildNodes);
        }

        private static string CreateExpectedGraphNodeContent(string name, RoundedDouble beta)
        {
            RoundedDouble roundedBeta = beta.ToPrecision(5);
            string probability = ProbabilityFormattingHelper.Format(StatisticsConverter.ReliabilityToProbability(beta));

            return $"<text><bold>{name}</bold>{Environment.NewLine}" +
                   $"{Environment.NewLine}" +
                   $"Beta = {roundedBeta}{Environment.NewLine}" +
                   $"Pf = {probability}</text>";
        }

        private static string CreateExpectedGraphConnectingNodeContent(CombinationType combinationType)
        {
            string name = combinationType == CombinationType.And
                              ? "En"
                              : "Of";
            return $"<text>{name}</text>";
        }
    }
}