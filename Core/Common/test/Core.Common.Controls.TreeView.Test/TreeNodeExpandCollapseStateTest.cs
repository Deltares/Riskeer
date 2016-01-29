//using System;
//using System.Collections.Generic;
//using System.Windows.Forms;
//using NUnit.Framework;
//using Rhino.Mocks;
//
//namespace Core.Common.Controls.TreeView.Test
//{
//    [TestFixture]
//    public class TreeNodeExpandCollapseStateTest
//    {
//        [Test]
//        public void ParameteredConstructor_NodeIsNull_ThrowArgumentNullException()
//        {
//            // Call
//            TestDelegate call = () => new TreeNodeExpandCollapseState(null);
//
//            // Assert
//            var exception = Assert.Throws<ArgumentNullException>(call);
//            string customMessagePart = exception.Message.Split(new []{Environment.NewLine}, StringSplitOptions.None)[0];
//            Assert.AreEqual("Knoop moet opgegeven zijn om diens toestand op te kunnen nemen.", customMessagePart);
//        }
//
//        [Test]
//        public void ParameterdConstructor_NodeTagIsNull_ThrowArgumentException()
//        {
//            // Setup
//            var mocks = new MockRepository();
//            var node = mocks.StrictMock<TreeNode>();
//            node.Expect(n => n.Tag).Return(null);
//            mocks.ReplayAll();
//
//            // Call
//            TestDelegate call = () => new TreeNodeExpandCollapseState(node);
//
//            // Assert
//            var exception = Assert.Throws<ArgumentException>(call);
//            string customMessagePart = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
//            Assert.AreEqual("Knoop data moet aanwezig zijn om de toestand van diens knoop op te kunnen nemen.", customMessagePart);
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void Restore_SingleNodeCollapsed_CollapseNode()
//        {
//            // Setup
//            var sourceData = new object();
//            var mocks = new MockRepository();
//
//            var sourceNode = mocks.StrictMock<TreeNode>();
//            sourceNode.Expect(n => n.IsExpanded).Return(false);
//            sourceNode.Expect(n => n.Tag).Return(sourceData);
//            sourceNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            var targetNode = mocks.StrictMock<TreeNode>();
//            targetNode.Expect(n => n.IsExpanded).Return(true);
//            targetNode.Expect(n => n.Tag).Return(sourceData);
//            targetNode.Expect(n => n.Collapse());
//            targetNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            mocks.ReplayAll();
//
//            var nodeState = new TreeNodeExpandCollapseState(sourceNode);
//
//            // Call
//            nodeState.Restore(targetNode);
//
//            // Assert
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void Restore_SingleNodeExpanded_ExpandNode()
//        {
//            // Setup
//            var sourceData = new object();
//            var mocks = new MockRepository();
//
//            var sourceNode = mocks.StrictMock<TreeNode>();
//            sourceNode.Expect(n => n.IsExpanded).Return(true);
//            sourceNode.Expect(n => n.Tag).Return(sourceData);
//            sourceNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            var targetNode = mocks.StrictMock<TreeNode>();
//            targetNode.Expect(n => n.IsExpanded).Return(false);
//            targetNode.Expect(n => n.Expand());
//            targetNode.Expect(n => n.Tag).Return(sourceData);
//            targetNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            mocks.ReplayAll();
//
//            var nodeState = new TreeNodeExpandCollapseState(sourceNode);
//
//            // Call
//            nodeState.Restore(targetNode);
//
//            // Assert
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        [TestCase(true)]
//        [TestCase(false)]
//        public void Restore_SingleNodeAndTargetInSameState_DoNothing(bool isExpanded)
//        {
//            // Setup
//            var sourceData = new object();
//            var mocks = new MockRepository();
//
//            var sourceNode = mocks.StrictMock<TreeNode>();
//            sourceNode.Expect(n => n.IsExpanded).Return(isExpanded);
//            sourceNode.Expect(n => n.Tag).Return(sourceData);
//            sourceNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            var targetNode = mocks.StrictMock<TreeNode>();
//            targetNode.Expect(n => n.IsExpanded).Return(isExpanded);
//            targetNode.Expect(n => n.Tag).Return(sourceData);
//            targetNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            mocks.ReplayAll();
//
//            var nodeState = new TreeNodeExpandCollapseState(sourceNode);
//
//            // Call
//            nodeState.Restore(targetNode);
//
//            // Assert
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void Restore_TargetNodeNotSameTag_ThrowArgumentException()
//        {
//            // Setup
//            var sourceData = new object();
//            var targetData = new object();
//            var mocks = new MockRepository();
//
//            var sourceNode = mocks.Stub<TreeNode>();
//            sourceNode.Stub(n => n.IsExpanded).Return(true);
//            sourceNode.Tag = sourceData;
//            sourceNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            var targetNode = mocks.Stub<TreeNode>();
//            targetNode.Tag = targetData;
//            targetNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            mocks.ReplayAll();
//
//            var nodeState = new TreeNodeExpandCollapseState(sourceNode);
//
//            // Call
//            TestDelegate call = () => nodeState.Restore(targetNode);
//
//            // Assert
//            var exception = Assert.Throws<ArgumentException>(call);
//            string userMessage = exception.Message.Split(new []{Environment.NewLine}, StringSplitOptions.None)[0];
//            Assert.AreEqual("Knoop heeft niet dezelfde data als de opgenomen knoop.", userMessage);
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void Restore_NodeWithExpandedChild_ExpandChildNode()
//        {
//            // Setup
//            var sourceData = new object();
//            var childData = new object();
// 
//            var mocks = new MockRepository();
//
//            var stubNode = mocks.Stub<TreeNode>();
//            stubNode.Tag = new object();
//            stubNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            var sourceChildNode = mocks.Stub<TreeNode>();
//            sourceChildNode.Tag = childData;
//            sourceChildNode.Stub(n => n.IsExpanded).Return(true);
//            sourceChildNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                stubNode
//            });
//
//            var sourceNode = mocks.Stub<TreeNode>();
//            sourceNode.Stub(n => n.IsExpanded).Return(true);
//            sourceNode.Tag = sourceData;
//            sourceNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                sourceChildNode
//            });
//
//            var targetChildNode = mocks.StrictMock<TreeNode>();
//            targetChildNode.Stub(n => n.Tag).Return(childData);
//            targetChildNode.Stub(n => n.IsExpanded).Return(false);
//            targetChildNode.Expect(n => n.Expand());
//            targetChildNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                stubNode
//            });
//
//            var targetNode = mocks.Stub<TreeNode>();
//            targetNode.Stub(n => n.IsExpanded).Return(true);
//            targetNode.Tag = sourceData;
//            targetNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                targetChildNode
//            });
//
//            mocks.ReplayAll();
//
//            var nodeState = new TreeNodeExpandCollapseState(sourceNode);
//
//            // Call
//            nodeState.Restore(targetNode);
//
//            // Assert
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void Restore_NodeWithCollapsedChild_CollapseChildNode()
//        {
//            // Setup
//            var sourceData = new object();
//            var childData = new object();
//
//            var mocks = new MockRepository();
//
//            var stubNode = mocks.Stub<TreeNode>();
//            stubNode.Tag = new object();
//            stubNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            var sourceChildNode = mocks.Stub<TreeNode>();
//            sourceChildNode.Tag = childData;
//            sourceChildNode.Stub(n => n.IsExpanded).Return(false);
//            sourceChildNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                stubNode
//            });
//
//            var sourceNode = mocks.Stub<TreeNode>();
//            sourceNode.Stub(n => n.IsExpanded).Return(true);
//            sourceNode.Tag = sourceData;
//            sourceNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                sourceChildNode
//            });
//
//            var targetChildNode = mocks.StrictMock<TreeNode>();
//            targetChildNode.Stub(n => n.Tag).Return(childData);
//            targetChildNode.Stub(n => n.IsExpanded).Return(true);
//            targetChildNode.Expect(n => n.Collapse());
//            targetChildNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                stubNode
//            });
//
//            var targetNode = mocks.Stub<TreeNode>();
//            targetNode.Stub(n => n.IsExpanded).Return(true);
//            targetNode.Tag = sourceData;
//            targetNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                targetChildNode
//            });
//
//            mocks.ReplayAll();
//
//            var nodeState = new TreeNodeExpandCollapseState(sourceNode);
//
//            // Call
//            nodeState.Restore(targetNode);
//
//            // Assert
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void Restore_NodeWithChildAddedAfterRecordingState_IgnoreChild()
//        {
//            // Setup
//            var sourceData = new object();
//            var childData = new object();
//
//            var mocks = new MockRepository();
//
//            var stubNode = mocks.Stub<TreeNode>();
//            stubNode.Tag = new object();
//            stubNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            var sourceNode = mocks.Stub<TreeNode>();
//            sourceNode.Stub(n => n.IsExpanded).Return(true);
//            sourceNode.Tag = sourceData;
//            sourceNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            var targetChildNode = mocks.StrictMock<TreeNode>();
//            targetChildNode.Stub(n => n.Tag).Return(childData);
//            targetChildNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                stubNode
//            });
//
//            var targetNode = mocks.Stub<TreeNode>();
//            targetNode.Stub(n => n.IsExpanded).Return(true);
//            targetNode.Tag = sourceData;
//            targetNode.Stub(n => n.Nodes).Return(new List<TreeNode>
//            {
//                targetChildNode
//            });
//
//            mocks.ReplayAll();
//
//            var nodeState = new TreeNodeExpandCollapseState(sourceNode);
//
//            // Call
//            nodeState.Restore(targetNode);
//
//            // Assert
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void Restore_NodeWithDifferentEqualDataInstances_RestoreState()
//        {
//            // Setup
//            var data1 = new IntegerWrapperWithCustomEquals
//            {
//                WrappedInteger = 1
//            };
//            var data2 = new IntegerWrapperWithCustomEquals
//            {
//                WrappedInteger = 1
//            };
//
//            var mocks = new MockRepository();
//            var sourceNode = mocks.StrictMock<TreeNode>();
//            sourceNode.Expect(n => n.IsExpanded).Return(false);
//            sourceNode.Expect(n => n.Tag).Return(data1);
//            sourceNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            var targetNode = mocks.StrictMock<TreeNode>();
//            targetNode.Expect(n => n.IsExpanded).Return(true);
//            targetNode.Expect(n => n.Tag).Return(data2);
//            targetNode.Expect(n => n.Collapse());
//            targetNode.Stub(n => n.Nodes).Return(new List<TreeNode>());
//
//            mocks.ReplayAll();
//
//            // Precondition:
//            Assert.True(data1.Equals(data2));
//            Assert.True(data2.Equals(data1));
//            Assert.AreNotSame(data1, data2);
//
//            var nodeState = new TreeNodeExpandCollapseState(sourceNode);
//
//            // Call
//            nodeState.Restore(targetNode);
//
//            // Assert
//            mocks.VerifyAll();
//        }
//
//        private class IntegerWrapperWithCustomEquals
//        {
//            public int WrappedInteger { get; set; }
//
//            public override bool Equals(object obj)
//            {
//                var otherIntWrapper = obj as IntegerWrapperWithCustomEquals;
//                if (otherIntWrapper != null)
//                {
//                    return WrappedInteger.Equals(otherIntWrapper.WrappedInteger);
//                }
//                return base.Equals(obj);
//                }
//
//            public override int GetHashCode()
//            {
//                return WrappedInteger.GetHashCode();
//            }
//        }
//    }
//}