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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using log4net;
using Ringtoets.Integration.Forms.Properties;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Integration.Forms
{
    /// <summary>
    /// Reader class for <see cref="Forms.WmtsConnectionInfo"/>.
    /// </summary>
    public class WmtsConnectionInfoReader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WmtsConnectionInfoReader));
        private string filePath;

        /// <summary>
        /// Reads the WMTS Connection info objects from <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The file path that contains the <see cref="Forms.WmtsConnectionInfo"/> information.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="path"/> could not successfully be read.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public IEnumerable<WmtsConnectionInfo> ReadWmtsConnectionInfos(string path)
        {
            filePath = path;
            ValidateFilePath();

            try
            {
                return ReadWmtsConnectionInfos();
            }
            catch (Exception exception) when (exception is XmlException || exception is IOException)
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilsResources.Error_General_IO_Import_ErrorMessage);
                throw new CriticalFileReadException(message, exception);
            }
        }

        private void ValidateFilePath()
        {
            IOUtils.ValidateFilePath(filePath);
            if (!File.Exists(filePath))
            {
                string message = new FileReaderErrorMessageBuilder(filePath).Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }
        }

        private IEnumerable<WmtsConnectionInfo> ReadWmtsConnectionInfos()
        {
            var connectionInfos = new List<WmtsConnectionInfo>();
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element
                        || !reader.IsStartElement()
                        || reader.Name != WmtsConnectionInfoXmlDefinitions.WmtsConnectionElement)
                    {
                        continue;
                    }

                    WmtsConnectionInfo readWmtsConnectionElement = ReadWmtsConnectionElement(reader);
                    if (readWmtsConnectionElement != null)
                    {
                        connectionInfos.Add(readWmtsConnectionElement);
                    }
                }
            }
            return connectionInfos;
        }

        private WmtsConnectionInfo ReadWmtsConnectionElement(XmlReader reader)
        {
            using (XmlReader subtreeReader = reader.ReadSubtree())
            {
                XElement wmtsConnectionElement = XElement.Load(subtreeReader);

                return WmtsConnectionInfo(wmtsConnectionElement);
            }
        }

        private WmtsConnectionInfo WmtsConnectionInfo(XContainer element)
        {
            XElement nameElement = element.Element(WmtsConnectionInfoXmlDefinitions.WmtsConnectionNameElement);
            XElement urlElement = element.Element(WmtsConnectionInfoXmlDefinitions.WmtsConnectionUrlElement);

            if (nameElement == null || urlElement == null)
            {
                return null;
            }

            return TryCreateWmtsConnectionInfo(nameElement.Value, urlElement.Value);
        }

        private WmtsConnectionInfo TryCreateWmtsConnectionInfo(string name, string url)
        {
            try
            {
                return new WmtsConnectionInfo(name, url);
            }
            catch (ArgumentException exception)
            {
                string locationDescription = string.Format(Resources.WmtsConnectionInfoReader_XML_Location_Name_0_URL_1, name, url);
                string message = new FileReaderErrorMessageBuilder(filePath).WithLocation(locationDescription)
                                                                         .Build(Resources.WmtsConnectionInfoReader_Unable_To_Create_WmtsConnectionInfo);

                log.Warn(message, exception);
            }
            return null;
        }
    }
}