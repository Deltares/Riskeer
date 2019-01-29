// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Windows.Input;
using Core.Components.GraphSharp.Data;

namespace Core.Components.GraphSharp.Commands
{
    /// <summary>
    /// Command that is executed when a <see cref="PointedTreeElementVertex"/> is selected.
    /// </summary>
    public class VertexSelectedCommand : ICommand
    {
        private readonly PointedTreeElementVertex vertex;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="VertexSelectedCommand"/>.
        /// </summary>
        /// <param name="vertex">The <see cref="PointedTreeElementVertex"/>
        /// to create the command for.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="vertex"/> is <c>null</c>.</exception>
        public VertexSelectedCommand(PointedTreeElementVertex vertex)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            this.vertex = vertex;
        }

        public void Execute(object parameter)
        {
            vertex.IsSelected = true;
        }

        public bool CanExecute(object parameter)
        {
            return vertex.IsSelectable;
        }
    }
}