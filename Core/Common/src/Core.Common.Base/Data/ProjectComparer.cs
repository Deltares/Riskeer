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

using System.Linq;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// This class can be used to compare <see cref="Project"/> objects.
    /// </summary>
    public static class ProjectComparer
    {
        /// <summary>
        /// Checks if <paramref name="other"/> is equal to a new instance of <see cref="Project"/>.
        /// </summary>
        /// <param name="other"><see cref="Project"/> to check.</param>
        /// <returns><c>True</c> if <paramref name="other"/> is equal to a new instance of <see cref="Project"/>, <c>false</c> otherwise.</returns>
        public static bool EqualsToNew(Project other)
        {
            var newProject = new Project();
            return Equals(newProject, other);
        }

        private static bool Equals(Project x, Project y)
        {
            return string.Equals(x.Name, y.Name) &&
                   string.Equals(x.Description, y.Description) &&
                   x.StorageId == y.StorageId &&
                   x.Items.SequenceEqual(y.Items);
        }
    }
}