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

using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms;
using Core.Components.GraphSharp.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class IllustrationPointsFaultTreeControlTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var control = new IllustrationPointsFaultTreeControl())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(control);
                Assert.IsInstanceOf<ISelectionProvider>(control);
                Assert.IsNull(control.Data);
                Assert.IsNull(control.Selection);

                Assert.AreEqual(1, control.Controls.Count);
                var pointedTreeGraphControl = control.Controls[0] as PointedTreeGraphControl;
                Assert.IsNotNull(pointedTreeGraphControl);
            }
        }

        [Test]
        public void GivenControlWithData_WhenDataSetToNull_ThenPointedTreeGraphUpdated()
        {
            // Given
            using (var control = new IllustrationPointsFaultTreeControl())
            {
                var rootNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
                rootNode.SetChildren(new[]
                {
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint()),
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint())
                });

                control.Data = new TopLevelFaultTreeIllustrationPoint(
                    WindDirectionTestFactory.CreateTestWindDirection(),
                    "closing situation",
                    rootNode);

                PointedTreeGraph graph = GetPointedTreeGraph(control);

                // Precondition
                Assert.AreEqual(4, graph.VertexCount);
                Assert.AreEqual(3, graph.EdgeCount);

                // When
                control.Data = null;

                // Then
                graph = GetPointedTreeGraph(control);

                Assert.IsNull(control.Selection);
                Assert.AreEqual(0, graph.VertexCount);
                Assert.AreEqual(0, graph.EdgeCount);
            }
        }

        [Test]
        public void GivenControlWithData_WhenVertexSelected_SelectionSetToCorrespondingIllustrationPointNodeSelectionChangedFired()
        {
            // Given
            using (var control = new IllustrationPointsFaultTreeControl())
            {
                var illustrationPointNode = new IllustrationPointNode(new TestSubMechanismIllustrationPoint());

                var rootNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
                rootNode.SetChildren(new[]
                {
                    illustrationPointNode,
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint())
                });

                control.Data = new TopLevelFaultTreeIllustrationPoint(
                    WindDirectionTestFactory.CreateTestWindDirection(),
                    "closing situation",
                    rootNode);

                var selectionChanged = 0;
                control.SelectionChanged += (sender, args) => selectionChanged++;

                PointedTreeElementVertex selectedVertex = GetPointedTreeGraph(control).Vertices.ElementAt(2);

                // When
                selectedVertex.IsSelected = true;

                // Then
                object selection = control.Selection;
                Assert.AreSame(illustrationPointNode, selection);
                Assert.AreEqual(1, selectionChanged);
            }
        }

        [Test]
        [TestCase(2, TestName = "GivenControlWithDuplicatedData_WhenVertex2Selected_SelectionSetToCorrespondingIllustrationPointNode")]
        [TestCase(3, TestName = "GivenControlWithDuplicatedData_WhenVertex3Selected_SelectionSetToCorrespondingIllustrationPointNode")]
        public void GivenControlWithDuplicatedData_WhenVertexSelected_SelectionSetToCorrespondingIllustrationPointNodeSelectionChangedFired(int vertexIndex)
        {
            // Given
            using (var control = new IllustrationPointsFaultTreeControl())
            {
                var illustrationPointNode = new IllustrationPointNode(new TestSubMechanismIllustrationPoint());

                var rootNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
                rootNode.SetChildren(new[]
                {
                    illustrationPointNode,
                    illustrationPointNode
                });

                control.Data = new TopLevelFaultTreeIllustrationPoint(
                    WindDirectionTestFactory.CreateTestWindDirection(),
                    "closing situation",
                    rootNode);

                var selectionChanged = 0;
                control.SelectionChanged += (sender, args) => selectionChanged++;

                PointedTreeElementVertex selectedVertex = GetPointedTreeGraph(control).Vertices.ElementAt(vertexIndex);

                // When
                selectedVertex.IsSelected = true;

                // Then
                object selection = control.Selection;
                Assert.AreSame(illustrationPointNode, selection);
                Assert.AreEqual(1, selectionChanged);
            }
        }

        private static PointedTreeGraph GetPointedTreeGraph(IllustrationPointsFaultTreeControl control)
        {
            var pointedTreeGraphControl = TypeUtils.GetField<PointedTreeGraphControl>(control, "pointedTreeGraphControl");
            return PointedTreeGraphControlHelper.GetPointedTreeGraph(pointedTreeGraphControl);
        }
    }
}