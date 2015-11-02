using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base.Tests.TestObjects;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;
using SharpTestsEx;
using TreeView = Core.Common.Controls.Swf.TreeViewControls.TreeView;

namespace Core.Common.Base.Tests.Controls.Swf.TreeViewControls
{
    [TestFixture]
    public class TreeViewTest
    {
        private readonly MockRepository mockRepository = new MockRepository();

        /// <summary>
        /// Assure the correct node is returned containing a specific tag
        /// </summary>
        [Test]
        public void GetNodeByTag()
        {
            var o1 = new object();

            var treeView = new TreeView();
            ITreeNode node = treeView.NewNode();
            node.Tag = o1;
            treeView.Nodes.Add(node);
            ITreeNode node1 = treeView.GetNodeByTag(o1);
            Assert.AreEqual(node, node1);
        }

        /// <summary>
        /// Assure a nodepresenter is returned that corresponds to the given datatype
        /// </summary>
        [Test]
        public void GetNodePresenterForDataType()
        {
            var presenter = mockRepository.Stub<ITreeNodePresenter>();
            Expect.Call(presenter.NodeTagType).Return(typeof(object));
            var treeView = new TreeView();

            //treeview is assigned to presenter when it's added to the list of nodepresenters
            treeView.NodePresenters.Add(presenter);
            Assert.AreEqual(treeView, presenter.TreeView);

            mockRepository.ReplayAll();
            Assert.AreEqual(presenter, treeView.GetTreeViewNodePresenter(new object(), null));
            mockRepository.VerifyAll();
        }

        [Test]
        public void HideSelectionIsFalseByDefault()
        {
            new TreeView().HideSelection.Should().Be.False();
        }

        [Test]
        public void RefreshShouldNotRefreshNodesWhichAreNotLoaded()
        {
            var treeView = new TreeView();

            var parent = new Parent
            {
                Name = "parent1"
            };
            var child = new Child();
            parent.Children.Add(child);

            var parentNodePresenter = new ParentNodePresenter();
            var childNodePresenter = new ChildNodePresenter();

            treeView.NodePresenters.Add(parentNodePresenter);
            treeView.NodePresenters.Add(childNodePresenter);

            childNodePresenter.AfterUpdate +=
                delegate { Assert.Fail("Child nodes which are not loaded should not be updated"); };

            treeView.Refresh();
        }

        [Test]
        public void GetAllLoadedNodes()
        {
            /* 
             * RootNode
               |-LoadedChild
               |-NotLoadedChild
               |-LoadedChild2
                    |-LoadedGrandChild
               |-NotLoadedChild2
                    |-LoadedGrandChild2
             */
            var treeView = new TreeView();

            ITreeNode rootNode = new MockTestNode(treeView, true)
            {
                Text = "RootNode"
            };
            var loadedChild = new MockTestNode(treeView, true)
            {
                Text = "LoadedChild"
            };
            rootNode.Nodes.Add(loadedChild);
            var notLoadedChild = new MockTestNode(treeView, false)
            {
                Text = "NotLoadedChild"
            };
            rootNode.Nodes.Add(notLoadedChild);

            var loadedChild2 = new MockTestNode(treeView, true)
            {
                Text = "LoadedChild2"
            };
            rootNode.Nodes.Add(loadedChild2);
            var loadedGrandChild = new MockTestNode(treeView, true)
            {
                Text = "LoadedGrandChild"
            };
            loadedChild2.Nodes.Add(loadedGrandChild);

            var notLoadedChild2 = new MockTestNode(treeView, false)
            {
                Text = "NotLoadedChild2"
            };
            rootNode.Nodes.Add(notLoadedChild2);
            notLoadedChild2.Nodes.Add(new MockTestNode(treeView, true)
            {
                Text = "LoadedGrandChild2"
            });
            //reset the loaded flag. It was set set to true by the previous call
            notLoadedChild2.SetLoaded(false);

            treeView.Nodes.Add(rootNode);

            Assert.AreEqual(
                new[]
                {
                    rootNode,
                    loadedChild,
                    notLoadedChild,
                    loadedChild2,
                    loadedGrandChild,
                    notLoadedChild2
                },
                treeView.AllLoadedNodes.ToArray());
        }

        [Test]
        public void NodeShouldRememberExpansionHistory()
        {
            // create tree view with 3 nodes
            //
            // node1 
            //   node2
            //     node3
            var treeView = new TreeView();

            var node1 = treeView.NewNode();

            var node2 = treeView.NewNode();
            node1.Nodes.Add(node2);

            var node3 = treeView.NewNode();
            node2.Nodes.Add(node3);

            treeView.Nodes.Add(node1);

            // expand all nodes
            treeView.ExpandAll();

            // collapse and expand root node
            node1.Collapse();
            node1.Expand();

            // asserts
            Assert.IsTrue(node2.IsExpanded, "node2 should remain expanded after parent node is collapsed/expanded");
        }

        [Test]
        public void NodeCreatedWithNodePresentersShouldRememberExpansionHistory()
        {
            // n1
            //   n2
            //     n3
            var parent = new Child
            {
                Name = "n1", Children =
                {
                    new Child
                    {
                        Name = "n2", Children =
                        {
                            new Child
                            {
                                Name = "n3"
                            }
                        }
                    }
                }
            };

            var treeView = new TreeView
            {
                NodePresenters =
                {
                    new ChildNodePresenter()
                },
                Data = parent
            };

            // expand / collapse / expand
            treeView.ExpandAll();

            treeView.Nodes[0].Collapse();
            treeView.Nodes[0].Expand();

            // checks
            Assert.IsTrue(treeView.Nodes[0].Nodes[0].IsExpanded, "n2 remains expanded after collapse / expand of parent node");
        }

        [Test]
        public void AddChildNodes()
        {
            var treeView = new TreeView();

            var node1 = treeView.NewNode();
            treeView.Nodes.Add(node1);

            var node2 = treeView.NewNode();
            var node3 = treeView.NewNode();
            node1.Nodes.Add(node2);
            node1.Nodes.Add(node3);

            node1.Nodes.Count
                 .Should().Be.EqualTo(2);
        }

        [Test]
        public void NodesAreNotClearedAfterExpand()
        {
            var treeView = new TreeView();

            var node1 = treeView.NewNode();
            treeView.Nodes.Add(node1);

            var node2 = treeView.NewNode();
            var node3 = treeView.NewNode();
            node1.Nodes.Add(node2);
            node1.Nodes.Add(node3);

            node1.Expand();

            node1.Nodes.Count
                 .Should().Be.EqualTo(2);
        }

        [Test]
        public void SelectedNodeSetToRootNodeAfterDataSet()
        {
            var treeView = new TreeView
            {
                NodePresenters =
                {
                    new ParentNodePresenter()
                }
            };

            var rootObject = new Parent();

            // set data to tree view
            treeView.Data = rootObject;

            treeView.SelectedNode.Should().Not.Be.Null();
            treeView.SelectedNode.Tag.Should().Be.SameInstanceAs(rootObject);
        }

        /// <summary>
        /// In some cases every refresh of the tree view returns a new object as a child item (e.g. folder which does not exist in the Data objects).
        /// 
        /// Actually this is a design problem, objects which do not exist in the 
        /// </summary>
        [Test]
        public void TreeNodesRemainExpandedForDynamicNodes()
        {
            var treeView = new TreeView
            {
                NodePresenters =
                {
                    new DynamicParentNodePresenter(),
                    new ChildNodePresenter()
                }
            };

            var parent = new Parent();
            treeView.Data = parent;

            WindowsFormsTestHelper.Show(treeView); // show it to make sure that expand / refresh node really loads nodes.

            treeView.ExpandAll();
            treeView.Refresh();

            // asserts
            treeView.Nodes[0].Nodes[0].IsExpanded.Should("node remains expanded").Be.True();
        }

        [Test]
        public void TreeViewUpdateOnManyPropertyChangesShouldBeFast()
        {
            var parent = new Child
            {
                Name = "parent"
            };

            for (var i = 0; i < 100; i++)
            {
                parent.Children.Add(new Child
                {
                    Name = i.ToString()
                });
            }

            // measure time to perform action without tree view
            Func<double> processingAction = () =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var rnd = new Random();
                for (var i = 0; i < 99; i++)
                {
                    var child = parent.Children[rnd.Next(99)];
                    child.Name = i.ToString();
                }
                stopwatch.Stop();

                return stopwatch.ElapsedMilliseconds;
            };

            Console.WriteLine("Elapsed time to perform action without tree view: " + processingAction());

            var treeView = new TreeView
            {
                NodePresenters =
                {
                    new ChildNodePresenter()
                },
                Data = parent
            };

            // expand / collapse / expand
            treeView.ExpandAll();

            double elapsedMillisecondsWithTreeView = 0;
            Action<Form> onShow = delegate
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                elapsedMillisecondsWithTreeView = processingAction();
                Console.WriteLine("Elapsed time to perform action with tree view: " + elapsedMillisecondsWithTreeView);

                treeView.WaitUntilAllEventsAreProcessed();

                stopwatch.Stop();
                Console.WriteLine("Elapsed time to refresh tree view: " + stopwatch.ElapsedMilliseconds);
            };

            WindowsFormsTestHelper.ShowModal(treeView, onShow);

            TestHelper.AssertIsFasterThan(10, () => Thread.Sleep((int) elapsedMillisecondsWithTreeView));
        }

        public class DynamicParentNodePresenter : TreeViewNodePresenterBase<Parent>
        {
            public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Parent nodeData)
            {
                node.Text = nodeData.Name;
            }

            public override IEnumerable GetChildNodeObjects(Parent parentNodeData, ITreeNode node)
            {
                // always returns a single child with the same name
                yield return new Child
                {
                    Name = "child", Children =
                    {
                        new Child
                        {
                            Name = "grandchild"
                        }
                    }
                };
            }
        }
    }
}