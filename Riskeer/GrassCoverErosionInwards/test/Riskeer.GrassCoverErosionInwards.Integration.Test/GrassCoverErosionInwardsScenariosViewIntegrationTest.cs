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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Views;
using Riskeer.GrassCoverErosionInwards.Plugin.FileImporters;
using Riskeer.Integration.Data;
using Riskeer.Integration.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsScenariosViewIntegrationTest
    {
        private readonly string filePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.GrassCoverErosionInwards.Integration,
                                                                      Path.Combine("DikeProfiles", "Voorlanden 6-3.shp"));

        [Test]
        public void ScenariosView_ImportDikeSection_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                DataImportHelper.ImportReferenceLine(assessmentSection);

                var view = new GrassCoverErosionInwardsScenariosView(assessmentSection.GrassCoverErosionInwards.CalculationsGroup,
                                                                     assessmentSection.GrassCoverErosionInwards);
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.RowCount);

                // Call
                IFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                assessmentSection.GrassCoverErosionInwards.NotifyObservers();

                // Assert
                Assert.AreEqual(283, dataGridView.RowCount);

                IEnumerable<string> expectedValues = assessmentSection.GrassCoverErosionInwards.SectionResults.Select(sr => sr.Section.Name);
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
            using (var form = new Form())
            {
                var mocks = new MockRepository();
                var messageProvider = mocks.Stub<IImporterMessageProvider>();
                mocks.ReplayAll();

                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                DataImportHelper.ImportReferenceLine(assessmentSection);
                IFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);

                CalculationGroup calculationsGroup = assessmentSection.GrassCoverErosionInwards.CalculationsGroup;
                var view = new GrassCoverErosionInwardsScenariosView(assessmentSection.GrassCoverErosionInwards.CalculationsGroup,
                                                                     assessmentSection.GrassCoverErosionInwards);
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var dikeProfilesImporter = new DikeProfilesImporter(assessmentSection.GrassCoverErosionInwards.DikeProfiles,
                                                                    assessmentSection.ReferenceLine,
                                                                    filePath, new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(
                                                                        (GrassCoverErosionInwardsFailureMechanism) failureMechanism),
                                                                    messageProvider);
                dikeProfilesImporter.Import();

                // Call
                foreach (DikeProfile profile in assessmentSection.GrassCoverErosionInwards.DikeProfiles)
                {
                    calculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculationScenario
                    {
                        Name = NamingHelper.GetUniqueName(((CalculationGroup) view.Data).Children, profile.Name, c => c.Name),
                        InputParameters =
                        {
                            DikeProfile = profile
                        }
                    });
                }

                calculationsGroup.NotifyObservers();

                // Assert
                DataGridViewCell dataGridViewCell = dataGridView.Rows[13].Cells[1];
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridViewCell).Items.Count);
                Assert.AreEqual("<selecteer>", ((DataGridViewComboBoxCell) dataGridViewCell).Items[0].ToString());
                Assert.AreEqual("profiel63p1Naam", ((DataGridViewComboBoxCell) dataGridViewCell).Items[1].ToString());

                mocks.VerifyAll();
            }
        }

        [Test]
        public void ScenariosView_RenameCalculations_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var mocks = new MockRepository();
                var messageProvider = mocks.Stub<IImporterMessageProvider>();
                mocks.ReplayAll();

                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                DataImportHelper.ImportReferenceLine(assessmentSection);
                IFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);

                CalculationGroup calculationsGroup = assessmentSection.GrassCoverErosionInwards.CalculationsGroup;
                var view = new GrassCoverErosionInwardsScenariosView(assessmentSection.GrassCoverErosionInwards.CalculationsGroup,
                                                                     assessmentSection.GrassCoverErosionInwards);
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var dikeProfilesImporter = new DikeProfilesImporter(assessmentSection.GrassCoverErosionInwards.DikeProfiles,
                                                                    assessmentSection.ReferenceLine,
                                                                    filePath,
                                                                    new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(
                                                                        (GrassCoverErosionInwardsFailureMechanism) failureMechanism),
                                                                    messageProvider);
                dikeProfilesImporter.Import();

                foreach (DikeProfile profile in assessmentSection.GrassCoverErosionInwards.DikeProfiles)
                {
                    calculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculationScenario
                    {
                        Name = NamingHelper.GetUniqueName(calculationsGroup.Children, profile.Name, c => c.Name),
                        InputParameters =
                        {
                            DikeProfile = profile
                        }
                    });
                }

                calculationsGroup.NotifyObservers();

                // Call
                foreach (GrassCoverErosionInwardsCalculationScenario calculation in calculationsGroup.Children.Cast<GrassCoverErosionInwardsCalculationScenario>())
                {
                    calculation.Name += "_changed";
                }

                // Assert
                DataGridViewCell dataGridViewCell = dataGridView.Rows[13].Cells[1];
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridViewCell).Items.Count);
                Assert.AreEqual("<selecteer>", ((DataGridViewComboBoxCell) dataGridViewCell).Items[0].ToString());
                Assert.AreEqual("profiel63p1Naam_changed", ((DataGridViewComboBoxCell) dataGridViewCell).Items[1].ToString());

                mocks.VerifyAll();
            }
        }

        [Test]
        public void ScenariosView_ChangeDikeProfileOfCalculation_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var mocks = new MockRepository();
                var messageProvider = mocks.Stub<IImporterMessageProvider>();
                mocks.ReplayAll();

                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                DataImportHelper.ImportReferenceLine(assessmentSection);
                IFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);

                var view = new GrassCoverErosionInwardsScenariosView(assessmentSection.GrassCoverErosionInwards.CalculationsGroup,
                                                                     assessmentSection.GrassCoverErosionInwards);
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var dikeProfilesImporter = new DikeProfilesImporter(assessmentSection.GrassCoverErosionInwards.DikeProfiles,
                                                                    assessmentSection.ReferenceLine,
                                                                    filePath, new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(
                                                                        (GrassCoverErosionInwardsFailureMechanism) failureMechanism),
                                                                    messageProvider);
                dikeProfilesImporter.Import();

                foreach (DikeProfile profile in assessmentSection.GrassCoverErosionInwards.DikeProfiles)
                {
                    assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculationScenario
                    {
                        Name = NamingHelper.GetUniqueName(assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children, profile.Name + "Calculation", c => c.Name),
                        InputParameters =
                        {
                            DikeProfile = profile
                        }
                    });
                }

                // Call
                CalculationGroup calculationsGroup = assessmentSection.GrassCoverErosionInwards.CalculationsGroup;
                ((GrassCoverErosionInwardsCalculationScenario) calculationsGroup.Children[1]).InputParameters.DikeProfile =
                    ((GrassCoverErosionInwardsCalculationScenario) calculationsGroup.Children[0]).InputParameters.DikeProfile;
                calculationsGroup.NotifyObservers();

                // Assert
                DataGridViewCell dataGridViewCell = dataGridView.Rows[13].Cells[1];
                Assert.AreEqual(3, ((DataGridViewComboBoxCell) dataGridViewCell).Items.Count);
                Assert.AreEqual("<selecteer>", ((DataGridViewComboBoxCell) dataGridViewCell).Items[0].ToString());
                Assert.AreEqual("profiel63p1NaamCalculation", ((DataGridViewComboBoxCell) dataGridViewCell).Items[1].ToString());
                Assert.AreEqual("profiel63p2NaamCalculation", ((DataGridViewComboBoxCell) dataGridViewCell).Items[2].ToString());

                DataGridViewCell dataGridViewCellWithRemovedCalculation = dataGridView.Rows[56].Cells[1];
                Assert.AreEqual(1, ((DataGridViewComboBoxCell) dataGridViewCellWithRemovedCalculation).Items.Count);
                Assert.AreEqual("<selecteer>", ((DataGridViewComboBoxCell) dataGridViewCellWithRemovedCalculation).Items[0].ToString());

                mocks.VerifyAll();
            }
        }
    }
}