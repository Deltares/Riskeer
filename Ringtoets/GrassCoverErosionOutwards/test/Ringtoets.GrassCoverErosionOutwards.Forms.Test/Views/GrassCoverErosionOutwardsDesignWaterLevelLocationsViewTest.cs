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
using Core.Common.Controls.DataGrid;
using Core.Common.Utils.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
        private const int locationDesignWaterlevelColumnIndex = 5;

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
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationsView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingRowInLocationsTable_ThenReturnSelectedLocation()
        {
            // Given
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);

            DataGridView dataGridView = GetDataGridView();
            DataGridViewRow currentRow = dataGridView.Rows[1];

            HydraulicBoundaryLocation location = ((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).CalculatableObject;

            // When
            dataGridView.CurrentCell = currentRow.Cells[0];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));
            var selection = view.Selection as GrassCoverErosionOutwardsDesignWaterLevelLocationContext;

            // Then
            Assert.IsNotNull(selection);
            Assert.AreSame(location, selection.HydraulicBoundaryLocation);
        }

        [Test]
        public void Selection_WithoutLocations_ReturnsNull()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(assessmentSection))
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void Selection_LocationWithoutOutput_IllustrationPointsControlDataSetToNull()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridView = GetDataGridViewControl();

            // Call
            dataGridView.SetCurrentCell(dataGridView.GetCell(0, 1));

            // Assert
            Assert.IsNull(illustrationPointsControl.Data);
        }

        [Test]
        public void Selection_LocationWithoutGeneralResult_IllustrationPointsControlDataSetToNull()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridView = GetDataGridViewControl();

            // Call
            dataGridView.SetCurrentCell(dataGridView.GetCell(1, 0));

            // Assert
            Assert.IsNull(illustrationPointsControl.Data);
        }

        [Test]
        public void Selection_LocationWithGeneralResult_GeneralResultSetOnIllustrationPointsControlData()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridView = GetDataGridViewControl();

            // Call
            dataGridView.SetCurrentCell(dataGridView.GetCell(4, 0));

            // Assert
            Assert.IsNotNull(illustrationPointsControl.Data);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            ShowDesignWaterLevelLocationsView(assessmentSection);

            // Assert
            DataGridView dataGridView = GetDataGridView();
            Assert.AreEqual(6, dataGridView.ColumnCount);

            var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[locationCalculateColumnIndex];
            Assert.AreEqual("Berekenen", locationCalculateColumn.HeaderText);

            var includeIllustrationPointsColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[includeIllustrationPointsColumnIndex];
            Assert.AreEqual("Illustratiepunten inlezen", includeIllustrationPointsColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationColumnIndex];
            Assert.AreEqual("Coördinaten [m]", locationColumn.HeaderText);

            var locationDesignWaterlevelColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationDesignWaterlevelColumnIndex];
            Assert.AreEqual("Waterstand bij doorsnede-eis [m+NAP]", locationDesignWaterlevelColumn.HeaderText);

            var button = (Button) testForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void DesignWaterLevelLocationsView_WithNonIObservableList_ThrowsInvalidCastException()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowDesignWaterLevelLocationsView(assessmentSection);

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
        public void DesignWaterLevelLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);

            // Assert
            DataGridViewControl dataGridView = GetDataGridViewControl();
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(5, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[4].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(5, 5).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryDatabaseUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

            // Precondition
            DataGridView dataGridView = GetDataGridView();
            object dataGridViewSource = dataGridView.DataSource;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;
            Assert.AreEqual(5, rows.Count);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10.0, 10.0)
            {
                DesignWaterLevelCalculation =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(10.23)
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
            Assert.AreEqual(hydraulicBoundaryLocation.DesignWaterLevel, cells[locationDesignWaterlevelColumnIndex].Value);
            Assert.AreNotSame(dataGridViewSource, dataGridView.DataSource);
            Assert.IsFalse((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void DesignWaterLevelLocationsView_EachHydraulicBoundaryLocationUpdated_DataGridViewRefreshedWithNewValues()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

            // Precondition
            DataGridView dataGridView = GetDataGridView();
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(5, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), rows[1].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[3].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), rows[4].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            locations.ForEach(loc => loc.DesignWaterLevelCalculation.Output = null);

            var refreshed = false;
            dataGridView.Invalidated += (sender, args) => refreshed = true;

            // Call
            locations.NotifyObservers();

            // Assert
            Assert.IsTrue(refreshed);
            Assert.AreEqual(5, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[1].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[3].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[4].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateDesignWaterLevelsSelectionNotChanged(bool isSuccessful)
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(ass => ass.Id).Return(string.Empty);
            assessmentSection.Stub(ass => ass.FailureMechanismContribution)
                             .Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1));
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var observer = mockRepository.StrictMock<IObserver>();

            if (isSuccessful)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            ICalculationMessageProvider messageProvider = null;
            HydraulicBoundaryLocation[] calculatedLocations = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    calculatedLocations = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[1]).ToArray();
                    messageProvider = (ICalculationMessageProvider) invocation.Arguments[3];
                }).Return(isSuccessful);

            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;
            DataGridView dataGridView = GetDataGridView();
            object dataGridViewSource = dataGridView.DataSource;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            locations.Attach(observer);

            view.FailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            view.CalculationGuiService = guiService;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider>(messageProvider);
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
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);

            DataGridViewControl dataGridView = GetDataGridViewControl();
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
        public void GivenDesignWaterLevelLocationsView_WhenSpecificCombinationOfRowSelectionAndFailureMechanismContributionSet_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                                                          bool contributionNotZero,
                                                                                                                                                                          string expectedErrorMessage)
        {
            // Given
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);

            // When
            if (rowSelected)
            {
                DataGridViewControl dataGridView = GetDataGridViewControl();
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

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryDatabaseNotifyObservers_UpdateIllustrationPointsControlData()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridView = GetDataGridViewControl();

            dataGridView.SetCurrentCell(dataGridView.GetCell(3, 0));

            // Precondition
            Assert.IsNull(illustrationPointsControl.Data);

            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            var output = new TestHydraulicBoundaryLocationOutput(generalResult);

            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

            // Call
            locations[3].DesignWaterLevelCalculation.Output = output;
            locations.NotifyObservers();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }

        private static IEnumerable<IllustrationPointControlItem> CreateControlItems(
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult)
        {
            return generalResult.TopLevelIllustrationPoints
                                .Select(topLevelIllustrationPoint =>
                                {
                                    SubMechanismIllustrationPoint illustrationPoint = topLevelIllustrationPoint.SubMechanismIllustrationPoint;
                                    return new IllustrationPointControlItem(topLevelIllustrationPoint,
                                                                            topLevelIllustrationPoint.WindDirection.Name,
                                                                            topLevelIllustrationPoint.ClosingSituation,
                                                                            illustrationPoint.Stochasts,
                                                                            illustrationPoint.Beta);
                                });
        }

        private DataGridViewControl GetDataGridViewControl()
        {
            return GetControls<DataGridViewControl>("DataGridViewControl").Single();
        }

        private DataGridView GetDataGridView()
        {
            return GetControls<DataGridView>("DataGridView").First();
        }

        private IllustrationPointsControl GetIllustrationPointsControl()
        {
            return GetControls<IllustrationPointsControl>("IllustrationPointsControl").Single();
        }

        /// <summary>
        /// Gets the controls by name.
        /// </summary>
        /// <param name="controlName">The name of the controls to find.</param>
        /// <returns>The found control.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="controlName"/> is <c>null</c> or empty.</exception>
        private IEnumerable<TView> GetControls<TView>(string controlName) where TView : Control
        {
            return testForm.Controls.Find(controlName, true).OfType<TView>();
        }

        private GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowDesignWaterLevelLocationsView(IAssessmentSection assessmentSection)
        {
            var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowFullyConfiguredDesignWaterLevelLocationsView(IAssessmentSection assessmentSection)
        {
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowDesignWaterLevelLocationsView(assessmentSection);

            var output = new TestHydraulicBoundaryLocationOutput(1.01, new TestGeneralResultSubMechanismIllustrationPoint());
            view.Data = new ObservableList<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "1", 1.0, 1.0),
                new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(1.23)
                    }
                },
                new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    WaveHeightCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(2.45)
                    }
                },
                new HydraulicBoundaryLocation(4, "4", 4.0, 4.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        }
                    }
                },
                new HydraulicBoundaryLocation(5, "5", 5.0, 5.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = output
                    }
                }
            };
            return view;
        }
    }
}