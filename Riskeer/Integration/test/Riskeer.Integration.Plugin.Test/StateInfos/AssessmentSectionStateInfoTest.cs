using System.Linq;
using Core.Common.Base.Data;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;

namespace Riskeer.Integration.Plugin.Test.StateInfos
{
    [TestFixture]
    public class AssessmentSectionStateInfoTest
    {
        private RiskeerPlugin plugin;
        private StateInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RiskeerPlugin();
            info = plugin.GetStateInfos().First(si => si.Name == "Traject");
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual("\uE94E", info.Symbol);
        }

        [Test]
        public void GetRootData_RiskeerProject_ReturnsExpectedRootData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var project = new RiskeerProject
            {
                AssessmentSections =
                {
                    assessmentSection
                }
            };

            // Call
            object rootData = info.GetRootData(project);

            // Assert
            var rootDataCollection = rootData as object[];
            Assert.IsNotNull(rootDataCollection);
            Assert.AreEqual(6, rootDataCollection.Length);
            Assert.AreEqual(new ReferenceLineContext(assessmentSection.ReferenceLine, assessmentSection), rootDataCollection[0]);
            Assert.AreEqual(new NormContext(assessmentSection.FailureMechanismContribution, assessmentSection), rootDataCollection[1]);
            Assert.AreEqual(new FailureMechanismContributionContext(assessmentSection.FailureMechanismContribution, assessmentSection), rootDataCollection[2]);
            Assert.AreEqual(new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, assessmentSection), rootDataCollection[3]);
            Assert.AreSame(assessmentSection.BackgroundData, rootDataCollection[4]);
            Assert.AreSame(assessmentSection.Comments, rootDataCollection[5]);
        }

        [Test]
        public void GetRootData_ProjectMock_ReturnsNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            var project = mockRepository.StrictMock<IProject>();

            mockRepository.ReplayAll();

            // Call
            object rootData = info.GetRootData(project);

            // Assert
            Assert.IsNull(rootData);

            mockRepository.VerifyAll();
        }
    }
}