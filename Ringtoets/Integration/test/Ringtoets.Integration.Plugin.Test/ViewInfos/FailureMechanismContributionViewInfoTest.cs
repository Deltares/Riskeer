using System.Linq;

using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Forms.PresentationObjects;
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
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var view = new FailureMechanismContributionView();

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 1000);

            var context = new FailureMechanismContributionContext(failureMechanismContribution, assessmentSection);

            // Call
            var viewName = info.GetViewName(view, context);

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
            Assert.AreEqual(typeof(FailureMechanismContributionContext), dataType);
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
            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 123456);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution)
                             .Return(contribution);
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);
            mocks.ReplayAll();

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);

            var view = new FailureMechanismContributionView
            {
                Data = context
            };

            // Call
            var closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var contribution1 = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 123456);
            var contribution2 = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 789123);

            var assessmentSection1 = mocks.Stub<IAssessmentSection>();
            assessmentSection1.Stub(section => section.FailureMechanismContribution)
                              .Return(contribution1);
            assessmentSection1.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.DikeAndDune);
            var assessmentSection2 = mocks.Stub<IAssessmentSection>();
            assessmentSection2.Stub(section => section.FailureMechanismContribution)
                              .Return(contribution2);
            assessmentSection2.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.DikeAndDune);
            mocks.ReplayAll();


            var context = new FailureMechanismContributionContext(contribution1, assessmentSection1);

            var view = new FailureMechanismContributionView
            {
                Data = context
            };

            // Call
            var closeForData = info.CloseForData(view, assessmentSection2);

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewWithoutData_ReturnsFalse()
        {
            // Setup
            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 789123);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution)
                              .Return(contribution);
            mocks.ReplayAll();

            var view = new FailureMechanismContributionView();

            // Call
            var closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
        }
    }
}