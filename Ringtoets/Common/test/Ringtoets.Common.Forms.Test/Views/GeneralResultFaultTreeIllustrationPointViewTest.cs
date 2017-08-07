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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
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
                Assert.IsEmpty(splitContainerPanel2Controls);
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
        public void GivenViewWithGeneralResultFuncReturningNull_WhenSettingData_ThenIllustrationPointsControlSyncedAccordingly()
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

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            var view = new GeneralResultFaultTreeIllustrationPointView(() => generalResult);

            ShowTestView(view);

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            // When
            view.Data = data;

            // Then
            Assert.AreNotEqual(0, selectionChangedCount);
            Assert.AreSame(generalResult.TopLevelIllustrationPoints.ElementAt(0), ((SelectedTopLevelFaultTreeIllustrationPoint) view.Selection).TopLevelFaultTreeIllustrationPoint);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithGeneralResultFuncReturningEmptyData_WhenSettingData_ThenNoSelectionChangedAndPropagated()
        {
            // Given
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            var view = new GeneralResultFaultTreeIllustrationPointView(GetGeneralResultWithoutTopLevelIllustrationPoints);

            ShowTestView(view);

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            // When
            view.Data = data;

            // Then
            Assert.AreEqual(0, selectionChangedCount);
            Assert.IsNull(view.Selection);

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

            var view = new GeneralResultFaultTreeIllustrationPointView(() => generalResult)
            {
                Data = data
            };

            ShowTestView(view);

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[0];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(1, selectionChangedCount);
            Assert.AreSame(generalResult.TopLevelIllustrationPoints.ElementAt(1), ((SelectedTopLevelFaultTreeIllustrationPoint) view.Selection).TopLevelFaultTreeIllustrationPoint);

            mocks.VerifyAll();
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
                                                                         new List<TopLevelFaultTreeIllustrationPoint>
                                                                         {
                                                                             topLevelFaultTreeIllustrationPoint1,
                                                                             topLevelFaultTreeIllustrationPoint2
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