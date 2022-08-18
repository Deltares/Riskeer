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
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Riskeer.AssemblyTool.IO.Helpers;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Properties;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.AssemblyTool.IO
{
    /// <summary>
    /// Writer for writing the assembly to GML.
    /// </summary>
    public static class AssemblyGmlWriter
    {
        /// <summary>
        /// Writes a <see cref="ExportableAssembly"/> to a file.
        /// </summary>
        /// <param name="assembly">The <see cref="ExportableAssembly"/> to be written to the file.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write to <paramref name="filePath"/>.</exception>
        public static void Write(ExportableAssembly assembly, string filePath)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
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
                    writer.WriteAttributeString(AssemblyXmlIdentifiers.Id, AssemblyXmlIdentifiers.GmlNamespace, assembly.Id);

                    WriteFeatureMember(() => WriteAssessmentSection(assembly.AssessmentSection, writer), writer);
                    WriteFeatureMember(() => WriteAssessmentProcess(assembly.AssessmentProcess, writer), writer);

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), e);
            }
        }

        private static void WriteFeatureMember(Action writeElementAction, XmlWriter writer)
        {
            writer.WriteStartElement(AssemblyXmlIdentifiers.FeatureMember, AssemblyXmlIdentifiers.UboiNamespace);

            writeElementAction();

            writer.WriteEndElement();
        }

        private static void WriteAssessmentSection(ExportableAssessmentSection assessmentSection, XmlWriter writer)
        {
            WriteStartElementWithId(AssemblyXmlIdentifiers.AssessmentSection, AssemblyXmlIdentifiers.ImwapNamespace, assessmentSection.Id, writer);

            writer.WriteElementString(AssemblyXmlIdentifiers.Name, AssemblyXmlIdentifiers.ImwapNamespace, assessmentSection.Name);

            writer.WriteStartElement(AssemblyXmlIdentifiers.Geometry2D, AssemblyXmlIdentifiers.ImwapNamespace);
            writer.WriteStartElement(AssemblyXmlIdentifiers.LineString, AssemblyXmlIdentifiers.GmlNamespace);
            writer.WriteAttributeString(AssemblyXmlIdentifiers.CoordinateSystem, Resources.CoordinateSystemName);
            writer.WriteElementString(AssemblyXmlIdentifiers.Geometry, AssemblyXmlIdentifiers.GmlNamespace, GeometryGmlFormatHelper.Format(assessmentSection.Geometry));
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteElementString(AssemblyXmlIdentifiers.Length, AssemblyXmlIdentifiers.ImwapNamespace, XmlConvert.ToString(Math2D.Length(assessmentSection.Geometry)));
            writer.WriteElementString(AssemblyXmlIdentifiers.AssessmentSectionType, AssemblyXmlIdentifiers.ImwapNamespace, Resources.AssessmentSectionType);

            writer.WriteEndElement();
        }

        private static void WriteAssessmentProcess(ExportableAssessmentProcess assessmentProcess, XmlWriter writer)
        {
            WriteStartElementWithId(AssemblyXmlIdentifiers.AssessmentProcess, AssemblyXmlIdentifiers.UboiNamespace, assessmentProcess.Id, writer);
            
            writer.WriteElementString(AssemblyXmlIdentifiers.StartYear, AssemblyXmlIdentifiers.UboiNamespace, XmlConvert.ToString(assessmentProcess.StartYear));
            writer.WriteElementString(AssemblyXmlIdentifiers.EndYear, AssemblyXmlIdentifiers.UboiNamespace, XmlConvert.ToString(assessmentProcess.EndYear));

            WriteLink(AssemblyXmlIdentifiers.Assesses, assessmentProcess.AssessmentSectionId, writer);

            writer.WriteEndElement();
        }

        private static void WriteStartElementWithId(string elementName, string elementNamespace, string id, XmlWriter writer)
        {
            writer.WriteStartElement(elementName, elementNamespace);
            writer.WriteAttributeString(AssemblyXmlIdentifiers.Id, AssemblyXmlIdentifiers.GmlNamespace, id);
        }

        private static void WriteLink(string elementName, string linkedId, XmlWriter writer)
        {
            writer.WriteStartElement(elementName, AssemblyXmlIdentifiers.UboiNamespace);
            writer.WriteAttributeString(AssemblyXmlIdentifiers.Link, AssemblyXmlIdentifiers.XLinkNamespace, linkedId);
            writer.WriteEndElement();
        }
    }
}