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

using System.Collections.Generic;
using System.Linq;
using Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints
{
    public static class GeneralResultTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/>
        /// with non distinct stochasts.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with non distinct stochasts.</returns>
        public static GeneralResult CreateGeneralResultWithNonDistinctStochasts()
        {
            var stochasts = new[]
            {
                new Stochast("Stochast A", 0, 0),
                new Stochast("Stochast A", 0, 0)
            };
            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>();
            return new GeneralResult(0.5, new TestWindDirection(), stochasts, illustrationPoints);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/> containing a fault tree
        /// with an incomplete list of top level stochasts.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with an incomplete list of top level stochasts.</returns>
        public static GeneralResult CreateGeneralResultFaultTreeWithIncorrectTopLevelStochasts()
        {
            var stochasts = new[]
            {
                new Stochast("Stochast A", 0, 0),
                new Stochast("Stochast B", 0, 0)
            };

            var faultTreeNode1 = new IllustrationPointTreeNode(new FaultTreeIllustrationPoint("Point A", 0.0, new[]
            {
                new Stochast("Stochast C", 0, 0)
            }, CombinationType.And));

            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
            {
                {
                    new WindDirectionClosingSituation(new TestWindDirection(), "closing A"),
                    faultTreeNode1
                }
            };
            return new GeneralResult(0.5, new TestWindDirection(), stochasts, illustrationPoints);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/> containing a sub mechanism
        /// with an incomplete list of top level stochasts.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with an incomplete list of top level stochasts.</returns>
        public static GeneralResult CreateGeneralResultSubMechanismWithIncorrectTopLevelStochasts()
        {
            var stochasts = new[]
            {
                new Stochast("Stochast A", 0, 0),
                new Stochast("Stochast B", 0, 0)
            };

            var faultTreeNode1 = new IllustrationPointTreeNode(new SubMechanismIllustrationPoint("Point A", new[]
            {
                new SubMechanismIllustrationPointStochast("Stochast C", 0, 0, 0)
            }, Enumerable.Empty<IllustrationPointResult>(), 0.0));

            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
            {
                {
                    new WindDirectionClosingSituation(new TestWindDirection(), "closing A"),
                    faultTreeNode1
                }
            };
            return new GeneralResult(0.5, new TestWindDirection(), stochasts, illustrationPoints);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/>
        /// with an incomplete list of stochasts in a fault tree illustration point in the tree.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with an incomplete list of stochasts 
        /// in a fault tree illustration point in the tree.</returns>
        public static GeneralResult CreateGeneralResultWithIncorrectStochastsInChildren()
        {
            var stochasts = new[]
            {
                new Stochast("Stochast A", 0, 0),
                new Stochast("Stochast D", 0, 0)
            };

            var illustrationPointNode = new IllustrationPointTreeNode(new FaultTreeIllustrationPoint("Point A", 0.0, new[]
            {
                new Stochast("Stochast A", 0, 0)
            }, CombinationType.And));

            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointTreeNode(new SubMechanismIllustrationPoint("Point SA", new[]
                {
                    new SubMechanismIllustrationPointStochast("Stochast D", 0, 0, 0)
                }, Enumerable.Empty<IllustrationPointResult>(), 0.0)),
                new IllustrationPointTreeNode(new TestFaultTreeIllustrationPoint())
            });

            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
            {
                {
                    new WindDirectionClosingSituation(new TestWindDirection(), "closing A"),
                    illustrationPointNode
                }
            };
            return new GeneralResult(0.5, new TestWindDirection(), stochasts, illustrationPoints);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/> containing a fault tree
        /// with non distinct illustration points.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with non distinct illustration points.</returns>
        public static GeneralResult CreateGeneralResultFaultTreeWithNonDistinctIllustrationPoints()
        {
            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
            {
                {
                    new WindDirectionClosingSituation(new WindDirection("N", 0.0), "closing A"),
                    new IllustrationPointTreeNode(new TestFaultTreeIllustrationPoint())
                },
                {
                    new WindDirectionClosingSituation(new WindDirection("S", 0.0), "closing A"),
                    new IllustrationPointTreeNode(new TestFaultTreeIllustrationPoint())
                }
            };
            return new GeneralResult(0.5, new TestWindDirection(), Enumerable.Empty<Stochast>(), illustrationPoints);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/> containing a sub mechanism
        /// with non distinct illustration points.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with non distinct illustration points.</returns>
        public static GeneralResult CreateGeneralResultSubMechanismWithNonDistinctIllustrationPoints()
        {
            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
            {
                {
                    new WindDirectionClosingSituation(new WindDirection("N", 0.0), "closing A"),
                    new IllustrationPointTreeNode(new TestSubMechanismIllustrationPoint())
                },
                {
                    new WindDirectionClosingSituation(new WindDirection("S", 0.0), "closing A"),
                    new IllustrationPointTreeNode(new TestSubMechanismIllustrationPoint())
                }
            };
            return new GeneralResult(0.5, new TestWindDirection(), Enumerable.Empty<Stochast>(), illustrationPoints);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/> containing a fault tree
        /// with non distinct illustration point results.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with non distinct illustration point results.</returns>
        public static GeneralResult CreateGeneralResultFaultTreeWithNonDistinctIllustrationPointResults()
        {
            var illustrationPointNode = new IllustrationPointTreeNode(new FaultTreeIllustrationPoint("Point A",
                                                                                                     0.5,
                                                                                                     Enumerable.Empty<Stochast>(),
                                                                                                     CombinationType.And));

            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointTreeNode(new SubMechanismIllustrationPoint("Point SA",
                                                                                Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                                new[]
                                                                                {
                                                                                    new IllustrationPointResult("Result A", 0.0),
                                                                                    new IllustrationPointResult("Result A", 1.0)
                                                                                }, 0.0)),
                new IllustrationPointTreeNode(new TestFaultTreeIllustrationPoint())
            });

            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
            {
                {
                    new WindDirectionClosingSituation(new TestWindDirection(), "closing A"),
                    illustrationPointNode
                }
            };
            return new GeneralResult(0.5, new TestWindDirection(), Enumerable.Empty<Stochast>(), illustrationPoints);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/> containing a fault tree
        /// with non distinct illustration point results.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with non distinct illustration point results.</returns>
        public static GeneralResult CreateGeneralResultSubMechanismWithNonDistinctIllustrationPointResults()
        {
            var illustrationPointNode = new IllustrationPointTreeNode(new SubMechanismIllustrationPoint("Point SA",
                                                                                                        Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                                                        new[]
                                                                                                        {
                                                                                                            new IllustrationPointResult("Result A", 0.0),
                                                                                                            new IllustrationPointResult("Result A", 1.0)
                                                                                                        }, 0.0));

            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
            {
                {
                    new WindDirectionClosingSituation(new TestWindDirection(), "closing A"),
                    illustrationPointNode
                }
            };
            return new GeneralResult(0.5, new TestWindDirection(), Enumerable.Empty<Stochast>(), illustrationPoints);
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneralResult"/>
        /// with non distinct illustration point names.
        /// </summary>
        /// <returns>A <see cref="GeneralResult"/> with non distinct illustration point names.</returns>
        public static GeneralResult CreateGeneralResultWithNonDistinctNamesInChildren()
        {
            var illustrationPointNode = new IllustrationPointTreeNode(new FaultTreeIllustrationPoint("Point A",
                                                                                                     0.5,
                                                                                                     Enumerable.Empty<Stochast>(),
                                                                                                     CombinationType.And));
            illustrationPointNode.SetChildren(new[]
            {
                new IllustrationPointTreeNode(new TestSubMechanismIllustrationPoint("Point B")),
                new IllustrationPointTreeNode(new TestSubMechanismIllustrationPoint("Point B"))
            });
            var illustrationPoints = new Dictionary<WindDirectionClosingSituation, IllustrationPointTreeNode>
            {
                {
                    new WindDirectionClosingSituation(new TestWindDirection(), "closing A"),
                    illustrationPointNode
                }
            };
            return new GeneralResult(0.5, new TestWindDirection(), Enumerable.Empty<Stochast>(), illustrationPoints);
        }
    }
}