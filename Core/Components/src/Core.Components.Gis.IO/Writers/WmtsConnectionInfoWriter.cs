// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.IO;
using System.Xml;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Core.Components.Gis.IO.Writers
{
    /// <summary>
    /// Writer class for <see cref="WmtsConnectionInfo"/>.
    /// </summary>
    public class WmtsConnectionInfoWriter
    {
        private readonly string filePath;

        /// <summary>
        /// Creates a new instance of <see cref="WmtsConnectionInfoWriter"/>.
        /// </summary>
        /// <param name="filePath">Path to write to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public WmtsConnectionInfoWriter(string filePath)
        {
            IOUtils.ValidateFilePath(filePath);
            this.filePath = filePath;
        }

        /// <summary>
        /// Writes the <paramref name="wmtsConnectionInfos"/> to <see cref="filePath"/>.
        /// </summary>
        /// <param name="wmtsConnectionInfos">The <see cref="WmtsConnectionInfo"/> objects to write.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="wmtsConnectionInfos"/> is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when writing <paramref name="wmtsConnectionInfos"/> 
        /// to <see cref="filePath"/> failed.</exception>
        public void WriteWmtsConnectionInfo(IEnumerable<WmtsConnectionInfo> wmtsConnectionInfos)
        {
            if (wmtsConnectionInfos == null)
            {
                throw new ArgumentNullException(nameof(wmtsConnectionInfos));
            }

            try
            {
                WriteWmtsConnectionInfosToXml(wmtsConnectionInfos);
            }
            catch (SystemException exception)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), exception);
            }
        }

        private void WriteWmtsConnectionInfosToXml(IEnumerable<WmtsConnectionInfo> wmtsConnectionInfos)
        {
            EnsureParentDirectoryExists();

            var settings = new XmlWriterSettings
            {
                Indent = true
            };

            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(WmtsConnectionInfoXmlDefinitions.RootElement);

                foreach (WmtsConnectionInfo wmtsConnectionInfo in wmtsConnectionInfos)
                {
                    WriteWmtsConnectionInfoToXml(writer, wmtsConnectionInfo);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }
        }

        /// <summary>
        /// Creates the parent folder of <see cref="filePath"/> if it does not exist.
        /// </summary>
        /// <exception cref="IOException">Thrown when the network name is not known. </exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the 
        /// required permission.</exception>
        /// <exception cref="PathTooLongException">Thrown when the specified path, file name, or 
        /// both exceed the system-defined maximum length.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the specified path is invalid.</exception>
        private void EnsureParentDirectoryExists()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        private static void WriteWmtsConnectionInfoToXml(XmlWriter writer, WmtsConnectionInfo wmtsConnectionInfo)
        {
            writer.WriteStartElement(WmtsConnectionInfoXmlDefinitions.WmtsConnectionElement);

            writer.WriteElementString(WmtsConnectionInfoXmlDefinitions.WmtsConnectionNameElement, wmtsConnectionInfo.Name);
            writer.WriteElementString(WmtsConnectionInfoXmlDefinitions.WmtsConnectionUrlElement, wmtsConnectionInfo.Url);

            writer.WriteEndElement();
        }
    }
}