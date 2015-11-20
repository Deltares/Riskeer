using System.Drawing;
using System.Linq;

using Core.Common.Controls;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.NodePresenters;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.NodePresenters
{
    [TestFixture]
    public class FailureMechanismNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new FailureMechanismNodePresenter();

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<FailureMechanismPlaceholder>>(nodePresenter);
        }

        [Test]
        public void UpdateNode_ValidOutputPlaceholderData_UpdateTreeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new FailureMechanismPlaceholder("test");

            var nodePresenter = new FailureMechanismNodePresenter();

            // Call
            nodePresenter.UpdateNode(parentNode, nodeToUpdate, dataObject);

            // Assert
            Assert.AreEqual(dataObject.Name, nodeToUpdate.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), nodeToUpdate.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.FailureMechanismIcon, nodeToUpdate.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new FailureMechanismNodePresenter();

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNode(null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRenamceTo_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new FailureMechanismNodePresenter();

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNodeTo(null, null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new FailureMechanismNodePresenter();

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(null, null);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnEmptyEnumerable()
        {
            // Setup
            var nodePresenter = new FailureMechanismNodePresenter();
            var failureMechanism = new FailureMechanismPlaceholder("test");

            // Call
            object[] children = nodePresenter.GetChildNodeObjects(failureMechanism, null).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);
            var inputFolder = (CategoryTreeFolder)children[0];
            Assert.AreEqual("Invoer", inputFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputFolder.Category);
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.SectionDivisions,
                failureMechanism.Locations,
                failureMechanism.BoundaryConditions
            }, inputFolder.Contents);

            var outputFolder = (CategoryTreeFolder)children[1];
            Assert.AreEqual("Uitvoer", outputFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputFolder.Category);
            Assert.AreEqual(new[]{failureMechanism.AssessmentResult}, outputFolder.Contents);
        }
    }
}