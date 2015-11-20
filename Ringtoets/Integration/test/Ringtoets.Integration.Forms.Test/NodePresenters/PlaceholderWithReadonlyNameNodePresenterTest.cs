using System.Collections;
using System.Drawing;

using Core.Common.Controls;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Placeholder;
using Ringtoets.Integration.Forms.NodePresenters;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PlaceholderWithReadonlyNameNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<PlaceholderWithReadonlyName>>(nodePresenter);
        }

        [Test]
        public void UpdateNode_ValidNodeData_UpdateTreeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new PlaceholderWithReadonlyName("test");

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            nodePresenter.UpdateNode(parentNode, nodeToUpdate, dataObject);

            // Assert
            Assert.AreEqual(dataObject.Name, nodeToUpdate.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), nodeToUpdate.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.PlaceholderIcon, nodeToUpdate.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateNode_ValidInputPlaceholderData_UpdateTreeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new InputPlaceholder("test");

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            nodePresenter.UpdateNode(parentNode, nodeToUpdate, dataObject);

            // Assert
            Assert.AreEqual(dataObject.Name, nodeToUpdate.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), nodeToUpdate.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GenericInputOutputIcon, nodeToUpdate.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateNode_ValidOutputPlaceholderData_UpdateTreeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new OutputPlaceholder("test");

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            nodePresenter.UpdateNode(parentNode, nodeToUpdate, dataObject);

            // Assert
            Assert.AreEqual(dataObject.Name, nodeToUpdate.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), nodeToUpdate.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GenericInputOutputIcon, nodeToUpdate.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNode(null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRenamceTo_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNodeTo(null, null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(null, null);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnEmptyEnumerable()
        {
            // Setup
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            IEnumerable children = nodePresenter.GetChildNodeObjects(null, null);

            // Assert
            CollectionAssert.IsEmpty(children);
        }
    }
}