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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Util.Reflection;
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
        public void GivenViewWithGeneralResultFuncReturningNotSupportedIllustrationPoint_WhenSettingData_ThenThrowsNotSupportedException()
        {
            // Given
            var data = new TestCalculation
            {
                Output = new object()
            };

            using (var view = new GeneralResultFaultTreeIllustrationPointView(GetGeneralResultWithTopLevelIllustrationPointsOfNotSupportedType))
            {
                // When
                TestDelegate test = () => view.Data = data;

                // Then
                var exception = Assert.Throws<NotSupportedException>(test);
                Assert.AreEqual($"IllustrationPointNode of type {nameof(TestIllustrationPoint)} is not supported. " +
                                $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}", exception.Message);
            }
        }

        [Test]
        public void GivenDisposedViewWithDataSetAndGeneralResultFuncReturningData_WhenDataNotifiesObserver_ThenControlsNoLongerSynced()
        {
            // Given
            var data = new TestCalculation();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(GetGeneralResultWithTopLevelIllustrationPointsWithChildren)
            {
                Data = data
            })
            {
                ShowTestView(view);

                // Precondition
                IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
                Assert.IsNotNull(illustrationPointsControl.Data);

                IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
                Assert.IsNotNull(illustrationPointsFaultTreeControl.Data);

                // When
                view.Dispose();
                data.NotifyObservers();

                // Then
                Assert.IsNotNull(illustrationPointsControl.Data);
                Assert.IsNotNull(illustrationPointsFaultTreeControl.Data);
            }
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
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("A")),
                new IllustrationPointNode(new TestSubMechanismIllustrationPoint("B"))
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

        private static GeneralResult<TopLevelFaultTreeIllustrationPoint> GetGeneralResultWithoutTopLevelIllustrationPoints()
        {
            return new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                WindDirectionTestFactory.CreateTestWindDirection(),
                Enumerable.Empty<Stochast>(),
                Enumerable.Empty<TopLevelFaultTreeIllustrationPoint>());
        }

        #region Selection synchronization

        [Test]
        public void GivenViewWithGeneralResultFuncReturningEmptyData_WhenSettingData_ThenSelectionChangedAndPropagated()
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
                Assert.AreEqual(1, selectionChangedCount);
                Assert.IsNull(view.Selection);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithGeneralResultFuncReturningTestData_WhenSettingData_ThenSelectionChangedAndPropagatedAccordingly()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => generalResult))
            {
                ShowTestView(view);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                // When
                view.Data = data;

                // Then
                Assert.AreEqual(1, selectionChangedCount);
                IEnumerable<TopLevelFaultTreeIllustrationPoint> topLevelFaultTreeIllustrationPoints = generalResult.TopLevelIllustrationPoints.ToArray();
                AssertIllustrationPointSelection(topLevelFaultTreeIllustrationPoints.First(),
                                                 topLevelFaultTreeIllustrationPoints.Select(ip => ip.ClosingSituation),
                                                 view.Selection);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFullyConfiguredView_WhenOutputChangesAndNotifyObserver_ThenSelectionChangedAndPropagated(bool withInitialOutput)
        {
            // Given
            var data = new TestCalculation();
            if (withInitialOutput)
            {
                data.Output = new object();
            }

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => withInitialOutput ? GetGeneralResultWithTwoTopLevelIllustrationPoints() : null)
            {
                Data = data
            })
            {
                ShowTestView(view);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                // When
                data.Output = withInitialOutput ? null : new object();
                data.NotifyObservers();

                // Then
                Assert.AreEqual(1, selectionChangedCount);
                if (withInitialOutput)
                {
                    Assert.IsNotNull(view.Selection);
                }
                else
                {
                    Assert.IsNull(view.Selection);
                }
            }
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

                IEnumerable<TopLevelFaultTreeIllustrationPoint> topLevelFaultTreeIllustrationPoints = generalResult.TopLevelIllustrationPoints.ToArray();
                TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint = topLevelFaultTreeIllustrationPoints.ElementAt(1);
                AssertIllustrationPointSelection(topLevelFaultTreeIllustrationPoint,
                                                 topLevelFaultTreeIllustrationPoints.Select(ip => ip.ClosingSituation),
                                                 view.Selection);

                IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
                Assert.AreSame(topLevelFaultTreeIllustrationPoint, illustrationPointsFaultTreeControl.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFullyConfiguredView_WhenSelectingFaultTreeIllustrationPointInTree_ThenSelectionChangedAndPropagatedAccordingly(bool sameClosingSituations)
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResultFunc = GetGeneralResultWithThreeTopLevelIllustrationPointsWithChildren(sameClosingSituations);
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => generalResultFunc)
            {
                Data = data
            })
            {
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
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFullyConfiguredView_WhenSelectingSubMechanismIllustrationPointInTree_ThenSelectionChangedAndPropagatedAccordingly(bool sameClosingSituations)
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResultFunc = GetGeneralResultWithThreeTopLevelIllustrationPointsWithChildren(sameClosingSituations);
            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => generalResultFunc)
            {
                Data = data
            })
            {
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
            }

            mocks.VerifyAll();
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

        #endregion

        #region Data synchronization

        [Test]
        public void GivenViewWithData_WhenDataSetToNull_ThenControlsDataNull()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(GetGeneralResultWithTwoTopLevelIllustrationPoints)
            {
                Data = data
            })
            {
                // When
                view.Data = null;

                // Then
                IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
                CollectionAssert.IsEmpty(illustrationPointsControl.Data);

                IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
                Assert.IsNull(illustrationPointsFaultTreeControl.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithGeneralResultFuncReturningNull_WhenSettingCalculationWithoutOutput_ThenControlsDataCleared()
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
                IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
                CollectionAssert.IsEmpty(illustrationPointsControl.Data);

                IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
                Assert.IsNull(illustrationPointsFaultTreeControl.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithGeneralResultFuncReturningNull_WhenSettingCalculationWithoutGeneralResult_ThenIllustrationPointsControlDataCleared()
        {
            // Given
            var data = new TestCalculation
            {
                Output = new object()
            };

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => null))
            {
                // When
                view.Data = data;

                // Then
                IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
                CollectionAssert.IsEmpty(illustrationPointsControl.Data);

                IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
                Assert.IsNull(illustrationPointsFaultTreeControl.Data);
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

                IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
                CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithDataSetAndGeneralResultFuncReturningEmptyData_WhenDataGetsOutputWithoutIllustrationPointsAndNotifiesObserver_ThenControlsSyncedAccordingly()
        {
            // Given
            var returnGeneralResult = false;
            var data = new TestCalculation
            {
                Output = new object()
            };

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => returnGeneralResult
                                                                                        ? GetGeneralResultWithoutTopLevelIllustrationPoints()
                                                                                        : null)
            {
                Data = data
            })
            {
                returnGeneralResult = true;

                // Precondition
                IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
                CollectionAssert.IsEmpty(illustrationPointsControl.Data);

                IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
                Assert.IsNull(illustrationPointsFaultTreeControl.Data);

                // When
                data.NotifyObservers();

                // Then
                CollectionAssert.IsEmpty(illustrationPointsControl.Data);
                Assert.IsNull(illustrationPointsFaultTreeControl.Data);
            }
        }

        [Test]
        public void GivenFullyConfiguredViewWithIllustrationPoints_WhenOutputCleared_ThenControlsSyncedAccordingly()
        {
            // Given
            var returnGeneralResult = true;

            var data = new TestCalculation();

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            using (var view = new GeneralResultFaultTreeIllustrationPointView(() => returnGeneralResult
                                                                                        ? generalResult
                                                                                        : null))
            {
                view.Data = data;

                ShowTestView(view);

                // Precondition
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

                IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
                CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());

                IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl = GetIllustrationPointsFaultTreeControl(view);
                Assert.AreSame(topLevelFaultTreeIllustrationPoint1, illustrationPointsFaultTreeControl.Data);

                // When
                returnGeneralResult = false;
                data.NotifyObservers();

                // Then
                CollectionAssert.IsEmpty(illustrationPointsControl.Data);
                Assert.IsNull(illustrationPointsFaultTreeControl.Data);
            }
        }

        #endregion
    }
}