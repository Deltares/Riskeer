﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

        #region Link identifiers

        /// <summary>
        /// Identifier for a link attribute.
        /// </summary>
        public const string Link = "href";

        /// <summary>
        /// Identifier for an assesses element.
        /// </summary>
        public const string Assesses = "beoordeelt";

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
        public const string ImwapNamespace = "http://www.aquo.nl/BOI2023/imwaproxies/v20210113";

        /// <summary>
        /// The XML namespace identifier for UBOI objects.
        /// </summary>
        public const string UboiNamespaceIdentifier = "uboi";

        /// <summary>
        /// The XML namespace for UBOI objects.
        /// </summary>
        public const string UboiNamespace = "http://www.aquo.nl/BOI2023/uitwisselmodel/v20210113";

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
        /// Identifier for a length attribute.
        /// </summary>
        public const string Length = "lengte";

        #endregion
    }
}