// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Ringtoets.Piping.IO.SoilProfile;

namespace Ringtoets.Piping.IO.Test.TestHelpers
{
    /// <summary>
    /// This class helps to create parameters for testing the <see cref="SoilLayer2DReader"/>.
    /// </summary>
    public static class StringGeometryHelper
    {
        /// <summary>
        /// Takes a <paramref name="str"/> which describes an XML document and returns 
        /// an <see cref="XDocument"/> from this.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to convert to an <see cref="XDocument"/>.</param>
        /// <returns>The <see cref="XDocument"/> constructed from <paramref name="str"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="str"/> is <c>null</c>.</exception>
        /// <exception cref="XmlException">Thrown when <paramref name="str"/> does not describe
        /// a valid XML document.</exception>
        public static XDocument GetXmlDocument(string str)
        {
            return XDocument.Load(new MemoryStream(GetByteArray(str)));
        }

        /// <summary>
        /// Takes a <paramref name="str"/> and returns an <see cref="Array"/> of <see cref="byte"/>,
        /// which contains the same information as the original <paramref name="str"/>.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to convert to an <see cref="Array"/> of 
        /// <see cref="byte"/>.</param>
        /// <returns>The <see cref="Array"/> of <see cref="byte"/> constructed from 
        /// <paramref name="str"/>.</returns>
        public static byte[] GetByteArray(string str)
        {
            byte[] bytes = new byte[str.Length*sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}