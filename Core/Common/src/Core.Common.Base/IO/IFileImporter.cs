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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Drawing;

namespace Core.Common.Base.IO
{
    /// <summary>
    /// Interface for data import from external formats.
    /// </summary>
    public interface IFileImporter
    {
        /// <summary>
        /// Gets the name of the <see cref="IFileImporter"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the category of the <see cref="IFileImporter"/>.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Gets the image of the <see cref="IFileImporter"/>.
        /// </summary>
        /// <remarks>This image can be used in selection and/or progress dialogs.</remarks>
        Bitmap Image { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the item supported by the <see cref="IFileImporter"/>.
        /// </summary>
        Type SupportedItemType { get; }

        /// <summary>
        /// Gets the file filter of the <see cref="IFileImporter"/>.
        /// </summary>
        /// <example>
        /// An example string would be:
        /// <code>"My file format1 (*.ext1)|*.ext1|My file format2 (*.ext2)|*.ext2"</code>
        /// </example>
        string FileFilter { get; }

        /// <summary>
        /// Sets the action to perform when progress has changed.
        /// </summary>
        ProgressChangedDelegate ProgressChanged { set; }

        /// <summary>
        /// This method imports the data to an item from a file at the given location.
        /// </summary>
        /// <param name="targetItem">The item to perform the import on.</param>
        /// <param name="filePath">The path of the file to import the data from.</param>
        /// <returns><c>true</c> if the import was successful. <c>false</c> otherwise.</returns>
        /// <remarks>Implementations of this import method are allowed to throw exceptions of any kind.</remarks>
        bool Import(object targetItem, string filePath);

        /// <summary>
        /// This method cancels an import.
        /// </summary>
        void Cancel();
    }
}