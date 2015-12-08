using System;
using System.Collections.Generic;

using Core.Common.Controls.Swf.TreeViewControls;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Controls.Swf.Test.TreeViewControls
{
    [TestFixture]
    public class TreeNodeExpandCollapseStateTest
    {
        [Test]
        public void ParameteredConstructor_NodeIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TreeNodeExpandCollapseState(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessagePart = exception.Message.Split(new []{Environment.NewLine}, StringSplitOptions.None)[0];
            Assert.AreEqual("Knoop moet opgegeven zijn om diens toestand op te kunnen nemen.", customMessagePart);
        }

        [Test]
        public void ParameterdConstructor_NodeTagIsNull_ThrowArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var node = mocks.StrictMock<ITreeNode>();
            node.Expect(n => n.Tag).Return(null);
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new TreeNodeExpandCollapseState(node);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            string customMessagePart = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
            Assert.AreEqual("Knoop data moet aanwezig zijn om de toestand van diens knoop op te kunnen nemen.", customMessagePart);
            mocks.VerifyAll();
        }

        [Test]
        public void RestoreState_SingleNodeCollapsed_CollapseNode()
        {
            // Setup
            var sourceData = new object();
            var mocks = new MockRepository();

            var sourceNode = mocks.StrictMock<ITreeNode>();
            sourceNode.Expect(n => n.IsExpanded).Return(false);
            sourceNode.Expect(n => n.Tag).Return(sourceData);
            sourceNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var targetNode = mocks.StrictMock<ITreeNode>();
            targetNode.Expect(n => n.IsExpanded).Return(true);
            targetNode.Expect(n => n.Tag).Return(sourceData);
            targetNode.Expect(n => n.Collapse());
            targetNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            mocks.ReplayAll();

            var nodeState = new TreeNodeExpandCollapseState(sourceNode);

            // Call
            nodeState.Restore(targetNode);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void RestoreState_SingleNodeExpanded_ExpandNode()
        {
            // Setup
            var sourceData = new object();
            var mocks = new MockRepository();

            var sourceNode = mocks.StrictMock<ITreeNode>();
            sourceNode.Expect(n => n.IsExpanded).Return(true);
            sourceNode.Expect(n => n.Tag).Return(sourceData);
            sourceNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var targetNode = mocks.StrictMock<ITreeNode>();
            targetNode.Expect(n => n.IsExpanded).Return(false);
            targetNode.Expect(n => n.Expand());
            targetNode.Expect(n => n.Tag).Return(sourceData);
            targetNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            mocks.ReplayAll();

            var nodeState = new TreeNodeExpandCollapseState(sourceNode);

            // Call
            nodeState.Restore(targetNode);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void RestoreState_SingleNodeAndTargetInSameState_DoNothing(bool isExpanded)
        {
            // Setup
            var sourceData = new object();
            var mocks = new MockRepository();

            var sourceNode = mocks.StrictMock<ITreeNode>();
            sourceNode.Expect(n => n.IsExpanded).Return(isExpanded);
            sourceNode.Expect(n => n.Tag).Return(sourceData);
            sourceNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var targetNode = mocks.StrictMock<ITreeNode>();
            targetNode.Expect(n => n.IsExpanded).Return(isExpanded);
            targetNode.Expect(n => n.Tag).Return(sourceData);
            targetNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            mocks.ReplayAll();

            var nodeState = new TreeNodeExpandCollapseState(sourceNode);

            // Call
            nodeState.Restore(targetNode);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Restore_TargetNodeNotSameTag_ThrowArgumentException()
        {
            // Setup
            var sourceData = new object();
            var targetData = new object();
            var mocks = new MockRepository();

            var sourceNode = mocks.Stub<ITreeNode>();
            sourceNode.Stub(n => n.IsExpanded).Return(true);
            sourceNode.Tag = sourceData;
            sourceNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var targetNode = mocks.Stub<ITreeNode>();
            targetNode.Tag = targetData;
            targetNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            mocks.ReplayAll();

            var nodeState = new TreeNodeExpandCollapseState(sourceNode);

            // Call
            TestDelegate call = () => nodeState.Restore(targetNode);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("Knoop heeft niet dezelfde data als de opgenomen knoop.", exception.Message);
            mocks.VerifyAll();
        }

        [Test]
        public void Restore_NodeWithExpandedChild_ExpandChildNode()
        {
            // Setup
            var sourceData = new object();
            var childData = new object();
 
            var mocks = new MockRepository();

            var stubNode = mocks.Stub<ITreeNode>();
            stubNode.Tag = new object();
            stubNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var sourceChildNode = mocks.Stub<ITreeNode>();
            sourceChildNode.Tag = childData;
            sourceChildNode.Stub(n => n.IsExpanded).Return(true);
            sourceChildNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                stubNode
            });

            var sourceNode = mocks.Stub<ITreeNode>();
            sourceNode.Stub(n => n.IsExpanded).Return(true);
            sourceNode.Tag = sourceData;
            sourceNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                sourceChildNode
            });

            var targetChildNode = mocks.StrictMock<ITreeNode>();
            targetChildNode.Stub(n => n.Tag).Return(childData);
            targetChildNode.Stub(n => n.IsExpanded).Return(false);
            targetChildNode.Expect(n => n.Expand());
            targetChildNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                stubNode
            });

            var targetNode = mocks.Stub<ITreeNode>();
            targetNode.Stub(n => n.IsExpanded).Return(true);
            targetNode.Tag = sourceData;
            targetNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                targetChildNode
            });

            mocks.ReplayAll();

            var nodeState = new TreeNodeExpandCollapseState(sourceNode);

            // Call
            nodeState.Restore(targetNode);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Restore_NodeWithCollapsedChild_CollapseChildNode()
        {
            // Setup
            var sourceData = new object();
            var childData = new object();

            var mocks = new MockRepository();

            var stubNode = mocks.Stub<ITreeNode>();
            stubNode.Tag = new object();
            stubNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var sourceChildNode = mocks.Stub<ITreeNode>();
            sourceChildNode.Tag = childData;
            sourceChildNode.Stub(n => n.IsExpanded).Return(false);
            sourceChildNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                stubNode
            });

            var sourceNode = mocks.Stub<ITreeNode>();
            sourceNode.Stub(n => n.IsExpanded).Return(true);
            sourceNode.Tag = sourceData;
            sourceNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                sourceChildNode
            });

            var targetChildNode = mocks.StrictMock<ITreeNode>();
            targetChildNode.Stub(n => n.Tag).Return(childData);
            targetChildNode.Stub(n => n.IsExpanded).Return(true);
            targetChildNode.Expect(n => n.Collapse());
            targetChildNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                stubNode
            });

            var targetNode = mocks.Stub<ITreeNode>();
            targetNode.Stub(n => n.IsExpanded).Return(true);
            targetNode.Tag = sourceData;
            targetNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                targetChildNode
            });

            mocks.ReplayAll();

            var nodeState = new TreeNodeExpandCollapseState(sourceNode);

            // Call
            nodeState.Restore(targetNode);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Restore_SourceTreeDifferentFromTargetTree_ThrowKeyNotFoundException()
        {
            // Setup
            var sourceData = new object();
            var childData = new object();

            var mocks = new MockRepository();

            var stubNode = mocks.Stub<ITreeNode>();
            stubNode.Tag = new object();
            stubNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var sourceChildNode = mocks.Stub<ITreeNode>();
            sourceChildNode.Tag = childData;
            sourceChildNode.Stub(n => n.IsExpanded).Return(false);
            sourceChildNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                stubNode
            });

            var sourceNode = mocks.Stub<ITreeNode>();
            sourceNode.Stub(n => n.IsExpanded).Return(true);
            sourceNode.Tag = sourceData;
            sourceNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var targetChildNode = mocks.StrictMock<ITreeNode>();
            targetChildNode.Stub(n => n.Tag).Return(childData);
            targetChildNode.Stub(n => n.IsExpanded).Return(true);
            targetChildNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                stubNode
            });

            var targetNode = mocks.Stub<ITreeNode>();
            targetNode.Stub(n => n.IsExpanded).Return(true);
            targetNode.Tag = sourceData;
            targetNode.Stub(n => n.Nodes).Return(new List<ITreeNode>
            {
                targetChildNode
            });

            mocks.ReplayAll();

            var nodeState = new TreeNodeExpandCollapseState(sourceNode);

            // Call
            TestDelegate call = () => nodeState.Restore(targetNode);

            // Assert
            Assert.Throws<KeyNotFoundException>(call);
            mocks.VerifyAll();
        }
    }
}