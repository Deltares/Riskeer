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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.Views;
using Riskeer.HeightStructures.IO;
using Riskeer.HeightStructures.Plugin.FileImporters;
using Riskeer.Integration.TestUtil;
using Riskeer.Integration.Data;

namespace Riskeer.HeightStructures.Integration.Test
{
    [TestFixture]
    public class HeightStructuresScenariosViewIntegrationTest
    {
        private readonly string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.HeightStructures.Integration,
                                                                      Path.Combine("HeightStructures", "kunstwerken_6_3.shp"));

        [Test]
        public void ScenariosView_ImportFailureMechanismSections_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                DataImportHelper.ImportReferenceLine(assessmentSection);

                var view = new HeightStructuresScenariosView
                {
                    Data = assessmentSection.HeightStructures.CalculationsGroup,
                    FailureMechanism = assessmentSection.HeightStructures
                };
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.RowCount);

                // Call
                IFailureMechanism failureMechanism = assessmentSection.HeightStructures;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                assessmentSection.HeightStructures.NotifyObservers();

                // Assert
                Assert.AreEqual(283, dataGridView.RowCount);

                IEnumerable<string> expectedValues = assessmentSection.HeightStructures.SectionResults.Select(sr => sr.Section.Name);
                var foundValues = new List<string>();
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    foundValues.Add(row.Cells[0].FormattedValue.ToString());
                }

                CollectionAssert.AreEqual(expectedValues, foundValues);
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
                HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                new HeightStructuresImporter(assessmentSection.HeightStructures.HeightStructures,
                                             assessmentSection.ReferenceLine,
                                             filePath, messageProvider, new HeightStructureReplaceDataStrategy(failureMechanism))
                    .Import();

                CalculationGroup calculationsGroup = assessmentSection.HeightStructures.CalculationsGroup;
                var view = new HeightStructuresScenariosView
                {
                    Data = calculationsGroup,
                    FailureMechanism = assessmentSection.HeightStructures
                };
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                foreach (HeightStructure structure in assessmentSection.HeightStructures.HeightStructures)
                {
                    calculationsGroup.Children.Add(new StructuresCalculation<HeightStructuresInput>
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
                DataGridViewCell dataGridViewCell = dataGridView.Rows[13].Cells[1];
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridViewCell).Items.Count);
                Assert.AreEqual("<selecteer>", ((DataGridViewComboBoxCell) dataGridViewCell).Items[0].ToString());
                Assert.AreEqual("Eerste kunstwerk 6-3", ((DataGridViewComboBoxCell) dataGridViewCell).Items[1].ToString());
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
                HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                new HeightStructuresImporter(assessmentSection.HeightStructures.HeightStructures,
                                             assessmentSection.ReferenceLine,
                                             filePath, messageProvider, new HeightStructureReplaceDataStrategy(failureMechanism))
                    .Import();

                CalculationGroup calculationsGroup = assessmentSection.HeightStructures.CalculationsGroup;
                var view = new HeightStructuresScenariosView
                {
                    Data = calculationsGroup,
                    FailureMechanism = assessmentSection.HeightStructures
                };
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                foreach (HeightStructure structure in assessmentSection.HeightStructures.HeightStructures)
                {
                    calculationsGroup.Children.Add(new StructuresCalculation<HeightStructuresInput>
                    {
                        Name = NamingHelper.GetUniqueName(calculationsGroup.Children, structure.Name, c => c.Name),
                        InputParameters =
                        {
                            Structure = structure
                        }
                    });
                }

                calculationsGroup.NotifyObservers();

                // Call
                foreach (ICalculationBase calculationBase in calculationsGroup.Children)
                {
                    var calculation = (StructuresCalculation<HeightStructuresInput>) calculationBase;
                    calculation.Name += "_changed";
                }

                // Assert
                DataGridViewCell dataGridViewCell = dataGridView.Rows[13].Cells[1];
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridViewCell).Items.Count);
                Assert.AreEqual("<selecteer>", ((DataGridViewComboBoxCell) dataGridViewCell).Items[0].ToString());
                Assert.AreEqual("Eerste kunstwerk 6-3_changed", ((DataGridViewComboBoxCell) dataGridViewCell).Items[1].ToString());
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
                HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                new HeightStructuresImporter(assessmentSection.HeightStructures.HeightStructures,
                                             assessmentSection.ReferenceLine,
                                             filePath, messageProvider, new HeightStructureReplaceDataStrategy(failureMechanism))
                    .Import();

                var view = new HeightStructuresScenariosView
                {
                    Data = assessmentSection.HeightStructures.CalculationsGroup,
                    FailureMechanism = assessmentSection.HeightStructures
                };
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                foreach (HeightStructure structure in assessmentSection.HeightStructures.HeightStructures)
                {
                    assessmentSection.HeightStructures.CalculationsGroup.Children.Add(new StructuresCalculation<HeightStructuresInput>
                    {
                        Name = NamingHelper.GetUniqueName(assessmentSection.HeightStructures.CalculationsGroup.Children, structure.Name + "Calculation", c => c.Name),
                        InputParameters =
                        {
                            Structure = structure
                        }
                    });
                }

                // Call
                CalculationGroup calculationsGroup = assessmentSection.HeightStructures.CalculationsGroup;
                ((StructuresCalculation<HeightStructuresInput>) calculationsGroup.Children[1]).InputParameters.Structure =
                    ((StructuresCalculation<HeightStructuresInput>) calculationsGroup.Children[0]).InputParameters.Structure;
                calculationsGroup.NotifyObservers();

                // Assert
                DataGridViewCell dataGridViewCell = dataGridView.Rows[13].Cells[1];
                Assert.AreEqual(3, ((DataGridViewComboBoxCell) dataGridViewCell).Items.Count);
                Assert.AreEqual("<selecteer>", ((DataGridViewComboBoxCell) dataGridViewCell).Items[0].ToString());
                Assert.AreEqual("Eerste kunstwerk 6-3Calculation", ((DataGridViewComboBoxCell) dataGridViewCell).Items[1].ToString());
                Assert.AreEqual("Tweede kunstwerk 6-3Calculation", ((DataGridViewComboBoxCell) dataGridViewCell).Items[2].ToString());

                DataGridViewCell dataGridViewCellWithRemovedCalculation = dataGridView.Rows[56].Cells[1];
                Assert.AreEqual(1, ((DataGridViewComboBoxCell) dataGridViewCellWithRemovedCalculation).Items.Count);
                Assert.AreEqual("<selecteer>", ((DataGridViewComboBoxCell) dataGridViewCellWithRemovedCalculation).Items[0].ToString());
            }

            mocks.VerifyAll();
        }
    }
}