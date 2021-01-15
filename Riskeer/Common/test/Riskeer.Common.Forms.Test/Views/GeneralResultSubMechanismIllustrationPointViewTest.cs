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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class GeneralResultSubMechanismIllustrationPointViewTest
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
            using (var view = new GeneralResultSubMechanismIllustrationPointView(calculation, () => null))
            {
                // Assert
                Assert.IsInstanceOf<GeneralResultIllustrationPointView<TopLevelSubMechanismIllustrationPoint>>(view);

                Assert.AreEqual(1, view.Controls.Count);

                var splitContainer = view.Controls[0] as SplitContainer;
                Assert.IsNotNull(splitContainer);
                Control.ControlCollection splitContainerPanel1Controls = splitContainer.Panel1.Controls;
                Assert.AreEqual(1, splitContainerPanel1Controls.Count);
                Assert.IsInstanceOf<IllustrationPointsControl>(splitContainerPanel1Controls[0]);

                CollectionAssert.IsEmpty(splitContainer.Panel2.Controls);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_GeneralResultWithIllustrationPoints_DataSetOnIllustrationPointControl()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();

            // Call
            var view = new GeneralResultSubMechanismIllustrationPointView(calculation, () => generalResult);
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

            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult = GetGeneralResultWithTwoTopLevelIllustrationPoints();
            var view = new GeneralResultSubMechanismIllustrationPointView(calculation, () => generalResult);
            ShowTestView(view);

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[0];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(1, selectionChangedCount);

            TopLevelSubMechanismIllustrationPoint[] topLevelIllustrationPoints = generalResult.TopLevelIllustrationPoints.ToArray();
            TopLevelSubMechanismIllustrationPoint topLevelIllustrationPoint = topLevelIllustrationPoints.ElementAt(1);
            AssertIllustrationPointSelection(topLevelIllustrationPoint,
                                             topLevelIllustrationPoints.Select(ip => ip.ClosingSituation),
                                             view.Selection);
            mocks.VerifyAll();
        }

        private static IllustrationPointsControl GetIllustrationPointsControl(GeneralResultSubMechanismIllustrationPointView view)
        {
            return TypeUtils.GetField<IllustrationPointsControl>(view, "illustrationPointsControl");
        }

        private void ShowTestView(GeneralResultSubMechanismIllustrationPointView view)
        {
            testForm.Controls.Add(view);
            testForm.Show();
        }

        private static GeneralResult<TopLevelSubMechanismIllustrationPoint> GetGeneralResultWithTwoTopLevelIllustrationPoints()
        {
            var topLevelSubMechanismIllustrationPoint1 =
                new TopLevelSubMechanismIllustrationPoint(
                    WindDirectionTestFactory.CreateTestWindDirection(),
                    "Closing situation 1",
                    new TestSubMechanismIllustrationPoint());

            var topLevelSubMechanismIllustrationPoint2 =
                new TopLevelSubMechanismIllustrationPoint(
                    WindDirectionTestFactory.CreateTestWindDirection(),
                    "Closing situation 2",
                    new TestSubMechanismIllustrationPoint());

            return new GeneralResult<TopLevelSubMechanismIllustrationPoint>(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                            Enumerable.Empty<Stochast>(),
                                                                            new[]
                                                                            {
                                                                                topLevelSubMechanismIllustrationPoint1,
                                                                                topLevelSubMechanismIllustrationPoint2
                                                                            });
        }

        private static void AssertIllustrationPointSelection(TopLevelSubMechanismIllustrationPoint expectedSelection,
                                                             IEnumerable<string> expectedClosingSituations,
                                                             object selection)
        {
            var illustrationPointSelection = selection as SelectedTopLevelSubMechanismIllustrationPoint;
            Assert.IsNotNull(illustrationPointSelection);
            Assert.AreSame(expectedSelection, illustrationPointSelection.TopLevelSubMechanismIllustrationPoint);
            CollectionAssert.AreEqual(expectedClosingSituations, illustrationPointSelection.ClosingSituations);
        }

        private static void AssertIllustrationPointControlItems(GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult, IllustrationPointsControl illustrationPointsControl)
        {
            TopLevelSubMechanismIllustrationPoint topLevelSubMechanismIllustrationPoint1 = generalResult.TopLevelIllustrationPoints.ElementAt(0);
            TopLevelSubMechanismIllustrationPoint topLevelSubMechanismIllustrationPoint2 = generalResult.TopLevelIllustrationPoints.ElementAt(1);
            var expectedControlItems = new[]
            {
                new IllustrationPointControlItem(topLevelSubMechanismIllustrationPoint1,
                                                 topLevelSubMechanismIllustrationPoint1.WindDirection.Name,
                                                 topLevelSubMechanismIllustrationPoint1.ClosingSituation,
                                                 topLevelSubMechanismIllustrationPoint1.SubMechanismIllustrationPoint.Stochasts,
                                                 topLevelSubMechanismIllustrationPoint1.SubMechanismIllustrationPoint.Beta),
                new IllustrationPointControlItem(topLevelSubMechanismIllustrationPoint2,
                                                 topLevelSubMechanismIllustrationPoint2.WindDirection.Name,
                                                 topLevelSubMechanismIllustrationPoint2.ClosingSituation,
                                                 topLevelSubMechanismIllustrationPoint2.SubMechanismIllustrationPoint.Stochasts,
                                                 topLevelSubMechanismIllustrationPoint2.SubMechanismIllustrationPoint.Beta)
            };

            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }
    }
}