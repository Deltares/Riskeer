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
using Core.Common.Controls.Views;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class GeneralResultIllustrationPointViewTest
    {
        private Form testForm;

        private static readonly Stochast[] stochasts =
        {
            new Stochast("stochast 1", 0.1, 0.2)
        };

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
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestGeneralResultIllustrationPointView(null, () => new TestGeneralResultTopLevelIllustrationPoint());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Constructor_GetGeneralResultFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestGeneralResultIllustrationPointView(calculation, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getGeneralResultFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            var view = new TestGeneralResultIllustrationPointView(calculation, GetGeneralResultWithoutTopLevelIllustrationPoints);

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
            Assert.IsInstanceOf<ISelectionProvider>(view);
            Assert.AreSame(calculation, view.Data);
            Assert.AreEqual("GeneralResultIllustrationPointView", view.Name);

            Assert.AreEqual(1, view.Controls.Count);

            var splitContainer = view.Controls[0] as SplitContainer;
            Assert.IsNotNull(splitContainer);
            Control.ControlCollection splitContainerPanel1Controls = splitContainer.Panel1.Controls;
            Assert.AreEqual(1, splitContainerPanel1Controls.Count);
            Assert.IsInstanceOf<IllustrationPointsControl>(splitContainerPanel1Controls[0]);

            Control.ControlCollection splitContainerPanel2Controls = splitContainer.Panel2.Controls;
            Assert.AreEqual(1, splitContainerPanel2Controls.Count);
            Assert.IsInstanceOf<IllustrationPointsFaultTreeControl>(splitContainerPanel2Controls[0]);

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
            var view = new TestGeneralResultIllustrationPointView(calculation, () => null);

            // Assert
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GeneralResultWithIllustrationPoints_DataSetOnIllustrationPointControl()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            GeneralResult<TestTopLevelIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            // Call
            var view = new TestGeneralResultIllustrationPointView(calculation, () => generalResult);

            // Assert
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
            AssertIllustrationPointControlItems(generalResult, illustrationPointsControl);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GeneralResultFuncReturningEmptyData_SelectionNull()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            var view = new TestGeneralResultIllustrationPointView(calculation, GetGeneralResultWithoutTopLevelIllustrationPoints);
            ShowTestView(view);

            // Assert
            Assert.IsNull(view.Selection);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GeneralResultFuncReturningData_SelectionSetToFirstTopLevelIllustrationPoint()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            GeneralResult<TestTopLevelIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();
            var view = new TestGeneralResultIllustrationPointView(calculation, () => generalResult);

            ShowTestView(view);

            // Assert
            IEnumerable<TestTopLevelIllustrationPoint> topLevelFaultTreeIllustrationPoints = generalResult.TopLevelIllustrationPoints.ToArray();
            Assert.AreSame(topLevelFaultTreeIllustrationPoints.First(), view.Selection);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFullyConfiguredView_WhenOutputChangesAndNotifyObserver_ThenSelectionChangedAndPropagated(bool withInitialOutput)
        {
            // Given
            var calculation = new TestCalculation();
            if (withInitialOutput)
            {
                calculation.Output = new object();
            }

            var view = new TestGeneralResultIllustrationPointView(
                calculation, () => withInitialOutput
                                       ? GetGeneralResultWithTwoTopLevelIllustrationPoints()
                                       : null);

            ShowTestView(view);

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            // When
            calculation.Output = withInitialOutput ? null : new object();
            calculation.NotifyObservers();

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

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCellInRow_ThenSelectionChangedAndPropagatedAccordingly()
        {
            // Given
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            GeneralResult<TestTopLevelIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();
            var view = new TestGeneralResultIllustrationPointView(calculation, () => generalResult);
            ShowTestView(view);

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[0];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(1, selectionChangedCount);

            TestTopLevelIllustrationPoint[] topLevelIllustrationPoints = generalResult.TopLevelIllustrationPoints.ToArray();
            TestTopLevelIllustrationPoint topLevelIllustrationPoint = topLevelIllustrationPoints.ElementAt(1);
            Assert.AreSame(topLevelIllustrationPoint, view.Selection);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenViewWithoutIllustrationPoints_WhenIllustrationPointsSetAndNotifiesObserver_ThenControlsSyncedAccordingly()
        {
            // Given
            var returnGeneralResult = false;

            var calculation = new TestCalculation();
            GeneralResult<TestTopLevelIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            using (var view = new TestGeneralResultIllustrationPointView(calculation, () => returnGeneralResult
                                                                                                ? generalResult
                                                                                                : null))
            {
                returnGeneralResult = true;

                // Precondition
                IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
                CollectionAssert.IsEmpty(illustrationPointsControl.Data);

                // When
                calculation.NotifyObservers();

                // Then
                AssertIllustrationPointControlItems(generalResult, illustrationPointsControl);
            }
        }

        [Test]
        public void GivenFullyConfiguredViewWithIllustrationPoints_WhenIllustrationPointsCleared_ThenControlsSyncedAccordingly()
        {
            // Given
            var returnGeneralResult = true;

            var calculation = new TestCalculation();
            GeneralResult<TestTopLevelIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            var view = new TestGeneralResultIllustrationPointView(calculation, () => returnGeneralResult
                                                                                         ? generalResult
                                                                                         : null);

            ShowTestView(view);

            // Precondition
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl(view);
            AssertIllustrationPointControlItems(generalResult, illustrationPointsControl);

            // When
            returnGeneralResult = false;
            calculation.NotifyObservers();

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        private static void AssertIllustrationPointControlItems(GeneralResult<TestTopLevelIllustrationPoint> generalResult, IllustrationPointsControl illustrationPointsControl)
        {
            TestTopLevelIllustrationPoint topLevelFaultTreeIllustrationPoint1 = generalResult.TopLevelIllustrationPoints.ElementAt(0);
            TestTopLevelIllustrationPoint topLevelFaultTreeIllustrationPoint2 = generalResult.TopLevelIllustrationPoints.ElementAt(1);

            var illustrationPoint = new TestIllustrationPoint();

            var expectedControlItems = new[]
            {
                new IllustrationPointControlItem(topLevelFaultTreeIllustrationPoint1,
                                                 topLevelFaultTreeIllustrationPoint1.WindDirection.Name,
                                                 topLevelFaultTreeIllustrationPoint1.ClosingSituation,
                                                 stochasts,
                                                 illustrationPoint.Beta),
                new IllustrationPointControlItem(topLevelFaultTreeIllustrationPoint2,
                                                 topLevelFaultTreeIllustrationPoint2.WindDirection.Name,
                                                 topLevelFaultTreeIllustrationPoint2.ClosingSituation,
                                                 stochasts,
                                                 illustrationPoint.Beta)
            };

            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }

        private static IllustrationPointsControl GetIllustrationPointsControl(TestGeneralResultIllustrationPointView view)
        {
            return TypeUtils.GetField<IllustrationPointsControl>(view, "illustrationPointsControl");
        }

        private void ShowTestView(TestGeneralResultIllustrationPointView view)
        {
            testForm.Controls.Add(view);
            testForm.Show();
        }

        private static GeneralResult<TestTopLevelIllustrationPoint> GetGeneralResultWithTwoTopLevelIllustrationPoints()
        {
            return new GeneralResult<TestTopLevelIllustrationPoint>(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                    Enumerable.Empty<Stochast>(),
                                                                    new[]
                                                                    {
                                                                        new TestTopLevelIllustrationPoint("Closing situation 1"),
                                                                        new TestTopLevelIllustrationPoint("Closing situation 2")
                                                                    });
        }

        private static GeneralResult<TestTopLevelIllustrationPoint> GetGeneralResultWithoutTopLevelIllustrationPoints()
        {
            return new GeneralResult<TestTopLevelIllustrationPoint>(
                WindDirectionTestFactory.CreateTestWindDirection(),
                Enumerable.Empty<Stochast>(),
                Enumerable.Empty<TestTopLevelIllustrationPoint>());
        }

        private class TestGeneralResultIllustrationPointView : GeneralResultIllustrationPointView<TestTopLevelIllustrationPoint>
        {
            public TestGeneralResultIllustrationPointView(ICalculation calculation, Func<GeneralResult<TestTopLevelIllustrationPoint>> getGeneralResultFunc)
                : base(calculation, getGeneralResultFunc) {}

            protected override IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems()
            {
                GeneralResult<TestTopLevelIllustrationPoint> generalResult = GetGeneralResultFunc();

                if (generalResult == null)
                {
                    return Enumerable.Empty<IllustrationPointControlItem>();
                }

                var illustrationPoint = new TestIllustrationPoint();

                return generalResult.TopLevelIllustrationPoints.Select(
                    p => new IllustrationPointControlItem(
                        p,
                        p.WindDirection.Name,
                        p.ClosingSituation,
                        stochasts,
                        illustrationPoint.Beta)).ToArray();
            }

            protected override void UpdateSpecificIllustrationPointsControl() {}

            protected override object GetSelectedTopLevelIllustrationPoint(IllustrationPointControlItem selection)
            {
                return selection.Source;
            }
        }
    }
}