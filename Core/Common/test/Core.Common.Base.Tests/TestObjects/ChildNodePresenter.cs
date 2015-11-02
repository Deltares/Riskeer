﻿using System;
using System.Collections;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;

namespace Core.Common.Base.Tests.TestObjects
{
    public class ChildNodePresenter : TreeViewNodePresenterBase<Child>
    {
        public event EventHandler AfterUpdate;

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Child nodeData)
        {
            node.Text = nodeData.Name;

            if (AfterUpdate != null)
            {
                AfterUpdate(this, null);
            }
        }

        public override IEnumerable GetChildNodeObjects(Child parentNodeData, ITreeNode node)
        {
            return parentNodeData.Children;
        }
    }
}