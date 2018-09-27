// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Drawing;
using Core.Components.PointedTree.Data;
using NUnit.Framework;

namespace Core.Components.PointedTree.Test.Data
{
    [TestFixture]
    public class GraphNodeStyleTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            Color fillColor = Color.DarkRed;
            Color lineColor = Color.DarkBlue;
            const int lineWidth = 2;
            const GraphNodeShape shape = GraphNodeShape.None;

            // Call
            var style = new GraphNodeStyle(shape, fillColor, lineColor, lineWidth);

            // Assert
            Assert.AreEqual(shape, style.Shape);
            Assert.AreEqual(fillColor, style.FillColor);
            Assert.AreEqual(lineColor, style.LineColor);
            Assert.AreEqual(lineWidth, style.LineWidth);
        }
    }
}