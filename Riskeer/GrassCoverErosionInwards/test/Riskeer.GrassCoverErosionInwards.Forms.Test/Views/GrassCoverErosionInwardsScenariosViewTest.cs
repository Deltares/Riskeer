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
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsScenariosViewTest
    {
        private const int isRelevantColumnIndex = 0;
        private const int contributionColumnIndex = 1;
        private const int nameColumnIndex = 2;
        private const int failureProbabilityColumnIndex = 3;

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
            void Call() => new GrassCoverErosionInwardsScenariosView(new CalculationGroup(), new GrassCoverErosionInwardsFailureMechanism(), null);

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
            using (var view = new GrassCoverErosionInwardsScenariosView(calculationGroup, new GrassCoverErosionInwardsFailureMechanism(), assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<ScenariosView<GrassCoverErosionInwardsCalculationScenario, GrassCoverErosionInwardsInput, GrassCoverErosionInwardsScenarioRow, GrassCoverErosionInwardsFailureMechanism>>(view);
                Assert.AreSame(calculationGroup, view.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowGrassCoverErosionInwardsScenariosView(new GrassCoverErosionInwardsFailureMechanism());

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(4, dataGridView.ColumnCount);
            Assert.AreEqual("In oordeel", dataGridView.Columns[isRelevantColumnIndex].HeaderText);
            Assert.AreEqual("Bijdrage aan\r\nscenario\r\n[%]", dataGridView.Columns[contributionColumnIndex].HeaderText);
            Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
            Assert.AreEqual("Faalkans\r\n[1/jaar]", dataGridView.Columns[failureProbabilityColumnIndex].HeaderText);
        }

        [Test]
        public void GrassCoverErosionInwardsScenarioView_CalculationsWithAllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Call
            ShowFullyConfiguredGrassCoverErosionInwardsScenariosView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Assert
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 1", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[failureProbabilityColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
            Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
            Assert.AreEqual("Calculation 2", cells[nameColumnIndex].FormattedValue);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(0.5), cells[failureProbabilityColumnIndex].FormattedValue);
        }

        private void ShowFullyConfiguredGrassCoverErosionInwardsScenariosView()
        {
            DikeProfile dikeProfile1 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(0.0, 0.0));
            DikeProfile dikeProfile2 = DikeProfileTestFactory.CreateDikeProfile(new Point2D(5.0, 0.0));

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

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
                new GrassCoverErosionInwardsCalculationScenario
                {
                    Name = "Calculation 1",
                    InputParameters =
                    {
                        DikeProfile = dikeProfile1
                    }
                },
                new GrassCoverErosionInwardsCalculationScenario
                {
                    Name = "Calculation 2",
                    InputParameters =
                    {
                        DikeProfile = dikeProfile2
                    },
                    Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0.2), null, null)
                }
            });

            ShowGrassCoverErosionInwardsScenariosView(failureMechanism);
        }

        private void ShowGrassCoverErosionInwardsScenariosView(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            var scenariosView = new GrassCoverErosionInwardsScenariosView(failureMechanism.CalculationsGroup, failureMechanism, new AssessmentSectionStub());
            testForm.Controls.Add(scenariosView);
            testForm.Show();
        }
    }
}