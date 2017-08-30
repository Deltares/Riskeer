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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms;
using Core.Components.GraphSharp.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
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
        public void Constructor_GetGeneralResultFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GeneralResultFaultTreeIllustrationPointView(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("getGeneralResultFunc", paramName);
        }

        [Test]
        public void Constructor_ValidArguments_ValuesAsExpected()
        {
            // Call
            using (GeneralResultFaultTreeIllustrationPointView view = GetValidView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsNull(view.Data);

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
        }

        [Test]
        public void Data_ICalculation_DataSet()
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            using (GeneralResultFaultTreeIllustrationPointView view = GetValidView())
            {
                // Call
                view.Data = data;

                // Assert
                Assert.AreSame(data, view.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Data_OtherThanICalculation_NullSet()
        {
            // Setup
            var data = new object();

            using (GeneralResultFaultTreeIllustrationPointView view = GetValidView())
            {
                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_Null_NullSet()
        {
            // Setup
            using (GeneralResultFaultTreeIllustrationPointView view = GetValidView())
            {
                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void GivenViewWithGeneralResultFuncReturningTestData_WhenSettingData_ThenIllustrationPointsControlSyncedAccordingly()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => generalResult))
            {
                // When
                view.Data = data;

                // Then
                TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint1 = generalResult.TopLevelIllustrationPoints.ElementAt(0);
                TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint2 = generalResult.TopLevelIllustrationPoints.ElementAt(1);
                var illustrationPointsControl = TypeUtils.GetField<IllustrationPointsControl>(view, "illustrationPointsControl");
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

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithGeneralResultFuncReturningNull_WhenSettingData_ThenControlsSyncedAccordingly()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => null))
            {
                // When
                view.Data = data;

                // Then
                var illustrationPointsControl = TypeUtils.GetField<IllustrationPointsControl>(view, "illustrationPointsControl");
                Assert.IsNull(illustrationPointsControl.Data);

                var illustrationPointsFaultTreeControl = TypeUtils.GetField<IllustrationPointsFaultTreeControl>(view, "illustrationPointsFaultTreeControl");
                Assert.IsNull(illustrationPointsFaultTreeControl.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithDataSetAndGeneralResultFuncReturningEmptyData_WhenDataNotifiesObserver_ThenIllustrationPointsControlSyncedAccordingly()
        {
            // Given
            var returnGeneralResult = false;
            var data = new TestCalculation();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => returnGeneralResult
                                                                                        ? GetGeneralResultWithoutTopLevelIllustrationPoints()
                                                                                        : null)
            {
                Data = data
            })
            {
                returnGeneralResult = true;

                // Precondition
                var illustrationPointsControl = TypeUtils.GetField<IllustrationPointsControl>(view, "illustrationPointsControl");
                Assert.IsNull(illustrationPointsControl.Data);

                // When
                data.NotifyObservers();

                // Then
                CollectionAssert.IsEmpty(illustrationPointsControl.Data);
            }
        }

        [Test]
        public void GivenDisposedViewWithDataSetAndGeneralResultFuncReturningEmptyData_WhenDataNotifiesObserver_ThenIllustrationPointsControlNoLongerSynced()
        {
            // Given
            var returnGeneralResult = false;
            var data = new TestCalculation();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => returnGeneralResult
                                                                                        ? GetGeneralResultWithoutTopLevelIllustrationPoints()
                                                                                        : null)
            {
                Data = data
            })
            {
                view.Dispose();

                returnGeneralResult = true;

                // Precondition
                var illustrationPointsControl = TypeUtils.GetField<IllustrationPointsControl>(view, "illustrationPointsControl");
                Assert.IsNull(illustrationPointsControl.Data);

                // When
                data.NotifyObservers();

                // Then
                Assert.IsNull(illustrationPointsControl.Data);
            }
        }

        [Test]
        public void GivenViewWithGeneralResultFuncReturningTestData_WhenSettingData_ThenSelectionChangedAndPropagatedAccordingly()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(GetGeneralResultWithTwoTopLevelIllustrationPoints))
            {
                ShowTestView(view);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                // When
                view.Data = data;

                // Then
                Assert.AreNotEqual(0, selectionChangedCount);
                Assert.AreSame(null, view.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithGeneralResultFuncReturningEmptyData_WhenSettingData_ThenNoSelectionChangedAndPropagated()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(GetGeneralResultWithoutTopLevelIllustrationPoints))
            {
                ShowTestView(view);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                // When
                view.Data = data;

                // Then
                Assert.AreEqual(0, selectionChangedCount);
                Assert.IsNull(view.Selection);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCellInRow_ThenSelectionChangedAndPropagatedAccordingly()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => generalResult)
            {
                Data = data
            })
            {
                ShowTestView(view);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

                // When
                dataGridView.CurrentCell = dataGridView.Rows[1].Cells[0];
                EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

                // Then
                Assert.AreEqual(1, selectionChangedCount);
                TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint = generalResult.TopLevelIllustrationPoints.ElementAt(1);
                Assert.AreSame(topLevelFaultTreeIllustrationPoint, ((SelectedTopLevelFaultTreeIllustrationPoint) view.Selection).TopLevelFaultTreeIllustrationPoint);

                var illustrationPointsFaultTreeControl = TypeUtils.GetField<IllustrationPointsFaultTreeControl>(view, "illustrationPointsFaultTreeControl");
                Assert.AreSame(topLevelFaultTreeIllustrationPoint, illustrationPointsFaultTreeControl.Data);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingFaultTreeIllustrationPointInTree_ThenSelectionChangedAndPropagatedAccordingly()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResultFunc = GetGeneralResultWithTwoTopLevelIllustrationPoints();
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => generalResultFunc)
            {
                Data = data
            })
            {
                ShowTestView(view);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                var illustrationPointsFaultTreeControl = TypeUtils.GetField<IllustrationPointsFaultTreeControl>(view, "illustrationPointsFaultTreeControl");
                PointedTreeGraph pointedTreeGraph = GetPointedTreeGraph(illustrationPointsFaultTreeControl);

                // When
                PointedTreeElementVertex selectedVertex = pointedTreeGraph.Vertices.ElementAt(0);
                selectedVertex.IsSelected = true;

                // Then
                Assert.AreEqual(1, selectionChangedCount);

                TopLevelFaultTreeIllustrationPoint topLevel = generalResultFunc.TopLevelIllustrationPoints.First();
                IllustrationPointNode expectedSelectedNode = topLevel.FaultTreeNodeRoot;

                var selectedFaultTreeContext = view.Selection as IllustrationPointNodeFaultTreeContext;
                Assert.IsNotNull(selectedFaultTreeContext);
                Assert.AreSame(expectedSelectedNode, selectedFaultTreeContext.IllustrationPointNode);
                Assert.AreEqual(topLevel.ClosingSituation, selectedFaultTreeContext.ClosingSituation);
                Assert.AreEqual(topLevel.WindDirection.Name, selectedFaultTreeContext.WindDirectionName);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingSubMechanismIllustrationPointInTree_ThenSelectionChangedAndPropagatedAccordingly()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResultFunc = GetGeneralResultWithTopLevelIllustrationPointsWithChildren();
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => generalResultFunc)
            {
                Data = data
            })
            {
                ShowTestView(view);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                var illustrationPointsFaultTreeControl = TypeUtils.GetField<IllustrationPointsFaultTreeControl>(view, "illustrationPointsFaultTreeControl");
                PointedTreeGraph pointedTreeGraph = GetPointedTreeGraph(illustrationPointsFaultTreeControl);

                // When
                PointedTreeElementVertex selectedVertex = pointedTreeGraph.Vertices.ElementAt(2);
                selectedVertex.IsSelected = true;

                // Then
                Assert.AreEqual(1, selectionChangedCount);

                TopLevelFaultTreeIllustrationPoint topLevel = generalResultFunc.TopLevelIllustrationPoints.ElementAt(0);
                IllustrationPointNode expectedSelectedNode = topLevel.FaultTreeNodeRoot.Children.First();

                var selectedSubMechanismContext = view.Selection as IllustrationPointNodeSubMechanismContext;
                Assert.IsNotNull(selectedSubMechanismContext);
                Assert.AreSame(expectedSelectedNode, selectedSubMechanismContext.IllustrationPointNode);
                Assert.AreEqual(topLevel.ClosingSituation, selectedSubMechanismContext.ClosingSituation);
                Assert.AreEqual(topLevel.WindDirection.Name, selectedSubMechanismContext.WindDirectionName);
            }
            mocks.VerifyAll();
        }

        private static PointedTreeGraph GetPointedTreeGraph(IllustrationPointsFaultTreeControl control)
        {
            var pointedTreeGraphControl = TypeUtils.GetField<PointedTreeGraphControl>(control, "pointedTreeGraphControl");
            return PointedTreeGraphControlHelper.GetPointedTreeGraph(pointedTreeGraphControl);
        }

        private static GeneralResultFaultTreeIllustrationPointView GetValidView()
        {
            return new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint());
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

        private static GeneralResult<TopLevelFaultTreeIllustrationPoint> GetGeneralResultWithTopLevelIllustrationPointsWithChildren()
        {
            var faultTreeNodeRootWithChildren = new IllustrationPointNode(new TestFaultTreeIllustrationPoint());
            faultTreeNodeRootWithChildren.SetChildren(new[]
            {
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint()),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint())
            });

            return new GeneralResult<TopLevelFaultTreeIllustrationPoint>(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                         Enumerable.Empty<Stochast>(),
                                                                         new[]
                                                                         {
                                                                             new TopLevelFaultTreeIllustrationPoint(
                                                                                 WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                 "Closing situation 2",
                                                                                 faultTreeNodeRootWithChildren)
                                                                         });
        }

        private static GeneralResult<TopLevelFaultTreeIllustrationPoint> GetGeneralResultWithoutTopLevelIllustrationPoints()
        {
            return new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                WindDirectionTestFactory.CreateTestWindDirection(),
                Enumerable.Empty<Stochast>(),
                Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>());
        }
    }
}