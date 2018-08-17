// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class containing definitions for XML identifiers and namespaces for
    /// the serializable assembly model.
    /// </summary>
    public static class AssemblyXmlIdentifiers
    {
        /// <summary>
        /// Identifier for an assembly element.
        /// </summary>
        public const string Assembly = "Assemblage";

        /// <summary>
        /// Identifier for an assembly group element.
        /// </summary>
        public const string AssemblyGroup = "toetsspoorGroep";

        /// <summary>
        /// Identifier for an assembly element.
        /// </summary>
        public const string AssemblyMethod = "assemblagemethode";

        /// <summary>
        /// Identifier for an assembly result without probability element.
        /// </summary>
        public const string AssemblyResultWithoutProbability = "toetsoordeelZonderKansschatting";

        /// <summary>
        /// Identifier for an assembly result without probability element.
        /// </summary>
        public const string AssemblyResultWithProbability = "toetsoordeelMetKansschatting";

        /// <summary>
        /// The XML namespace for assembly objects.
        /// </summary>
        public const string AssemblyNamespace = "http://localhost/standaarden/assemblage";

        /// <summary>
        /// The XML namespace identifier for assembly objects.
        /// </summary>
        public const string AssemblyNamespaceIdentifier = "asm";

        /// <summary>
        /// Identifier for an assessment level element.
        /// </summary>
        public const string AssessmentLevel = "toets";

        /// <summary>
        /// Identifier for an assessment process element.
        /// </summary>
        public const string AssessmentProcess = "Beoordelingsproces";

        /// <summary>
        /// Identifier for an assessment process ID attribute.
        /// </summary>
        public const string AssessmentProcessId = "BeoordelingsprocesID";

        /// <summary>
        /// Identifier for an assessment process ID attribute.
        /// </summary>
        public const string AssessmentProcessIdRef = "BeoordelingsprocesIDRef";

        /// <summary>
        /// Identifier for an assessment section category group element.
        /// </summary>
        public const string AssessmentSectionAssemblyResult = "veiligheidsoordeel";
        
        /// <summary>
        /// Identifier for an assessment section category group element.
        /// </summary>
        public const string AssessmentSectionCategoryGroup = "categorie";

        /// <summary>
        /// Identifier for an assessment section ID reference attribute.
        /// </summary>
        public const string AssessmentSectionIdRef = "WaterkeringstelselIDRef";

        /// <summary>
        /// Identifier for an assessment section type element.
        /// </summary>
        public const string AssessmentSectionType = "typeWaterkeringstelsel";

        /// <summary>
        /// Identifier for an assessment section element.
        /// </summary>
        public const string AssessmentSection = "Waterkeringstelsel";

        /// <summary>
        /// Identifier for a failure mechanism category group element.
        /// </summary>
        public const string FailureMechanismCategoryGroup = "categorieTraject";

        /// <summary>
        /// Identifier for a bounded by element.
        /// </summary>
        public const string BoundedBy = "boundedBy";

        /// <summary>
        /// Identifier for a combined failure mechanism section assembly element.
        /// </summary>
        public const string CombinedFailureMechanismSectionAssembly = "GecombineerdToetsoordeel";
        
        /// <summary>
        /// Identifier for a combined failure mechanism section assembly ID attribute.
        /// </summary>
        public const string CombinedFailureMechanismSectionAssemblyId = "GecombineerdToetsoordeelID";

        /// <summary>
        /// Identifier for a combined section result element.
        /// </summary>
        public const string CombinedCombinedSectionResult = "toetsoordeelGecombineerd";
        
        /// <summary>
        /// Identifier for a combined section result element.
        /// </summary>
        public const string CombinedSectionResult = "eindtoetsoordeel";

        /// <summary>
        /// Identifier for a combined section failure mechanism result element.
        /// </summary>
        public const string CombinedSectionFailureMechanismResult = "eindtoetsoordeelToetsspoor";

        /// <summary>
        /// Identifier for a coordinate system type attribute.
        /// </summary>
        public const string CoordinateSystem = "srsName";

        /// <summary>
        /// Identifier for a direct failure mechanism element.
        /// </summary>
        public const string DirectFailureMechanism = "typeFaalmechanisme";

        /// <summary>
        /// Identifier for an end distance element.
        /// </summary>
        public const string EndDistance = "afstandEinde";

        /// <summary>
        /// Identifier for an end year element.
        /// </summary>
        public const string EndYear = "eindJaarBeoordelingsronde";

        /// <summary>
        /// Identifier for an Envelope element.
        /// </summary>
        public const string Envelope = "Envelope";

        /// <summary>
        /// Identifier for a failure mechanism element.
        /// </summary>
        public const string FailureMechanism = "Toetsspoor";

        /// <summary>
        /// Identifier for a failure mechanism assembly result element.
        /// </summary>
        public const string FailureMechanismAssemblyResult = "toetsoordeel";

        /// <summary>
        /// Identifier for a failure mechanism ID attribute.
        /// </summary>
        public const string FailureMechanismId = "ToetsspoorID";

        /// <summary>
        /// Identifier for a failure mechanism ID reference attribute.
        /// </summary>
        public const string FailureMechanismIdRef = "ToetsspoorIDRef";

        /// <summary>
        /// Identifier for a failure mechanism section element.
        /// </summary>
        public const string FailureMechanismSection = "Waterkeringsectie";

        /// <summary>
        /// Identifier for a failure mechanism section ID reference attribute.
        /// </summary>
        public const string FailureMechanismSectionIdRef = "WaterkeringsectieIDRef";

        /// <summary>
        /// Identifier for a failure mechanism section assembly element.
        /// </summary>
        public const string FailureMechanismSectionAssembly = "Toets";

        /// <summary>
        /// Identifier for a failure mechanism section assembly element.
        /// </summary>
        public const string FailureMechanismSectionAssemblyId = "ToetsID";

        /// <summary>
        /// Identifier for a failure mechanism section category group element.
        /// </summary>
        public const string FailureMechanismSectionCategoryGroup = "categorieVak";

        /// <summary>
        /// Identifier for a failure mechanism sections element.
        /// </summary>
        public const string FailureMechanismSections = "Vakindeling";

        /// <summary>
        /// Identifier for a failure mechanism section collection ID attribute.
        /// </summary>
        public const string FailureMechanismSectionCollectionId = "VakindelingID";

        /// <summary>
        /// Identifier for a failure mechanism section collection ID reference attribute.
        /// </summary>
        public const string FailureMechanismSectionCollectionIdRef = "VakindelingIDRef";

        /// <summary>
        /// Identifier for a failure mechanism section type element.
        /// </summary>
        public const string FailureMechanismSectionType = "typeWaterkeringsectie";

        /// <summary>
        /// Identifier for a failure mechanism group element.
        /// </summary>
        public const string FailureMechanismType = "typeToetsspoor";

        /// <summary>
        /// Identifier for a feature member element.
        /// </summary>
        public const string FeatureMember = "featureMember";

        /// <summary>
        /// Identifier for a GML geometry element.
        /// </summary>
        public const string Geometry = "posList";

        /// <summary>
        /// Identifier for a 2D geometry element.
        /// </summary>
        public const string Geometry2D = "geometrie2D";

        /// <summary>
        /// Identifier for a 2D line geometry element.
        /// </summary>
        public const string GeometryLine2D = "geometrieLijn2D";

        /// <summary>
        /// The XML namespace for GML objects.
        /// </summary>
        public const string GmlNamespace = "http://www.opengis.net/gml/3.2";

        /// <summary>
        /// The XML namespace identifier for GML objects.
        /// </summary>
        public const string GmlNamespaceIdentifier = "gml";

        /// <summary>
        /// Identifier for an ID attribute.
        /// </summary>
        public const string Id = "id";

        /// <summary>
        /// Identifier for a length attribute.
        /// </summary>
        public const string Length = "lengte";

        /// <summary>
        /// Identifier for a line string element.
        /// </summary>
        public const string LineString = "LineString";

        /// <summary>
        /// Identifier for a lower corner attribute.
        /// </summary>
        public const string LowerCorner = "lowerCorner";

        /// <summary>
        /// Identifier for a name attribute.
        /// </summary>
        public const string Name = "naam";

        /// <summary>
        /// Identifier for a probability element.
        /// </summary>
        public const string Probability = "faalkans";

        /// <summary>
        /// Identifier for section result elements.
        /// </summary>
        public const string SectionResults = "toetsoordeelVak";

        /// <summary>
        /// Identifier for a start distance element.
        /// </summary>
        public const string StartDistance = "afstandBegin";

        /// <summary>
        /// Identifier for a start year element.
        /// </summary>
        public const string StartYear = "beginJaarBeoordelingsronde";

        /// <summary>
        /// Identifier for a status element.
        /// </summary>
        public const string Status = "status";

        /// <summary>
        /// Identifier for an assessment section total assembly result element.
        /// </summary>
        public const string TotalAssemblyResult = "Veiligheidsoordeel";

        /// <summary>
        /// Identifier for an assessment section assembly result ID attribute.
        /// </summary>
        public const string TotalAssemblyResultId = "VeiligheidsoordeelID";

        /// <summary>
        /// Identifier for an assessment section assembly result ID reference attribute.
        /// </summary>
        public const string TotalAssemblyResultIdRef = "VeiligheidsoordeelIDRef";

        /// <summary>
        /// Identifier for a unit of measure attribute.
        /// </summary>
        public const string UnitOfMeasure = "uom";

        /// <summary>
        /// Identifier for a lower corner attribute.
        /// </summary>
        public const string UpperCorner = "upperCorner";
    }
}