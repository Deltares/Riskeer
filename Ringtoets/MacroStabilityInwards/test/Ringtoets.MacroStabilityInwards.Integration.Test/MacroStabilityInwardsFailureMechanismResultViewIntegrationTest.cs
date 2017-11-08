﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.TestUtils;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.Service;

namespace Ringtoets.MacroStabilityInwards.Integration.Test
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismResultViewIntegrationTest
    {
        private const int assessmentLayerTwoAIndex = 2;

        [Test]
        public void FailureMechanismResultView_DataImportedOrChanged_ChangesCorrectlyObservedAndSynced()
        {
            using (var form = new Form())
            {
                // Show the view
                var failureMechanismResultView = new MacroStabilityInwardsFailureMechanismResultView();
                form.Controls.Add(failureMechanismResultView);
                form.Show();

                // Obtain the data grid view
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Set all necessary data to the view
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                failureMechanismResultView.Data = assessmentSection.MacroStabilityInwards.SectionResults;
                failureMechanismResultView.FailureMechanism = assessmentSection.MacroStabilityInwards;

                // Import failure mechanism sections and ensure the data grid view is updated
                DataImportHelper.ImportReferenceLine(assessmentSection);
                IFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                Assert.AreEqual(283, dataGridView.Rows.Count);
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden geselecteerd.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Import surface lines
                DataImportHelper.ImportMacroStabilityInwardsSurfaceLines(assessmentSection);

                // Setup some calculations
                var calculation1 = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(
                            sl => sl.Name == "PK001_0001")
                    }
                };
                var calculation2 = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(
                            sl => sl.Name == "PK001_0001")
                    }
                };

                // Add a calculation and ensure it is shown in the data grid view
                assessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(calculation1);
                assessmentSection.MacroStabilityInwards.CalculationsGroup.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Add group and ensure the data grid view is not changed
                var nestedCalculationGroup = new CalculationGroup("New group", false);
                assessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(nestedCalculationGroup);
                assessmentSection.MacroStabilityInwards.CalculationsGroup.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Add another, nested calculation and ensure the data grid view is updated
                nestedCalculationGroup.Children.Add(calculation2);
                nestedCalculationGroup.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Set the second calculation to not relevant and ensure the data grid view is updated
                calculation2.IsRelevant = false;
                calculation2.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Execute the first calculation and ensure the data grid view is updated
                const double probability = 1.0 / 31846382.0;
                calculation1.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
                calculation1.SemiProbabilisticOutput = MacroStabilityInwardsSemiProbabilisticOutputTestFactory.CreateOutput(probability);
                calculation1.NotifyObservers();
                Assert.AreEqual($"1/{1.0 / calculation1.Probability:N0}",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.IsEmpty(dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Add another, nested calculation without surface line and ensure the data grid view is updated when the surface line is set
                var calculation3 = new MacroStabilityInwardsCalculationScenario();
                nestedCalculationGroup.Children.Add(calculation3);
                nestedCalculationGroup.NotifyObservers();
                Assert.AreEqual($"1/{1.0 / calculation1.Probability:N0}",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.IsEmpty(dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                calculation3.InputParameters.SurfaceLine = assessmentSection.MacroStabilityInwards.SurfaceLines.First(
                    sl => sl.Name == "PK001_0001");
                calculation3.InputParameters.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Change the contribution of the calculation and make sure the data grid view is updated
                calculation3.Contribution = (RoundedDouble) 0.3;
                calculation3.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                calculation1.Contribution = (RoundedDouble) 0.7;
                calculation1.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Remove a calculation and make sure the data grid view is updated
                nestedCalculationGroup.Children.Remove(calculation3);
                nestedCalculationGroup.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Set contribution again so we have a probability.
                calculation1.Contribution = (RoundedDouble) 1.0;
                calculation1.NotifyObservers();
                Assert.AreEqual($"1/{1.0 / calculation1.Probability:N0}",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.IsEmpty(dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Clear the output of the calculation and make sure the data grid view is updated
                MacroStabilityInwardsDataSynchronizationService.ClearCalculationOutput(calculation1);
                calculation1.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);
            }
        }
    }
}