// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingScenariosViewTest
    {
        private const int isRelevantColumnIndex = 0;
        private const int contributionColumnIndex = 1;
        private const int nameColumnIndex = 2;
        private const int failureProbabilityUpliftColumnIndex = 3;
        private const int failureProbabilityHeaveColumnIndex = 4;
        private const int failureProbabilitySellmeijerColumnIndex = 5;
        private const int failureProbabilityPipingColumnIndex = 6;
        private const int sectionFailureProbabilityPipingColumnIndex = 7;
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
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new PipingScenariosView(null, new PipingFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new PipingScenariosView(new CalculationGroup(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingScenariosView(new CalculationGroup(), new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            // Call
            using (var pipingScenarioView = new PipingScenariosView(calculationGroup, new PipingFailureMechanism(), assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(pipingScenarioView);
                Assert.IsInstanceOf<IView>(pipingScenarioView);
                Assert.AreSame(calculationGroup, pipingScenarioView.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new[]
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection1,
                failureMechanismSection2,
                failureMechanismSection3
            });

            // Call
            ShowPipingScenariosView(failureMechanism);

            // Assert
            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            Assert.AreEqual(nameof(FailureMechanismSection.Name), listBox.DisplayMember);
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            Assert.AreSame(failureMechanismSection1, listBox.SelectedItem);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowPipingScenariosView(new PipingFailureMechanism());

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(8, dataGridView.ColumnCount);
            Assert.AreEqual("In oordeel", dataGridView.Columns[isRelevantColumnIndex].HeaderText);
            Assert.AreEqual("Bijdrage aan\r\nscenario\r\n[%]", dataGridView.Columns[contributionColumnIndex].HeaderText);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nopbarsten\r\n[1/jaar]", dataGridView.Columns[failureProbabilityUpliftColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nheave\r\n[1/jaar]", dataGridView.Columns[failureProbabilityHeaveColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nterugschrijdende erosie\r\n[1/jaar]", dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans per doorsnede\r\n[1/jaar]", dataGridView.Columns[failureProbabilityPipingColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans per vak\r\n[1/jaar]", dataGridView.Columns[sectionFailureProbabilityPipingColumnIndex].HeaderText);
        }

        [Test]
        public void PipingScenarioView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredPipingScenariosView(new PipingFailureMechanism());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(8, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityUpliftColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityHeaveColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilitySellmeijerColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[sectionFailureProbabilityPipingColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(8, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(2.425418e-4), cells[failureProbabilityUpliftColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.038461838), cells[failureProbabilityHeaveColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.027777778), cells[failureProbabilitySellmeijerColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(2.425418e-4), cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(2.44140625e-4), cells[sectionFailureProbabilityPipingColumnIndex].FormattedValue);
        }

        [Test]
        public void PipingScenariosView_ContributionValueInvalid_ShowsErrorTooltip()
        {
            // Setup
            ShowFullyConfiguredPipingScenariosView(new PipingFailureMechanism());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = "test";

            // Assert
            Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);
        }

        [Test]
        [TestCase(1)]
        [TestCase(1e-6)]
        [TestCase(100)]
        [TestCase(14.3)]
        public void PipingScenariosView_EditValueValid_DoNotShowErrorToolTipAndEditValue(double newValue)
        {
            // Setup
            ShowFullyConfiguredPipingScenariosView(new PipingFailureMechanism());

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[contributionColumnIndex].Value = (RoundedDouble) newValue;

            // Assert
            Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
        }

        [Test]
        [TestCase(isRelevantColumnIndex, true)]
        [TestCase(contributionColumnIndex, 30.0)]
        public void PipingScenariosView_EditingPropertyViaDataGridView_ObserversCorrectlyNotified(int cellIndex, object newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            ICalculation calculation = failureMechanism.CalculationsGroup.GetCalculations().First();
            calculation.Attach(calculationObserver);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.Rows[0].Cells[cellIndex].Value = newValue is double value ? (RoundedDouble) value : newValue;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingScenariosView_WhenSelectingDifferentItemInSectionsListBox_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Precondition
            Assert.AreSame(failureMechanism.Sections.First(), listBox.SelectedItem);

            PipingScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                .Select(r => r.DataBoundItem)
                                                                .Cast<PipingScenarioRow>()
                                                                .ToArray();

            // When
            listBox.SelectedItem = failureMechanism.Sections.Last();

            // Then
            PipingScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                          .Select(r => r.DataBoundItem)
                                                          .Cast<PipingScenarioRow>()
                                                          .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenPipingScenariosView_WhenFailureMechanismNotifiesObserver_ThenSectionsListBoxCorrectlyUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowPipingScenariosView(failureMechanism);

            var listBox = (ListBox) new ControlTester("listBox").TheObject;

            // Precondition
            CollectionAssert.IsEmpty(listBox.Items);

            // When
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new[]
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection1,
                failureMechanismSection2,
                failureMechanismSection3
            });
            failureMechanism.NotifyObservers();

            // Then
            Assert.AreEqual(3, listBox.Items.Count);
            Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
            Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
            Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            Assert.AreSame(failureMechanismSection1, listBox.SelectedItem);
        }

        [Test]
        public void GivenPipingScenariosView_WhenFailureMechanismNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            PipingScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                .Select(r => r.DataBoundItem)
                                                                .Cast<PipingScenarioRow>()
                                                                .ToArray();

            // When
            failureMechanism.NotifyObservers();

            // Then
            PipingScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                          .Select(r => r.DataBoundItem)
                                                          .Cast<PipingScenarioRow>()
                                                          .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenPipingScenariosView_WhenCalculationGroupNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            PipingScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                .Select(r => r.DataBoundItem)
                                                                .Cast<PipingScenarioRow>()
                                                                .ToArray();

            // When
            failureMechanism.CalculationsGroup.NotifyObservers();

            // Then
            PipingScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                          .Select(r => r.DataBoundItem)
                                                          .Cast<PipingScenarioRow>()
                                                          .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        [Test]
        public void GivenPipingScenariosView_WhenCalculationNotifiesObserver_ThenViewUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            var refreshed = 0;
            dataGridView.Invalidated += (sender, args) => refreshed++;

            ICalculation calculation = failureMechanism.CalculationsGroup.GetCalculations().First();

            // When
            calculation.NotifyObservers();

            // Then
            Assert.AreEqual(1, refreshed);
        }

        [Test]
        public void GivenPipingScenariosView_WhenCalculationInputNotifiesObserver_ThenDataGridViewUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            ShowFullyConfiguredPipingScenariosView(failureMechanism);

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            PipingScenarioRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                .Select(r => r.DataBoundItem)
                                                                .Cast<PipingScenarioRow>()
                                                                .ToArray();

            SemiProbabilisticPipingCalculationScenario calculation = failureMechanism.CalculationsGroup.GetCalculations()
                                                                                     .Cast<SemiProbabilisticPipingCalculationScenario>()
                                                                                     .First();

            // When
            calculation.InputParameters.NotifyObservers();

            // Then
            PipingScenarioRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                          .Select(r => r.DataBoundItem)
                                                          .Cast<PipingScenarioRow>()
                                                          .ToArray();

            CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
        }

        private void ShowFullyConfiguredPipingScenariosView(PipingFailureMechanism failureMechanism)
        {
            var surfaceLine1 = new PipingSurfaceLine("Surface line 1")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new PipingSurfaceLine("Surface line 2")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("Section 1", new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(5.0, 0.0)
                }),
                new FailureMechanismSection("Section 2", new[]
                {
                    new Point2D(5.0, 0.0),
                    new Point2D(10.0, 0.0)
                })
            });

            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                new SemiProbabilisticPipingCalculationScenario
                {
                    Name = "Calculation 1",
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine1,
                        DampingFactorExit =
                        {
                            Mean = (RoundedDouble) 1.1111
                        },
                        PhreaticLevelExit =
                        {
                            Mean = (RoundedDouble) 2.2222
                        },
                        EntryPointL = (RoundedDouble) 3.3333,
                        ExitPointL = (RoundedDouble) 4.4444
                    }
                },
                new SemiProbabilisticPipingCalculationScenario
                {
                    Name = "Calculation 2",
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine2,
                        DampingFactorExit =
                        {
                            Mean = (RoundedDouble) 5.5555
                        },
                        PhreaticLevelExit =
                        {
                            Mean = (RoundedDouble) 6.6666
                        },
                        EntryPointL = (RoundedDouble) 7.7777,
                        ExitPointL = (RoundedDouble) 8.8888
                    },
                    Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(0.26065, 0.81398, 0.38024)
                }
            });

            ShowPipingScenariosView(failureMechanism);
        }

        private void ShowPipingScenariosView(PipingFailureMechanism failureMechanism)
        {
            var pipingScenarioView = new PipingScenariosView(failureMechanism.CalculationsGroup, failureMechanism, new AssessmentSectionStub());

            testForm.Controls.Add(pipingScenarioView);
            testForm.Show();
        }
    }
}