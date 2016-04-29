using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Integration.Test
{
    [TestFixture]
    public class FailureMechanismResultViewIntegrationTest
    {
        [Test]
        public void FailureMechanismResultView_DataImportedOrChanged_ChangesCorrectlyObservedAndSynced()
        {
            using (var form = new Form())
            {
                // Show the view
                var failureMechanismResultView = new FailureMechanismResultView();
                form.Controls.Add(failureMechanismResultView);
                form.Show();

                // Obtain the data grid view
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                // Set all necessary data to the view
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                failureMechanismResultView.Data = assessmentSection.PipingFailureMechanism.SectionResults;
                failureMechanismResultView.FailureMechanism = assessmentSection.PipingFailureMechanism;

                // Import failure mechanisn sections and ensure the data grid view is updated
                IntegrationTestHelper.ImportReferenceLine(assessmentSection);
                IntegrationTestHelper.ImportFailureMechanismSections(assessmentSection, assessmentSection.PipingFailureMechanism);
                Assert.AreEqual(283, dataGridView.Rows.Count);
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(string.Empty, dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Import surface lines
                IntegrationTestHelper.ImportSurfaceLines(assessmentSection);

                // Setup some calculations
                var pipingCalculation1 = new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };
                var pipingCalculation2 = new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };

                // Add a piping calculation and ensure it is shown in the data grid view;
                assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation1);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.NotifyObservers();
                assessmentSection.PipingFailureMechanism.CalculationsGroup.AddCalculationScenariosToFailureMechanismSectionResult(assessmentSection.PipingFailureMechanism);
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Niet alle berekeningen voor dit vak zijn uitgevoerd.", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Add group and ensure the data grid view is not changed
                var nestedPipingCalculationGroup = new CalculationGroup("New group", false);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(nestedPipingCalculationGroup);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Niet alle berekeningen voor dit vak zijn uitgevoerd.", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Add another, nested calculation and ensure the data grid view is updated
                nestedPipingCalculationGroup.Children.Add(pipingCalculation2);
                nestedPipingCalculationGroup.NotifyObservers();
                assessmentSection.PipingFailureMechanism.CalculationsGroup.AddCalculationScenariosToFailureMechanismSectionResult(assessmentSection.PipingFailureMechanism);
                Assert.AreEqual(double.NaN.ToString(CultureInfo.InvariantCulture), dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak zijn opgeteld niet gelijk aan 100%.", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Set the second calculation to not relevant and ensure the data grid view is updated
                pipingCalculation2.IsRelevant = false;
                pipingCalculation2.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Niet alle berekeningen voor dit vak zijn uitgevoerd.", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Execute the first calculation and ensure the data grid view is updated
                const double probability = 31846382;
                pipingCalculation1.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, probability, 0, 0);
                pipingCalculation1.NotifyObservers();
                Assert.AreEqual(string.Format("1/{0:N0}", pipingCalculation1.Probability), dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(string.Empty, dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Add another, nested calculation without surface line and ensure the data grid view is updated when the surface line is set.
                var pipingCalculation3 = new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput());
                nestedPipingCalculationGroup.Children.Add(pipingCalculation3);
                nestedPipingCalculationGroup.NotifyObservers();
                assessmentSection.PipingFailureMechanism.CalculationsGroup.AddCalculationScenariosToFailureMechanismSectionResult(assessmentSection.PipingFailureMechanism);
                Assert.AreEqual(string.Format("1/{0:N0}", pipingCalculation1.Probability), dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(string.Empty, dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                pipingCalculation3.InputParameters.SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001");
                PipingCalculationScenarioService.SyncCalculationScenarioWithNewSurfaceLine(pipingCalculation3, assessmentSection.PipingFailureMechanism, null);
                pipingCalculation3.InputParameters.NotifyObservers();
                Assert.AreEqual(double.NaN.ToString(CultureInfo.InvariantCulture), dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak zijn opgeteld niet gelijk aan 100%.", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                // Change the contribution of the calculation and make sure the data grid view is updated
                pipingCalculation3.Contribution = (RoundedDouble) 0.3;
                pipingCalculation3.NotifyObservers();
                Assert.AreEqual(double.NaN.ToString(CultureInfo.InvariantCulture), dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak zijn opgeteld niet gelijk aan 100%.", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);

                pipingCalculation1.Contribution = (RoundedDouble) 0.7;
                pipingCalculation1.NotifyObservers();
                Assert.AreEqual("-", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("Niet alle berekeningen voor dit vak zijn uitgevoerd.", dataGridView.Rows[22].Cells[assessmentLayerTwoAIndex].ErrorText);
            }
        }

        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerTwoBIndex = 3;
        private const int assessmentLayerThreeIndex = 4;
    }
}