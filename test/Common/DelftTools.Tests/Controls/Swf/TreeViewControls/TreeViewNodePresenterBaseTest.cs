using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Controls.Swf.TreeViewControls;
using DelftTools.TestUtils;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.TreeViewControls
{
    [TestFixture]
    public class TreeViewNodePresenterBaseTest
    {
        private static IEnumerable<ITreeNode> GetAllNodes(IEnumerable<ITreeNode> nodes)
        {
            var result = new List<ITreeNode>();
            if (nodes == null)
            {
                return result;
            }

            foreach (var node in nodes)
            {
                result.Add(node);
                result.AddRange(GetAllNodes(node.Nodes));
            }
            return result;
        }

        #region Remove

        [Test]
        public void OnCollectionRemoveWithCollectionBasedNodePresenter()
        {
            var group = new TestGroup();
            var child1 = new TestPerson
            {
                Name = "child1"
            };
            var child2 = new TestPerson
            {
                Name = "child2"
            };
            group.Children.Add(child1);
            group.Children.Add(child2);

            using (var treeView = new TreeView())
            {
                treeView.NodePresenters.Add(new GroupNodePresenterUsingCollection());
                treeView.NodePresenters.Add(new PersonNodePresenter());
                treeView.Data = group;

                WindowsFormsTestHelper.Show(treeView);

                Assert.AreEqual(3, GetAllNodes(treeView.Nodes).Count());

                group.Children.Remove(child1);

                treeView.WaitUntilAllEventsAreProcessed();

                Assert.AreEqual(2, GetAllNodes(treeView.Nodes).Count());

                WindowsFormsTestHelper.CloseAll();
            }
        }

        [Test]
        public void ModifyCollectionShouldUpdateNodeHasChildren()
        {
            var group = new TestGroup();
            var child1 = new TestPerson
            {
                Name = "child1"
            };
            group.Children.Add(child1);

            using (var treeView = new TreeView())
            {
                treeView.NodePresenters.Add(new GroupNodePresenterUsingCollection());
                treeView.NodePresenters.Add(new PersonNodePresenter());
                treeView.Data = group;

                var groupNode = (TreeNode) treeView.Nodes[0];
                Assert.AreEqual(true, groupNode.HasChildren);

                WindowsFormsTestHelper.Show(treeView);

                group.Children.Remove(child1);

                treeView.WaitUntilAllEventsAreProcessed();

                Assert.AreEqual(false, groupNode.HasChildren);

                group.Children.Add(child1);

                treeView.WaitUntilAllEventsAreProcessed();

                Assert.AreEqual(true, groupNode.HasChildren);

                WindowsFormsTestHelper.CloseAll();
            }
        }

        [Test]
        public void OnCollectionRemoveWithPropertyBasedNodePresenter()
        {
            var group = new TestGroup();
            var child1 = new TestPerson
            {
                Name = "child1"
            };
            var child2 = new TestPerson
            {
                Name = "child2"
            };
            group.Children.Add(child1);
            group.Children.Add(child2);

            using (var treeView = new TreeView())
            {
                treeView.NodePresenters.Add(new GroupNodePresenterUsingProperty());
                treeView.NodePresenters.Add(new PersonNodePresenter());
                treeView.Data = group;

                WindowsFormsTestHelper.Show(treeView);

                Assert.AreEqual(3, GetAllNodes(treeView.Nodes).Count());

                group.Children.Remove(child1);

                treeView.WaitUntilAllEventsAreProcessed();

                Assert.AreEqual(2, GetAllNodes(treeView.Nodes).Count());

                WindowsFormsTestHelper.CloseAll();
            }
        }

        [Test]
        public void OnWrongCollectionRemove()
        {
            var group = new TestGroup();
            var person = new TestPerson
            {
                Name = "adolecent"
            };
            group.Children.Add(person);
            group.Adults.Add(person);

            using (var treeView = new TreeView())
            {
                treeView.NodePresenters.Add(new GroupNodePresenterUsingCollection());
                treeView.NodePresenters.Add(new PersonNodePresenter());
                treeView.Data = group;

                Assert.AreEqual(2, GetAllNodes(treeView.Nodes).Count());

                group.Adults.Remove(person);

                Assert.AreEqual(2, GetAllNodes(treeView.Nodes).Count());
            }
        }

        #endregion Remove

        #region Add

        [Test]
        public void OnCollectionAddWithCollectionBasedNodePresenter()
        {
            var group = new TestGroup();
            var child1 = new TestPerson
            {
                Name = "child1"
            };
            var child2 = new TestPerson
            {
                Name = "child2"
            };

            using (var treeView = new TreeView())
            {
                treeView.NodePresenters.Add(new GroupNodePresenterUsingCollection());
                treeView.NodePresenters.Add(new PersonNodePresenter());
                treeView.Data = group;

                WindowsFormsTestHelper.Show(treeView);

                group.Children.Add(child1);

                treeView.WaitUntilAllEventsAreProcessed();

                Assert.AreEqual(2, GetAllNodes(treeView.Nodes).Count());

                group.Children.Add(child2);
                treeView.WaitUntilAllEventsAreProcessed();

                Assert.AreEqual(3, GetAllNodes(treeView.Nodes).Count());

                WindowsFormsTestHelper.CloseAll();
            }
        }

        [Test]
        public void OnCollectionAddWithPropertyBasedNodePresenter()
        {
            var group = new TestGroup();
            var child1 = new TestPerson
            {
                Name = "child1"
            };
            var child2 = new TestPerson
            {
                Name = "child2"
            };

            using (var treeView = new TreeView())
            {
                treeView.NodePresenters.Add(new GroupNodePresenterUsingProperty());
                treeView.NodePresenters.Add(new PersonNodePresenter());
                treeView.Data = group;

                WindowsFormsTestHelper.Show(treeView);

                Assert.AreEqual(1, GetAllNodes(treeView.Nodes).Count());

                group.Children.Add(child1);
                treeView.WaitUntilAllEventsAreProcessed();

                Assert.AreEqual(2, GetAllNodes(treeView.Nodes).Count());

                group.Children.Add(child2);
                treeView.WaitUntilAllEventsAreProcessed();

                Assert.AreEqual(3, GetAllNodes(treeView.Nodes).Count());

                WindowsFormsTestHelper.CloseAll();
            }
        }

        #endregion Add

        #region Test Classes

        [Entity(FireOnCollectionChange = false)]
        private class TestPerson
        {
            public string Name { get; set; }

            [NoNotifyPropertyChange]
            public TestGroup TestGroup { get; set; }
        }

        [Entity]
        private class TestGroup
        {
            public TestGroup()
            {
                Children = new EventedList<TestPerson>();
                Children.CollectionChanged += CollectionChanged;
                Adults = new EventedList<TestPerson>();
                Adults.CollectionChanged += CollectionChanged;
            }

            public string Name { get; set; }
            public IEventedList<TestPerson> Children { get; set; }
            public IEventedList<TestPerson> Adults { get; set; }

            private void CollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
            {
                if (e.Action == NotifyCollectionChangeAction.Add)
                {
                    ((TestPerson) e.Item).TestGroup = this;
                }
                else if (e.Action == NotifyCollectionChangeAction.Remove)
                {
                    ((TestPerson) e.Item).TestGroup = null;
                }
            }
        }

        private class GroupNodePresenterUsingCollection : TreeViewNodePresenterBase<TestGroup>
        {
            public override void UpdateNode(ITreeNode parentNode, ITreeNode node, TestGroup nodeData)
            {
                node.Text = nodeData.Name;
            }

            public override IEnumerable GetChildNodeObjects(TestGroup parentNodeData, ITreeNode node)
            {
                return parentNodeData.Children;
            }
        }

        private class GroupNodePresenterUsingProperty : TreeViewNodePresenterBase<TestGroup>
        {
            public override void UpdateNode(ITreeNode parentNode, ITreeNode node, TestGroup nodeData)
            {
                node.Text = nodeData.Name;
            }

            public override IEnumerable GetChildNodeObjects(TestGroup parentNodeData, ITreeNode node)
            {
                return parentNodeData.Children;
            }
        }

        private class PersonNodePresenter : TreeViewNodePresenterBase<TestPerson>
        {
            public override void UpdateNode(ITreeNode parentNode, ITreeNode node, TestPerson nodeData)
            {
                node.Text = nodeData.Name;
            }
        }

        #endregion
    }
}