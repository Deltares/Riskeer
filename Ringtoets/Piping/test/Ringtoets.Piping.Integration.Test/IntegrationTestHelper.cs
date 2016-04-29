using System.IO;
using Core.Common.Base.Service;
using Core.Common.Utils.IO;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Integration.Test
{
    /// <summary>
    /// Helper methods related to integration tests.
    /// </summary>
    public static class IntegrationTestHelper
    {
        /// <summary>
        /// Imports the reference line on the <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import the reference line on.</param>
        public static void ImportReferenceLine(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(IntegrationTestHelper).Assembly,
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

        /// <summary>
        /// Imports the failure mechanism sections.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to import on.</param>
        public static void ImportFailureMechanismSections(AssessmentSection assessmentSection, IFailureMechanism failureMechanism)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(IntegrationTestHelper).Assembly,
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

        /// <summary>
        /// Imports the hydraulic boundary database.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        public static void ImportHydraulicBoundaryDatabase(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(IntegrationTestHelper).Assembly,
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

        /// <summary>
        /// Imports the surface lines.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        public static void ImportSurfaceLines(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(IntegrationTestHelper).Assembly,
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

        /// <summary>
        /// Imports the soil profiles.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to import on.</param>
        public static void ImportSoilProfiles(AssessmentSection assessmentSection)
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(IntegrationTestHelper).Assembly,
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