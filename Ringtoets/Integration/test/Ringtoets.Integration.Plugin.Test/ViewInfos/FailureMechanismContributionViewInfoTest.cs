using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class FailureMechanismContributionViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(FailureMechanismContributionView));
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
            Assert.AreEqual(typeof(FailureMechanismContribution), info.DataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 1000);
            var viewMock = mocks.StrictMock<FailureMechanismContributionView>();

            mocks.ReplayAll();

            // Call
            var viewName = info.GetViewName(viewMock, failureMechanismContribution);

            // Assert
            Assert.AreEqual("Faalkansverdeling", viewName);
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismContributionView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismContribution), dataType);
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
            var viewMock = mocks.StrictMock<FailureMechanismContributionView>();
            var assessmentSection = new TestAssessmentSectionBase();

            viewMock.Expect(vm => vm.Data).Return(assessmentSection.FailureMechanismContribution);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSection);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewMock = mocks.StrictMock<FailureMechanismContributionView>();
            var assessmentSection = new TestAssessmentSectionBase();
            var assessmentSection2 = new TestAssessmentSectionBase();

            viewMock.Expect(vm => vm.Data).Return(assessmentSection2.FailureMechanismContribution);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(viewMock, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
        }

        private class TestAssessmentSectionBase : Observable, IAssessmentSection
        {
            public TestAssessmentSectionBase()
            {
                FailureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 1000);
            }

            public string Name { get; set; }
            public AssessmentSectionComment Comments { get; private set; }
            public ReferenceLine ReferenceLine { get; set; }
            public FailureMechanismContribution FailureMechanismContribution { get; private set; }
            public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }
            public IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                throw new System.NotImplementedException();
            }

            public long StorageId { get; set; }
        }
    }
}