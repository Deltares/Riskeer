using System.Drawing;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.ImportInfos
{
    [TestFixture]
    public class DikeProfilesContextImportInfoTest
    {
        private ImportInfo importInfo;
        private GrassCoverErosionInwardsPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionInwardsPlugin();
            importInfo = plugin.GetImportInfos().First(i => i.DataType == typeof(DikeProfilesContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Name_Always_ReturnExpectedName()
        {
            // Call
            string name = importInfo.Name;

            // Assert
            Assert.AreEqual("Dijkprofiel locaties", name);
        }

        [Test]
        public void Category_Always_ReturnExpectedCategory()
        {
            // Call
            string category = importInfo.Category;

            // Assert
            Assert.AreEqual("Algemeen", category);
        }

        [Test]
        public void Image_Always_ReturnExpectedIcon()
        {
            // Call
            Image image = importInfo.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.DikeProfile, image);
        }

        [Test]
        public void IsEnabled_ReferenceLineSet_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = new ReferenceLine();
            mocks.ReplayAll();

            var list = new ObservableList<DikeProfile>();

            var context = new DikeProfilesContext(list, assessmentSection);

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsTrue(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void IsEnabled_ReferenceLineNotSet_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = null;
            mocks.ReplayAll();

            var list = new ObservableList<DikeProfile>();

            var context = new DikeProfilesContext(list, assessmentSection);

            // Call
            bool isEnabled = importInfo.IsEnabled(context);

            // Assert
            Assert.IsFalse(isEnabled);
            mocks.VerifyAll();
        }

        [Test]
        public void FileFilter_Always_ReturnExpectedFileFilter()
        {
            // Call
            string fileFilter = importInfo.FileFilter;

            // Assert
            Assert.AreEqual("Shapebestand (*.shp)|*.shp", fileFilter);
        }

        [Test]
        public void CreateFileImporter_ValidInput_SuccessfulImport()
        {
            // Setup
            var mocks = new MockRepository();
            ReferenceLine referenceLine = CreateMatchingReferenceLine();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.ReferenceLine = referenceLine;
            mocks.ReplayAll();

            var list = new ObservableList<DikeProfile>();

            string path = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                     Path.Combine("DikeProfiles", "AllOkTestData", "Voorlanden 12-2.shp"));

            var importTarget = new DikeProfilesContext(list, assessmentSection);

            // Call
            IFileImporter importer = importInfo.CreateFileImporter(importTarget, path);

            // Assert
            Assert.IsTrue(importer.Import());

            mocks.VerifyAll();
        }

        private ReferenceLine CreateMatchingReferenceLine()
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(131223.2, 548393.4),
                new Point2D(133854.3, 545323.1),
                new Point2D(135561.0, 541920.3),
                new Point2D(136432.1, 538235.2),
                new Point2D(136039.4, 533920.2)
            });
            return referenceLine;
        }
    }
}