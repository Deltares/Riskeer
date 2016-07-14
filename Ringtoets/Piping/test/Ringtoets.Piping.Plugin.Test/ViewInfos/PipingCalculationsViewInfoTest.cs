using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingCalculationsViewInfoTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingCalculationsView));
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
            Assert.AreEqual(typeof(PipingCalculationGroupContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, info.Image);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingCalculationsGroupMock, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<StochasticSoilModel>(), pipingFailureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.AreEqual(pipingCalculationsGroupMock, info.GetViewData(pipingCalculationGroupContext));
        }

        [Test]
        public void GetViewName_Always_ReturnsCalculationGroupName()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingCalculationsView>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();

            mocks.ReplayAll();

            pipingCalculationsGroupMock.Name = "Test";

            // Call & Assert
            Assert.AreEqual("Test", info.GetViewName(viewMock, pipingCalculationsGroupMock));
        }

        [Test]
        public void AdditionalDataCheck_PipingCalculationGroupContextWithPipingFailureMechanismParent_ReturnsTrue()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingFailureMechanismMock.CalculationsGroup, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<StochasticSoilModel>(), pipingFailureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.AdditionalDataCheck(pipingCalculationGroupContext));
        }

        [Test]
        public void AdditionalDataCheck_PipingCalculationGroupContextWithoutPipingFailureMechanismParent_ReturnsFalse()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.Stub<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingCalculationsGroupMock, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<StochasticSoilModel>(), pipingFailureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.AdditionalDataCheck(pipingCalculationGroupContext));
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutPipingFailureMechanism_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingCalculationsView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();

            viewMock.Expect(vm => vm.Data).Return(pipingCalculationsGroupMock);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingCalculationsView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.Stub<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();

            viewMock.Expect(vm => vm.Data).Return(pipingCalculationsGroupMock);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                pipingFailureMechanismMock
            });

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingCalculationsView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            viewMock.Expect(vm => vm.Data).Return(pipingFailureMechanismMock.CalculationsGroup);
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                pipingFailureMechanismMock
            });

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.CloseForData(viewMock, assessmentSectionMock));
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var view = new PipingCalculationsView();
            var failureMechanism = new PipingFailureMechanism();

            view.Data = new CalculationGroup();

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var view = new PipingCalculationsView();
            var failureMechanism = new PipingFailureMechanism();

            view.Data = failureMechanism.CalculationsGroup;

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var view = new PipingCalculationsView();
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSectionMock);

            view.Data = failureMechanism.CalculationsGroup;

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var view = new PipingCalculationsView();
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSectionMock);

            view.Data = failureMechanism.CalculationsGroup;

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingCalculationsView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<CalculationGroup>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingCalculationsGroupMock, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<StochasticSoilModel>(), pipingFailureMechanismMock, assessmentSectionMock);

            viewMock.Expect(v => v.AssessmentSection = assessmentSectionMock);
            viewMock.Expect(v => v.PipingFailureMechanism = pipingFailureMechanismMock);
            viewMock.Expect(v => v.ApplicationSelection = plugin.Gui);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, pipingCalculationGroupContext);

            // Assert
            mocks.VerifyAll();
        }
    }
}