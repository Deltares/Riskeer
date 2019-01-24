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

using System.Threading;
using System.Windows.Forms;
using Core.Components.GraphSharp.Forms;
using Core.Components.PointedTree.Data;
using Demo.Riskeer.Views;
using NUnit.Framework;

namespace Demo.Riskeer.Test.Views
{
    [TestFixture]
    public class PointedTreeGraphViewTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void DefaultConstructor_Always_AddsPointedTreeGraphControl()
        {
            // Call
            using (var view = new PointedTreeGraphView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                object control = view.Controls[0];
                Assert.IsInstanceOf<PointedTreeGraphControl>(control);

                var pointedTreeGraph = (PointedTreeGraphControl) control;
                Assert.AreEqual(DockStyle.Fill, pointedTreeGraph.Dock);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Data_SetToObject_DoesNotThrow()
        {
            // Setup
            using (var view = new PointedTreeGraphView())
            {
                // Call
                TestDelegate testDelegate = () => view.Data = new object();

                // Assert
                Assert.DoesNotThrow(testDelegate);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Data_SetToGraphNode_DataSet()
        {
            // Setup
            using (var view = new PointedTreeGraphView())
            {
                var graphNode = new GraphNode("<text>Root node</text>", new GraphNode[0], false);

                // Call
                view.Data = graphNode;

                // Assert
                Assert.AreSame(graphNode, view.Data);
            }
        }
    }
}