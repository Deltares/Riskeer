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
using Core.Common.Base.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.TestUtils;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Integration.Test
{
    [TestFixture]
    public class PipingFailureMechanismResultViewIntegrationTest
    {
        private const int assessmentLayerTwoAIndex = 2;

        [Test]
        public void FailureMechanismResultView_DataImportedOrChanged_ChangesCorrectlyObservedAndSynced()
        {
            using (var form = new Form())
            {
                // Show the view
                var failureMechanismResultView = new PipingFailureMechanismResultView();
                form.Controls.Add(failureMechanismResultView);
                form.Show();

                // Obtain the data grid view
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Set all necessary data to the view
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                failureMechanismResultView.Data = assessmentSection.PipingFailureMechanism.SectionResults;
                failureMechanismResultView.FailureMechanism = assessmentSection.PipingFailureMechanism;

                // Import failure mechanism sections and ensure the data grid view is updated
                DataImportHelper.ImportReferenceLine(assessmentSection);
                IFailureMechanism failureMechanism = assessmentSection.PipingFailureMechanism;
                DataImportHelper.ImportFailureMechanismSections(assessmentSection, failureMechanism);
                Assert.AreEqual(283, dataGridView.Rows.Count);
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden geselecteerd.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Import surface lines
                DataImportHelper.ImportPipingSurfaceLines(assessmentSection);

                // Setup some calculations
                var pipingCalculation1 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(
                            sl => sl.Name == "PK001_0001")
                    }
                };
                var pipingCalculation2 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(
                            sl => sl.Name == "PK001_0001")
                    }
                };

                // Add a piping calculation and ensure it is shown in the data grid view
                assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation1);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Add group and ensure the data grid view is not changed
                var nestedPipingCalculationGroup = new CalculationGroup("New group", false);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(nestedPipingCalculationGroup);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Add another, nested calculation and ensure the data grid view is updated
                nestedPipingCalculationGroup.Children.Add(pipingCalculation2);
                nestedPipingCalculationGroup.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Set the second calculation to not relevant and ensure the data grid view is updated
                pipingCalculation2.IsRelevant = false;
                pipingCalculation2.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Execute the first calculation and ensure the data grid view is updated
                const double probability = 1.0 / 31846382.0;
                pipingCalculation1.Output = new TestPipingOutput();
                pipingCalculation1.SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput(probability);
                pipingCalculation1.NotifyObservers();
                Assert.AreEqual(string.Format("1/{0:N0}", 1.0 / pipingCalculation1.Probability),
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.IsEmpty(dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Add another, nested calculation without surface line and ensure the data grid view is updated when the surface line is set
                var pipingCalculation3 = new PipingCalculationScenario(new GeneralPipingInput());
                nestedPipingCalculationGroup.Children.Add(pipingCalculation3);
                nestedPipingCalculationGroup.NotifyObservers();
                Assert.AreEqual(string.Format("1/{0:N0}", 1.0 / pipingCalculation1.Probability),
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.IsEmpty(dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                pipingCalculation3.InputParameters.SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(
                    sl => sl.Name == "PK001_0001");
                pipingCalculation3.InputParameters.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Change the contribution of the calculation and make sure the data grid view is updated
                pipingCalculation3.Contribution = (RoundedDouble) 0.3;
                pipingCalculation3.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                pipingCalculation1.Contribution = (RoundedDouble) 0.7;
                pipingCalculation1.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Remove a calculation and make sure the data grid view is updated
                nestedPipingCalculationGroup.Children.Remove(pipingCalculation3);
                nestedPipingCalculationGroup.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Set contribution again so we have a probability.
                pipingCalculation1.Contribution = (RoundedDouble) 1.0;
                pipingCalculation1.NotifyObservers();
                Assert.AreEqual(string.Format("1/{0:N0}", 1.0 / pipingCalculation1.Probability),
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.IsEmpty(dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Clear the output of the calculation and make sure the data grid view is updated
                PipingDataSynchronizationService.ClearCalculationOutput(pipingCalculation1);
                pipingCalculation1.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);
            }
        }
    }
}