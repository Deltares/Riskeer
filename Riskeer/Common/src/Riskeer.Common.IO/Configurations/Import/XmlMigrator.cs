// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Riskeer.Common.IO.Configurations.Import
{
    public static class XmlMigrator
    {
        private const string versionZeroToVersionOneXsl =
            "<xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">" +
            "  <xsl:output method=\"xml\" version=\"1.0\" indent=\"yes\" omit-xml-declaration=\"yes\"/>" +
            "  <xsl:template match=\"node()|@*\">" +
            "    <xsl:copy>" +
            "      <xsl:apply-templates select=\"node()|@*\"/>" +
            "    </xsl:copy>" +
            "  </xsl:template>" +
            "  <xsl:template match=\"configuratie\">" +
            "      <xsl:apply-templates select=\"@*\"/>" +
            "    <xsl:copy>" +
            "      <xsl:attribute name=\"versie\">1</xsl:attribute>" +
            "      <xsl:apply-templates select=\"node()\"/>" +
            "    </xsl:copy>" +
            "  </xsl:template>" +
            "  <xsl:template match=\"stochast/@naam[.='polderpeil']\">" +
            "    <xsl:attribute name=\"naam\">" +
            "      <xsl:value-of select=\"'binnendijksewaterstand'\"/>" +
            "    </xsl:attribute>" +
            "  </xsl:template>" +
            "</xsl:stylesheet>";

        // public static void Migrate(XDocument xmlDocument, string )
        // {
        //     using (var stringReader = new StringReader(startingConfiguration))
        //     {
        //         using (var xmlReader = XmlReader.Create(stringReader))
        //         {
        //             using (var stringWriter = new StringWriter())
        //             {
        //                 XslCompiledTransform transformer = CreateTransformer(migrationXsl);
        //
        //                 transformer.Transform(xmlReader, null, stringWriter);
        //             }
        //         }
        //     }
        // }

        private static XslCompiledTransform CreateTransformer(string xsl)
        {
            var xslCompiledTransform = new XslCompiledTransform();

            using (var stringReader = new StringReader(xsl))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    xslCompiledTransform.Load(xmlReader);
                }
            }

            return xslCompiledTransform;
        }
    }
}