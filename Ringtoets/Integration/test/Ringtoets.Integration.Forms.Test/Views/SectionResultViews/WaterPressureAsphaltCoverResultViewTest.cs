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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Util.Extensions;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;
using Ringtoets.Integration.Forms.Views.SectionResultViews;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class WaterPressureAsphaltCoverResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentResultIndex = 1;
        private const int tailorMadeAssessmentResultIndex = 2;
        private const int simpleAssemblyCategoryGroupIndex = 3;
        private const int tailorMadeAssemblyCategoryGroupIndex = 4;
        private const int combinedAssemblyCategoryGroupIndex = 5;
        private const int useManualAssemblyCategoryGroupIndex = 6;
        private const int manualAssemblyCategoryGroupIndex = 7;
        private const int columnCount = 8;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();

            // Call
            using (var view = new WaterPressureAsphaltCoverResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<WaterPressureAsphaltCoverFailureMechanismSectionResult,
                    WaterPressureAsphaltCoverSectionResultRow,
                    WaterPressureAsphaltCoverFailureMechanism,
                    FailureMechanismAssemblyCategoryGroupControl>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreAdded()
        {
            // Given
            using (var form = new Form())
            using (var view = new WaterPressureAsphaltCoverResultView(new ObservableList<WaterPressureAsphaltCoverFailureMechanismSectionResult>(),
                                                                      new WaterPressureAsphaltCoverFailureMechanism()))
            {
                form.Controls.Add(view);
                form.Show();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[tailorMadeAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[simpleAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[combinedAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[useManualAssemblyCategoryGroupIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[manualAssemblyCategoryGroupIndex]);

                Assert.AreEqual("Vak", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[tailorMadeAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\neenvoudige toets", dataGridView.Columns[simpleAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\ntoets op maat", dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\ngecombineerd", dataGridView.Columns[combinedAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Overschrijf\r\nassemblageresultaat", dataGridView.Columns[useManualAssemblyCategoryGroupIndex].HeaderText);
                Assert.AreEqual("Assemblageresultaat\r\nhandmatig", dataGridView.Columns[manualAssemblyCategoryGroupIndex].HeaderText);

                Assert.IsTrue(dataGridView.Columns[nameColumnIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[simpleAssessmentResultIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[tailorMadeAssessmentResultIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[simpleAssemblyCategoryGroupIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[tailorMadeAssemblyCategoryGroupIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[combinedAssemblyCategoryGroupIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[useManualAssemblyCategoryGroupIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[manualAssemblyCategoryGroupIndex].ReadOnly);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1")
            });

            // Call
            using (var form = new Form())
            using (new AssemblyToolCalculatorFactoryConfig())
            using (var view = new WaterPressureAsphaltCoverResultView(failureMechanism.SectionResults, failureMechanism))
            {
                form.Controls.Add(view);
                form.Show();

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(columnCount, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultType.None, cells[simpleAssessmentResultIndex].Value);
                Assert.AreEqual(TailorMadeAssessmentResultType.None, cells[tailorMadeAssessmentResultIndex].Value);
                Assert.AreEqual("Iv", cells[simpleAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual("IIv", cells[tailorMadeAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual("IIv", cells[combinedAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual(false, cells[useManualAssemblyCategoryGroupIndex].Value);
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroup.None, cells[manualAssemblyCategoryGroupIndex].Value);
            }
        }

        [Test]
        public void FailureMechanismResultView_WithFailureMechanismWithManualSectionAssemblyResults_ThenWarningSet()
        {
            // Setup
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 2);
            failureMechanism.SectionResults.First().UseManualAssemblyCategoryGroup = true;

            using (var form = new Form())
            using (var view = new WaterPressureAsphaltCoverResultView(failureMechanism.SectionResults,
                                                                      failureMechanism))
            {
                form.Controls.Add(view);
                form.Show();

                FailureMechanismAssemblyCategoryGroupControl failureMechanismAssemblyControl = GetFailureMechanismAssemblyControl();
                ErrorProvider warningProvider = GetWarningProvider(failureMechanismAssemblyControl);

                // Call
                string warningMessage = warningProvider.GetError(failureMechanismAssemblyControl);

                // Assert
                Assert.AreEqual("Toetsoordeel is (deels) gebaseerd op handmatig overschreven toetsoordelen.", warningMessage);
            }
        }

        [Test]
        public void GivenFailureMechanismResultsViewWithWarnings_WhenFailureMechanismWithoutManualSectionAssemblyResultsAndFailureMechanismNotifiesObservers_ThenWarningCleared()
        {
            // Given
            var failureMechanism = new WaterPressureAsphaltCoverFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, 2);
            failureMechanism.SectionResults.First().UseManualAssemblyCategoryGroup = true;

            using (var form = new Form())
            using (var view = new WaterPressureAsphaltCoverResultView(failureMechanism.SectionResults,
                                                                      failureMechanism))
            {
                form.Controls.Add(view);
                form.Show();

                FailureMechanismAssemblyCategoryGroupControl failureMechanismAssemblyControl = GetFailureMechanismAssemblyControl();
                ErrorProvider warningProvider = GetWarningProvider(failureMechanismAssemblyControl);

                // Precondition
                Assert.AreEqual("Toetsoordeel is (deels) gebaseerd op handmatig overschreven toetsoordelen.", warningProvider.GetError(failureMechanismAssemblyControl));

                // When
                failureMechanism.SectionResults.ForEachElementDo(sr => sr.UseManualAssemblyCategoryGroup = false);
                failureMechanism.NotifyObservers();

                // Then
                Assert.IsEmpty(warningProvider.GetError(failureMechanismAssemblyControl));
            }
        }

        [TestFixture]
        public class WaterPressureAsphaltCoverFailureMechanismResultControlTest : FailureMechanismAssemblyCategoryGroupControlTestFixture<
            WaterPressureAsphaltCoverResultView,
            WaterPressureAsphaltCoverFailureMechanism,
            WaterPressureAsphaltCoverFailureMechanismSectionResult,
            WaterPressureAsphaltCoverSectionResultRow>
        {
            protected override WaterPressureAsphaltCoverResultView CreateResultView(WaterPressureAsphaltCoverFailureMechanism failureMechanism)
            {
                return new WaterPressureAsphaltCoverResultView(failureMechanism.SectionResults,
                                                               failureMechanism);
            }
        }

        private static FailureMechanismAssemblyCategoryGroupControl GetFailureMechanismAssemblyControl()
        {
            var control = (FailureMechanismAssemblyCategoryGroupControl) ((TableLayoutPanel) new ControlTester("TableLayoutPanel").TheObject).GetControlFromPosition(1, 0);
            return control;
        }

        private static ErrorProvider GetWarningProvider(FailureMechanismAssemblyCategoryGroupControl control)
        {
            return TypeUtils.GetField<ErrorProvider>(control, "warningProvider");
        }
    }
}