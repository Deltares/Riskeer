// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView.Properties;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.TreeView.Test
{
    [TestFixture]
    public class TreeViewControlTest : NUnitFormTest
    {
        [Test]
        public void DefaultConstructor_InitializedTreeViewControl()
        {
            // Call
            using (var treeViewControl = new TreeViewControl())
            {
                // Assert
                var treeView = treeViewControl.Controls[0] as System.Windows.Forms.TreeView;
                Assert.NotNull(treeView);
                Assert.IsTrue(treeView.AllowDrop);
                Assert.IsTrue(treeView.LabelEdit);
                Assert.IsFalse(treeView.HideSelection);
                Assert.AreEqual(TreeViewDrawMode.Normal, treeView.DrawMode);
                Assert.AreEqual("\\", treeView.PathSeparator);

                Assert.NotNull(treeView.ImageList);
                Assert.AreEqual(0, treeView.ImageList.Images.Count);

                Assert.NotNull(treeView.StateImageList);
                Assert.AreEqual(2, treeView.StateImageList.Images.Count);
            }
        }

        [Test]
        public void RegisterTreeNodeInfo_Null_ThrowsNullReferenceException()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => treeViewControl.RegisterTreeNodeInfo(null);

                // Assert
                Assert.Throws<NullReferenceException>(test);
            }
        }

        [Test]
        public void RegisterTreeNodeInfo_NodeInfoWithoutTagType_ThrowsArgumentNullException()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo();

                // Call
                TestDelegate test = () => treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

                // Assert
                Assert.Throws<ArgumentNullException>(test);
            }
        }

        [Test]
        public void RegisterTreeNodeInfo_NodeInfoWithTagType_DoesNotThrow()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object)
                };

                // Call
                TestDelegate test = () => treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

                // Assert
                Assert.DoesNotThrow(test);
            }
        }

        [Test]
        public void RegisterTreeNodeInfo_NodeInfoForTagTypeAlreadySet_OverridesNodeInfo()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRename = (o, p) => false
                };
                var treeNodeInfoOverride = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRename = (o, p) => true
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

                var dataObject = new object();
                treeViewControl.Data = dataObject;

                // Call
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfoOverride);

                // Assert
                Assert.IsTrue(treeViewControl.CanRenameNodeForData(dataObject));
            }
        }

        [Test]
        public void Data_SetToNull_DataIsNull()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                treeViewControl.Data = null;

                // Assert
                Assert.IsNull(treeViewControl.Data);
            }
        }

        [Test]
        public void Data_NoNodeInfoSet_ThrowsInvalidOperationException()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var testNodeData = new object();

                // Call
                TestDelegate test = () => treeViewControl.Data = testNodeData;

                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }
        }

        [Test]
        public void Data_NodeInfoSet_DataSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo
                {
                    TagType = typeof(object)
                });

                var testNodeData = new object();

                // Call
                treeViewControl.Data = testNodeData;

                // Assert
                Assert.AreSame(testNodeData, treeViewControl.Data);
                Assert.AreSame(testNodeData, treeViewControl.SelectedData);
            }
        }

        [Test]
        public void CanRenameNodeForData_Null_ReturnsFalse()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRename = (o, p) => true
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.Data = new object();

                // Call
                var result = treeViewControl.CanRenameNodeForData(null);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void CanRenameNodeForData_DataNotSet_ReturnsFalse()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRename = (o, p) => true
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

                // Call
                var result = treeViewControl.CanRenameNodeForData(new object());

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRenameNodeForData_DataSet_ReturnsValueOfCanRename(bool expected)
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRename = (o, p) => expected
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                // Call
                var result = treeViewControl.CanRenameNodeForData(dataObject);

                // Assert
                Assert.AreEqual(expected, result);
            }
        }

        [Test]
        [RequiresSTA]
        public void TryRenameNodeForData_NotRenameable_ShowsDialog()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRename = (o, p) => false
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                string messageBoxText = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new MessageBoxTester(wnd);

                    messageBoxText = helper.Text;

                    helper.ClickOk();
                };

                // Call
                treeViewControl.TryRenameNodeForData(dataObject);

                // Assert
                Assert.AreEqual(Resources.TreeViewControl_The_selected_item_cannot_be_renamed, messageBoxText);
            }
        }

        [Test]
        [RequiresSTA]
        public void TryRenameNodeForData_Renameable_BeginEdit()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRename = (o, p) => true
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    // Call
                    treeViewControl.TryRenameNodeForData(dataObject);

                    // Assert
                    var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                    Assert.IsTrue(treeView.Nodes[0].IsEditing);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        public void CanRemoveNodeForData_Null_ReturnsFalse()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRemove = (o, p) => true
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.Data = new object();

                // Call
                var result = treeViewControl.CanRemoveNodeForData(null);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void CanRemoveNodeForData_DataNotSet_ReturnsFalse()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRemove = (o, p) => true
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

                // Call
                var result = treeViewControl.CanRemoveNodeForData(new object());

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRemoveNodeForData_DataSet_ReturnsValueOfCanRename(bool expected)
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var onNodeRemovedHit = 0;
                var onDataDeletedHit = 0;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRemove = (o, p) => expected,
                    OnNodeRemoved = (o, p) => onNodeRemovedHit++
                };
                treeViewControl.DataDeleted += (sender, args) => onDataDeletedHit++;
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                // Call
                var result = treeViewControl.CanRemoveNodeForData(dataObject);

                // Assert
                Assert.AreEqual(expected, result);
                Assert.AreEqual(0, onNodeRemovedHit);
                Assert.AreEqual(0, onDataDeletedHit);
            }
        }

        [Test]
        [RequiresSTA]
        public void TryRemoveNodeForData_NotRemoveable_ShowsDialog()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var onNodeRemovedHit = 0;
                var onDataDeletedHit = 0;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRemove = (o, p) => false,
                    OnNodeRemoved = (o, p) => onNodeRemovedHit++
                };
                treeViewControl.DataDeleted += (sender, args) => onDataDeletedHit++;
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                string messageBoxText = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new MessageBoxTester(wnd);

                    messageBoxText = helper.Text;

                    helper.ClickOk();
                };

                // Call
                treeViewControl.TryRemoveNodeForData(dataObject);

                // Assert
                Assert.AreEqual(0, onNodeRemovedHit);
                Assert.AreEqual(0, onDataDeletedHit);

                Assert.AreEqual(Resources.TreeViewControl_The_selected_item_cannot_be_removed, messageBoxText);
            }
        }

        [Test]
        [RequiresSTA]
        public void TryRemoveNodeForData_Removeable_OnNodeRemovedAndDataDeletedCalled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var onNodeRemovedHit = 0;
                var onDataDeletedHit = 0;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRemove = (o, p) => true,
                    OnNodeRemoved = (o, p) => onNodeRemovedHit++
                };
                treeViewControl.DataDeleted += (sender, args) => onDataDeletedHit++;
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    string messageBoxText = null;
                    DialogBoxHandler = (name, wnd) =>
                    {
                        var helper = new MessageBoxTester(wnd);

                        messageBoxText = helper.Text;

                        helper.ClickOk();
                    };

                    // Call
                    treeViewControl.TryRemoveNodeForData(dataObject);

                    // Assert
                    Assert.AreEqual(1, onNodeRemovedHit);
                    Assert.AreEqual(1, onDataDeletedHit);

                    Assert.AreEqual(Resources.TreeViewControl_Are_you_sure_you_want_to_remove_the_selected_item, messageBoxText);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void TryRemoveChildNodesOfData_CancelClicked_OnNodeRemovedOnChildrenNotCalled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var onNodeRemovedHit = 0;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    CanRemove = (o, p) => true,
                    OnNodeRemoved = (o, p) => onNodeRemovedHit++
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    string messageBoxText = null;
                    DialogBoxHandler = (name, wnd) =>
                    {
                        var helper = new MessageBoxTester(wnd);

                        messageBoxText = helper.Text;

                        helper.ClickCancel();
                    };

                    // Call
                    treeViewControl.TryRemoveChildNodesOfData(dataObject);

                    // Assert
                    Assert.AreEqual(0, onNodeRemovedHit);

                    Assert.AreEqual("Weet u zeker dat u de subonderdelen van het geselecteerde element wilt verwijderen?", messageBoxText);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void TryRemoveChildNodesOfData_OkClicked_OnNodeRemovedAndDataDeletedOnChildrenCalled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var onNodeRemovedHit = 0;
                var onDataDeletedHit = 0;

                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    CanRemove = (o, p) => true,
                    OnNodeRemoved = (o, p) => onNodeRemovedHit++
                };
                treeViewControl.DataDeleted += (sender, args) => onDataDeletedHit++;
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    string messageBoxText = null;
                    DialogBoxHandler = (name, wnd) =>
                    {
                        var helper = new MessageBoxTester(wnd);

                        messageBoxText = helper.Text;

                        helper.ClickOk();
                    };

                    // Call
                    treeViewControl.TryRemoveChildNodesOfData(dataObject);

                    // Assert
                    Assert.AreEqual(1, onNodeRemovedHit);
                    Assert.AreEqual(1, onDataDeletedHit);

                    Assert.AreEqual("Weet u zeker dat u de subonderdelen van het geselecteerde element wilt verwijderen?", messageBoxText);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void CanRemoveChildNodesOfData_NoChildren_ReturnsFalse()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    // Call
                    var result = treeViewControl.CanRemoveChildNodesOfData(dataObject);

                    // Assert
                    Assert.IsFalse(result);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void CanRemoveChildNodesOfData_NoRemovableChildren_ReturnsFalse()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    CanRemove = (o, p) => false
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    // Call
                    var result = treeViewControl.CanRemoveChildNodesOfData(dataObject);

                    // Assert
                    Assert.IsFalse(result);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void CanRemoveChildNodesOfData_OneRemovableChild_ReturnsTrue()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        0,
                        string.Empty,
                        1
                    }
                };
                var childStringTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    CanRemove = (o, p) => true
                };
                var childIntTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(int),
                    CanRemove = (o, p) => false
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childStringTreeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childIntTreeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    // Call
                    var result = treeViewControl.CanRemoveChildNodesOfData(dataObject);

                    // Assert
                    Assert.IsTrue(result);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void TryRemoveNodeForData_RemoveableCancelClicked_OnNodeRemovedNotCalled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var hit = 0;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRemove = (o, p) => true,
                    OnNodeRemoved = (o, p) => hit++
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var dataObject = new object();
                treeViewControl.Data = dataObject;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    string messageBoxText = null;
                    DialogBoxHandler = (name, wnd) =>
                    {
                        var helper = new MessageBoxTester(wnd);

                        messageBoxText = helper.Text;

                        helper.ClickCancel();
                    };

                    // Call
                    treeViewControl.TryRemoveNodeForData(dataObject);

                    // Assert
                    Assert.AreEqual(0, hit);

                    Assert.AreEqual(Resources.TreeViewControl_Are_you_sure_you_want_to_remove_the_selected_item, messageBoxText);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        public void CanExpandOrCollapseForData_NoDataForNull_ReturnsFalse()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

                // Call
                var result = treeViewControl.CanExpandOrCollapseForData(null);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void CanExpandOrCollapseForData_NoChildren_ReturnsFalse()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                // Call
                var result = treeViewControl.CanExpandOrCollapseForData(data);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void CanExpandOrCollapseForData_WithChildren_ReturnsTrue()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                // Call
                var result = treeViewControl.CanExpandOrCollapseForData(data);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void TryCollapseAllNodesForData_WithoutChildrenForNull_NoChange()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                var treeNode = treeView.Nodes[0];
                treeNode.Expand();

                // Call
                treeViewControl.TryCollapseAllNodesForData(null);

                // Assert
                Assert.IsTrue(treeNode.IsExpanded);
            }
        }

        [Test]
        public void TryCollapseAllNodesForData_WithoutChildren_Collapsed()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                var treeNode = treeView.Nodes[0];
                treeNode.Expand();

                // Call
                treeViewControl.TryCollapseAllNodesForData(data);

                // Assert
                Assert.IsFalse(treeNode.IsExpanded);
            }
        }

        [Test]
        public void TryCollapseAllNodesForData_WithChildren_CollapsesNodeAndChildren()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                var treeNode = treeView.Nodes[0];
                treeNode.Expand();
                var childNode = treeNode.Nodes[0];
                childNode.Expand();

                // Call
                treeViewControl.TryCollapseAllNodesForData(data);

                // Assert
                Assert.IsFalse(treeNode.IsExpanded);
                Assert.IsFalse(childNode.IsExpanded);
            }
        }

        [Test]
        public void TryExpandAllNodesForData_ForNull_NoChange()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                var treeNode = treeView.Nodes[0];
                treeNode.Collapse();

                // Call
                treeViewControl.TryExpandAllNodesForData(null);

                // Assert
                Assert.IsFalse(treeNode.IsExpanded);
            }
        }

        [Test]
        public void TryExpandAllNodesForData_WithoutChildren_Expanded()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                var treeNode = treeView.Nodes[0];
                treeNode.Collapse();

                // Call
                treeViewControl.TryExpandAllNodesForData(data);

                // Assert
                Assert.IsTrue(treeNode.IsExpanded);
            }
        }

        [Test]
        public void TryExpandAllNodesForData_WithChildren_ExpandsNodeAndChildren()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                var treeNode = treeView.Nodes[0];
                treeNode.Collapse();
                var childNode = treeNode.Nodes[0];
                childNode.Collapse();

                // Call
                treeViewControl.TryExpandAllNodesForData(data);

                // Assert
                Assert.IsTrue(treeNode.IsExpanded);
                Assert.IsTrue(childNode.IsExpanded);
            }
        }

        [Test]
        public void TrySelectNodeForData_NoData_SelectNull()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

                // Call
                treeViewControl.TrySelectNodeForData(null);

                // Assert
                Assert.IsNull(treeViewControl.SelectedData);
            }
        }

        [Test]
        public void TrySelectNodeForData_WithChildDataSelectRoot_RootSelected()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                // Call
                treeViewControl.TrySelectNodeForData(data);

                // Assert
                Assert.AreSame(data, treeViewControl.SelectedData);
            }
        }

        [Test]
        public void TrySelectNodeForData_WithChildDataSelectChild_ChildSelected()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                // Call
                treeViewControl.TrySelectNodeForData(string.Empty);

                // Assert
                Assert.AreSame(string.Empty, treeViewControl.SelectedData);
            }
        }

        [Test]
        public void TrySelectNodeForData_WithChildDataSelectNull_NoSelection()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string)
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                // Call
                treeViewControl.TrySelectNodeForData(null);

                // Assert
                Assert.IsNull(treeViewControl.SelectedData);
            }
        }

        [Test]
        public void TryGetPathForData_WithChildDataForRoot_ReturnsPathToRoot()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var rootText = "root";
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    },
                    Text = o => rootText
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    Text = o => "child"
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                // Call
                var result = treeViewControl.TryGetPathForData(data);

                // Assert
                Assert.AreEqual(rootText, result);
            }
        }

        [Test]
        public void TryGetPathForData_WithChildDataForChild_ReturnsPathToChild()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var rootText = "root";
                var childText = "child";
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    },
                    Text = o => rootText
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    Text = o => "child"
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];

                // Call
                var result = treeViewControl.TryGetPathForData(string.Empty);

                // Assert
                Assert.AreEqual(rootText + treeView.PathSeparator + childText, result);
            }
        }

        [Test]
        public void TryGetPathForData_WithChildDataForNull_ReturnsNull()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    },
                    Text = o => "root"
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    Text = o => "child"
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                // Call
                var result = treeViewControl.TryGetPathForData(null);

                // Assert
                Assert.IsNull(result);
            }
        }

        [Test]
        [RequiresSTA]
        public void SelectedDataChanged_ListenerNotSet_DoesNotThrow()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    },
                    Text = o => "root"
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    Text = o => "child"
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    // Call
                    TestDelegate test = () => treeViewControl.TrySelectNodeForData(string.Empty);

                    // Assert
                    Assert.DoesNotThrow(test);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void SelectedDataChanged_ListenerSetOnInitAndDataSetToNull_SelectedDataChangedInvokedOnce()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var hit = 0;
                treeViewControl.SelectedDataChanged += (sender, args) => hit++;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    },
                    Text = o => "root"
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    Text = o => "child"
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    // Call
                    treeViewControl.Data = null;

                    // Assert
                    Assert.AreEqual(1, hit);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void SelectedDataChanged_ListenerSetOnInitAndDataSetToObject_SelectedDataChangedInvokedOnce()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var hit = 0;
                treeViewControl.SelectedDataChanged += (sender, args) => hit++;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    },
                    Text = o => "root"
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    Text = o => "child"
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);

                    // Call
                    treeViewControl.Data = data;

                    // Assert
                    Assert.AreEqual(1, hit);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void SelectedDataChanged_ListenerSetOnSelectChild_SelectedDataChangedInvokedOnce()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var hit = 0;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        string.Empty
                    },
                    Text = o => "root"
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    Text = o => "child"
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);
                    treeViewControl.Data = data;
                    treeViewControl.SelectedDataChanged += (sender, args) => hit++;

                    // Call
                    treeViewControl.TrySelectNodeForData(string.Empty);

                    // Assert
                    Assert.AreEqual(1, hit);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void DataDoubleClick_ListenerSetOnNodeDoubleClick_DataDoubleClickInvoked()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var identifier = "identifier";
                var hit = 0;
                treeViewControl.DataDoubleClick += (n, o) => hit++;

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                treeView.Name = identifier;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);
                    var treeViewTester = new TreeViewTester(identifier);

                    // Call
                    treeViewTester.FireEvent("DoubleClick", new NodeLabelEditEventArgs(null, null));

                    // Assert
                    Assert.AreEqual(1, hit);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void DataDoubleClick_ListenerNotSetOnNodeDoubleClick_DoesNotThrow()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var identifier = "identifier";

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                treeView.Name = identifier;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);
                    var treeViewTester = new TreeViewTester(identifier);

                    // Call
                    TestDelegate test = () => treeViewTester.FireEvent("DoubleClick", new NodeLabelEditEventArgs(null, null));

                    // Assert
                    Assert.DoesNotThrow(test);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void BeforeLabelEdit_Always_CanRenameInvoked()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var identifier = "identifier";
                var hit = 0;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRename = (o, p) =>
                    {
                        hit++;
                        return false;
                    }
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.Data = new object();

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                treeView.Name = identifier;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);
                    var treeViewTester = new TreeViewTester(identifier);

                    // Call
                    treeViewTester.FireEvent("BeforeLabelEdit", new NodeLabelEditEventArgs(treeView.SelectedNode, null));

                    // Assert
                    Assert.AreEqual(1, hit);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void AfterLabelEdit_DependingOnNewLabel_OnNodeRenamedInvoked(bool newLabel)
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var identifier = "identifier";
                var hit = 0;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    CanRename = (o, p) => true,
                    OnNodeRenamed = (o, s) => hit++
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.Data = new object();

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                treeView.Name = identifier;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);
                    var treeViewTester = new TreeViewTester(identifier);

                    // Call
                    treeViewTester.FireEvent("AfterLabelEdit", new NodeLabelEditEventArgs(treeView.SelectedNode, newLabel ? string.Empty : null));

                    // Assert
                    Assert.AreEqual(newLabel ? 1 : 0, hit);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void OnNodeChecked_Always_InvokesOnNodeChecked()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var identifier = "identifier";
                var hit = 0;
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    OnNodeChecked = (o, p) => { hit++; }
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.Data = new object();

                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                treeView.Name = identifier;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);
                    var treeViewTester = new TreeViewTester(identifier);

                    // Call
                    treeViewTester.FireEvent("AfterCheck", new TreeViewEventArgs(treeView.SelectedNode));

                    // Assert
                    Assert.AreEqual(1, hit);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        public void GivenObservableDataOnTreeControl_WhenObserversNotified_ThenNodeForDataChanges()
        {
            // Given
            var observable = new TestObservable();

            using (var treeViewControl = new TreeViewControl())
            {
                var testString = "test";
                var expectedText = "newTest";

                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(IObservable),
                    Text = o => testString // Note: Access to modified closure intended!
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.Data = observable;

                var node = ((System.Windows.Forms.TreeView) treeViewControl.Controls[0]).Nodes[0];

                testString = expectedText;

                // When
                observable.NotifyObservers();

                // Then
                Assert.AreEqual(expectedText, node.Text);
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(MouseButtons.Left, false)]
        [TestCase(MouseButtons.Right, true)]
        [TestCase(MouseButtons.Middle, false)]
        [TestCase(MouseButtons.XButton1, false)]
        [TestCase(MouseButtons.XButton2, false)]
        public void GivenTreeViewControl_WhenMouseClickOnNode_SelectThatNodeForRightMouseButtons(
            MouseButtons mouseButtonDown, bool changeSelectionToTarget)
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var selectionTarget = "I'm the target!";
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        selectionTarget
                    }
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    Text = o => o.ToString()
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;
                treeViewControl.TryExpandAllNodesForData(data);

                // Precondition:
                Assert.AreSame(data, treeViewControl.SelectedData);

                var identifier = "identifier";
                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                treeView.Name = identifier;

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);
                    var treeViewTester = new TreeViewTester(identifier);

                    // When
                    treeViewTester.FireEvent("MouseClick", new MouseEventArgs(mouseButtonDown, 1, 60, 30, 0));

                    // Then
                    object expectedSelectedData = changeSelectionToTarget ? selectionTarget : data;
                    Assert.AreEqual(expectedSelectedData, treeViewControl.SelectedData);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenTreeViewControl_WhenTreeViewItemDragOnNode_SelectThatNode(
            bool canRenameNode)
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var selectionTarget = "I'm the target!";
                var treeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ChildNodeObjects = o => new object[]
                    {
                        selectionTarget
                    },
                    CanRename = (d, p) => canRenameNode
                };
                var childTreeNodeInfo = new TreeNodeInfo
                {
                    TagType = typeof(string),
                    Text = o => o.ToString()
                };
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
                var data = new object();
                treeViewControl.Data = data;
                treeViewControl.TryExpandAllNodesForData(data);

                // Precondition:
                Assert.AreSame(data, treeViewControl.SelectedData);

                var identifier = "identifier";
                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                treeView.Name = identifier;
                var childNode = treeView.Nodes[0].Nodes[0];

                try
                {
                    WindowsFormsTestHelper.Show(treeViewControl);
                    var treeViewTester = new TreeViewTester(identifier);

                    // When
                    treeViewTester.FireEvent("ItemDrag", new ItemDragEventArgs(MouseButtons.Left, childNode));

                    // Then
                    object expectedSelectedData = selectionTarget;
                    Assert.AreEqual(expectedSelectedData, treeViewControl.SelectedData);
                }
                finally
                {
                    WindowsFormsTestHelper.CloseAll();
                }
            }
        }

        [Test]
        public void OnNodeAddedForData_TreeNodeInfoRegisteredWithExpandOnCreateNull_NodeNotExpanded()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ExpandOnCreate = null
                });

                // Call
                treeViewControl.Data = new object();

                // Assert
                var treeView = (System.Windows.Forms.TreeView)treeViewControl.Controls[0];
                var treeNode = treeView.Nodes[0];
                Assert.IsFalse(treeNode.IsExpanded);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeAddedForData_TreeNodeInfoRegisteredWithExpandOnCreateSet_NodeExpandedAccordingly(bool expandOnCreate)
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo
                {
                    TagType = typeof(object),
                    ExpandOnCreate = o => expandOnCreate
                });

                // Call
                treeViewControl.Data = new object();

                // Assert
                var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
                var treeNode = treeView.Nodes[0];
                Assert.AreEqual(expandOnCreate, treeNode.IsExpanded);
            }
        }
    }

    public class TestObservable : Observable {}
}