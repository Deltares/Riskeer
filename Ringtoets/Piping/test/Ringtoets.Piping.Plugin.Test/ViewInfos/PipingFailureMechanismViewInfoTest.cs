using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingFailureMechanismView));
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
            Assert.AreEqual(typeof(PipingFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(PipingFailureMechanismContext), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsTextFromResources()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismView>();

            mocks.ReplayAll();

            // Call & Assert
            Assert.AreEqual(PipingDataResources.PipingFailureMechanism_DisplayName, info.GetViewName(viewMock, null));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismView>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var otherAssessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingFailureMechanismContextMock = mocks.StrictMock<PipingFailureMechanismContext>(pipingFailureMechanismMock, assessmentSectionMock);

            viewMock.Expect(vm => vm.Data).Return(pipingFailureMechanismContextMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(viewMock, otherAssessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismView>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingFailureMechanismContextMock = mocks.StrictMock<PipingFailureMechanismContext>(pipingFailureMechanismMock, assessmentSectionMock);

            viewMock.Expect(vm => vm.Data).Return(pipingFailureMechanismContextMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.CloseForData(viewMock, assessmentSectionMock));
        }
    }
}
