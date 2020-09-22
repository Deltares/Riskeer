// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Util.Reflection;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms;
using Core.Components.GraphSharp.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
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
                Assert.AreEqual("IllustrationPointsFaultTreeControl", control.Name);

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
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint("A")),
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint("B"))
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
        public void GivenControl_WhenDataSetToInvalidIllustrationPointType_ThenThrowsNotSupportedException()
        {
            // Given
            using (var control = new IllustrationPointsFaultTreeControl())
            {
                var notSupported = new TopLevelFaultTreeIllustrationPoint(
                    WindDirectionTestFactory.CreateTestWindDirection(),
                    "closing situation",
                    new IllustrationPointNode(new TestIllustrationPoint()));

                // When
                TestDelegate test = () => control.Data = notSupported;

                // Then
                var exception = Assert.Throws<NotSupportedException>(test);
                Assert.AreEqual($"IllustrationPointNode of type {nameof(TestIllustrationPoint)} is not supported. " +
                                $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}", exception.Message);
            }
        }

        [Test]
        public void GivenControl_WhenDataSetWithInvalidIllustrationPointChildType_ThenThrowsNotSupportedException()
        {
            // Given
            using (var control = new IllustrationPointsFaultTreeControl())
            {
                var rootNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
                rootNode.SetChildren(new[]
                {
                    new IllustrationPointNode(new TestIllustrationPoint()),
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint())
                });

                var topLevelFaultTreeIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(
                    WindDirectionTestFactory.CreateTestWindDirection(),
                    "closing situation",
                    rootNode);

                // When
                TestDelegate test = () => control.Data = topLevelFaultTreeIllustrationPoint;

                // Then
                var exception = Assert.Throws<NotSupportedException>(test);
                Assert.AreEqual($"IllustrationPointNode of type {nameof(TestIllustrationPoint)} is not supported. " +
                                $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}", exception.Message);
            }
        }

        [Test]
        public void GivenControlWithData_WhenVertexSelected_SelectionSetToCorrespondingIllustrationPointNodeSelectionChangedFired()
        {
            // Given
            using (var control = new IllustrationPointsFaultTreeControl())
            {
                var illustrationPointNode = new IllustrationPointNode(new TestSubMechanismIllustrationPoint());

                var rootNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint("A"));
                rootNode.SetChildren(new[]
                {
                    illustrationPointNode,
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint("B"))
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

        private static PointedTreeGraph GetPointedTreeGraph(IllustrationPointsFaultTreeControl control)
        {
            var pointedTreeGraphControl = TypeUtils.GetField<PointedTreeGraphControl>(control, "pointedTreeGraphControl");
            return PointedTreeGraphControlHelper.GetPointedTreeGraph(pointedTreeGraphControl);
        }
    }
}