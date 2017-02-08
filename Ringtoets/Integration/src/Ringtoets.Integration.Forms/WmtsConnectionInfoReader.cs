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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Core.Common.Utils;

namespace Ringtoets.Integration.Forms
{
    /// <summary>
    /// Reader class for <see cref="Forms.WmtsConnectionInfo"/>.
    /// </summary>
    public class WmtsConnectionInfoReader
    {
        /// <summary>
        /// Reads the WMTS Connection infos from <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path that contains the <see cref="Forms.WmtsConnectionInfo"/> information.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public List<WmtsConnectionInfo> ReadWmtsConnectionInfos(string filePath)
        {
            IOUtils.ValidateFilePath(filePath);

            var connectionInfos = new List<WmtsConnectionInfo>();
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element || !reader.IsStartElement() || reader.Name != WmtsConnectionInfoXmlDefinitions.WmtsConnectionElement)
                    {
                        continue;
                    }

                    connectionInfos.Add(ReadWmtsConnectionElement(reader));
                }
            }
            return connectionInfos;
        }

        private static WmtsConnectionInfo ReadWmtsConnectionElement(XmlReader reader)
        {
            using (XmlReader subtreeReader = reader.ReadSubtree())
            {
                XElement wmtsConnectionElement = XElement.Load(subtreeReader);

                WmtsConnectionInfo readWmtsConnectionInfo = WmtsConnectionInfo(wmtsConnectionElement);
                if (readWmtsConnectionInfo != null)
                {
                    return readWmtsConnectionInfo;
                }
            }
            return null;
        }

        private static WmtsConnectionInfo WmtsConnectionInfo(XContainer element)
        {
            XElement nameElement = element.Element(WmtsConnectionInfoXmlDefinitions.WmtsConnectionNameElement);
            XElement urlElement = element.Element(WmtsConnectionInfoXmlDefinitions.WmtsConnectionUrlElement);

            if (nameElement == null || urlElement == null)
            {
                return null;
            }
            return new WmtsConnectionInfo(nameElement.Value, urlElement.Value);
        }
    }
}