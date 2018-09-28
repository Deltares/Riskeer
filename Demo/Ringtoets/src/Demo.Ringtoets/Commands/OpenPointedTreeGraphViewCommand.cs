// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.Commands;
using Core.Common.Gui.Commands;
using Core.Components.PointedTree.Data;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// This class describes the command for opening a view for <see cref="GraphNode"/> with some arbitrary data.
    /// </summary>
    public class OpenPointedTreeGraphViewCommand : ICommand
    {
        private readonly IViewCommands viewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="OpenPointedTreeGraphViewCommand"/>.
        /// </summary>
        /// <param name="viewCommands">The <see cref="IViewCommands"/> to use internally.</param>
        public OpenPointedTreeGraphViewCommand(IViewCommands viewCommands)
        {
            this.viewCommands = viewCommands;
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public void Execute()
        {
            var connectingNodeStyle = new GraphNodeStyle(GraphNodeShape.None, Color.BlanchedAlmond, Color.Black, 1);
            var lastNodeStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.LightSkyBlue, Color.Black, 1);
            var treeNodeStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.LightGray, Color.Black, 1);

            var doubleUsedNode = new GraphNode("<text>Double " + Environment.NewLine +
                                               "<bold>used</bold></text>", new[]
            {
                new GraphNode("<text>En</text>", new[]
                {
                    new GraphNode("<text>Child 2</text>", new GraphNode[0], true, lastNodeStyle),
                    new GraphNode("<text>Child 3</text>", new GraphNode[0], true, lastNodeStyle)
                }, false, connectingNodeStyle),
                new GraphNode("<text>Child 4</text>", new GraphNode[0], true, lastNodeStyle)
            }, true, treeNodeStyle);

            var node = new GraphNode("<text>Root</text>", new[]
            {
                new GraphNode("<text>Of</text>", new[]
                {
                    doubleUsedNode,
                    new GraphNode("<text>Child 1</text>", new[]
                    {
                        new GraphNode("<text>Of</text>", new[]
                        {
                            new GraphNode("<text>Child 5</text>", new GraphNode[0], true, lastNodeStyle),
                            doubleUsedNode
                        }, false, connectingNodeStyle)
                    }, false, treeNodeStyle)
                }, false, connectingNodeStyle)
            }, false, treeNodeStyle);

            viewCommands.OpenView(node);
        }
    }
}