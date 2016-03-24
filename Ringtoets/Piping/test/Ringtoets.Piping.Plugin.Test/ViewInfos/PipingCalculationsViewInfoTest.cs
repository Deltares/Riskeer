using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingCalculationsViewInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
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
            Assert.AreEqual(typeof(PipingCalculationGroup), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.FolderIcon, info.Image);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<PipingCalculationGroup>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingCalculationsGroupMock, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<PipingSoilProfile>(), pipingFailureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.AreEqual(pipingCalculationsGroupMock, info.GetViewData(pipingCalculationGroupContext));
        }

        [Test]
        public void GetViewName_Always_ReturnsCalculationGroupName()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingCalculationsView>();
            var pipingCalculationsGroupMock = mocks.StrictMock<PipingCalculationGroup>();

            mocks.ReplayAll();

            pipingCalculationsGroupMock.Name = "Test";

            // Call & Assert
            Assert.AreEqual("Test", info.GetViewName(viewMock, pipingCalculationsGroupMock));
        }

        [Test]
        public void AdditionalDataCheck_PipingCalculationGroupContextWithPipingFailureMechanimsParent_ReturnsTrue()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingFailureMechanismMock.CalculationsGroup, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<PipingSoilProfile>(), pipingFailureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsTrue(info.AdditionalDataCheck(pipingCalculationGroupContext));
        }

        [Test]
        public void AdditionalDataCheck_PipingCalculationGroupContextWithoutPipingFailureMechanimsParent_ReturnsFalse()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<PipingCalculationGroup>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingCalculationsGroupMock, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<PipingSoilProfile>(), pipingFailureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call & Assert
            Assert.IsFalse(info.AdditionalDataCheck(pipingCalculationGroupContext));
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutPipingFailureMechanism_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingCalculationsView>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingCalculationsGroupMock = mocks.StrictMock<PipingCalculationGroup>();

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
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<PipingCalculationGroup>();

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
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
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
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingCalculationsView>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var pipingCalculationsGroupMock = mocks.StrictMock<PipingCalculationGroup>();
            var pipingCalculationGroupContext = new PipingCalculationGroupContext(pipingCalculationsGroupMock, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), Enumerable.Empty<PipingSoilProfile>(), pipingFailureMechanismMock, assessmentSectionMock);

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
