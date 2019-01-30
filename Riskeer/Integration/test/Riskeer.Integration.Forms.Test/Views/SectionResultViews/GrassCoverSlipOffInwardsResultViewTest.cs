﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Controls;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Forms.Views.SectionResultRows;
using Riskeer.Integration.Forms.Views.SectionResultViews;

namespace Riskeer.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class GrassCoverSlipOffInwardsResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultIndex = 2;
        private const int tailorMadeAssessmentResultIndex = 3;
        private const int simpleAssemblyCategoryGroupIndex = 4;
        private const int detailedAssemblyCategoryGroupIndex = 5;
        private const int tailorMadeAssemblyCategoryGroupIndex = 6;
        private const int combinedAssemblyCategoryGroupIndex = 7;
        private const int useManualAssemblyIndex = 8;
        private const int manualAssemblyCategoryGroupIndex = 9;
        private const int columnCount = 10;

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
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            // Call
            using (var view = new GrassCoverSlipOffInwardsResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<GrassCoverSlipOffInwardsFailureMechanismSectionResult,
                    GrassCoverSlipOffInwardsSectionResultRow,
                    GrassCoverSlipOffInwardsFailureMechanism,
                    FailureMechanismAssemblyCategoryGroupControl>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreAdded()
        {
            // Given
            using (ShowFailureMechanismResultsView(new GrassCoverSlipOffInwardsFailureMechanism()))
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[tailorMadeAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[simpleAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[detailedAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[combinedAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[useManualAssemblyIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[manualAssemblyCategoryGroupIndex]);

                Assert.AreEqual("Vak", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[detailedAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[tailorMadeAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\neenvoudige toets", dataGridView.Columns[simpleAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\ngedetailleerde toets per vak", dataGridView.Columns[detailedAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\ntoets op maat", dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\ngecombineerd", dataGridView.Columns[combinedAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Overschrijf\r\ntoetsoordeel", dataGridView.Columns[useManualAssemblyIndex].HeaderText);
                Assert.AreEqual("Toetsoordeel\r\nhandmatig", dataGridView.Columns[manualAssemblyCategoryGroupIndex].HeaderText);

                Assert.IsTrue(dataGridView.Columns[nameColumnIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[simpleAssessmentResultIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[detailedAssessmentResultIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[tailorMadeAssessmentResultIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[simpleAssemblyCategoryGroupIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[detailedAssemblyCategoryGroupIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[combinedAssemblyCategoryGroupIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[useManualAssemblyIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[manualAssemblyCategoryGroupIndex].ReadOnly);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1")
            });

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                Assert.AreEqual(columnCount, dataGridView.ColumnCount);

                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultType.None, cells[simpleAssessmentResultIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultIndex].Value);
                Assert.AreEqual(TailorMadeAssessmentResultType.None, cells[tailorMadeAssessmentResultIndex].Value);
                Assert.AreEqual("Iv", cells[simpleAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual("IIv", cells[detailedAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual("IIv", cells[tailorMadeAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual("IIv", cells[combinedAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual(false, cells[useManualAssemblyIndex].Value);
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroup.None, cells[manualAssemblyCategoryGroupIndex].Value);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithManualAssembly_WhenShown_ThenManualAssemblyUsed()
        {
            // Given
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;
            sectionResult.UseManualAssembly = true;

            // When
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                calculator.FailureMechanismSectionCategories.Single());
            }
        }

        [TestFixture]
        public class GrassCoverSlipOffInwardsFailureMechanismResultControlTest : FailureMechanismAssemblyCategoryGroupControlTestFixture<
            GrassCoverSlipOffInwardsResultView,
            GrassCoverSlipOffInwardsFailureMechanism,
            GrassCoverSlipOffInwardsFailureMechanismSectionResult,
            GrassCoverSlipOffInwardsSectionResultRow>
        {
            protected override GrassCoverSlipOffInwardsResultView CreateResultView(GrassCoverSlipOffInwardsFailureMechanism failureMechanism)
            {
                return new GrassCoverSlipOffInwardsResultView(failureMechanism.SectionResults,
                                                              failureMechanism);
            }
        }

        private static FailureMechanismAssemblyCategoryGroupControl GetFailureMechanismAssemblyControl()
        {
            var control = (FailureMechanismAssemblyCategoryGroupControl) ((TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject).GetControlFromPosition(1, 0);
            return control;
        }

        private GrassCoverSlipOffInwardsResultView ShowFailureMechanismResultsView(
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism)
        {
            var failureMechanismResultView = new GrassCoverSlipOffInwardsResultView(failureMechanism.SectionResults,
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