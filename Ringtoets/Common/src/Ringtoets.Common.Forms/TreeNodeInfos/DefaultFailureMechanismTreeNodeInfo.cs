using System;
using System.Drawing;
using System.Windows.Forms;

using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.TreeNodeInfos
{
    public class DefaultFailureMechanismTreeNodeInfo<TContext, TFailureMechanism> : TreeNodeInfo<TContext> where TContext : FailureMechanismContext<TFailureMechanism> where TFailureMechanism: IFailureMechanism
    {
        private readonly Func<TContext, object[]> getEnabledFailureMechanismChildNodes;
        private readonly Func<TContext, object, TreeViewControl, ContextMenuStrip> getEnabledFailureMechanismContextMenuStrip;
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;

        public DefaultFailureMechanismTreeNodeInfo(Func<TContext, object[]> getEnabledFailureMechanismChildNodes,
            Func<TContext, object, TreeViewControl, ContextMenuStrip> getEnabledFailureMechanismContextMenuStrip,
            IContextMenuBuilderProvider provider)
        {
            this.getEnabledFailureMechanismChildNodes = getEnabledFailureMechanismChildNodes;
            this.getEnabledFailureMechanismContextMenuStrip = getEnabledFailureMechanismContextMenuStrip;
            contextMenuBuilderProvider = provider;

            Text = GetNodeText;
            Image = GetImage;
            ForeColor = GetForeColor;
            ChildNodeObjects = GetChildNodeObjects;
            ContextMenuStrip = GetContextMenuStrip;
        }

        private ContextMenuStrip GetContextMenuStrip(TContext context, object parent, TreeViewControl treeView)
        {
            if (context.WrappedData.IsRelevant && getEnabledFailureMechanismContextMenuStrip != null)
            {
                return getEnabledFailureMechanismContextMenuStrip(context, parent, treeView);
            }
            return contextMenuBuilderProvider.Get(context, treeView)
                                             .AddExpandAllItem()
                                             .AddCollapseAllItem()
                                             .Build();
        }

        private object[] GetChildNodeObjects(TContext failureMechanismContext)
        {
            if (failureMechanismContext.WrappedData.IsRelevant)
            {
                return getEnabledFailureMechanismChildNodes != null ? 
                    getEnabledFailureMechanismChildNodes(failureMechanismContext) : 
                    new object[0];
            }

            return GetDisbledFailureMechanismChildNodeObjects(failureMechanismContext);
        }

        private static object[] GetDisbledFailureMechanismChildNodeObjects(TContext failureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(failureMechanismContext.WrappedData)
            };
        }

        private Color GetForeColor(TContext failureMechanismContext)
        {
            return failureMechanismContext.WrappedData.IsRelevant ?
                       Color.FromKnownColor(KnownColor.ControlText) :
                       Color.FromKnownColor(KnownColor.GrayText);
        }

        private Image GetImage(TContext failureMechanismContext)
        {
            return Resources.FailureMechanismIcon;
        }

        private string GetNodeText(TContext failureMechanismContext)
        {
            return failureMechanismContext.WrappedData.Name;
        }
    }
}