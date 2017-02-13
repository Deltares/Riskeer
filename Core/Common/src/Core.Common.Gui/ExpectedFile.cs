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

namespace Core.Common.Gui
{
    /// <summary>
    /// Class which produces a file filter based on the expected extension. If no specific
    /// extension is expected, then a filter for all file types will be produced.
    /// </summary>
    public class ExpectedFile {

        private readonly string extension = "*";
        private readonly string description = "Alle bestanden";

        /// <summary>
        /// Creates a new instance of <see cref="ExpectedFile"/> which filters
        /// for all files.
        /// </summary>
        public ExpectedFile()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ExpectedFile"/> which filters files based on
        /// a specified file extension.
        /// </summary>
        /// <param name="typeExtension">The extension of the files to filter on.</param>
        public ExpectedFile(string typeExtension)
        {
            extension = typeExtension;
            description = $"{typeExtension.ToUpperInvariant()}-bestanden";
        }

        /// <summary>
        /// Creates a new instance of <see cref="ExpectedFile"/> which filters files based on
        /// a specified file extension.
        /// </summary>
        /// <param name="typeExtension">The extension of the files to filter on.</param>
        /// <param name="typeDescription">The description of files which have 
        /// <paramref name="typeExtension"/> as their extension.</param>
        public ExpectedFile(string typeExtension, string typeDescription)
        {
            description = typeDescription;
            extension = typeExtension;
        }

        /// <summary>
        /// Gets a filter string for the <see cref="ExpectedFile"/>.
        /// </summary>
        public string Filter
        {
            get
            {
                return $"{description} (*.{extension})|*.{extension}";
            }
        }
    }
}