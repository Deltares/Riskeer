using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.TestUtils;
using NUnit.Framework;

namespace Core.Common.Base.Tests.Controls.Swf.TreeViewControls
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
                group.NotifyObservers();

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
                group.NotifyObservers();

                Assert.AreEqual(false, groupNode.HasChildren);

                group.Children.Add(child1);
                group.NotifyObservers();

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
                group.NotifyObservers();

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
                group.NotifyObservers();

                Assert.AreEqual(2, GetAllNodes(treeView.Nodes).Count());

                group.Children.Add(child2);
                group.NotifyObservers();

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
                group.NotifyObservers();

                Assert.AreEqual(2, GetAllNodes(treeView.Nodes).Count());

                group.Children.Add(child2);
                group.NotifyObservers();

                Assert.AreEqual(3, GetAllNodes(treeView.Nodes).Count());

                WindowsFormsTestHelper.CloseAll();
            }
        }

        #endregion Add

        #region Test Classes

        private class TestPerson : Observable
        {
            public string Name { get; set; }

            public TestGroup TestGroup { get; set; }
        }

        private class TestGroup : Observable
        {
            public TestGroup()
            {
                Children = new ObservableList<TestPerson>();
                Adults = new ObservableList<TestPerson>();
            }

            public string Name { get; set; }

            public ObservableList<TestPerson> Children { get; set; }

            public ObservableList<TestPerson> Adults { get; set; }
        }

        private class GroupNodePresenterUsingCollection : TreeViewNodePresenterBase<TestGroup>
        {
            public override void UpdateNode(ITreeNode parentNode, ITreeNode node, TestGroup nodeData)
            {
                node.Text = nodeData.Name;
            }

            public override IEnumerable GetChildNodeObjects(TestGroup parentNodeData)
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

            public override IEnumerable GetChildNodeObjects(TestGroup parentNodeData)
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