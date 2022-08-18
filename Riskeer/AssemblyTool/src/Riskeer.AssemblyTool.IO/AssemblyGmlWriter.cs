// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Xml;
using Core.Common.IO.Exceptions;
using Riskeer.AssemblyTool.IO.Model;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.AssemblyTool.IO
{
    /// <summary>
    /// Writer for writing the assembly results to GML.
    /// </summary>
    public static class AssemblyGmlWriter
    {
        /// <summary>
        /// Writes a <see cref="ExportableAssessmentSection"/> to a file.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="ExportableAssessmentSection"/> to be written to the file.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write to <paramref name="filePath"/>.</exception>
        public static void Write(ExportableAssessmentSection assessmentSection, string filePath)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            try
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true
                };

                using (var writer = XmlWriter.Create(filePath, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(AssemblyXmlIdentifiers.UboiNamespaceIdentifier, AssemblyXmlIdentifiers.AssemblyCollection, AssemblyXmlIdentifiers.UboiNamespace);
                    writer.WriteAttributeString(AssemblyXmlIdentifiers.XmlnsIdentifier, AssemblyXmlIdentifiers.XLinkNamespaceIdentifier, null, AssemblyXmlIdentifiers.XLinkNamespace);
                    writer.WriteAttributeString(AssemblyXmlIdentifiers.XmlnsIdentifier, AssemblyXmlIdentifiers.GmlNamespaceIdentifier, null, AssemblyXmlIdentifiers.GmlNamespace);
                    writer.WriteAttributeString(AssemblyXmlIdentifiers.XmlnsIdentifier, AssemblyXmlIdentifiers.ImwapNamespaceIdentifier, null, AssemblyXmlIdentifiers.ImwapNamespace);
                    
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), e);
            }
        }
    }
}