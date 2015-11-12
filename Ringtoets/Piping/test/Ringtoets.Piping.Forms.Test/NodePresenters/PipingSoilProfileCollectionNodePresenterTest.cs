using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Controls;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.NodePresenters;
using RingtoetsFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingSoilProfileCollectionNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(IEnumerable<PipingSoilProfile>), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfileCollectionNodeStub = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

            IEnumerable<PipingSoilProfile> soilProfilesCollection = new PipingSoilProfile[0];

            // Call
            nodePresenter.UpdateNode(null, soilProfileCollectionNodeStub, soilProfilesCollection);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.PipingSoilProfilesCollection_DisplayName, soilProfileCollectionNodeStub.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), soilProfileCollectionNodeStub.ForegroundColor);
            Assert.AreEqual(16, soilProfileCollectionNodeStub.Image.Height);
            Assert.AreEqual(16, soilProfileCollectionNodeStub.Image.Width);
        }

        [Test]
        public void GetChildNodeObjects_WithData_ReturnCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

            IEnumerable<object> soilProfilesCollection = new []
            {
                new TestPipingSoilProfile(),
                new TestPipingSoilProfile()
            };

            // Call
            var children = nodePresenter.GetChildNodeObjects(soilProfilesCollection, nodeMock);

            // Assert
            CollectionAssert.AreEqual(soilProfilesCollection, children);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void GetContextMenu_DefaultScenario_ReturnNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNull(contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_SurfaceLinesImportActionSet_HaveImportSurfaceLinesItemInContextMenu()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();
            var actionStub = mocks.Stub<Action>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter
            {
                ImportSoilProfilesAction = actionStub
            };

            // Call
            var returnedContextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(1, returnedContextMenu.Items.Count);
            var importItem = returnedContextMenu.Items[0];
            Assert.AreEqual("Importeer ondergrondprofielen", importItem.Text);
            Assert.AreEqual("Importeer nieuwe ondergrondprofielen van een *.soil bestand.", importItem.ToolTipText);
            Assert.AreEqual(16, importItem.Image.Width);
            Assert.AreEqual(16, importItem.Image.Height);
            mocks.VerifyAll(); // Expect no calls on arguments
        }
    }
}