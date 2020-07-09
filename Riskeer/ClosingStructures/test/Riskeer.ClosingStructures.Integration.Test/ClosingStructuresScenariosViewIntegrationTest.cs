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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.Views;
using Riskeer.ClosingStructures.IO;
using Riskeer.ClosingStructures.Plugin.FileImporters;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Integration.Data;
using Riskeer.Integration.TestUtil;

namespace Riskeer.ClosingStructures.Integration.Test
{
    [TestFixture]
    public class ClosingStructuresScenariosViewIntegrationTest
    {
        private const int isRelevantColumnIndex = 0;
        private const int contributionColumnIndex = 1;
        private const int nameColumnIndex = 2;
        private const int failureProbabilityColumnIndex = 3;

        private readonly string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.ClosingStructures.Integration,
                                                                      Path.Combine("ClosingStructures", "kunstwerken_6_3.shp"));

        [Test]
        public void ScenariosView_ImportFailureMechanismSections_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                DataImportHelper.ImportReferenceLine(assessmentSection);

                var view = new ClosingStructuresScenariosView(assessmentSection.ClosingStructures.CalculationsGroup, assessmentSection.ClosingStructures, assessmentSection);
                form.Controls.Add(view);
                form.Show();

                var listBox = (ListBox) new ControlTester("listBox").TheObject;

                // Precondition
                CollectionAssert.IsEmpty(listBox.Items);

                // Call
                IFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                assessmentSection.ClosingStructures.NotifyObservers();

                // Assert
                CollectionAssert.AreEqual(assessmentSection.ClosingStructures.Sections, listBox.Items);
            }
        }

        [Test]
        public void ScenariosView_GenerateCalculations_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                DataImportHelper.ImportReferenceLine(assessmentSection);
                ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);

                CalculationGroup calculationsGroup = assessmentSection.ClosingStructures.CalculationsGroup;
                var view = new ClosingStructuresScenariosView(calculationsGroup, assessmentSection.ClosingStructures, assessmentSection);

                form.Controls.Add(view);
                form.Show();

                var structuresImporter = new ClosingStructuresImporter(assessmentSection.ClosingStructures.ClosingStructures,
                                                                       assessmentSection.ReferenceLine, filePath, messageProvider,
                                                                       new ClosingStructureReplaceDataStrategy(failureMechanism));
                structuresImporter.Import();

                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                listBox.SelectedItem = failureMechanism.Sections.ElementAt(32);

                // Precondition
                DataGridViewRowCollection rows = dataGridView.Rows;
                CollectionAssert.IsEmpty(rows);

                // Call
                foreach (ClosingStructure structure in assessmentSection.ClosingStructures.ClosingStructures)
                {
                    calculationsGroup.Children.Add(new StructuresCalculationScenario<ClosingStructuresInput>
                    {
                        Name = NamingHelper.GetUniqueName(((CalculationGroup) view.Data).Children, structure.Name, c => c.Name),
                        InputParameters =
                        {
                            Structure = structure
                        }
                    });
                }

                calculationsGroup.NotifyObservers();

                // Assert
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
                Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
                Assert.AreEqual("Eerste kunstwerk sluiting 6-3", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("-", cells[failureProbabilityColumnIndex].FormattedValue);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ScenariosView_RenameCalculations_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                DataImportHelper.ImportReferenceLine(assessmentSection);
                ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);

                CalculationGroup calculationsGroup = assessmentSection.ClosingStructures.CalculationsGroup;
                var view = new ClosingStructuresScenariosView(calculationsGroup, assessmentSection.ClosingStructures, assessmentSection);

                form.Controls.Add(view);
                form.Show();

                var structuresImporter = new ClosingStructuresImporter(assessmentSection.ClosingStructures.ClosingStructures,
                                                                       assessmentSection.ReferenceLine, filePath, messageProvider,
                                                                       new ClosingStructureReplaceDataStrategy(failureMechanism));
                structuresImporter.Import();

                foreach (ClosingStructure structure in assessmentSection.ClosingStructures.ClosingStructures)
                {
                    calculationsGroup.Children.Add(new StructuresCalculationScenario<ClosingStructuresInput>
                    {
                        Name = NamingHelper.GetUniqueName(calculationsGroup.Children, structure.Name, c => c.Name),
                        InputParameters =
                        {
                            Structure = structure
                        }
                    });
                }

                calculationsGroup.NotifyObservers();

                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                listBox.SelectedItem = failureMechanism.Sections.ElementAt(32);

                // Precondition
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual("Eerste kunstwerk sluiting 6-3", cells[nameColumnIndex].FormattedValue);

                // Call
                foreach (StructuresCalculationScenario<ClosingStructuresInput> calculation in calculationsGroup.Children.Cast<StructuresCalculationScenario<ClosingStructuresInput>>())
                {
                    calculation.Name += "_changed";
                }

                // Assert
                Assert.AreEqual("Eerste kunstwerk sluiting 6-3_changed", cells[nameColumnIndex].FormattedValue);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ScenariosView_ChangeStructureOfCalculation_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IImporterMessageProvider>();
            mocks.ReplayAll();

            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                DataImportHelper.ImportReferenceLine(assessmentSection);
                ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);

                var view = new ClosingStructuresScenariosView(assessmentSection.ClosingStructures.CalculationsGroup, assessmentSection.ClosingStructures, assessmentSection);
                form.Controls.Add(view);
                form.Show();

                var structuresImporter = new ClosingStructuresImporter(assessmentSection.ClosingStructures.ClosingStructures,
                                                                       assessmentSection.ReferenceLine, filePath, messageProvider,
                                                                       new ClosingStructureReplaceDataStrategy(failureMechanism));
                structuresImporter.Import();

                foreach (ClosingStructure structure in assessmentSection.ClosingStructures.ClosingStructures)
                {
                    assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(new StructuresCalculationScenario<ClosingStructuresInput>
                    {
                        Name = NamingHelper.GetUniqueName(assessmentSection.ClosingStructures.CalculationsGroup.Children, structure.Name + " Calculation", c => c.Name),
                        InputParameters =
                        {
                            Structure = structure
                        }
                    });
                }

                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                listBox.SelectedItem = failureMechanism.Sections.ElementAt(32);

                // Precondition
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);
                Assert.AreEqual("Eerste kunstwerk sluiting 6-3 Calculation", rows[0].Cells[nameColumnIndex].FormattedValue);

                // Call
                CalculationGroup calculationsGroup = assessmentSection.ClosingStructures.CalculationsGroup;
                ((StructuresCalculation<ClosingStructuresInput>) calculationsGroup.Children[1]).InputParameters.Structure =
                    ((StructuresCalculation<ClosingStructuresInput>) calculationsGroup.Children[0]).InputParameters.Structure;
                calculationsGroup.NotifyObservers();

                // Assert
                Assert.AreEqual(2, rows.Count);
                Assert.AreEqual("Eerste kunstwerk sluiting 6-3 Calculation", rows[0].Cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("Tweede kunstwerk sluiting 6-3 Calculation", rows[1].Cells[nameColumnIndex].FormattedValue);
            }

            mocks.VerifyAll();
        }
    }
}