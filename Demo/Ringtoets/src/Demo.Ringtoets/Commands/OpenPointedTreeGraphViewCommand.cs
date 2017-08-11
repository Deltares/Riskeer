// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
            var diamondStyle = new GraphNodeStyle(GraphNodeShape.Diamond, Color.BlanchedAlmond, Color.Black, 1);
            var lastNodeStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.LightSkyBlue, Color.Black, 1);
            var treeNodeStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.LightGray, Color.Black, 1);

            var doubleUsedNode = new GraphNode("Double <bold>used</bold>", new[]
            {
                new GraphNode("En", new []
                {
                    new GraphNode("Child 2", new GraphNode[0], false, lastNodeStyle), 
                    new GraphNode("Child 3", new GraphNode[0], false, lastNodeStyle), 
                }, false, diamondStyle),
                new GraphNode("Child 4", new GraphNode[0], false, lastNodeStyle), 
            }, false, treeNodeStyle);

            var node = new GraphNode("Root", new[]
            {
                new GraphNode("Of", new[]
                {
                    doubleUsedNode,
                    new GraphNode("Child 1", new[]
                    {
                        new GraphNode("Of", new[]
                        {
                            new GraphNode("Child 5", new GraphNode[0], false, lastNodeStyle),
                            doubleUsedNode
                        }, false, diamondStyle),
                    }, false, treeNodeStyle)
                }, false, diamondStyle)
            }, false, treeNodeStyle);

            viewCommands.OpenView(node);
        }
    }
}