using System;
using System.Collections;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;

namespace Core.Common.Base.Tests.TestObjects
{
    public class ChildNodePresenter : TreeViewNodePresenterBase<Child>
    {
        public event EventHandler AfterUpdate;

        /// <summary>
        /// Specifies if node can be deleted by user
        /// </summary>
        protected override bool CanRemove(Child nodeData)
        {
            return true;
        }

        protected override bool RemoveNodeData(object parentNodeData, Child nodeData)
        {
            if (parentNodeData is Parent)
            {
                ((Parent)parentNodeData).Children.Remove(nodeData);
                ((Parent)parentNodeData).NotifyObservers();
            }
            else
            {
                if (parentNodeData is Child)
                {
                    ((Child)parentNodeData).Children.Remove(nodeData);
                    ((Child)parentNodeData).NotifyObservers();
                }
            }

            return true;
        }

        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, Child nodeData)
        {
            node.Text = nodeData.Name;

            if (AfterUpdate != null)
            {
                AfterUpdate(this, null);
            }
        }

        public override IEnumerable GetChildNodeObjects(Child parentNodeData)
        {
            return parentNodeData.Children;
        }
    }
}