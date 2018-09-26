// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using Core.Common.Util.Properties;

namespace Core.Common.Util
{
    /// <summary>
    /// Class which produces a file filter based on the expected extension. If no specific
    /// extension is expected, then a filter which filter out no file type will be produced.
    /// </summary>
    public class FileFilterGenerator
    {
        /// <summary>
        /// Creates a new instance of <see cref="FileFilterGenerator"/> which filters out no
        /// file type.
        /// </summary>
        public FileFilterGenerator()
        {
            Extension = "*";
            Description = Resources.FileFilterGenerator_All_Files;
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileFilterGenerator"/> which filters files based on
        /// a specified file extension.
        /// </summary>
        /// <param name="typeExtension">The extension of the files to filter on.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="typeExtension"/>
        /// is <c>null</c> or empty.</exception>
        public FileFilterGenerator(string typeExtension)
        {
            if (string.IsNullOrEmpty(typeExtension))
            {
                throw new ArgumentException($@"Value required for the '{nameof(typeExtension)}'.", nameof(typeExtension));
            }

            Extension = typeExtension;
            Description = string.Format(Resources.FileFilterGenerator_Files_of_type_0_, typeExtension.ToUpperInvariant());
        }

        /// <summary>
        /// Creates a new instance of <see cref="FileFilterGenerator"/> which filters files based on
        /// a specified file extension.
        /// </summary>
        /// <param name="typeExtension">The extension of the files to filter on.</param>
        /// <param name="typeDescription">The description of files which have 
        /// <paramref name="typeExtension"/> as their extension.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="typeExtension"/>
        /// or <paramref name="typeDescription"/> is <c>null</c> or empty.</exception>
        public FileFilterGenerator(string typeExtension, string typeDescription)
        {
            if (string.IsNullOrEmpty(typeExtension))
            {
                throw new ArgumentException($@"Value required for the '{nameof(typeExtension)}'.", nameof(typeExtension));
            }

            if (string.IsNullOrEmpty(typeDescription))
            {
                throw new ArgumentException($@"Value required for the '{nameof(typeDescription)}'.", nameof(typeDescription));
            }

            Description = typeDescription;
            Extension = typeExtension;
        }

        /// <summary>
        /// Gets the extension of the <see cref="FileFilterGenerator"/>.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Gets the description of the <see cref="FileFilterGenerator"/>.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets a filter string for the <see cref="FileFilterGenerator"/>.
        /// </summary>
        public string Filter
        {
            get
            {
                return string.Format(Resources.FileFilterGenerator_File_filter_format, Description, Extension);
            }
        }

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

            return Equals((FileFilterGenerator) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Extension.GetHashCode() * 397) ^ Description.GetHashCode();
            }
        }

        private bool Equals(FileFilterGenerator other)
        {
            return string.Equals(Extension, other.Extension) && string.Equals(Description, other.Description);
        }
    }
}