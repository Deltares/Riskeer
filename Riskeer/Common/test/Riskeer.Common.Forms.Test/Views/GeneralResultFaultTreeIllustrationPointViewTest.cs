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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Util.Reflection;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms;
using Core.Components.GraphSharp.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class GeneralResultFaultTreeIllustrationPointViewTest
    {
        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void Constructor_WithData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            using (var view = new GeneralResultFaultTreeIllustrationPointView(calculation, () => null))
            {
                // Assert
                Assert.IsInstanceOf<GeneralResultIllustrationPointView<TopLevelFaultTreeIllustrationPoint>>(view);

                Assert.AreEqual(1, view.Controls.Count);

                var splitContainer = view.Controls[0] as SplitContainer;
                Assert.IsNotNull(splitContainer);
                Control.ControlCollection splitContainerPanel1Controls = splitContainer.Panel1.Controls;
                Assert.AreEqual(1, splitContainerPanel1Controls.Count);
                Assert.IsInstanceOf<IllustrationPointsControl>(splitContainerPanel1Controls[0]);

                Control.ControlCollection splitContainerPanel2Controls = splitContainer.Panel2.Controls;
                Assert.AreEqual(1, splitContainerPanel2Controls.Count);
                Assert.IsInstanceOf<IllustrationPointsFaultTreeControl>(splitContainerPanel2Controls[0]);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GeneralResultWithoutIllustrationPoints_DataSetOnIllustrationPointControl()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            var view = new GeneralResultFaultTreeIllustrationPointView(calculation, () => null);

            // Assert
            IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
            Assert.IsNull(illustrationPointsFaultTreeControl.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GeneralResultWithIllustrationPoints_DataSetOnIllustrationPointControl()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            // Call
            var view = new GeneralResultFaultTreeIllustrationPointView(calculation, () => generalResult);
            ShowTestView(view);

            // Assert
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
            AssertIllustrationPointControlItems(generalResult, illustrationPointsControl);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCellInRow_ThenSelectionChangedAndPropagatedAccordingly()
        {
            // Given
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();
            var view = new GeneralResultFaultTreeIllustrationPointView(calculation, () => generalResult);
            ShowTestView(view);

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[0];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(1, selectionChangedCount);

            IEnumerable<TopLevelFaultTreeIllustrationPoint> topLevelFaultTreeIllustrationPoints = generalResult.TopLevelIllustrationPoints.ToArray();
            TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint = topLevelFaultTreeIllustrationPoints.ElementAt(1);
            AssertIllustrationPointSelection(topLevelFaultTreeIllustrationPoint,
                                             topLevelFaultTreeIllustrationPoints.Select(ip => ip.ClosingSituation),
                                             view.Selection);

            IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
            Assert.AreSame(topLevelFaultTreeIllustrationPoint, illustrationPointsFaultTreeControl.Data);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFullyConfiguredView_WhenSelectingFaultTreeIllustrationPointInTree_ThenSelectionChangedAndPropagatedAccordingly(bool sameClosingSituations)
        {
            // Given
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResultFunc = GetGeneralResultWithThreeTopLevelIllustrationPointsWithChildren(sameClosingSituations);
            var view = new GeneralResultFaultTreeIllustrationPointView(calculation, () => generalResultFunc);
            ShowTestView(view);

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
            PointedTreeGraph pointedTreeGraph = GetPointedTreeGraph(illustrationPointsFaultTreeControl);

            // When
            PointedTreeElementVertex selectedVertex = pointedTreeGraph.Vertices.ElementAt(0);
            selectedVertex.IsSelected = true;

            // Then
            Assert.AreEqual(1, selectionChangedCount);

            TopLevelFaultTreeIllustrationPoint topLevel = generalResultFunc.TopLevelIllustrationPoints.First();
            IllustrationPointNode expectedSelectedNode = topLevel.FaultTreeNodeRoot;

            var selectedFaultTreeContext = view.Selection as IllustrationPointContext<FaultTreeIllustrationPoint>;
            Assert.IsNotNull(selectedFaultTreeContext);
            Assert.AreSame(expectedSelectedNode, selectedFaultTreeContext.IllustrationPointNode);

            Assert.AreEqual(sameClosingSituations
                                ? string.Empty
                                : topLevel.ClosingSituation,
                            selectedFaultTreeContext.ClosingSituation);
            Assert.AreEqual(topLevel.WindDirection.Name, selectedFaultTreeContext.WindDirectionName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFullyConfiguredView_WhenSelectingSubMechanismIllustrationPointInTree_ThenSelectionChangedAndPropagatedAccordingly(bool sameClosingSituations)
        {
            // Given
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResultFunc = GetGeneralResultWithThreeTopLevelIllustrationPointsWithChildren(sameClosingSituations);
            var view = new GeneralResultFaultTreeIllustrationPointView(calculation, () => generalResultFunc);
            ShowTestView(view);

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
            PointedTreeGraph pointedTreeGraph = GetPointedTreeGraph(illustrationPointsFaultTreeControl);

            // When
            PointedTreeElementVertex selectedVertex = pointedTreeGraph.Vertices.ElementAt(2);
            selectedVertex.IsSelected = true;

            // Then
            Assert.AreEqual(1, selectionChangedCount);

            TopLevelFaultTreeIllustrationPoint topLevel = generalResultFunc.TopLevelIllustrationPoints.ElementAt(0);
            IllustrationPointNode expectedSelectedNode = topLevel.FaultTreeNodeRoot.Children.First();

            var selectedSubMechanismContext = view.Selection as IllustrationPointContext<SubMechanismIllustrationPoint>;
            Assert.IsNotNull(selectedSubMechanismContext);
            Assert.AreSame(expectedSelectedNode, selectedSubMechanismContext.IllustrationPointNode);
            Assert.AreEqual(sameClosingSituations
                                ? string.Empty
                                : topLevel.ClosingSituation,
                            selectedSubMechanismContext.ClosingSituation);
            Assert.AreEqual(topLevel.WindDirection.Name, selectedSubMechanismContext.WindDirectionName);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithoutIllustrationPoints_WhenIllustrationPointsSetAndNotifiesObserver_ThenControlsSyncedAccordingly()
        {
            // Given
            var returnGeneralResult = false;

            var calculation = new TestCalculation();
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            var view = new GeneralResultFaultTreeIllustrationPointView(calculation, () => returnGeneralResult
                                                                                              ? generalResult
                                                                                              : null);
            ShowTestView(view);

            returnGeneralResult = true;

            // Precondition
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
            IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
            Assert.IsNull(illustrationPointsFaultTreeControl.Data);

            // When
            calculation.NotifyObservers();

            // Then
            AssertIllustrationPointControlItems(generalResult, illustrationPointsControl);
            Assert.AreSame(generalResult.TopLevelIllustrationPoints.First(), illustrationPointsFaultTreeControl.Data);
        }

        [Test]
        public void GivenViewWithIllustrationPoints_WhenIllustrationPointsCleared_ThenControlsSyncedAccordingly()
        {
            // Given
            var returnGeneralResult = true;

            var calculation = new TestCalculation();
            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            var view = new GeneralResultFaultTreeIllustrationPointView(calculation, () => returnGeneralResult
                                                                                              ? generalResult
                                                                                              : null);
            ShowTestView(view);

            // Precondition
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
            IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);

            AssertIllustrationPointControlItems(generalResult, illustrationPointsControl);
            Assert.AreSame(generalResult.TopLevelIllustrationPoints.First(), illustrationPointsFaultTreeControl.Data);

            // When
            returnGeneralResult = false;
            calculation.NotifyObservers();

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
            Assert.IsNull(illustrationPointsFaultTreeControl.Data);
        }

        [Test]
        public void GivenViewWithoutIllustrationPoints_WhenIllustrationPointsChangedAndContainingNotSupportedIllustrationPoints_ThenThrowsNotSupportedException()
        {
            // Given
            var calculation = new TestCalculation();
            var hasGeneralResult = false;

            var view = new GeneralResultFaultTreeIllustrationPointView(calculation, () => hasGeneralResult
                                                                                              ? GetGeneralResultWithTopLevelIllustrationPointsOfNotSupportedType()
                                                                                              : null);
            ShowTestView(view);

            hasGeneralResult = true;

            // When
            void Call() => calculation.NotifyObservers();

            // Assert
            var exception = Assert.Throws<NotSupportedException>(Call);
            Assert.AreEqual($"IllustrationPointNode of type {nameof(TestIllustrationPoint)} is not supported. " +
                            $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}", exception.Message);
        }

        private static GeneralResult<TopLevelFaultTreeIllustrationPoint> GetGeneralResultWithTopLevelIllustrationPointsOfNotSupportedType()
        {
            return new GeneralResult<TopLevelFaultTreeIllustrationPoint>(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                         Enumerable.Empty<Stochast>(),
                                                                         new[]
                                                                         {
                                                                             new TopLevelFaultTreeIllustrationPoint(
                                                                                 WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                 "Closing situation 2",
                                                                                 new IllustrationPointNode(new TestIllustrationPoint()))
                                                                         });
        }

        private static IllustrationPointsFaultTreeControl GetIllustrationPointsFaultTreeControl(GeneralResultFaultTreeIllustrationPointView view)
        {
            return TypeUtils.GetField<IllustrationPointsFaultTreeControl>(view, "illustrationPointsFaultTreeControl");
        }

        private static IllustrationPointsControl GetIllustrationPointsControl(GeneralResultFaultTreeIllustrationPointView view)
        {
            return TypeUtils.GetField<IllustrationPointsControl>(view, "illustrationPointsControl");
        }

        private static PointedTreeGraph GetPointedTreeGraph(IllustrationPointsFaultTreeControl control)
        {
            var pointedTreeGraphControl = TypeUtils.GetField<PointedTreeGraphControl>(control, "pointedTreeGraphControl");
            return PointedTreeGraphControlHelper.GetPointedTreeGraph(pointedTreeGraphControl);
        }

        private void ShowTestView(GeneralResultFaultTreeIllustrationPointView view)
        {
            testForm.Controls.Add(view);
            testForm.Show();
        }

        private static GeneralResult<TopLevelFaultTreeIllustrationPoint> GetGeneralResultWithTwoTopLevelIllustrationPoints()
        {
            var topLevelFaultTreeIllustrationPoint1 =
                new TopLevelFaultTreeIllustrationPoint(
                    WindDirectionTestFactory.CreateTestWindDirection(),
                    "Closing situation 1",
                    new IllustrationPointNode(new TestFaultTreeIllustrationPoint()));

            var topLevelFaultTreeIllustrationPoint2 =
                new TopLevelFaultTreeIllustrationPoint(
                    WindDirectionTestFactory.CreateTestWindDirection(),
                    "Closing situation 2",
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint()));

            return new GeneralResult<TopLevelFaultTreeIllustrationPoint>(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                         Enumerable.Empty<Stochast>(),
                                                                         new[]
                                                                         {
                                                                             topLevelFaultTreeIllustrationPoint1,
                                                                             topLevelFaultTreeIllustrationPoint2
                                                                         });
        }

        private static GeneralResult<TopLevelFaultTreeIllustrationPoint> GetGeneralResultWithThreeTopLevelIllustrationPointsWithChildren(bool sameClosingSituations)
        {
            var faultTreeNodeRootWithChildren = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
            faultTreeNodeRootWithChildren.SetChildren(new[]
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("SubMechanismIllustrationPoint 1")),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("SubMechanismIllustrationPoint 2"))
            });

            var topLevelFaultTreeIllustrationPoint1 =
                new TopLevelFaultTreeIllustrationPoint(
                    new WindDirection("Wind direction 1", 1.0),
                    sameClosingSituations ? "same closing situation" : "first closing situation",
                    faultTreeNodeRootWithChildren);

            var topLevelFaultTreeIllustrationPoint2 =
                new TopLevelFaultTreeIllustrationPoint(
                    new WindDirection("Wind direction 2", 2.0),
                    sameClosingSituations ? "same closing situation" : "second closing situation",
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint()));

            var topLevelFaultTreeIllustrationPoint3 =
                new TopLevelFaultTreeIllustrationPoint(
                    new WindDirection("Wind direction 3", 3.0),
                    sameClosingSituations ? "same closing situation" : "second closing situation",
                    new IllustrationPointNode(new TestSubMechanismIllustrationPoint()));

            var generalResultFunc = new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                WindDirectionTestFactory.CreateTestWindDirection(),
                Enumerable.Empty<Stochast>(),
                new[]
                {
                    topLevelFaultTreeIllustrationPoint1,
                    topLevelFaultTreeIllustrationPoint2,
                    topLevelFaultTreeIllustrationPoint3
                });
            return generalResultFunc;
        }

        private static void AssertIllustrationPointSelection(TopLevelFaultTreeIllustrationPoint expectedSelection,
                                                             IEnumerable<string> expectedClosingSituations,
                                                             object selection)
        {
            var illustrationPointSelection = selection as SelectedTopLevelFaultTreeIllustrationPoint;
            Assert.IsNotNull(illustrationPointSelection);
            Assert.AreSame(expectedSelection, illustrationPointSelection.TopLevelFaultTreeIllustrationPoint);
            CollectionAssert.AreEqual(expectedClosingSituations, illustrationPointSelection.ClosingSituations);
        }

        private static void AssertIllustrationPointControlItems(GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult, IllustrationPointsControl illustrationPointsControl)
        {
            TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint1 = generalResult.TopLevelIllustrationPoints.ElementAt(0);
            TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint2 = generalResult.TopLevelIllustrationPoints.ElementAt(1);
            var faultTreeIllustrationPoint = (FaultTreeIllustrationPoint) topLevelFaultTreeIllustrationPoint1.FaultTreeNodeRoot.Data;
            var subMechanismIllustrationPoint = (SubMechanismIllustrationPoint) topLevelFaultTreeIllustrationPoint2.FaultTreeNodeRoot.Data;
            var expectedControlItems = new[]
            {
                new IllustrationPointControlItem(topLevelFaultTreeIllustrationPoint1,
                                                 topLevelFaultTreeIllustrationPoint1.WindDirection.Name,
                                                 topLevelFaultTreeIllustrationPoint1.ClosingSituation,
                                                 faultTreeIllustrationPoint.Stochasts,
                                                 faultTreeIllustrationPoint.Beta),
                new IllustrationPointControlItem(topLevelFaultTreeIllustrationPoint2,
                                                 topLevelFaultTreeIllustrationPoint2.WindDirection.Name,
                                                 topLevelFaultTreeIllustrationPoint2.ClosingSituation,
                                                 subMechanismIllustrationPoint.Stochasts,
                                                 subMechanismIllustrationPoint.Beta)
            };

            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }
    }
}