﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Components.PointedTree.Data;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.Factories;

namespace Riskeer.Common.Forms.Test.Factories
{
    [TestFixture]
    public class RiskeerGraphNodeFactoryTest
    {
        [Test]
        public void CreateGraphNode_IllustrationPointNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => RiskeerGraphNodeFactory.CreateGraphNode(null, Enumerable.Empty<GraphNode>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("illustrationPoint", exception.ParamName);
        }

        [Test]
        public void CreateGraphNode_WithSubMechanismIllustrationPointButChildrenNull_ThrowsArgumentNullException()
        {
            // Setup
            var illustrationPoint = new SubMechanismIllustrationPoint(
                "Illustration Point",
                new Random(31).NextRoundedDouble(),
                Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                Enumerable.Empty<IllustrationPointResult>());

            // Call
            void Call() => RiskeerGraphNodeFactory.CreateGraphNode(illustrationPoint, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("childNodes", exception.ParamName);
        }

        [Test]
        public void CreateGraphNode_WithSubMechanismIllustrationPointEmptyChildren_ReturnsGraphNodeWithExpectedStyling()
        {
            // Setup
            var illustrationPoint = new SubMechanismIllustrationPoint(
                "Illustration Point",
                new Random(31).NextRoundedDouble(),
                Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                Enumerable.Empty<IllustrationPointResult>());

            // Call
            GraphNode graphNode = RiskeerGraphNodeFactory.CreateGraphNode(illustrationPoint, new[]
            {
                CreateTestGraphNode()
            });

            // Assert
            Assert.AreEqual(CreateExpectedGraphNodeContent(illustrationPoint.Name, illustrationPoint.Beta), graphNode.Content);
            Assert.IsTrue(graphNode.IsSelectable);
            CollectionAssert.IsEmpty(graphNode.ChildNodes);

            var expectedStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.SkyBlue, Color.Black, 1);
            AssertEqualStyle(expectedStyle, graphNode.Style);
        }

        [Test]
        public void CreateGraphNode_FaultTreeIllustrationPointNodeDataWithoutChildren_ReturnsExpected()
        {
            // Setup
            var random = new Random(31);
            var illustrationPoint = new FaultTreeIllustrationPoint(
                "Illustration Point",
                random.NextRoundedDouble(),
                Enumerable.Empty<Stochast>(),
                random.NextEnumValue<CombinationType>());

            // Call
            GraphNode graphNode = RiskeerGraphNodeFactory.CreateGraphNode(illustrationPoint,
                                                                          Enumerable.Empty<GraphNode>());

            // Assert
            Assert.AreEqual(CreateExpectedGraphNodeContent(illustrationPoint.Name, illustrationPoint.Beta), graphNode.Content);
            Assert.IsTrue(graphNode.IsSelectable);

            var expectedStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.LightGray, Color.Black, 1);
            AssertEqualStyle(expectedStyle, graphNode.Style);

            Assert.AreEqual(1, graphNode.ChildNodes.Count());
            GraphNode connectingNode = graphNode.ChildNodes.First();
            AssertGraphConnectingNode(CreateExpectedGraphConnectingNodeContent(illustrationPoint.CombinationType), connectingNode);
            CollectionAssert.IsEmpty(connectingNode.ChildNodes);
        }

        [Test]
        public void CreateGraphNode_FaultTreeIllustrationPointNodeDataWithChildren_ReturnsExpected()
        {
            // Setup
            var random = new Random(31);
            var illustrationPoint = new FaultTreeIllustrationPoint(
                "Illustration Point",
                random.NextRoundedDouble(),
                Enumerable.Empty<Stochast>(),
                random.NextEnumValue<CombinationType>());

            IEnumerable<GraphNode> childGraphNodes = new[]
            {
                CreateTestGraphNode()
            };

            // Call
            GraphNode graphNode = RiskeerGraphNodeFactory.CreateGraphNode(illustrationPoint,
                                                                          childGraphNodes);

            // Assert
            Assert.AreEqual(CreateExpectedGraphNodeContent(illustrationPoint.Name, illustrationPoint.Beta), graphNode.Content);
            Assert.IsTrue(graphNode.IsSelectable);

            var expectedStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.LightGray, Color.Black, 1);
            AssertEqualStyle(expectedStyle, graphNode.Style);

            Assert.AreEqual(1, graphNode.ChildNodes.Count());
            GraphNode connectingNode = graphNode.ChildNodes.First();
            AssertGraphConnectingNode(CreateExpectedGraphConnectingNodeContent(illustrationPoint.CombinationType), connectingNode);
            CollectionAssert.AreEqual(childGraphNodes, connectingNode.ChildNodes);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1, "1/1")]
        [TestCase(0.1, "1/10")]
        [TestCase(0.00001, "1/100.000")]
        [TestCase(0.000012, "1/83.332")]
        [TestCase(0.00000123, "1,22998E-6")]
        [TestCase(0.0000099, "9,90019E-6")]
        [TestCase(0, "0")]
        public void CreateGraphNode_FaultTreeIllustrationPointNodeDataWithChildren_ReturnsExpectedGraphNodeContent(double probability, string expectedProbability)
        {
            // Setup
            var random = new Random(31);
            double beta = StatisticsConverter.ProbabilityToReliability(probability);
            var illustrationPoint = new FaultTreeIllustrationPoint(
                "Illustration Point",
                beta,
                Enumerable.Empty<Stochast>(),
                random.NextEnumValue<CombinationType>());

            IEnumerable<GraphNode> childGraphNodes = new[]
            {
                CreateTestGraphNode()
            };

            RoundedDouble roundedBeta = ((RoundedDouble) beta).ToPrecision(5);

            string expectedGraphNodeContent = $"<text><bold>{illustrationPoint.Name}</bold>{Environment.NewLine}" +
                                              $"{Environment.NewLine}" +
                                              $"Berekende kans = {expectedProbability}{Environment.NewLine}" +
                                              $"Betrouwbaarheidsindex = {roundedBeta}</text>";

            // Call
            GraphNode graphNode = RiskeerGraphNodeFactory.CreateGraphNode(illustrationPoint,
                                                                          childGraphNodes);

            // Assert
            Assert.AreEqual(expectedGraphNodeContent, graphNode.Content);
        }

        [Test]
        public void CreateGraphNode_WithFaultTreeIllustrationPointButChildrenNull_ThrowsArgumentNullException()
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();

            // Call
            void Call() => RiskeerGraphNodeFactory.CreateGraphNode(illustrationPoint, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("childNodes", exception.ParamName);
        }

        [Test]
        public void CreateGraphNode_WithNotSupportedIllustrationPoint_ThrowsNotSupportedException()
        {
            // Setup
            var illustrationPoint = new TestIllustrationPoint();

            // Call
            void Call() =>
                RiskeerGraphNodeFactory.CreateGraphNode(illustrationPoint, new[]
                {
                    CreateTestGraphNode()
                });

            // Assert
            var exception = Assert.Throws<NotSupportedException>(Call);
            Assert.AreEqual($"IllustrationPointNode of type {illustrationPoint.GetType().Name} is not supported. " +
                            $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}",
                            exception.Message);
        }

        private static void AssertGraphConnectingNode(string expectedContent, GraphNode actualNode)
        {
            Assert.IsFalse(actualNode.IsSelectable);
            Assert.AreEqual(expectedContent, actualNode.Content);

            var expectedConnectingStyle = new GraphNodeStyle(GraphNodeShape.None, Color.BlanchedAlmond, Color.Black, 1);
            AssertEqualStyle(expectedConnectingStyle, actualNode.Style);
        }

        private static void AssertEqualStyle(GraphNodeStyle expected, GraphNodeStyle actual)
        {
            Assert.AreEqual(expected.FillColor, actual.FillColor);
            Assert.AreEqual(expected.LineColor, actual.LineColor);
            Assert.AreEqual(expected.LineWidth, actual.LineWidth);
            Assert.AreEqual(expected.Shape, actual.Shape);
        }

        private static GraphNode CreateTestGraphNode()
        {
            return new GraphNode("<text>content</text>", new GraphNode[0], false, CreateTestGraphNodeStyle());
        }

        private static GraphNodeStyle CreateTestGraphNodeStyle()
        {
            return new GraphNodeStyle(GraphNodeShape.None, Color.Empty, Color.Empty, 1);
        }

        private static string CreateExpectedGraphNodeContent(string name, RoundedDouble beta)
        {
            RoundedDouble roundedBeta = beta.ToPrecision(5);

            return $"<text><bold>{name}</bold>{Environment.NewLine}" +
                   $"{Environment.NewLine}" +
                   $"Berekende kans = 1/6{Environment.NewLine}" +
                   $"Betrouwbaarheidsindex = {roundedBeta}</text>";
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