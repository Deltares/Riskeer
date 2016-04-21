// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
    /// <summary>
    /// This class provides an initial configuration of <see cref="TreeNodeInfo"/> for
    /// <see cref="IFailureMechanism"/> related presentation objects.
    /// </summary>
    /// <typeparam name="TContext">The type of the presentation object for the failure mechanism.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism.</typeparam>
    public class DefaultFailureMechanismTreeNodeInfo<TContext, TFailureMechanism> : TreeNodeInfo<TContext> where TContext : FailureMechanismContext<TFailureMechanism> where TFailureMechanism : IFailureMechanism
    {
        private readonly Func<TContext, object[]> getEnabledFailureMechanismChildNodes;
        private readonly Func<TContext, object, TreeViewControl, ContextMenuStrip> getEnabledFailureMechanismContextMenuStrip;
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFailureMechanismTreeNodeInfo{TContext, TFailureMechanism}"/> class.
        /// </summary>
        /// <param name="getEnabledFailureMechanismChildNodes">The implementation of <see cref="TreeNodeInfo.ChildNodeObjects"/>
        /// when <see cref="IFailureMechanism.IsRelevant"/> is <c>true</c>. Can be null.</param>
        /// <param name="getEnabledFailureMechanismContextMenuStrip">The implementation of
        /// <see cref="TreeNodeInfo.ContextMenuStrip"/> when <see cref="IFailureMechanism.IsRelevant"/>
        /// is <c>true</c>. Can be null.</param>
        /// <param name="provider">The provider of the <see cref="IContextMenuBuilder"/>.</param>
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