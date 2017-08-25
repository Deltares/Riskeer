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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using Core.Components.PointedTree.Data;

namespace Ringtoets.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="GraphNode"/> based on information used as input.
    /// </summary>
    public static class RingtoetsGraphNodeFactory
    {
        private static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.Replace
        };

        /// <summary>
        /// Create <see cref="GraphNode"/> with default styling for an end node.
        /// </summary>
        /// <param name="title">The title to set for this node.</param>
        /// <param name="content">The content to set for this node.</param>
        /// <returns>The created <see cref="GraphNode"/>.</returns>
        public static GraphNode CreateEndGraphNode(string title, string content)
        {
            return new GraphNode(GetGraphNodeContentXml(title, content),
                                 new GraphNode[0],
                                 true,
                                 new GraphNodeStyle(GraphNodeShape.Rectangle, Color.SkyBlue, Color.Black, 1));
        }

        /// <summary>
        /// Create <see cref="GraphNode"/> with default styling for a composite node.
        /// </summary>
        /// <param name="title">The title to set for this node.</param>
        /// <param name="content">The content to set for this node.</param>
        /// <param name="childNodes">The child nodes of this node.</param>
        /// <returns>The created <see cref="GraphNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="childNodes"/> is 
        /// <c>null</c>.</exception>
        public static GraphNode CreateCompositeGraphNode(string title, string content, IEnumerable<GraphNode> childNodes)
        {
            if (childNodes == null)
            {
                throw new ArgumentNullException(nameof(childNodes));
            }
            return new GraphNode(GetGraphNodeContentXml(title, content),
                                 childNodes.ToArray(),
                                 true,
                                 new GraphNodeStyle(GraphNodeShape.Rectangle, Color.LightGray, Color.Black, 1));
        }

        /// <summary>
        /// Create <see cref="GraphNode"/> with default styling for a connecting node.
        /// </summary>
        /// <param name="title">The title to set for this node.</param>
        /// <param name="childNodes">The child nodes of this node.</param>
        /// <returns>The created <see cref="GraphNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="childNodes"/> is 
        /// <c>null</c>.</exception>
        public static GraphNode CreateConnectingGraphNode(string title, IEnumerable<GraphNode> childNodes)
        {
            if (childNodes == null)
            {
                throw new ArgumentNullException(nameof(childNodes));
            }
            return new GraphNode(GetGraphNodeContentXml(title),
                                 childNodes.ToArray(),
                                 false,
                                 new GraphNodeStyle(GraphNodeShape.None, Color.BlanchedAlmond, Color.Black, 1));
        }

        private static string GetGraphNodeContentXml(string content)
        {
            var builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(GraphNodeContentXmlDefinitions.RootElement);

                writer.WriteString(content);

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }

            return builder.ToString();
        }

        private static string GetGraphNodeContentXml(string title, string content)
        {
            var builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(GraphNodeContentXmlDefinitions.RootElement);

                writer.WriteElementString(GraphNodeContentXmlDefinitions.BoldElement, title);
                writer.WriteString(Environment.NewLine);
                writer.WriteString(content);

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }

            return builder.ToString();
        }
    }
}