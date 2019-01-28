// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Ringtoets.Common.IO.Configurations.Import
{
    /// <summary>
    /// Container for related/nested schema definitions, representing a combined XML Schema Definition (XSD).
    /// </summary>
    public class CombinedXmlSchemaDefinition
    {
        private readonly XmlSchemaSet xmlSchemaSet;

        /// <summary>
        /// Creates a new instance of <see cref="CombinedXmlSchemaDefinition"/>.
        /// </summary>
        /// <param name="mainSchemaDefinition">A <c>string</c> representing the main schema definition.</param>
        /// <param name="nestedSchemaDefinitions">A <see cref="IDictionary{TKey,TValue}"/> containing
        /// zero to more nested schema definitions. The keys should represent unique file names by which
        /// the schema definitions can be referenced from <paramref name="mainSchemaDefinition"/>; the
        /// values should represent their corresponding schema definition <c>string</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nestedSchemaDefinitions"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mainSchemaDefinition"/> is invalid.</item>
        /// <item><paramref name="nestedSchemaDefinitions"/> contains invalid schema definition values.</item>
        /// <item><paramref name="mainSchemaDefinition"/>, all together with its referenced
        /// <paramref name="nestedSchemaDefinitions"/>, contains an invalid schema definition.</item>
        /// <item><paramref name="nestedSchemaDefinitions"/> contains schema definitions that are not
        /// referenced by <see cref="mainSchemaDefinition"/>.</item>
        /// </list>
        /// </exception>
        public CombinedXmlSchemaDefinition(string mainSchemaDefinition, IDictionary<string, string> nestedSchemaDefinitions)
        {
            CheckSchemaDefinitions(mainSchemaDefinition, nestedSchemaDefinitions);

            var nestedSchemaDefinitionsResolver = new NestedSchemaDefinitionsResolver(nestedSchemaDefinitions);
            xmlSchemaSet = new XmlSchemaSet
            {
                XmlResolver = nestedSchemaDefinitionsResolver
            };

            CompileSchemaSet(mainSchemaDefinition);

            if (!nestedSchemaDefinitionsResolver.AllNestedSchemaDefinitionsReferenced)
            {
                throw new ArgumentException($"'{nameof(nestedSchemaDefinitions)}' contains one or more schema definitions that are not referenced.");
            }
        }

        /// <summary>
        /// Validates the provided XML document based on the combined schema definition.
        /// </summary>
        /// <param name="document">The XML document to validate.</param>
        /// <exception cref="XmlSchemaValidationException">Thrown when the provided XML document does not
        /// match the combined schema definition.</exception>
        public void Validate(XDocument document)
        {
            document.Validate(xmlSchemaSet, null);
        }

        /// <summary>
        /// Check the provided schema definitions for not being <c>null</c>, empty or only containing white spaces.
        /// </summary>
        /// <param name="mainSchemaDefinition">A <c>string</c> representing the main schema definition.</param>
        /// <param name="nestedSchemaDefinitions">A <see cref="IDictionary{TKey,TValue}"/> containing
        /// zero to more nested schema definitions.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nestedSchemaDefinitions"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mainSchemaDefinition"/> is invalid.</item>
        /// <item><paramref name="nestedSchemaDefinitions"/> contains invalid schema definition values.</item>
        /// </list>
        /// </exception>
        private static void CheckSchemaDefinitions(string mainSchemaDefinition, IDictionary<string, string> nestedSchemaDefinitions)
        {
            if (string.IsNullOrWhiteSpace(mainSchemaDefinition))
            {
                throw new ArgumentException($"'{nameof(mainSchemaDefinition)}' null, empty or only containing white spaces.");
            }

            if (nestedSchemaDefinitions == null)
            {
                throw new ArgumentNullException(nameof(nestedSchemaDefinitions));
            }

            if (nestedSchemaDefinitions.Values.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException($"'{nameof(nestedSchemaDefinitions)}' contains one or more nested schema definitions that equal null, are empty or only contain white spaces.");
            }
        }

        /// <summary>
        /// Compiles the <see cref="xmlSchemaSet"/> based on the provided main schema definition.
        /// </summary>
        /// <param name="mainSchemaDefinition">A <c>string</c> representing the main schema definition.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="mainSchemaDefinition"/>,
        /// all together with its referenced nested schema definitions, contains an invalid schema
        /// definition.</exception>
        private void CompileSchemaSet(string mainSchemaDefinition)
        {
            try
            {
                xmlSchemaSet.Add(XmlSchema.Read(new StringReader(mainSchemaDefinition), null));
                xmlSchemaSet.Compile();
            }
            catch (Exception exception) when (exception is XmlException
                                              || exception is XmlSchemaException)
            {
                throw new ArgumentException($"'{nameof(mainSchemaDefinition)}' invalid: {exception.Message}", exception);
            }
        }

        /// <summary>
        /// Resolver for nested schema definitions.
        /// </summary>
        private class NestedSchemaDefinitionsResolver : XmlResolver
        {
            private readonly IDictionary<string, string> nestedSchemaDefinitions;
            private readonly IDictionary<string, bool> nestedSchemaDefinitionsUsage;

            /// <summary>
            /// Creates a new instance of <see cref="NestedSchemaDefinitionsResolver"/>.
            /// </summary>
            /// <param name="nestedSchemaDefinitions">A <see cref="IDictionary{TKey,TValue}"/> containing
            /// zero to more nested schema definitions.</param>
            public NestedSchemaDefinitionsResolver(IDictionary<string, string> nestedSchemaDefinitions)
            {
                this.nestedSchemaDefinitions = nestedSchemaDefinitions;
                nestedSchemaDefinitionsUsage = nestedSchemaDefinitions.Keys.ToDictionary(k => k, k => false);
            }

            public override ICredentials Credentials
            {
                set
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Gets whether all nested schema definitions are used.
            /// </summary>
            public bool AllNestedSchemaDefinitionsReferenced
            {
                get
                {
                    return nestedSchemaDefinitionsUsage.Values.All(v => v);
                }
            }

            public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
            {
                string fileName = Path.GetFileName(absoluteUri.ToString());

                if (nestedSchemaDefinitions.ContainsKey(fileName))
                {
                    nestedSchemaDefinitionsUsage[fileName] = true;
                    return new MemoryStream(Encoding.UTF8.GetBytes(nestedSchemaDefinitions[fileName]));
                }

                return null;
            }
        }
    }
}