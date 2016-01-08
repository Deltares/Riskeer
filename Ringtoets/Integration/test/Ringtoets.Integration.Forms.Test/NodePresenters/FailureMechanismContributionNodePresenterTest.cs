using System;
using System.Drawing;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Forms.NodePresenters;

namespace Ringtoets.Integration.Forms.Test.NodePresenters
{
    [TestFixture]
    public class FailureMechanismContributionNodePresenterTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FailureMechanismContributionNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            // Call
            var nodePresenter = new FailureMechanismContributionNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<FailureMechanismContribution>>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(FailureMechanismContribution), nodePresenter.NodeTagType);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            var projectNode = mockRepository.Stub<TreeNode>();
            var assessmentSection = mockRepository.Stub<FailureMechanismContribution>(new IFailureMechanism[] { }, 1, 1);
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new FailureMechanismContributionNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.UpdateNode(null, projectNode, assessmentSection);

            // Assert
            Assert.AreEqual(Data.Properties.Resources.FailureMechanismContribution_DisplayName, projectNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), projectNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(Properties.Resources.GenericInputOutputIcon, projectNode.Image);
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var assessmentSection = mockRepository.Stub<FailureMechanismContribution>(new IFailureMechanism[] { }, 1, 1);
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();
            var nodeMock = mockRepository.StrictMock<TreeNode>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mockRepository.ReplayAll();

            var nodePresenter = new FailureMechanismContributionNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, assessmentSection);

            // Assert
            mockRepository.VerifyAll();
        }
    }
}