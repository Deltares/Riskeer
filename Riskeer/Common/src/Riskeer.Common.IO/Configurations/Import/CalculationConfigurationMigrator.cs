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

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Riskeer.Common.IO.Configurations.Import
{
    /// <summary>
    /// Class to migrate different calculation configuration versions.
    /// </summary>
    public static class CalculationConfigurationMigrator
    {
        /// <summary>
        /// Migrates the given <paramref name="xmlDocument"/> with the <paramref name="migrationScriptDefinition"/>.
        /// </summary>
        /// <param name="xmlDocument">The <see cref="XDocument"/> to migrate.</param>
        /// <param name="migrationScriptDefinition">The migration script.</param>
        /// <returns>The migrated <see cref="XDocument"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="xmlDocument"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="CalculationConfigurationMigrationException">Thrown when
        /// something went wrong while migrating.</exception>
        public static XDocument Migrate(XDocument xmlDocument, string migrationScriptDefinition)
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }

            var stringBuilder = new StringBuilder();

            try
            {
                XslCompiledTransform transformer = CreateTransformer(migrationScriptDefinition);

                using (var writer = XmlWriter.Create(stringBuilder))
                {
                    transformer.Transform(xmlDocument.CreateReader(ReaderOptions.None), writer);
                    writer.Close();
                    writer.Flush();
                }
            }
            catch (Exception e) when (e is InvalidOperationException 
                                      || e is XsltException
                                      || e is IOException)
            {
                throw new CalculationConfigurationMigrationException(e.Message, e);
            }

            return XDocument.Parse(stringBuilder.ToString());
        }

        /// <summary>
        /// Creates a new <see cref="XslCompiledTransform"/>.
        /// </summary>
        /// <param name="migrationScriptDefinition">The migration script definition.</param>
        /// <returns>The created <see cref="XslCompiledTransform"/>.</returns>
        /// <exception cref="XsltException">Thrown when <paramref name="migrationScriptDefinition"/>
        /// is invalid.</exception>
        private static XslCompiledTransform CreateTransformer(string migrationScriptDefinition)
        {
            var xslCompiledTransform = new XslCompiledTransform();

            using (var stringReader = new StringReader(migrationScriptDefinition))
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                xslCompiledTransform.Load(xmlReader);
            }

            return xslCompiledTransform;
        }
    }
}