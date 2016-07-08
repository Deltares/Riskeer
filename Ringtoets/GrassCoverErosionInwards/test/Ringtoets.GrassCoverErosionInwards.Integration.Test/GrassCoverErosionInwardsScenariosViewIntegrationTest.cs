// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Plugin.FileImporter;
using Ringtoets.Integration.Data;

namespace Ringtoets.GrassCoverErosionInwards.Integration.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsScenariosViewIntegrationTest
    {
        private readonly string filePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.Integration,
                                                                      Path.Combine("DikeProfiles", "Voorlanden 6-3.shp"));

        [Test]
        public void ScenariosView_ImportDikeSection_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                IntegrationTestHelper.ImportReferenceLine(assessmentSection);

                var view = new GrassCoverErosionInwardsScenariosView()
                {
                    Data = assessmentSection.GrassCoverErosionInwards.CalculationsGroup,
                    FailureMechanism = assessmentSection.GrassCoverErosionInwards
                };
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.RowCount);

                // Call
                IntegrationTestHelper.ImportFailureMechanismSections(assessmentSection, assessmentSection.GrassCoverErosionInwards);
                assessmentSection.GrassCoverErosionInwards.NotifyObservers();

                // Assert
                Assert.AreEqual(283, dataGridView.RowCount);

                var expectedValues = assessmentSection.GrassCoverErosionInwards.SectionResults.Select(sr => sr.Section.Name);
                var foundValues = (from DataGridViewRow row in dataGridView.Rows select row.Cells[0].FormattedValue.ToString()).ToList();
                CollectionAssert.AreEqual(expectedValues, foundValues);
            }
        }

        [Test]
        public void ScenariosView_GenerateCalculations_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                IntegrationTestHelper.ImportReferenceLine(assessmentSection);
                IntegrationTestHelper.ImportFailureMechanismSections(assessmentSection, assessmentSection.GrassCoverErosionInwards);

                var view = new GrassCoverErosionInwardsScenariosView()
                {
                    Data = assessmentSection.GrassCoverErosionInwards.CalculationsGroup,
                    FailureMechanism = assessmentSection.GrassCoverErosionInwards
                };
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var targetContext = new DikeProfilesContext(assessmentSection.GrassCoverErosionInwards.DikeProfiles, assessmentSection);
                var dikeProfilesImporter = new DikeProfilesImporter();
                dikeProfilesImporter.Import(targetContext, filePath);

                // Call
                foreach (var profile in assessmentSection.GrassCoverErosionInwards.DikeProfiles)
                {
                    assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
                    {
                        Name = NamingHelper.GetUniqueName(((CalculationGroup) view.Data).Children, profile.Name, c => c.Name),
                        InputParameters =
                        {
                            DikeProfile = profile
                        }
                    });
                }
                assessmentSection.GrassCoverErosionInwards.CalculationsGroup.NotifyObservers();

                // Assert
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items.Count);
                Assert.AreEqual("<geen>", ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items[0].ToString());
                Assert.AreEqual("profiel63p1ID", ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items[1].ToString());
            }
        }

        [Test]
        public void ScenariosView_RenameCalculations_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                IntegrationTestHelper.ImportReferenceLine(assessmentSection);
                IntegrationTestHelper.ImportFailureMechanismSections(assessmentSection, assessmentSection.GrassCoverErosionInwards);

                var view = new GrassCoverErosionInwardsScenariosView()
                {
                    Data = assessmentSection.GrassCoverErosionInwards.CalculationsGroup,
                    FailureMechanism = assessmentSection.GrassCoverErosionInwards
                };
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var targetContext = new DikeProfilesContext(assessmentSection.GrassCoverErosionInwards.DikeProfiles, assessmentSection);
                var dikeProfilesImporter = new DikeProfilesImporter();
                dikeProfilesImporter.Import(targetContext, filePath);

                foreach (var profile in assessmentSection.GrassCoverErosionInwards.DikeProfiles)
                {
                    assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
                    {
                        Name = NamingHelper.GetUniqueName(assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children, profile.Name, c => c.Name),
                        InputParameters =
                        {
                            DikeProfile = profile
                        }
                    });
                }
                assessmentSection.GrassCoverErosionInwards.CalculationsGroup.NotifyObservers();

                // Call
                foreach (var calculationBase in assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children)
                {
                    var calculation = (GrassCoverErosionInwardsCalculation) calculationBase;
                    calculation.Name += "_changed";
                }

                // Assert
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items.Count);
                Assert.AreEqual("<geen>", ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items[0].ToString());
                Assert.AreEqual("profiel63p1ID_changed", ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items[1].ToString());
            }
        }

        [Test]
        public void ScenariosView_ChangeDikeProfileOfCalculation_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                IntegrationTestHelper.ImportReferenceLine(assessmentSection);
                IntegrationTestHelper.ImportFailureMechanismSections(assessmentSection, assessmentSection.GrassCoverErosionInwards);

                var view = new GrassCoverErosionInwardsScenariosView()
                {
                    Data = assessmentSection.GrassCoverErosionInwards.CalculationsGroup,
                    FailureMechanism = assessmentSection.GrassCoverErosionInwards
                };
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var targetContext = new DikeProfilesContext(assessmentSection.GrassCoverErosionInwards.DikeProfiles, assessmentSection);
                var dikeProfilesImporter = new DikeProfilesImporter();
                dikeProfilesImporter.Import(targetContext, filePath);

                foreach (var profile in assessmentSection.GrassCoverErosionInwards.DikeProfiles)
                {
                    assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
                    {
                        Name = NamingHelper.GetUniqueName(assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children, profile.Name, c => c.Name),
                        InputParameters =
                        {
                            DikeProfile = profile
                        }
                    });
                }

                // Call
                var calculationsGroup = assessmentSection.GrassCoverErosionInwards.CalculationsGroup;
                ((GrassCoverErosionInwardsCalculation) calculationsGroup.Children[1]).InputParameters.DikeProfile =
                    ((GrassCoverErosionInwardsCalculation) calculationsGroup.Children[0]).InputParameters.DikeProfile;
                calculationsGroup.NotifyObservers();

                // Assert
                Assert.AreEqual(3, ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items.Count);
                Assert.AreEqual("<geen>", ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items[0].ToString());
                Assert.AreEqual("profiel63p1ID", ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items[1].ToString());
                Assert.AreEqual("profiel63p2ID", ((DataGridViewComboBoxCell) dataGridView.Rows[13].Cells[1]).Items[2].ToString());
            }
        }
    }
}