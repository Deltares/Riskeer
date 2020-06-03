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
using System.Globalization;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
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
        private const int failureProbabilityPipingColumnIndex = 3;
        private const int failureProbabilityUpliftColumnIndex = 4;
        private const int failureProbabilityHeaveColumnIndex = 5;
        private const int failureProbabilitySellmeijerColumnIndex = 6;
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
                Assert.IsInstanceOf<ScenariosView<PipingCalculationScenario, PipingInput, PipingScenarioRow, PipingFailureMechanism>>(pipingScenarioView);
                Assert.AreSame(calculationGroup, pipingScenarioView.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowPipingScenarioView(new CalculationGroup(), new PipingFailureMechanism());
            
            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(7, dataGridView.ColumnCount);
            Assert.AreEqual("In oordeel", dataGridView.Columns[isRelevantColumnIndex].HeaderText);
            Assert.AreEqual("Bijdrage aan\r\nscenario\r\n[%]", dataGridView.Columns[contributionColumnIndex].HeaderText);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans\r\n[1/jaar]", dataGridView.Columns[failureProbabilityPipingColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nopbarsten\r\n[1/jaar]", dataGridView.Columns[failureProbabilityUpliftColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nheave\r\n[1/jaar]", dataGridView.Columns[failureProbabilityHeaveColumnIndex].HeaderText);
            Assert.AreEqual("Kans op\r\nterugschrijdende erosie\r\n[1/jaar]", dataGridView.Columns[failureProbabilitySellmeijerColumnIndex].HeaderText);
        }

        [Test]
        public void PipingScenarioView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredPipingScenarioView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(7, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityUpliftColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilityHeaveColumnIndex].FormattedValue);
            Assert.AreEqual("-".ToString(CultureInfo.CurrentCulture), cells[failureProbabilitySellmeijerColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(7, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(2.425418e-4), cells[failureProbabilityPipingColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(2.425418e-4), cells[failureProbabilityUpliftColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.038461838), cells[failureProbabilityHeaveColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.027777778), cells[failureProbabilitySellmeijerColumnIndex].FormattedValue);
        }

        private void ShowFullyConfiguredPipingScenarioView()
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

            var failureMechanism = new PipingFailureMechanism();

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

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    new PipingCalculationScenario(new GeneralPipingInput())
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
                    new PipingCalculationScenario(new GeneralPipingInput())
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
                        Output = PipingOutputTestFactory.Create(0.26065, 0.81398, 0.38024)
                    }
                }
            };

            ShowPipingScenarioView(calculationGroup, failureMechanism);
        }

        private void ShowPipingScenarioView(CalculationGroup calculationGroup, PipingFailureMechanism failureMechanism)
        {
            var pipingScenarioView = new PipingScenariosView(calculationGroup, failureMechanism, new AssessmentSectionStub());

            testForm.Controls.Add(pipingScenarioView);
            testForm.Show();
        }
    }
}