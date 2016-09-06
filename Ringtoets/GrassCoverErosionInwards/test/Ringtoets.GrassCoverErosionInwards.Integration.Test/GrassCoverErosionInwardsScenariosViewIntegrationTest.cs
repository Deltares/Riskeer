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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
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
                GrassCoverErosionInwardsScenariosView view = CreateView();
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
                CalculationGroup calculationGroup = (CalculationGroup)view.Data;

                // Call
                calculationGroup.NotifyObservers();

                // Assert
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[1];
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridViewCell).Items.Count);
                Assert.AreEqual("<geen>", ((DataGridViewComboBoxCell) dataGridViewCell).Items[0].ToString());
                Assert.AreEqual("calculation1", ((DataGridViewComboBoxCell) dataGridViewCell).Items[1].ToString());
            }
        }

        [Test]
        public void ScenariosView_RenameCalculations_ChangesCorrectlyObservedAndSynced()
        {
            // Setup
            using (var form = new Form())
            {
                GrassCoverErosionInwardsScenariosView view = CreateView();
                form.Controls.Add(view);
                form.Show();

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                CalculationGroup calculationGroup = (CalculationGroup) view.Data;
                calculationGroup.NotifyObservers();

                // Call
                foreach (var calculationBase in calculationGroup.Children)
                {
                    var calculation = (GrassCoverErosionInwardsCalculation) calculationBase;
                    calculation.Name += "_changed";
                }

                // Assert
                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[1];
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridViewCell).Items.Count);
                Assert.AreEqual("<geen>", ((DataGridViewComboBoxCell) dataGridViewCell).Items[0].ToString());
                Assert.AreEqual("calculation1_changed", ((DataGridViewComboBoxCell) dataGridViewCell).Items[1].ToString());
            }
        }

        private static GrassCoverErosionInwardsScenariosView CreateView()
        {
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("section1", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 10.0)
            }));

            var dikeProfile = new DikeProfile(new Point2D(0.0, 0.0), new RoughnessPoint[0], new Point2D[0], null, new DikeProfile.ConstructionProperties
            {
                Name = "profile1"
            });

            CalculationGroup calculationGroup = failureMechanism.CalculationsGroup;
            calculationGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "calculation1",
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            });

            var view = new GrassCoverErosionInwardsScenariosView()
            {
                Data = calculationGroup,
                FailureMechanism = failureMechanism
            };
            return view;
        }
    }
}