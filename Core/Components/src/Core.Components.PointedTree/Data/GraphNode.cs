// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Core.Components.PointedTree.Properties;

namespace Core.Components.PointedTree.Data
{
    /// <summary>
    /// Class for data with the purpose of becoming visible in pointed tree components.
    /// </summary>
    public class GraphNode
    {
        private readonly GraphNode[] childNodes;

        /// <summary>
        /// Creates a new instance of <see cref="GraphNode"/> with default styling.
        /// </summary>
        /// <param name="content">The content of the node.</param>
        /// <param name="childNodes">The child nodes of the node.</param>
        /// <param name="isSelectable">Indicator whether the node is selectable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/>
        /// or <paramref name="childNodes"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the content is not valid.</exception>
        /// <remarks>Content should have the following format: &lt;text&gt;Content&lt;/text&gt;.
        /// The following tags are supported as content:
        /// <list type="bullet">
        /// <item>&lt;bold&gt;&lt;/bold&gt;</item>
        /// </list>
        /// </remarks>
        public GraphNode(string content, GraphNode[] childNodes, bool isSelectable)
            : this(content, childNodes, isSelectable, CreateDefaultGraphNodeStyle()) {}

        /// <summary>
        /// Creates a new instance of <see cref="GraphNode"/>.
        /// </summary>
        /// <param name="content">The content of the node.</param>
        /// <param name="childNodes">The child nodes of the node.</param>
        /// <param name="isSelectable">Indicator whether the node is selectable.</param>
        /// <param name="style">The style of the node.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/>,
        /// <paramref name="childNodes"/> or <paramref name="style"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the content is not valid.</exception>
        /// <remarks>Content should have the following format: &lt;text&gt;Content&lt;/text&gt;.
        /// The following tags are supported as content:
        /// <list type="bullet">
        /// <item>&lt;bold&gt;&lt;/bold&gt;</item>
        /// </list>
        /// </remarks>
        public GraphNode(string content, GraphNode[] childNodes, bool isSelectable, GraphNodeStyle style)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (childNodes == null)
            {
                throw new ArgumentNullException(nameof(childNodes));
            }

            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            ValidateContent(content);

            Content = content;
            this.childNodes = childNodes;
            IsSelectable = isSelectable;
            Style = style;
        }

        /// <summary>
        /// Gets the content of the node.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the child nodes of the node.
        /// </summary>
        public IEnumerable<GraphNode> ChildNodes
        {
            get
            {
                return childNodes;
            }
        }

        /// <summary>
        /// Gets an indicator whether the node is selectable.
        /// </summary>
        public bool IsSelectable { get; }

        /// <summary>
        /// Gets the style of the node.
        /// </summary>
        public GraphNodeStyle Style { get; }

        /// <summary>
        /// Validates whether the content is valid XML.
        /// </summary>
        /// <param name="content">The content to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the
        /// content is not valid.</exception>
        private static void ValidateContent(string content)
        {
            var xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add(XmlSchema.Read(new StringReader(Resources.GraphNodeContentSchema), null));
            xmlSchemaSet.Compile();

            try
            {
                XDocument doc = XDocument.Parse(content);
                doc.Validate(xmlSchemaSet, null);
            }
            catch (Exception e) when (e is XmlException || e is XmlSchemaValidationException)
            {
                throw new ArgumentException("Content is of invalid format.", e);
            }
        }

        private static GraphNodeStyle CreateDefaultGraphNodeStyle()
        {
            return new GraphNodeStyle(GraphNodeShape.Rectangle, Color.Gray, Color.Black, 2);
        }
    }
}