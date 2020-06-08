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
        private const int isRelevantColumnIndex = 0;
        private const int contributionColumnIndex = 1;
        private const int nameColumnIndex = 2;
        private const int failureProbabilityColumnIndex = 3;

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
                                                                     assessmentSection.GrassCoverErosionInwards,
                                                                     assessmentSection);
                form.Controls.Add(view);
                form.Show();

                var listBox = (ListBox) new ControlTester("listBox").TheObject;

                // Precondition
                CollectionAssert.IsEmpty(listBox.Items);

                // Call
                IFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                assessmentSection.GrassCoverErosionInwards.NotifyObservers();

                // Assert
                CollectionAssert.AreEqual(assessmentSection.GrassCoverErosionInwards.Sections, listBox.Items);
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
                                                                     assessmentSection.GrassCoverErosionInwards,
                                                                     assessmentSection);
                form.Controls.Add(view);
                form.Show();

                var dikeProfilesImporter = new DikeProfilesImporter(assessmentSection.GrassCoverErosionInwards.DikeProfiles,
                                                                    assessmentSection.ReferenceLine,
                                                                    filePath, new GrassCoverErosionInwardsDikeProfileReplaceDataStrategy(
                                                                        (GrassCoverErosionInwardsFailureMechanism) failureMechanism),
                                                                    messageProvider);
                dikeProfilesImporter.Import();

                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                listBox.SelectedItem = failureMechanism.Sections.ElementAt(13);

                // Precondition
                DataGridViewRowCollection rows = dataGridView.Rows;
                CollectionAssert.IsEmpty(rows);

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
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.IsTrue(Convert.ToBoolean(cells[isRelevantColumnIndex].FormattedValue));
                Assert.AreEqual(new RoundedDouble(2, 100).ToString(), cells[contributionColumnIndex].FormattedValue);
                Assert.AreEqual("profiel63p1Naam", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("-", cells[failureProbabilityColumnIndex].FormattedValue);

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
                                                                     assessmentSection.GrassCoverErosionInwards,
                                                                     assessmentSection);
                form.Controls.Add(view);
                form.Show();

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

                var listBox = (ListBox)new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                listBox.SelectedItem = failureMechanism.Sections.ElementAt(13);

                // Precondition
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual("profiel63p1Naam", cells[nameColumnIndex].FormattedValue);

                // Call
                foreach (GrassCoverErosionInwardsCalculationScenario calculation in calculationsGroup.Children.Cast<GrassCoverErosionInwardsCalculationScenario>())
                {
                    calculation.Name += "_changed";
                }

                // Assert
                Assert.AreEqual("profiel63p1Naam_changed", cells[nameColumnIndex].FormattedValue);

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
                                                                     assessmentSection.GrassCoverErosionInwards,
                                                                     assessmentSection);
                form.Controls.Add(view);
                form.Show();

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
                
                var listBox = (ListBox)new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                listBox.SelectedItem = failureMechanism.Sections.ElementAt(13);

                // Precondition
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);
                Assert.AreEqual("profiel63p1NaamCalculation", rows[0].Cells[nameColumnIndex].FormattedValue);

                // Call
                CalculationGroup calculationsGroup = assessmentSection.GrassCoverErosionInwards.CalculationsGroup;
                ((GrassCoverErosionInwardsCalculationScenario) calculationsGroup.Children[1]).InputParameters.DikeProfile =
                    ((GrassCoverErosionInwardsCalculationScenario) calculationsGroup.Children[0]).InputParameters.DikeProfile;
                calculationsGroup.NotifyObservers();

                // Assert
                Assert.AreEqual(2, rows.Count);
                Assert.AreEqual("profiel63p1NaamCalculation", rows[0].Cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual("profiel63p2NaamCalculation", rows[1].Cells[nameColumnIndex].FormattedValue);

                mocks.VerifyAll();
            }
        }
    }
}