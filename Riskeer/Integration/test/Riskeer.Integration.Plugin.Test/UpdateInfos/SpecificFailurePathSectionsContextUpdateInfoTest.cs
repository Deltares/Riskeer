using System.Drawing;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Integration.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class SpecificFailurePathSectionsContextUpdateInfoTest
    {
        [Test]
        public void CreateFileImporter_Always_ReturnFileImporter()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.ReferenceLine).Return(new ReferenceLine());
            mocks.ReplayAll();

            var failurePath = new SpecificFailurePath();
            var importTarget = new SpecificFailurePathSectionsContext(failurePath, assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                IFileImporter importer = updateInfo.CreateFileImporter(importTarget, "test");

                // Assert
                Assert.IsInstanceOf<FailureMechanismSectionsImporter>(importer);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                string name = updateInfo.Name;

                // Assert
                Assert.AreEqual("Vakindeling", name);
            }
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                string category = updateInfo.Category;

                // Assert
                Assert.AreEqual("Algemeen", category);
            }
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                Image image = updateInfo.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, image);
            }
        }

        [Test]
        public void IsEnabled_SourcePathSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePath = new SpecificFailurePath();
            failurePath.SetSections(Enumerable.Empty<FailureMechanismSection>(),
                                    "path");

            var context = new SpecificFailurePathSectionsContext(failurePath, assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                bool isEnabled = updateInfo.IsEnabled(context);

                // Assert
                Assert.IsTrue(isEnabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_SourcePathNotSet_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failurePath = new SpecificFailurePath();
            var context = new SpecificFailurePathSectionsContext(failurePath, assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                bool isEnabled = updateInfo.IsEnabled(context);

                // Assert
                Assert.IsFalse(isEnabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void FileFilterGenerator_Always_ReturnExpectedFileFilter()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                FileFilterGenerator fileFilterGenerator = updateInfo.FileFilterGenerator;

                // Assert
                Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilterGenerator.Filter);
            }
        }

        [Test]
        public void CurrentPath_FailureMechanismSectionsHasPathSet_ReturnsExpectedPath()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();


            const string expectedFilePath = "path";
            var failurePath = new SpecificFailurePath();
            failurePath.SetSections(Enumerable.Empty<FailureMechanismSection>(),
                                    expectedFilePath);

            var context = new SpecificFailurePathSectionsContext(failurePath, assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                UpdateInfo updateInfo = GetUpdateInfo(plugin);

                // Call
                string currentFilePath = updateInfo.CurrentPath(context);

                // Assert
                Assert.AreEqual(expectedFilePath, currentFilePath);
                mocks.VerifyAll();
            }
        }

        private static UpdateInfo GetUpdateInfo(RiskeerPlugin plugin)
        {
            return plugin.GetUpdateInfos().First(ui => ui.DataType == typeof(SpecificFailurePathSectionsContext));
        }
    }
}