using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.Utils.IO;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Integration.Test
{
    [TestFixture]
    public class PipingCalculationsViewIntegrationTest
    {
        [Test]
        public void PipingCalculationsView_DataImportedOrChanged_ChangesCorrectlyObservedAndSynced()
        {
            using (var form = new Form())
            {
                // Show the view
                var pipingCalculationsView = new PipingCalculationsView();
                form.Controls.Add(pipingCalculationsView);
                form.Show();

                // Obtain some relevant controls
                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Set all necessary data to the view
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                pipingCalculationsView.Data = assessmentSection.PipingFailureMechanism.CalculationsGroup;
                pipingCalculationsView.AssessmentSection = assessmentSection;
                pipingCalculationsView.PipingFailureMechanism = assessmentSection.PipingFailureMechanism;

                // Import failure mechanism sections and ensure the listbox is updated
                ImportReferenceLine(assessmentSection);
                ImportFailureMechanismSections(assessmentSection, assessmentSection.PipingFailureMechanism);
                Assert.AreEqual(283, listBox.Items.Count);

                // Import surface lines
                ImportSurfaceLines(assessmentSection);

                // Setup some calculations
                var pipingCalculation1 = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };
                var pipingCalculation2 = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = assessmentSection.PipingFailureMechanism.SurfaceLines.First(sl => sl.Name == "PK001_0001")
                    }
                };

                // Add a piping calculation and ensure it is shown in the data grid view after selecting the corresponding dike section
                assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation1);
                listBox.SelectedItem = assessmentSection.PipingFailureMechanism.Sections.First(s => s.Name == "6-3_22");
                Assert.AreEqual(1, dataGridView.Rows.Count);

                // Import soil models and profiles and ensure the corresponding combobox items are updated
                ImportSoilProfiles(assessmentSection);
                Assert.AreEqual(2, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilModelsColumnIndex]).Items.Count);
                Assert.AreEqual(1, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex]).Items.Count);
                Assert.AreEqual("<geen>", dataGridView.Rows[0].Cells[stochasticSoilProfilesColumnIndex].FormattedValue);

                // Import hydraulic boundary locations and ensure the corresponding combobox items are updated
                ImportHydraulicBoundaryDatabase(assessmentSection);
                Assert.AreEqual(19, ((DataGridViewComboBoxCell) dataGridView.Rows[0].Cells[hydraulicBoundaryLocationsColumnIndex]).Items.Count);

                // Add another, nested calculation and ensure the data grid view is updated
                var nestedPipingCalculationGroup = new PipingCalculationGroup("New group", false);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(nestedPipingCalculationGroup);
                assessmentSection.PipingFailureMechanism.CalculationsGroup.NotifyObservers();
                Assert.AreEqual(1, dataGridView.Rows.Count);
                nestedPipingCalculationGroup.Children.Add(pipingCalculation2);
                nestedPipingCalculationGroup.NotifyObservers();
                Assert.AreEqual(2, dataGridView.Rows.Count);

                // Change the name of the first calculation and ensure the data grid view is updated
                pipingCalculation1.Name = "New name";
                pipingCalculation1.NotifyObservers();
                Assert.AreEqual("New name", dataGridView.Rows[0].Cells[nameColumnIndex].FormattedValue);

                // Change an input parameter of the second calculation and ensure the data grid view is updated
                pipingCalculation2.InputParameters.ExitPointL = (RoundedDouble) 111.11;
                pipingCalculation2.InputParameters.NotifyObservers();
                Assert.AreEqual(string.Format("{0}", 111.11), dataGridView.Rows[1].Cells[exitPointLColumnIndex].FormattedValue);
            }
        }

        private const int nameColumnIndex = 0;
        private const int stochasticSoilModelsColumnIndex = 1;
        private const int stochasticSoilProfilesColumnIndex = 2;
        private const int hydraulicBoundaryLocationsColumnIndex = 3;
        private const int dampingFactorExitMeanColumnIndex = 4;
        private const int phreaticLevelExitMeanColumnIndex = 5;
        private const int entryPointLColumnIndex = 6;
        private const int exitPointLColumnIndex = 7;

        private void ImportReferenceLine(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly,
                                                                                   true,
                                                                                   "traject_6-3.shp",
                                                                                   "traject_6-3.dbf",
                                                                                   "traject_6-3.prj",
                                                                                   "traject_6-3.shx"))
            {
                var activity = new FileImportActivity(new ReferenceLineImporter(),
                                                      new ReferenceLineContext(assessmentSection),
                                                      Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3.shp"));

                activity.Run();
                activity.Finish();
            }
        }

        private void ImportFailureMechanismSections(AssessmentSection assessmentSection, IFailureMechanism failureMechanism)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly,
                                                                                   true,
                                                                                   "traject_6-3_vakken.shp",
                                                                                   "traject_6-3_vakken.dbf",
                                                                                   "traject_6-3_vakken.prj",
                                                                                   "traject_6-3_vakken.shx"))
            {
                var activity = new FileImportActivity(new FailureMechanismSectionsImporter(),
                                                      new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                                                      Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "traject_6-3_vakken.shp"));

                activity.Run();
                activity.Finish();
            }
        }

        private void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly,
                                                                                   false,
                                                                                   "HRD dutch coast south.sqlite",
                                                                                   "HLCD.sqlite"))
            {
                using (var hydraulicBoundaryDatabaseImporter = new HydraulicBoundaryDatabaseImporter())
                {
                    var filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "HRD dutch coast south.sqlite");
                    hydraulicBoundaryDatabaseImporter.Import(assessmentSection, filePath);
                }
            }
        }

        private void ImportSurfaceLines(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly,
                                                                                   true,
                                                                                   "DR6_surfacelines.csv",
                                                                                   "DR6_surfacelines.krp.csv"))
            {
                var activity = new FileImportActivity(new PipingSurfaceLinesCsvImporter(),
                                                      new RingtoetsPipingSurfaceLinesContext(assessmentSection.PipingFailureMechanism, assessmentSection),
                                                      Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6_surfacelines.csv"));

                activity.Run();
                activity.Finish();
            }
        }

        private void ImportSoilProfiles(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(GetType().Assembly,
                                                                                   true,
                                                                                   "DR6.soil"))
            {
                var activity = new FileImportActivity(new PipingSoilProfilesImporter(),
                                                      new StochasticSoilModelContext(assessmentSection.PipingFailureMechanism, assessmentSection),
                                                      Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "DR6.soil"));

                activity.Run();
                activity.Finish();
            }
        }
    }
}