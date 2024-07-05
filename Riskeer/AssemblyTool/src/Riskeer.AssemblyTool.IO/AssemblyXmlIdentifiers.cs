// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

namespace Riskeer.AssemblyTool.IO
{
    /// <summary>
    /// Class containing definitions for XML identifiers and namespaces for
    /// the serializable assembly model.
    /// </summary>
    public static class AssemblyXmlIdentifiers
    {
        /// <summary>
        /// Identifier for a schema location.
        /// </summary>
        public const string SchemaLocationIdentifier = "schemaLocation";

        /// <summary>
        /// The schema location.
        /// </summary>
        public const string SchemaLocation = "https://data.aquo.nl/xsd/BOI/uitwisselmodel/v20220630 https://data.aquo.nl/xsd/BOI/uitwisselmodel/v20220630/BOI_Uitwisselmodel_v1_0.xsd";

        /// <summary>
        /// Identifier for an assembly collection element.
        /// </summary>
        public const string AssemblyCollection = "Assembleren-collectie";

        /// <summary>
        /// Identifier for an assessment section element.
        /// </summary>
        public const string AssessmentSection = "Waterkeringstelsel";

        /// <summary>
        /// Identifier for an assessment section type element.
        /// </summary>
        public const string AssessmentSectionType = "typeWaterkeringstelsel";

        /// <summary>
        /// Identifier for an assessment process element.
        /// </summary>
        public const string AssessmentProcess = "Beoordelingsproces";

        /// <summary>
        /// Identifier for a start year element.
        /// </summary>
        public const string StartYear = "beginJaarBeoordelingsronde";

        /// <summary>
        /// Identifier for an end year element.
        /// </summary>
        public const string EndYear = "eindJaarBeoordelingsronde";

        /// <summary>
        /// Identifier for a generic failure mechanism element.
        /// </summary>
        public const string GenericFailureMechanism = "GeneriekFaalmechanisme";

        /// <summary>
        /// Identifier for a generic failure mechanism name element.
        /// </summary>
        public const string GenericFailureMechanismName = "generiekFaalmechanisme";

        /// <summary>
        /// Identifier for a specific failure mechanism element.
        /// </summary>
        public const string SpecificFailureMechanism = "SpecifiekFaalmechanisme";

        /// <summary>
        /// Identifier for a specific failure mechanism name element.
        /// </summary>
        public const string SpecificFailureMechanismName = "specifiekFaalmechanisme";

        #region Link identifiers

        /// <summary>
        /// Identifier for a link attribute.
        /// </summary>
        public const string Link = "href";

        /// <summary>
        /// Identifier for an assesses element.
        /// </summary>
        public const string Assesses = "beoordeelt";

        /// <summary>
        /// Identifier for a result of element.
        /// </summary>
        public const string ResultOf = "uitkomstVan";

        /// <summary>
        /// Identifier for a determines element.
        /// </summary>
        public const string Determines = "bepaalt";

        /// <summary>
        /// Identifier for a part of element.
        /// </summary>
        public const string PartOf = "onderdeelVan";

        /// <summary>
        /// Identifier for an analyzes element.
        /// </summary>
        public const string Analyzes = "analyseert";

        /// <summary>
        /// Identifier for an applies to element.
        /// </summary>
        public const string AppliesTo = "geldtVoor";

        #endregion

        #region Namespaces

        /// <summary>
        /// Identifier for a xmlns attribute.
        /// </summary>
        public const string XmlnsIdentifier = "xmlns";

        /// <summary>
        /// The XML namespace identifier for XLink objects.
        /// </summary>
        public const string XLinkNamespaceIdentifier = "xlink";

        /// <summary>
        /// The XML namespace for XLink objects.
        /// </summary>
        public const string XLinkNamespace = "http://www.w3.org/1999/xlink";

        /// <summary>
        /// The XML namespace identifier for GML objects.
        /// </summary>
        public const string GmlNamespaceIdentifier = "gml";

        /// <summary>
        /// The XML namespace for GML objects.
        /// </summary>
        public const string GmlNamespace = "http://www.opengis.net/gml/3.2";

        /// <summary>
        /// The XML namespace identifier for IMWAP objects.
        /// </summary>
        public const string ImwapNamespaceIdentifier = "imwap";

        /// <summary>
        /// The XML namespace for IMWAP objects.
        /// </summary>
        public const string ImwapNamespace = "https://data.aquo.nl/xsd/BOI/imwaproxies/v20220630";

        /// <summary>
        /// The XML namespace identifier for UBOI objects.
        /// </summary>
        public const string UboiNamespaceIdentifier = "uboi";

        /// <summary>
        /// The XML namespace for UBOI objects.
        /// </summary>
        public const string UboiNamespace = "https://data.aquo.nl/xsd/BOI/uitwisselmodel/v20220630";

        /// <summary>
        /// The XML namespace identifier for XSI objects.
        /// </summary>
        public const string XsiNamespaceIdentifier = "xsi";

        /// <summary>
        /// The XML namespace for XSI objects.
        /// </summary>
        public const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

        #endregion

        #region Generic identifiers

        /// <summary>
        /// Identifier for an ID attribute.
        /// </summary>
        public const string Id = "id";

        /// <summary>
        /// Identifier for a name attribute.
        /// </summary>
        public const string Name = "naam";

        /// <summary>
        /// Identifier for a feature member element.
        /// </summary>
        public const string FeatureMember = "featureMember";

        /// <summary>
        /// Identifier for a status element.
        /// </summary>
        public const string Status = "status";

        #endregion

        #region Geometry

        /// <summary>
        /// Identifier for a line string element.
        /// </summary>
        public const string LineString = "LineString";

        /// <summary>
        /// Identifier for a coordinate system type attribute.
        /// </summary>
        public const string CoordinateSystem = "srsName";

        /// <summary>
        /// Identifier for a GML geometry element.
        /// </summary>
        public const string Geometry = "posList";

        /// <summary>
        /// Identifier for a 2D geometry element.
        /// </summary>
        public const string Geometry2D = "geometrie2D";

        /// <summary>
        /// Identifier for a 2D geometry line element.
        /// </summary>
        public const string GeometryLine2D = "geometrieLijn2D";

        /// <summary>
        /// Identifier for a length attribute.
        /// </summary>
        public const string Length = "lengte";

        #endregion

        #region Failure mechanism sections

        /// <summary>
        /// Identifier for a failure mechanism section collection element.
        /// </summary>
        public const string FailureMechanismSectionCollection = "Vakindeling";

        /// <summary>
        /// Identifier for a failure mechanism section element.
        /// </summary>
        public const string FailureMechanismSection = "Deelvak";

        /// <summary>
        /// Identifier for a failure mechanism section type element.
        /// </summary>
        public const string FailureMechanismSectionType = "typeWaterkeringsectie";

        /// <summary>
        /// Identifier for a start distance element.
        /// </summary>
        public const string StartDistance = "afstandBegin";

        /// <summary>
        /// Identifier for an end distance element.
        /// </summary>
        public const string EndDistance = "afstandEinde";

        /// <summary>
        /// Identifier for a failure mechanism section assembly element.
        /// </summary>
        public const string FailureMechanismSectionAssembly = "Faalanalyse";

        #endregion

        #region Assembly results

        /// <summary>
        /// Identifier for a total assembly result element.
        /// </summary>
        public const string TotalAssemblyResult = "Veiligheidsoordeel";

        /// <summary>
        /// Identifier for a total assembly result assembly group element.
        /// </summary>
        public const string TotalAssemblyResultAssemblyGroup = "categorie";

        /// <summary>
        /// Identifier for a total assembly result assembly method element.
        /// </summary>
        public const string TotalAssemblyResultAssemblyMethod = "assemblagemethodeVeiligheidsoordeel";

        /// <summary>
        /// Identifier for a probability element.
        /// </summary>
        public const string Probability = "faalkans";

        /// <summary>
        /// Identifier for a probability assembly method element.
        /// </summary>
        public const string ProbabilityAssemblyMethod = "assemblagemethodeFaalkans";

        /// <summary>
        /// Identifier for a failure mechanism section assembly group element.
        /// </summary>
        public const string FailureMechanismSectionAssemblyGroup = "duidingsklasse";

        /// <summary>
        /// Identifier for a failure mechanism section assembly group assembly method element.
        /// </summary>
        public const string FailureMechanismSectionAssemblyGroupAssemblyMethod = "assemblagemethodeDuidingsklasse";

        #endregion
    }
}