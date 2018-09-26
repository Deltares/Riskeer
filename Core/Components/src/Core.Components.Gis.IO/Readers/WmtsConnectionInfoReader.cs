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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Core.Components.Gis.IO.Properties;
using log4net;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Core.Components.Gis.IO.Readers
{
    /// <summary>
    /// Reader class for <see cref="WmtsConnectionInfo"/>.
    /// </summary>
    public class WmtsConnectionInfoReader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WmtsConnectionInfoReader));

        /// <summary>
        /// Reads the WMTS Connection info objects from <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The file path that contains the <see cref="WmtsConnectionInfo"/> information.</param>
        /// <returns>The read <see cref="WmtsConnectionInfo"/> information.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="path"/> could not successfully be read.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public ReadOnlyCollection<WmtsConnectionInfo> ReadWmtsConnectionInfos(string path)
        {
            IOUtils.ValidateFilePath(path);

            if (!File.Exists(path))
            {
                return new ReadOnlyCollection<WmtsConnectionInfo>(new WmtsConnectionInfo[0]);
            }

            try
            {
                return ReadWmtsConnectionInfosFromFile(path);
            }
            catch (Exception exception) when (exception is XmlException
                                              || exception is InvalidOperationException
                                              || exception is IOException)
            {
                string message = new FileReaderErrorMessageBuilder(path)
                    .Build(CoreCommonUtilResources.Error_General_IO_Import_ErrorMessage);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Reads the default WMTS Connection info objects.
        /// </summary>
        /// <returns>The read <see cref="WmtsConnectionInfo"/> information.</returns>
        public ReadOnlyCollection<WmtsConnectionInfo> ReadDefaultWmtsConnectionInfos()
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(Resources.defaultWmtsConnectionInfo)))
            {
                return ReadWmtsConnectionInfosFromReader(reader, CreateWmtsConnectionInfo);
            }
        }

        /// <summary>
        /// Reads the collection of <see cref="WmtsConnectionInfo"/> from <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The file path that contains the <see cref="WmtsConnectionInfo"/> information.</param>
        /// <returns>The read collection.</returns>
        /// <exception cref="XmlException">Thrown when an error occurred while parsing the XML.</exception>
        /// <exception cref="InvalidOperationException">Thrown when an error occurred while reading the XML.</exception>
        private static ReadOnlyCollection<WmtsConnectionInfo> ReadWmtsConnectionInfosFromFile(string path)
        {
            using (XmlReader reader = XmlReader.Create(path))
            {
                return ReadWmtsConnectionInfosFromReader(reader, element => TryCreateWmtsConnectionInfo(path, element));
            }
        }

        /// <summary>
        /// Reads the collection of <see cref="WmtsConnectionInfo"/> from an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="tryParseWmtsConnectionElement">Method responsible to turning an
        /// <see cref="XElement"/> into a <see cref="WmtsConnectionInfo"/>. This method may
        /// return <c>null</c> or throw any type of <see cref="Exception"/>.</param>
        /// <returns></returns>
        /// <remarks>This method only throws exceptions thrown by <paramref name="tryParseWmtsConnectionElement"/>.</remarks>
        /// <exception cref="XmlException">Thrown when an error occurred while parsing the XML.</exception>
        /// <exception cref="InvalidOperationException">Thrown when an error occurred while reading the XML.</exception>
        private static ReadOnlyCollection<WmtsConnectionInfo> ReadWmtsConnectionInfosFromReader(XmlReader reader,
                                                                                                Func<XElement, WmtsConnectionInfo> tryParseWmtsConnectionElement)
        {
            var connectionInfos = new List<WmtsConnectionInfo>();
            while (reader.Read())
            {
                if (IsReadElementWmtsConnectionElement(reader))
                {
                    continue;
                }

                WmtsConnectionInfo readWmtsConnectionElement;
                using (XmlReader subtreeReader = reader.ReadSubtree())
                {
                    XElement wmtsConnectionElement = XElement.Load(subtreeReader);

                    readWmtsConnectionElement = tryParseWmtsConnectionElement(wmtsConnectionElement);
                }

                if (readWmtsConnectionElement != null)
                {
                    connectionInfos.Add(readWmtsConnectionElement);
                }
            }

            return new ReadOnlyCollection<WmtsConnectionInfo>(connectionInfos);
        }

        /// <summary>
        /// Validates if the reader points to the <see cref="WmtsConnectionInfoXmlDefinitions.WmtsConnectionElement"/> element.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns><c>true</c> if the reader points to the WMTS connection element, <c>false</c> otherwise.</returns>
        /// <exception cref="XmlException">Thrown when the input stream encountered incorrect XML.</exception>
        private static bool IsReadElementWmtsConnectionElement(XmlReader reader)
        {
            return reader.NodeType != XmlNodeType.Element
                   || !reader.IsStartElement()
                   || reader.Name != WmtsConnectionInfoXmlDefinitions.WmtsConnectionElement;
        }

        private static WmtsConnectionInfo TryCreateWmtsConnectionInfo(string path, XContainer element)
        {
            XElement nameElement = element.Element(WmtsConnectionInfoXmlDefinitions.WmtsConnectionNameElement);
            XElement urlElement = element.Element(WmtsConnectionInfoXmlDefinitions.WmtsConnectionUrlElement);

            if (nameElement == null || urlElement == null)
            {
                return null;
            }

            try
            {
                return new WmtsConnectionInfo(nameElement.Value, urlElement.Value);
            }
            catch (ArgumentException exception)
            {
                string errorMessage = string.Format(Resources.WmtsConnectionInfoReader_Unable_To_Create_WmtsConnectionInfo,
                                                    nameElement.Value,
                                                    urlElement.Value);
                string message = new FileReaderErrorMessageBuilder(path).Build(errorMessage);

                log.Warn(message, exception);
            }

            return null;
        }

        private static WmtsConnectionInfo CreateWmtsConnectionInfo(XContainer element)
        {
            XElement nameElement = element.Element(WmtsConnectionInfoXmlDefinitions.WmtsConnectionNameElement);
            XElement urlElement = element.Element(WmtsConnectionInfoXmlDefinitions.WmtsConnectionUrlElement);

            return new WmtsConnectionInfo(nameElement.Value, urlElement.Value);
        }
    }
}