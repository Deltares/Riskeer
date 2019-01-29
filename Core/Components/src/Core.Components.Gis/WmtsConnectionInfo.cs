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

namespace Core.Components.Gis
{
    /// <summary>
    /// This class defines properties for a WMTS connection.
    /// </summary>
    public class WmtsConnectionInfo : IEquatable<WmtsConnectionInfo>
    {
        /// <summary>
        /// Creates a new instance of <see cref="WmtsConnectionInfo"/>.
        /// </summary>
        /// <param name="name">The name associated with the <paramref name="url"/>.</param>
        /// <param name="url">The WMTS URL.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="url"/> is <c>null</c>, 
        /// <see cref="string.Empty"/>, or consists exclusively of white-space characters.</exception>
        public WmtsConnectionInfo(string name, string url)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException($@"{nameof(url)} must have a value.", nameof(url));
            }

            Name = name;
            Url = url;
        }

        /// <summary>
        /// Gets the name associated with the URL.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        public string Url { get; }

        #region IEquatable members

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ Name.GetHashCode() ^ Url.GetHashCode();
        }

        public bool Equals(WmtsConnectionInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return string.Equals(Name, other.Name) && string.Equals(Url, other.Url);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WmtsConnectionInfo);
        }

        #endregion
    }
}