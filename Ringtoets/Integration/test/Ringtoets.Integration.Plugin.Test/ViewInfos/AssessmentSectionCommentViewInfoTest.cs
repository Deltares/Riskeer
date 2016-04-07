using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class AssessmentSectionCommentViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(AssessmentSectionCommentView));
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
            Assert.AreEqual(typeof(CommentContext<IComment>), info.DataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var viewMock = mocks.StrictMock<AssessmentSectionCommentView>();

            mocks.ReplayAll();

            // Call
            var viewName = info.GetViewName(viewMock, assessmentSectionMock);

            // Assert
            Assert.AreEqual("Opmerkingen", viewName);
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(AssessmentSectionCommentView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(CommentContext<IComment>), dataType);
        }

        [Test]
        public void ViewDataType_Always_ReturnViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IComment), viewDataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<AssessmentSectionCommentView>();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();

            viewMock.Expect(vm => vm.Data).Return(assessmentSectionMock);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<AssessmentSectionCommentView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var assessmentSectionMock2 = mocks.StrictMock<IAssessmentSection>();

            viewMock.Expect(vm => vm.Data).Return(assessmentSectionMock2);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsFalse(closeForData);
        }
    }
}