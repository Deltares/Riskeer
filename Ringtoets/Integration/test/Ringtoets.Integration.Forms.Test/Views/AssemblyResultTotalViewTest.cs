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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyResultTotalViewTest
    {
        private const int expectedColumnCount = 5;
        private const int failureMechanismNameColumnIndex = 0;
        private const int failureMechanismCodeColumnIndex = 1;
        private const int failureMechanismGroupColumnIndex = 2;
        private const int failureMechanismAssemblyCategoryColumnIndex = 3;
        private const int failureMechanisProbabilityColumnIndex = 4;

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
            TestDelegate call = () => new AssemblyResultTotalView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAssessmentSection_ExpectedValuesSet()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            using (var view = new AssemblyResultTotalView(assessmentSection))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreEqual(1, view.Controls.Count);

                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }

        [Test]
        public void GivenFormWithAssessmentSection_ThenExpectedColumnsAreVisible()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Then
            using (var view = new AssemblyResultTotalView(assessmentSection))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreEqual(expectedColumnCount, dataGridView.ColumnCount);

                DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismNameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismCodeColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismGroupColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanismAssemblyCategoryColumnIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewColumns[failureMechanisProbabilityColumnIndex]);

                Assert.AreEqual("Toetsspoor", dataGridViewColumns[failureMechanismNameColumnIndex].HeaderText);
                Assert.AreEqual("Label", dataGridViewColumns[failureMechanismCodeColumnIndex].HeaderText);
                Assert.AreEqual("Groep", dataGridViewColumns[failureMechanismGroupColumnIndex].HeaderText);
                Assert.AreEqual("Categorie", dataGridViewColumns[failureMechanismAssemblyCategoryColumnIndex].HeaderText);
                Assert.AreEqual("Benaderde faalkans", dataGridViewColumns[failureMechanisProbabilityColumnIndex].HeaderText);

                Assert.IsTrue(dataGridViewColumns[failureMechanismNameColumnIndex].ReadOnly);
                Assert.IsTrue(dataGridViewColumns[failureMechanismCodeColumnIndex].ReadOnly);
                Assert.IsTrue(dataGridViewColumns[failureMechanismGroupColumnIndex].ReadOnly);
            }
        }

        [Test]
        public void GivenFormWithAssessmentSection_ThenExpectedRowsVisible()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Then
            using (var view = new AssemblyResultTotalView(assessmentSection))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(assessmentSection.GetFailureMechanisms().Count(), rows.Count);

                PipingFailureMechanism piping = assessmentSection.Piping;
                AssertAssemblyRow(piping, rows[0].Cells);

                GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = assessmentSection.GrassCoverErosionInwards;
                AssertAssemblyRow(grassCoverErosionInwards, rows[1].Cells);

                MacroStabilityInwardsFailureMechanism macroStabilityInwards = assessmentSection.MacroStabilityInwards;
                AssertAssemblyRow(macroStabilityInwards, rows[2].Cells);

                MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = assessmentSection.MacroStabilityOutwards;
                AssertAssemblyRow(macroStabilityOutwards, rows[3].Cells);

                MicrostabilityFailureMechanism microStability = assessmentSection.Microstability;
                AssertAssemblyRow(microStability, rows[4].Cells);

                StabilityStoneCoverFailureMechanism stabilityStoneCover = assessmentSection.StabilityStoneCover;
                AssertAssemblyRow(stabilityStoneCover, rows[5].Cells);

                WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = assessmentSection.WaveImpactAsphaltCover;
                AssertAssemblyRow(waveImpactAsphaltCover, rows[6].Cells);

                WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover = assessmentSection.WaterPressureAsphaltCover;
                AssertAssemblyRow(waterPressureAsphaltCover, rows[7].Cells);

                GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = assessmentSection.GrassCoverErosionOutwards;
                AssertAssemblyRow(grassCoverErosionOutwards, rows[8].Cells);

                GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = assessmentSection.GrassCoverSlipOffOutwards;
                AssertAssemblyRow(grassCoverSlipOffOutwards, rows[9].Cells);

                GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = assessmentSection.GrassCoverSlipOffInwards;
                AssertAssemblyRow(grassCoverSlipOffInwards, rows[10].Cells);

                HeightStructuresFailureMechanism heightStructures = assessmentSection.HeightStructures;
                AssertAssemblyRow(heightStructures, rows[11].Cells);

                ClosingStructuresFailureMechanism closingStructures = assessmentSection.ClosingStructures;
                AssertAssemblyRow(closingStructures, rows[12].Cells);

                PipingStructureFailureMechanism pipingStructure = assessmentSection.PipingStructure;
                AssertAssemblyRow(pipingStructure, rows[13].Cells);

                StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSection.StabilityPointStructures;
                AssertAssemblyRow(stabilityPointStructures, rows[14].Cells);

                StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction = assessmentSection.StrengthStabilityLengthwiseConstruction;
                AssertAssemblyRow(strengthStabilityLengthwiseConstruction, rows[15].Cells);

                DuneErosionFailureMechanism duneErosion = assessmentSection.DuneErosion;
                AssertAssemblyRow(duneErosion, rows[16].Cells);

                TechnicalInnovationFailureMechanism technicalInnovation = assessmentSection.TechnicalInnovation;
                AssertAssemblyRow(technicalInnovation, rows[17].Cells);
            }
        }

        private static void AssertAssemblyRow(IFailureMechanism failureMechanism, DataGridViewCellCollection row)
        {
            Assert.AreEqual(expectedColumnCount, row.Count);

            Assert.AreEqual(failureMechanism.Name, row[failureMechanismNameColumnIndex].Value);
            Assert.AreEqual(failureMechanism.Code, row[failureMechanismCodeColumnIndex].Value);
            Assert.AreEqual(failureMechanism.Group, row[failureMechanismGroupColumnIndex].Value);
        }
    }
}