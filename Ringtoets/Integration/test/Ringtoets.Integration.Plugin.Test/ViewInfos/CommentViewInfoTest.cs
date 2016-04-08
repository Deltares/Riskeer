using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class CommentViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(CommentView));
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
            var commentMock = mocks.StrictMock<IComment>();
            var viewMock = mocks.StrictMock<CommentView>();

            mocks.ReplayAll();

            // Call
            var viewName = info.GetViewName(viewMock, commentMock);

            // Assert
            Assert.AreEqual("Opmerkingen", viewName);
        }

        [Test]
        public void GetViewData_Always_ReturnsIComment()
        {
            // Setup
            var commentMock = mocks.StrictMock<IComment>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var contextMock = mocks.StrictMock<CommentContext<IComment>>(commentMock, assessmentSectionMock);
            mocks.ReplayAll();

            // Call
            var viewData = info.GetViewData(contextMock);

            // Assert
            Assert.AreSame(commentMock, viewData);
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(CommentView), viewType);
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
            var viewMock = mocks.StrictMock<CommentView>();
            var commentMock = mocks.Stub<IComment>();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();

            viewMock.Expect(vm => vm.Data).Return(commentMock);
            viewMock.Expect(vm => vm.AssessmentSection).Return(assessmentSectionMock);

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
            var viewMock = mocks.StrictMock<CommentView>();
            var commentMock = mocks.Stub<IComment>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var assessmentSectionMock2 = mocks.StrictMock<IAssessmentSection>();

            viewMock.Expect(vm => vm.Data).Return(commentMock);
            viewMock.Expect(vm => vm.AssessmentSection).Return(assessmentSectionMock2);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSectionMock);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculationItem_ReturnsTrue()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var viewMock = mocks.StrictMock<CommentView>();
            var calculationContextMock = mocks.StrictMock<PipingCalculationContext>(calculation, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<StochasticSoilModel>(), pipingFailureMechanismMock, assessmentSectionMock);

            viewMock.Expect(vm => vm.Data).Return(calculationContextMock.WrappedData);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, calculationContextMock);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationItem_ReturnsFalse()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var calculation2 = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var viewMock = mocks.StrictMock<CommentView>();
            var calculationContextMock = mocks.StrictMock<PipingCalculationContext>(calculation, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<StochasticSoilModel>(), pipingFailureMechanismMock, assessmentSectionMock);
            var calculationContextMock2 = mocks.StrictMock<PipingCalculationContext>(calculation2, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<StochasticSoilModel>(), pipingFailureMechanismMock, assessmentSectionMock);

            viewMock.Expect(vm => vm.Data).Return(calculationContextMock2.WrappedData);
            viewMock.Expect(vm => vm.AssessmentSection).Return(assessmentSectionMock);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, calculationContextMock);

            // Assert
            Assert.IsFalse(closeForData);
        }
    }
}