// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Threading;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms.Layout;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using GraphSharp.Controls;
using NUnit.Framework;

namespace Core.Components.GraphSharp.Forms.Test.Layout
{
    [TestFixture]
    public class PointedTreeGraphLayoutTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Constructor_ExpectedValues()
        {
            // Call
            var layout = new PointedTreeGraphLayout();

            // Assert
            Assert.IsInstanceOf<GraphLayout<PointedTreeElementVertex, PointedTreeEdge, PointedTreeGraph>>(layout);
            Assert.AreEqual("Tree", layout.LayoutAlgorithmType);
            Assert.AreEqual("FSA", layout.OverlapRemovalAlgorithmType);
            Assert.AreEqual("Simple", layout.HighlightAlgorithmType);
            Assert.AreEqual(AlgorithmConstraints.Must, layout.OverlapRemovalConstraint);
            Assert.AreEqual(10, layout.OverlapRemovalParameters.HorizontalGap);
            Assert.AreEqual(10, layout.OverlapRemovalParameters.VerticalGap);
            Assert.AreEqual(new TimeSpan(0), layout.AnimationLength);
            Assert.IsFalse(layout.CanAnimate);

            var layoutParamaters = (SimpleTreeLayoutParameters) layout.LayoutParameters;
            Assert.AreEqual(10, layoutParamaters.VertexGap);
            Assert.AreEqual(LayoutDirection.TopToBottom, layoutParamaters.Direction);
            Assert.AreEqual(SpanningTreeGeneration.DFS, layoutParamaters.SpanningTreeGeneration);
        }
    }
}