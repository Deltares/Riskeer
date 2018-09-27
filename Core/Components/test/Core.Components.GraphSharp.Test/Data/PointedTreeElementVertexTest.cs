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

using System;
using System.ComponentModel;
using System.Windows.Media;
using Core.Components.GraphSharp.Commands;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.TestUtil;
using NUnit.Framework;

namespace Core.Components.GraphSharp.Test.Data
{
    [TestFixture]
    public class PointedTreeElementVertexTest
    {
        [Test]
        public void Constructor_ContentNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PointedTreeElementVertex(null, Colors.Gray, Colors.Gray, 3, PointedTreeVertexType.None, false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("content", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string content = "test";
            Color fillColor = Colors.Blue;
            Color lineColor = Colors.Gray;
            const int lineWidth = 3;
            const PointedTreeVertexType type = PointedTreeVertexType.None;
            const bool isSelectable = true;

            // Call
            var vertex = new PointedTreeElementVertex(content, fillColor, lineColor, lineWidth, type, isSelectable);

            // Assert
            Assert.IsInstanceOf<INotifyPropertyChanged>(vertex);
            Assert.AreEqual(content, vertex.Content);
            Assert.IsInstanceOf<SolidColorBrush>(vertex.FillColor);
            Assert.AreEqual(fillColor, ((SolidColorBrush) vertex.FillColor).Color);
            Assert.IsInstanceOf<SolidColorBrush>(vertex.LineColor);
            Assert.AreEqual(lineColor, ((SolidColorBrush) vertex.LineColor).Color);
            Assert.AreEqual(lineWidth, vertex.LineWidth);
            Assert.AreEqual(type, vertex.Type);
            Assert.AreEqual(isSelectable, vertex.IsSelectable);
            Assert.IsFalse(vertex.IsSelected);
            Assert.IsInstanceOf<VertexSelectedCommand>(vertex.VertexSelectedCommand);
        }

        [Test]
        public void IsSelected_SetNewValue_PropertyChangedEventFired()
        {
            // Setup
            PointedTreeElementVertex vertex = PointedTreeTestDataFactory.CreatePointedTreeElementVertex(true);

            var propertyChanged = 0;
            vertex.PropertyChanged += (sender, args) => propertyChanged++;

            // Call
            vertex.IsSelected = true;

            // Assert
            Assert.AreEqual(1, propertyChanged);
        }
    }
}