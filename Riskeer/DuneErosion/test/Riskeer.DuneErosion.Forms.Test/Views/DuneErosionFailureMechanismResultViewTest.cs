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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Controls;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.Views;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Forms;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;

namespace Riskeer.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneErosionFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultForFactorizedSignalingNormIndex = 2;
        private const int detailedAssessmentResultForSignalingNormIndex = 3;
        private const int detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex = 4;
        private const int detailedAssessmentResultForLowerLimitNormIndex = 5;
        private const int detailedAssessmentResultForFactorizedLowerLimitNormIndex = 6;
        private const int tailorMadeResultIndex = 7;
        private const int simpleAssemblyCategoryGroupIndex = 8;
        private const int detailedAssemblyCategoryGroupIndex = 9;
        private const int tailorMadeAssemblyCategoryGroupIndex = 10;
        private const int combinedAssemblyCategoryGroupIndex = 11;
        private const int useManualAssemblyIndex = 12;
        private const int manualAssemblyCategoryGroupIndex = 13;
        private const int columnCount = 14;

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
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            using (var view = new DuneErosionFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<DuneErosionFailureMechanismSectionResult,
                    DuneErosionSectionResultRow,
                    DuneErosionFailureMechanism,
                    FailureMechanismAssemblyCategoryGroupControl>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (ShowFailureMechanismResultsView(new DuneErosionFailureMechanism()))
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForFactorizedSignalingNormIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForSignalingNormIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForLowerLimitNormIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForFactorizedLowerLimitNormIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[tailorMadeResultIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[simpleAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[detailedAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[combinedAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[useManualAssemblyIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[manualAssemblyCategoryGroupIndex]);

                Assert.AreEqual("Vak", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens Iv", dataGridView.Columns[detailedAssessmentResultForFactorizedSignalingNormIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens IIv", dataGridView.Columns[detailedAssessmentResultForSignalingNormIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens IIIv", dataGridView.Columns[detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens IVv", dataGridView.Columns[detailedAssessmentResultForLowerLimitNormIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens Vv", dataGridView.Columns[detailedAssessmentResultForFactorizedLowerLimitNormIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[tailorMadeResultIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\neenvoudige toets", dataGridView.Columns[simpleAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\ngedetailleerde toets per vak", dataGridView.Columns[detailedAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\ntoets op maat", dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\ngecombineerd", dataGridView.Columns[combinedAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Overschrijf\r\ntoetsoordeel", dataGridView.Columns[useManualAssemblyIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\nhandmatig", dataGridView.Columns[manualAssemblyCategoryGroupIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1")
            });

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(columnCount, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentValidityOnlyResultType.None, cells[simpleAssessmentResultIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForFactorizedSignalingNormIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForSignalingNormIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForLowerLimitNormIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForFactorizedLowerLimitNormIndex].Value);
                Assert.AreEqual(TailorMadeAssessmentCategoryGroupResultType.None, cells[tailorMadeResultIndex].Value);
                Assert.AreEqual("VIIv", cells[simpleAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual("IIv", cells[detailedAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual("Iv", cells[tailorMadeAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual("Iv", cells[combinedAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual(false, cells[useManualAssemblyIndex].Value);
                Assert.AreEqual(SelectableFailureMechanismSectionAssemblyCategoryGroup.None, cells[manualAssemblyCategoryGroupIndex].Value);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithManualAssembly_WhenShown_ThenManualAssemblyUsed()
        {
            // Given
            var failureMechanism = new DuneErosionFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            const FailureMechanismSectionAssemblyCategoryGroup categoryGroup = FailureMechanismSectionAssemblyCategoryGroup.IIIv;
            sectionResult.ManualAssemblyCategoryGroup = categoryGroup;
            sectionResult.UseManualAssembly = true;

            // When
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(categoryGroup, calculator.FailureMechanismSectionCategories.Single());
            }
        }

        [TestFixture]
        public class DuneErosionFailureMechanismResultControlTest : FailureMechanismAssemblyCategoryGroupControlTestFixture<
            DuneErosionFailureMechanismResultView,
            DuneErosionFailureMechanism,
            DuneErosionFailureMechanismSectionResult,
            DuneErosionSectionResultRow>
        {
            protected override DuneErosionFailureMechanismResultView CreateResultView(DuneErosionFailureMechanism failureMechanism)
            {
                return new DuneErosionFailureMechanismResultView(failureMechanism.SectionResults,
                                                                 failureMechanism);
            }
        }

        private static FailureMechanismAssemblyCategoryGroupControl GetFailureMechanismAssemblyControl()
        {
            var control = (FailureMechanismAssemblyCategoryGroupControl) ((TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject).GetControlFromPosition(1, 0);
            return control;
        }

        private DuneErosionFailureMechanismResultView ShowFailureMechanismResultsView(
            DuneErosionFailureMechanism failureMechanism)
        {
            var failureMechanismResultView = new DuneErosionFailureMechanismResultView(failureMechanism.SectionResults,
                                                                                       failureMechanism);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private static ErrorProvider GetManualAssemblyWarningProvider(FailureMechanismAssemblyCategoryGroupControl control)
        {
            return TypeUtils.GetField<ErrorProvider>(control, "manualAssemblyWarningProvider");
        }
    }
}