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
using System.Collections.Generic;
using System.Xml;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.Util;
using Core.Common.Util.Enums;
using Riskeer.AssemblyTool.IO.Helpers;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Properties;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.AssemblyTool.IO
{
    /// <summary>
    /// Writer for writing the assembly to GML.
    /// </summary>
    public class AssemblyGmlWriter : IDisposable
    {
        private readonly string filePath;
        private XmlWriter writer;

        /// <summary>
        /// Creates a new instance of <see cref="AssemblyGmlWriter"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to write to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        public AssemblyGmlWriter(string filePath)
        {
            this.filePath = filePath;
            IOUtils.ValidateFilePath(filePath);
        }

        /// <summary>
        /// Writes a <see cref="ExportableAssembly"/> to a file.
        /// </summary>
        /// <param name="assembly">The <see cref="ExportableAssembly"/> to be written to the file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write the file to the provided file path.</exception>
        public void Write(ExportableAssembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            try
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true
                };

                writer = XmlWriter.Create(filePath, settings);

                writer.WriteStartDocument();
                writer.WriteStartElement(AssemblyXmlIdentifiers.UboiNamespaceIdentifier, AssemblyXmlIdentifiers.AssemblyCollection, AssemblyXmlIdentifiers.UboiNamespace);
                writer.WriteAttributeString(AssemblyXmlIdentifiers.XmlnsIdentifier, AssemblyXmlIdentifiers.XLinkNamespaceIdentifier, null, AssemblyXmlIdentifiers.XLinkNamespace);
                writer.WriteAttributeString(AssemblyXmlIdentifiers.XmlnsIdentifier, AssemblyXmlIdentifiers.GmlNamespaceIdentifier, null, AssemblyXmlIdentifiers.GmlNamespace);
                writer.WriteAttributeString(AssemblyXmlIdentifiers.XmlnsIdentifier, AssemblyXmlIdentifiers.ImwapNamespaceIdentifier, null, AssemblyXmlIdentifiers.ImwapNamespace);
                writer.WriteAttributeString(AssemblyXmlIdentifiers.Id, AssemblyXmlIdentifiers.GmlNamespace, assembly.Id);

                ExportableAssessmentSection assessmentSection = assembly.AssessmentSection;
                ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly = assessmentSection.AssessmentSectionAssembly;
                ExportableAssessmentProcess assessmentProcess = assembly.AssessmentProcess;

                WriteFeatureMember(() => WriteAssessmentSection(assessmentSection));
                WriteFeatureMember(() => WriteAssessmentProcess(assessmentProcess));
                WriteFeatureMember(() => WriteTotalAssemblyResult(assessmentSectionAssembly, assessmentProcess.Id));

                WriteFailureMechanisms(assembly, assessmentSectionAssembly);

                WriteFailureMechanismSectionCollections(assessmentSection.FailureMechanismSectionCollections);

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), e);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                writer?.Dispose();
            }
        }

        private void WriteFeatureMember(Action writeElementAction)
        {
            writer.WriteStartElement(AssemblyXmlIdentifiers.FeatureMember, AssemblyXmlIdentifiers.UboiNamespace);

            writeElementAction();

            writer.WriteEndElement();
        }

        private void WriteAssessmentSection(ExportableAssessmentSection assessmentSection)
        {
            WriteStartElementWithId(AssemblyXmlIdentifiers.AssessmentSection, AssemblyXmlIdentifiers.ImwapNamespace, assessmentSection.Id);

            writer.WriteElementString(AssemblyXmlIdentifiers.Name, AssemblyXmlIdentifiers.ImwapNamespace, assessmentSection.Name);

            WriteGeometry(AssemblyXmlIdentifiers.Geometry2D, assessmentSection.Geometry);

            writer.WriteElementString(AssemblyXmlIdentifiers.Length, AssemblyXmlIdentifiers.ImwapNamespace, XmlConvert.ToString(Math2D.Length(assessmentSection.Geometry)));
            writer.WriteElementString(AssemblyXmlIdentifiers.AssessmentSectionType, AssemblyXmlIdentifiers.ImwapNamespace, Resources.AssessmentSectionType);

            writer.WriteEndElement();
        }

        private void WriteAssessmentProcess(ExportableAssessmentProcess assessmentProcess)
        {
            WriteStartElementWithId(AssemblyXmlIdentifiers.AssessmentProcess, AssemblyXmlIdentifiers.UboiNamespace, assessmentProcess.Id);

            writer.WriteElementString(AssemblyXmlIdentifiers.StartYear, AssemblyXmlIdentifiers.UboiNamespace, XmlConvert.ToString(assessmentProcess.StartYear));
            writer.WriteElementString(AssemblyXmlIdentifiers.EndYear, AssemblyXmlIdentifiers.UboiNamespace, XmlConvert.ToString(assessmentProcess.EndYear));

            WriteLink(AssemblyXmlIdentifiers.Assesses, assessmentProcess.AssessmentSectionId);

            writer.WriteEndElement();
        }

        private void WriteTotalAssemblyResult(ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly, string assessmentProcessId)
        {
            WriteStartElementWithId(AssemblyXmlIdentifiers.TotalAssemblyResult, AssemblyXmlIdentifiers.UboiNamespace, assessmentSectionAssembly.Id);

            writer.WriteElementString(AssemblyXmlIdentifiers.TotalAssemblyResultAssemblyGroup, AssemblyXmlIdentifiers.UboiNamespace,
                                      EnumDisplayNameHelper.GetDisplayName(assessmentSectionAssembly.AssemblyGroup));
            writer.WriteElementString(AssemblyXmlIdentifiers.TotalAssemblyResultAssemblyMethod, AssemblyXmlIdentifiers.UboiNamespace,
                                      EnumDisplayNameHelper.GetDisplayName(assessmentSectionAssembly.AssemblyGroupAssemblyMethod));
            writer.WriteElementString(AssemblyXmlIdentifiers.Probability, AssemblyXmlIdentifiers.UboiNamespace,
                                      XmlConvert.ToString(assessmentSectionAssembly.Probability));
            writer.WriteElementString(AssemblyXmlIdentifiers.ProbabilityAssemblyMethod, AssemblyXmlIdentifiers.UboiNamespace,
                                      EnumDisplayNameHelper.GetDisplayName(assessmentSectionAssembly.ProbabilityAssemblyMethod));
            writer.WriteElementString(AssemblyXmlIdentifiers.Status, AssemblyXmlIdentifiers.UboiNamespace, Resources.FullAssembly);

            WriteLink(AssemblyXmlIdentifiers.ResultOf, assessmentProcessId);

            writer.WriteEndElement();
        }

        private void WriteFailureMechanisms(ExportableAssembly assembly, ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly)
        {
            foreach (ExportableFailureMechanism failureMechanism in assembly.AssessmentSection.FailureMechanisms)
            {
                Action writeFailureMechanismAction = null;

                if (failureMechanism is ExportableGenericFailureMechanism genericFailureMechanism)
                {
                    writeFailureMechanismAction = () => WriteFailureMechanism(
                        genericFailureMechanism, assessmentSectionAssembly, AssemblyXmlIdentifiers.GenericFailureMechanism,
                        AssemblyXmlIdentifiers.GenericFailureMechanismName, genericFailureMechanism.Code);
                }

                if (failureMechanism is ExportableSpecificFailureMechanism specificFailureMechanism)
                {
                    writeFailureMechanismAction = () => WriteFailureMechanism(
                        specificFailureMechanism, assessmentSectionAssembly, AssemblyXmlIdentifiers.SpecificFailureMechanism,
                        AssemblyXmlIdentifiers.SpecificFailureMechanismName, specificFailureMechanism.Name);
                }

                WriteFeatureMember(writeFailureMechanismAction);

                foreach (ExportableFailureMechanismSectionAssemblyWithProbabilityResult sectionAssemblyResult in failureMechanism.SectionAssemblyResults)
                {
                    WriteFeatureMember(() => WriteSectionAssemblyResult(sectionAssemblyResult, failureMechanism.Id));
                }
            }
        }

        private void WriteFailureMechanism(ExportableFailureMechanism failureMechanism, ExportableAssessmentSectionAssemblyResult assessmentSectionAssembly,
                                           string startElementName, string nameElementName, string nameElementValue)
        {
            WriteStartElementWithId(startElementName, AssemblyXmlIdentifiers.UboiNamespace, failureMechanism.Id);

            writer.WriteElementString(AssemblyXmlIdentifiers.Probability, AssemblyXmlIdentifiers.UboiNamespace,
                                      XmlConvert.ToString(failureMechanism.FailureMechanismAssembly.Probability));
            writer.WriteElementString(AssemblyXmlIdentifiers.ProbabilityAssemblyMethod, AssemblyXmlIdentifiers.UboiNamespace,
                                      EnumDisplayNameHelper.GetDisplayName(failureMechanism.FailureMechanismAssembly.AssemblyMethod));
            writer.WriteElementString(AssemblyXmlIdentifiers.Status, AssemblyXmlIdentifiers.UboiNamespace, Resources.FullAssembly);

            WriteLink(AssemblyXmlIdentifiers.Determines, assessmentSectionAssembly.Id);

            writer.WriteElementString(nameElementName, AssemblyXmlIdentifiers.UboiNamespace, nameElementValue);

            writer.WriteEndElement();
        }

        private void WriteSectionAssemblyResult(ExportableFailureMechanismSectionAssemblyWithProbabilityResult sectionAssemblyResult,
                                                string failureMechanismId)
        {
            WriteStartElementWithId(AssemblyXmlIdentifiers.FailureMechanismSectionAssembly, AssemblyXmlIdentifiers.UboiNamespace, sectionAssemblyResult.Id);

            writer.WriteElementString(AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyGroup, AssemblyXmlIdentifiers.UboiNamespace,
                                      EnumDisplayNameHelper.GetDisplayName(sectionAssemblyResult.AssemblyGroup));
            writer.WriteElementString(AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyGroupAssemblyMethod, AssemblyXmlIdentifiers.UboiNamespace,
                                      EnumDisplayNameHelper.GetDisplayName(sectionAssemblyResult.AssemblyGroupAssemblyMethod));
            writer.WriteElementString(AssemblyXmlIdentifiers.Probability, AssemblyXmlIdentifiers.UboiNamespace,
                                      XmlConvert.ToString(sectionAssemblyResult.Probability));
            writer.WriteElementString(AssemblyXmlIdentifiers.ProbabilityAssemblyMethod, AssemblyXmlIdentifiers.UboiNamespace,
                                      EnumDisplayNameHelper.GetDisplayName(sectionAssemblyResult.ProbabilityAssemblyMethod));
            writer.WriteElementString(AssemblyXmlIdentifiers.Status, AssemblyXmlIdentifiers.UboiNamespace, Resources.FullAssembly);

            WriteLink(AssemblyXmlIdentifiers.Analyses, failureMechanismId);
            WriteLink(AssemblyXmlIdentifiers.AppliesTo, sectionAssemblyResult.FailureMechanismSection.Id);

            writer.WriteEndElement();
        }

        private void WriteFailureMechanismSectionCollections(IEnumerable<ExportableFailureMechanismSectionCollection> failureMechanismSectionCollections)
        {
            foreach (ExportableFailureMechanismSectionCollection failureMechanismSectionCollection in failureMechanismSectionCollections)
            {
                WriteFeatureMember(() => WriteFailureMechanismSectionCollection(failureMechanismSectionCollection));

                foreach (ExportableFailureMechanismSection section in failureMechanismSectionCollection.Sections)
                {
                    WriteFeatureMember(() => WriteFailureMechanismSection(section, failureMechanismSectionCollection.Id));
                }
            }
        }

        private void WriteFailureMechanismSectionCollection(ExportableFailureMechanismSectionCollection failureMechanismSectionCollection)
        {
            WriteStartElementWithId(AssemblyXmlIdentifiers.FailureMechanismSectionCollection, AssemblyXmlIdentifiers.ImwapNamespace, failureMechanismSectionCollection.Id);
            writer.WriteEndElement();
        }

        private void WriteFailureMechanismSection(ExportableFailureMechanismSection section, string failureMechanismSectionCollectionId)
        {
            WriteStartElementWithId(AssemblyXmlIdentifiers.FailureMechanismSection, AssemblyXmlIdentifiers.UboiNamespace, section.Id);

            WriteGeometry(AssemblyXmlIdentifiers.GeometryLine2D, section.Geometry);

            writer.WriteElementString(AssemblyXmlIdentifiers.FailureMechanismSectionType, AssemblyXmlIdentifiers.ImwapNamespace, Resources.FailureMechanismSectionType_FailureMechanism);
            writer.WriteElementString(AssemblyXmlIdentifiers.StartDistance, AssemblyXmlIdentifiers.ImwapNamespace, XmlConvert.ToString(section.StartDistance));
            writer.WriteElementString(AssemblyXmlIdentifiers.EndDistance, AssemblyXmlIdentifiers.ImwapNamespace, XmlConvert.ToString(section.EndDistance));
            writer.WriteElementString(AssemblyXmlIdentifiers.Length, AssemblyXmlIdentifiers.ImwapNamespace, XmlConvert.ToString(Math2D.Length(section.Geometry)));

            WriteLink(AssemblyXmlIdentifiers.PartOf, failureMechanismSectionCollectionId);

            writer.WriteEndElement();
        }

        private void WriteStartElementWithId(string elementName, string elementNamespace, string id)
        {
            writer.WriteStartElement(elementName, elementNamespace);
            writer.WriteAttributeString(AssemblyXmlIdentifiers.Id, AssemblyXmlIdentifiers.GmlNamespace, id);
        }

        private void WriteGeometry(string geometryElementName, IEnumerable<Point2D> geometry)
        {
            writer.WriteStartElement(geometryElementName, AssemblyXmlIdentifiers.ImwapNamespace);
            writer.WriteStartElement(AssemblyXmlIdentifiers.LineString, AssemblyXmlIdentifiers.GmlNamespace);
            writer.WriteAttributeString(AssemblyXmlIdentifiers.CoordinateSystem, Resources.CoordinateSystemName);
            writer.WriteElementString(AssemblyXmlIdentifiers.Geometry, AssemblyXmlIdentifiers.GmlNamespace, GeometryGmlFormatHelper.Format(geometry));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void WriteLink(string elementName, string linkedId)
        {
            writer.WriteStartElement(elementName, AssemblyXmlIdentifiers.UboiNamespace);
            writer.WriteAttributeString(AssemblyXmlIdentifiers.Link, AssemblyXmlIdentifiers.XLinkNamespace, linkedId);
            writer.WriteEndElement();
        }
    }
}