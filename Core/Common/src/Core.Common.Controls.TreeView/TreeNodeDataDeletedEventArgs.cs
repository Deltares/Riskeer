// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Event arguments to be used in the event that node data has been deleted from a <see cref="TreeView"/>.
    /// </summary>
    public class TreeNodeDataDeletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeDataDeletedEventArgs"/> class.
        /// </summary>
        /// <param name="deletedDataInstance">The deleted data instance.</param>
        public TreeNodeDataDeletedEventArgs(object deletedDataInstance)
        {
            DeletedDataInstance = deletedDataInstance;
        }

        /// <summary>
        /// Gets the data instance deleted from the <see cref="TreeView"/>.
        /// </summary>
        public object DeletedDataInstance { get; private set; }
    }
}