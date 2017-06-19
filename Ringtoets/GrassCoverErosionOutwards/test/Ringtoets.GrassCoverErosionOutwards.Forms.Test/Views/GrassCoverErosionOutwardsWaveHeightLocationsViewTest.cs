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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
        private const int locationWaveHeightColumnIndex = 5;

        private Form testForm;
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
            mockRepository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
            mockRepository.VerifyAll();
        }

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
            {
                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationsView<WaveHeightLocationRow>>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Selection_WithoutLocations_ReturnsNull()
        {
            // Call
            using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView())
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void Selection_WithLocations_ReturnsSelectedLocationWrappedInContext()
        {
            // Call
            using (GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView())
            {
                var dataGridView = (DataGridView) testForm.Controls.Find("dataGridView", true).First();
                DataGridViewRow selectedLocationRow = dataGridView.Rows[0];
                selectedLocationRow.Cells[0].Value = true;

                // Assert
                var selection = view.Selection as GrassCoverErosionOutwardsWaveHeightLocationContext;
                var dataBoundItem = selectedLocationRow.DataBoundItem as WaveHeightLocationRow;

                Assert.NotNull(selection);
                Assert.NotNull(dataBoundItem);
                Assert.AreSame(dataBoundItem.CalculatableObject, selection.HydraulicBoundaryLocation);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowWaveHeightLocationsView();

            // Assert
            var dataGridView = (DataGridView) testForm.Controls.Find("dataGridView", true).First();
            Assert.AreEqual(6, dataGridView.ColumnCount);

            var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[locationCalculateColumnIndex];
            Assert.AreEqual("Berekenen", locationCalculateColumn.HeaderText);

            var includeIllustrationPointsColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[includeIllustrationPointsColumnIndex];
            Assert.AreEqual("Illustratiepunten", includeIllustrationPointsColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationColumnIndex];
            Assert.AreEqual("Coördinaten [m]", locationColumn.HeaderText);

            var locationWaveHeightColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationWaveHeightColumnIndex];
            Assert.AreEqual("Golfhoogte bij doorsnede-eis [m]", locationWaveHeightColumn.HeaderText);

            var button = (Button) testForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void GivenWaveHeightLocationsView_WhenSettingAssessmentSectionAndDisposingView_ThenCorrectlyAttachAndDetachAssessmentSection()
        {
            // Setup
            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();
            var assessmentSection = mockRepository.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.Attach(Arg<IObserver>.Is.NotNull)).IgnoreArguments();
            assessmentSection.Expect(a => a.Detach(Arg<IObserver>.Is.NotNull)).IgnoreArguments();
            mockRepository.ReplayAll();

            // Call
            view.AssessmentSection = assessmentSection;

            // Assert
            // Assertions based on mock expectancies
        }

        [Test]
        public void WaveHeightLocationsView_WithNonIObservableList_ThrowsInvalidCastException()
        {
            // Setup
            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowWaveHeightLocationsView();

            var locations = new List<HydraulicBoundaryLocation>
            {
                new TestHydraulicBoundaryLocation()
            };

            // Call
            TestDelegate action = () => view.Data = locations;

            // Assert
            Assert.Throws<InvalidCastException>(action);
        }

        [Test]
        public void WaveHeightLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredWaveHeightLocationsView();

            // Assert
            var dataGridView = (DataGridView) testForm.Controls.Find("dataGridView", true).First();
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(4, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationWaveHeightColumnIndex].FormattedValue);
        }

        [Test]
        public void WaveHeightLocationsView_HydraulicBoundaryLocationsUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

            // Precondition
            var dataGridView = (DataGridView) testForm.Controls.Find("dataGridView", true).First();
            object dataGridViewSource = dataGridView.DataSource;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;
            Assert.AreEqual(4, rows.Count);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10, 10)
            {
                WaveHeightCalculation =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(10)
                }
            };

            locations.Clear();
            locations.Add(hydraulicBoundaryLocation);

            // Call
            locations.NotifyObservers();

            // Assert
            Assert.AreEqual(1, rows.Count);
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(hydraulicBoundaryLocation.WaveHeight, cells[locationWaveHeightColumnIndex].Value);
            Assert.AreNotSame(dataGridViewSource, dataGridView.DataSource);
            Assert.IsFalse((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void WaveHeightLocationsView_EachHydraulicBoundaryLocationUpdated_DataGridViewRefreshedWithNewValues()
        {
            // Setup
            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

            // Precondition
            var dataGridView = (DataGridView) testForm.Controls.Find("dataGridView", true).First();
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(4, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), rows[1].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[3].Cells[locationWaveHeightColumnIndex].FormattedValue);

            // Call
            locations.ForEach(loc => loc.WaveHeightCalculation.Output = null);
            locations.NotifyObservers();

            // Assert
            Assert.AreEqual(4, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[1].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[3].Cells[locationWaveHeightColumnIndex].FormattedValue);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateWaveHeightsSelectionNotChanged(bool isSuccessful)
        {
            // Setup
            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;
            var dataGridView = (DataGridView) testForm.Controls.Find("dataGridView", true).First();
            object dataGridViewSource = dataGridView.DataSource;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var guiServiceMock = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var observer = mockRepository.StrictMock<IObserver>();
            locations.Attach(observer);

            if (isSuccessful)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            ICalculationMessageProvider messageProvider = null;
            HydraulicBoundaryLocation[] calculatedLocations = null;
            guiServiceMock.Expect(ch => ch.CalculateWaveHeights(null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    calculatedLocations = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[1]).ToArray();
                    messageProvider = (ICalculationMessageProvider) invocation.Arguments[3];
                }).Return(isSuccessful);

            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSectionStub.Stub(ass => ass.Id).Return(string.Empty);
            assessmentSectionStub.Stub(ass => ass.FailureMechanismContribution)
                                 .Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));
            assessmentSectionStub.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSectionStub.Stub(a => a.Detach(null)).IgnoreArguments();
            mockRepository.ReplayAll();

            view.AssessmentSection = assessmentSectionStub;
            view.CalculationGuiService = guiServiceMock;
            view.FailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider>(messageProvider);
            Assert.AreEqual(1, calculatedLocations.Length);
            HydraulicBoundaryLocation expectedLocation = locations.First();
            Assert.AreEqual(expectedLocation, calculatedLocations.First());
            Assert.AreSame(dataGridViewSource, dataGridView.DataSource);
            Assert.IsTrue((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[2].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredWaveHeightLocationsView();

            var dataGridView = (DataGridView) testForm.Controls.Find("dataGridView", true).First();
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase(false, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(false, false, message)")]
        [TestCase(true, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(true, false, message)")]
        [TestCase(false, true, "Er zijn geen berekeningen geselecteerd.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(false, true, message)")]
        [TestCase(true, true, "", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(true, true, message)")]
        public void GivenWaveHeightLocationsView_WhenSpecificCombinationOfRowSelectionAndFailureMechanismContributionSet_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                                                    bool contributionNotZero,
                                                                                                                                                                    string expectedErrorMessage)
        {
            // Given
            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();

            // When
            if (rowSelected)
            {
                var dataGridView = (DataGridView) testForm.Controls.Find("dataGridView", true).First();
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
            }

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            if (contributionNotZero)
            {
                failureMechanism.Contribution = 5;
            }
            view.FailureMechanism = failureMechanism;

            // Then
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.AreEqual(rowSelected && contributionNotZero, button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
        }

        private GrassCoverErosionOutwardsWaveHeightLocationsView ShowWaveHeightLocationsView()
        {
            var view = new GrassCoverErosionOutwardsWaveHeightLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private GrassCoverErosionOutwardsWaveHeightLocationsView ShowFullyConfiguredWaveHeightLocationsView()
        {
            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowWaveHeightLocationsView();

            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "1", 1.0, 1.0),
                new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    WaveHeightCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(1.23)
                    }
                },
                new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(2.45)
                    }
                },
                new HydraulicBoundaryLocation(4, "4", 4.0, 4.0)
                {
                    WaveHeightCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        }
                    }
                }
            };

            view.Data = locations;
            return view;
        }
    }
}