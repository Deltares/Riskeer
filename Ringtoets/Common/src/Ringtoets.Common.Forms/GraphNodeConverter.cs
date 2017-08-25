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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Utils;
using Core.Components.PointedTree.Data;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms
{
    /// <summary>
    /// The converter that converts <see cref="IllustrationPointNode"/> data into 
    /// <see cref="GraphNode"/> data.
    /// </summary>
    internal static class GraphNodeConverter
    {
        /// <summary>
        /// Creates a new instance of a <see cref="IllustrationPointBase"/> based on the
        /// <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The <see cref="IllustrationPointNode"/> to base the 
        /// <see cref="GraphNode"/> to create on.</param>
        /// <returns>A <see cref="GraphNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when no suitable conversion for 
        /// <paramref name="node"/> was found.</exception>
        public static GraphNode Convert(IllustrationPointNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            IllustrationPointBase illustrationPoint = node.Data;
            var subMechanismIllustrationPoint = illustrationPoint as SubMechanismIllustrationPoint;
            if (subMechanismIllustrationPoint != null)
            {
                return ConvertSubMechanismIllustrationPoint(subMechanismIllustrationPoint);
            }

            var faultTreeIllustrationPoint = illustrationPoint as FaultTreeIllustrationPoint;
            if (faultTreeIllustrationPoint != null)
            {
                IEnumerable<GraphNode> childGraphNodes = node.Children.Select(Convert);

                return ConvertFaultTreeIllustrationPoint(faultTreeIllustrationPoint,
                                                         childGraphNodes);
            }

            throw new NotSupportedException($"Cannot convert {illustrationPoint.GetType()}.");
        }

        private static GraphNode ConvertFaultTreeIllustrationPoint(FaultTreeIllustrationPoint illustrationPoint,
                                                                   IEnumerable<GraphNode> childGraphNodes)
        {
            string childRelationTitle = illustrationPoint.CombinationType == CombinationType.And
                                            ? Resources.GraphNode_CombinationType_And
                                            : Resources.GraphNode_CombinationType_Or;
            GraphNode connectionGraphNode = RingtoetsGraphNodeFactory.CreateConnectingGraphNode(childRelationTitle, childGraphNodes);

            return RingtoetsGraphNodeFactory.CreateCompositeGraphNode(
                illustrationPoint.Name,
                CreateGraphNodeContent(illustrationPoint.Beta),
                new[]
                {
                    connectionGraphNode
                });
        }

        private static GraphNode ConvertSubMechanismIllustrationPoint(SubMechanismIllustrationPoint illustrationPoint)
        {
            return RingtoetsGraphNodeFactory.CreateEndGraphNode(
                illustrationPoint.Name,
                CreateGraphNodeContent(illustrationPoint.Beta));
        }

        private static string CreateGraphNodeContent(RoundedDouble beta)
        {
            return string.Format(Resources.GraphNodeConverter_GraphNodeContent_Beta_0_Probability_1,
                                 beta,
                                 ProbabilityFormattingHelper.Format(StatisticsConverter.ReliabilityToProbability(beta)));
        }
    }
}