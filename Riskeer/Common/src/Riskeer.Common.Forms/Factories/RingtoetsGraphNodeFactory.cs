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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using Core.Common.Base.Data;
using Core.Common.Util;
using Core.Components.PointedTree.Data;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="GraphNode"/>.
    /// </summary>
    public static class RingtoetsGraphNodeFactory
    {
        private static readonly XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            NewLineHandling = NewLineHandling.Replace
        };

        /// <summary>
        /// Creates a new <see cref="GraphNode"/> based on the provided input.
        /// </summary>
        /// <param name="illustrationPoint">The illustration point.</param>
        /// <param name="childNodes">The child nodes of the illustration point.</param>
        /// <returns>The newly created <see cref="GraphNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is null.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="illustrationPoint"/> 
        /// is not of type <see cref="FaultTreeIllustrationPoint"/> or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        public static GraphNode CreateGraphNode(IllustrationPointBase illustrationPoint, IEnumerable<GraphNode> childNodes)
        {
            if (illustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(illustrationPoint));
            }

            if (childNodes == null)
            {
                throw new ArgumentNullException(nameof(childNodes));
            }

            var subMechanismIllustrationPoint = illustrationPoint as SubMechanismIllustrationPoint;
            if (subMechanismIllustrationPoint != null)
            {
                return CreateEndGraphNode(illustrationPoint.Name,
                                          CreateGraphNodeContent(illustrationPoint.Beta));
            }

            var faultTreeIllustrationPoint = illustrationPoint as FaultTreeIllustrationPoint;
            if (faultTreeIllustrationPoint != null)
            {
                return CreateFaultTreeGraphNode(faultTreeIllustrationPoint,
                                                childNodes);
            }

            throw new NotSupportedException($"IllustrationPointNode of type {illustrationPoint.GetType().Name} is not supported. " +
                                            $"Supported types: {nameof(FaultTreeIllustrationPoint)} and {nameof(SubMechanismIllustrationPoint)}");
        }

        /// <summary>
        /// Creates a new instance of <see cref="GraphNode"/>, based on the properties of <paramref name="illustrationPoint"/>.
        /// </summary>
        /// <param name="illustrationPoint">The <see cref="FaultTreeIllustrationPoint"/> to base the 
        /// <see cref="GraphNode"/> to create on.</param>
        /// <param name="childNodes">The child graph nodes of the <see cref="GraphNode"/> to create.</param>
        /// <returns>The created <see cref="GraphNode"/>.</returns>
        private static GraphNode CreateFaultTreeGraphNode(FaultTreeIllustrationPoint illustrationPoint,
                                                          IEnumerable<GraphNode> childNodes)
        {
            string childRelationTitle = illustrationPoint.CombinationType == CombinationType.And
                                            ? Resources.GraphNode_CombinationType_And
                                            : Resources.GraphNode_CombinationType_Or;
            GraphNode connectionGraphNode = CreateConnectingGraphNode(childRelationTitle, childNodes);

            return CreateCompositeGraphNode(
                illustrationPoint.Name,
                CreateGraphNodeContent(illustrationPoint.Beta),
                new[]
                {
                    connectionGraphNode
                });
        }

        private static GraphNode CreateEndGraphNode(string title, string content)
        {
            return new GraphNode(GetGraphNodeContentXml(content, title),
                                 new GraphNode[0],
                                 true,
                                 new GraphNodeStyle(GraphNodeShape.Rectangle, Color.SkyBlue, Color.Black, 1));
        }

        private static GraphNode CreateCompositeGraphNode(string title, string content, IEnumerable<GraphNode> childNodes)
        {
            return new GraphNode(GetGraphNodeContentXml(content, title),
                                 childNodes.ToArray(),
                                 true,
                                 new GraphNodeStyle(GraphNodeShape.Rectangle, Color.LightGray, Color.Black, 1));
        }

        private static GraphNode CreateConnectingGraphNode(string title, IEnumerable<GraphNode> childNodes)
        {
            return new GraphNode(GetGraphNodeContentXml(title),
                                 childNodes.ToArray(),
                                 false,
                                 new GraphNodeStyle(GraphNodeShape.None, Color.BlanchedAlmond, Color.Black, 1));
        }

        private static string GetGraphNodeContentXml(string content, string title = null)
        {
            var builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(GraphNodeContentXmlDefinitions.RootElement);

                if (title != null)
                {
                    writer.WriteElementString(GraphNodeContentXmlDefinitions.BoldElement, title);

                    writer.WriteString(Environment.NewLine);
                }

                writer.WriteString(content);

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }

            return builder.ToString();
        }

        private static string CreateGraphNodeContent(RoundedDouble beta)
        {
            return string.Format(Resources.GraphNodeConverter_GraphNodeContent_Probability_0_Beta_1,
                                 ProbabilityFormattingHelper.Format(StatisticsConverter.ReliabilityToProbability(beta)),
                                 beta);
        }
    }
}