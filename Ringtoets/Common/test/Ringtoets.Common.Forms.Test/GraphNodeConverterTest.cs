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
        public void ConvertFaultTreeIllustrationPoint_IllustrationNodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GraphNodeConverter.ConvertFaultTreeIllustrationPoint(null,
                                                                                           Enumerable.Empty<GraphNode>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("illustrationPoint", exception.ParamName);
        }

        [Test]
        public void ConvertFaultTreeIllustrationPoint_ChildrenNull_ThrowsArgumentNullException()
        {
            // Setup
            var illustrationPoint = new TestFaultTreeIllustrationPoint();

            // Call
            TestDelegate test = () => GraphNodeConverter.ConvertFaultTreeIllustrationPoint(illustrationPoint,
                                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("childGraphNodes", exception.ParamName);
        }

        [Test]
        [TestCase(CombinationType.And)]
        [TestCase(CombinationType.Or)]
        public void Convert_FaultTreeIllustrationPointNodeDataWithoutChildren_ReturnsExpected(CombinationType combinationType)
        {
            // Setup
            const string name = "Illustration Point";
            RoundedDouble beta = new Random(7).NextRoundedDouble();
            var illustrationPoint = new FaultTreeIllustrationPoint(
                name,
                beta,
                Enumerable.Empty<Stochast>(),
                combinationType);

            IEnumerable<GraphNode> childGraphNodes = new[]
            {
                GraphNodeConverter.ConvertSubMechanismIllustrationPoint(new TestSubMechanismIllustrationPoint())
            };

            // Call
            GraphNode graphNode = GraphNodeConverter.ConvertFaultTreeIllustrationPoint(illustrationPoint,
                                                                                       childGraphNodes);

            // Assert
            Assert.AreEqual(CreateExpectedGraphNodeContent(name, beta), graphNode.Content);
            Assert.IsTrue(graphNode.IsSelectable);

            Assert.AreEqual(1, graphNode.ChildNodes.Count());
            GraphNode childNode = graphNode.ChildNodes.First();
            Assert.AreEqual(CreateExpectedGraphConnectingNodeContent(combinationType), childNode.Content);
            Assert.IsFalse(childNode.IsSelectable);

            CollectionAssert.AreEqual(childGraphNodes, childNode.ChildNodes);
        }

        [Test]
        public void ConvertSubMechanismIllustrationPoint_IllustrationNodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GraphNodeConverter.ConvertSubMechanismIllustrationPoint(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("illustrationPoint", exception.ParamName);
        }

        [Test]
        public void Convert_ConvertSubMechanismIllustrationPoint_ReturnsExpected()
        {
            // Setup
            const string name = "Illustration Point";
            RoundedDouble beta = new Random(7).NextRoundedDouble();
            var illustrationPoint = new SubMechanismIllustrationPoint(
                name,
                beta,
                Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                Enumerable.Empty<IllustrationPointResult>());

            // Call
            GraphNode graphNode = GraphNodeConverter.ConvertSubMechanismIllustrationPoint(illustrationPoint);

            // Assert
            Assert.AreEqual(CreateExpectedGraphNodeContent(name, beta), graphNode.Content);
            Assert.IsTrue(graphNode.IsSelectable);
            CollectionAssert.IsEmpty(graphNode.ChildNodes);
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