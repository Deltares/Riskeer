﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections;
using System.Linq;
using System.Windows.Forms;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Object that allows for grouping child nodes of <see cref="TreeNode"/> instances.
    /// </summary>
    public class CategoryTreeFolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryTreeFolder"/> class.
        /// </summary>
        /// <param name="name">The name of the category folder.</param>
        /// <param name="contents">The contents of the folder.</param>
        /// <param name="category">Optional: The category descriptor of the folder. Default: <see cref="TreeFolderCategory.General"/>.</param>
        public CategoryTreeFolder(string name, IList contents, TreeFolderCategory category = TreeFolderCategory.General)
        {
            Name = name;
            Contents = contents.OfType<object>().ToArray();
            Category = category;
        }

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the contents of the folder.
        /// </summary>
        public IList Contents { get; }

        /// <summary>
        /// Gets the category of the folder.
        /// </summary>
        public TreeFolderCategory Category { get; private set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((CategoryTreeFolder) obj);
        }

        public override int GetHashCode()
        {
            return Contents.Cast<object>().Aggregate(Name != null ? Name.GetHashCode() : 0, (current, content) => current ^ content.GetHashCode());
        }

        private bool Equals(CategoryTreeFolder other)
        {
            if (Name != other.Name)
            {
                return false;
            }

            if (Contents.Count != other.Contents.Count)
            {
                return false;
            }

            for (var i = 0; i < Contents.Count; i++)
            {
                if (!Contents[i].Equals(other.Contents[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}