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
using Core.Common.Base.Geometry;
using Core.Common.Base.TestUtil.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Factories;

namespace Riskeer.MacroStabilityInwards.Forms.Test.Factories
{
    [TestFixture]
    public class MacroStabilityInwardsSliceChartDataPointsFactoryTest
    {
        [Test]
        public void CreateSliceAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateSliceAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSliceAreas_WithSlidingCurve_ReturnsAreas()
        {
            // Setup
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                new[]
                {
                    new MacroStabilityInwardsSlice(new Point2D(0, 1),
                                                   new Point2D(1, 1),
                                                   new Point2D(0, 0),
                                                   new Point2D(1, 0),
                                                   new MacroStabilityInwardsSlice.ConstructionProperties()),
                    new MacroStabilityInwardsSlice(new Point2D(3, 4),
                                                   new Point2D(4, 4),
                                                   new Point2D(3, 3),
                                                   new Point2D(4, 3),
                                                   new MacroStabilityInwardsSlice.ConstructionProperties())
                },
                0.0,
                0.0);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateSliceAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 1),
                    new Point2D(1, 1),
                    new Point2D(1, 0),
                    new Point2D(0, 0)
                },
                new[]
                {
                    new Point2D(3, 4),
                    new Point2D(4, 4),
                    new Point2D(4, 3),
                    new Point2D(3, 3)
                }
            }, areas);
        }

        [Test]
        [TestCaseSource(nameof(GetCreateParameterMethods))]
        public void CreateParameterAreas_ValueNaN_ReturnsExpectedAreas(Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>> createParameterAreas)
        {
            // Setup
            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(new MacroStabilityInwardsSlice.ConstructionProperties());

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = createParameterAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 0),
                    new Point2D(0, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(1, 1),
                    new Point2D(1, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(2, 1),
                    new Point2D(1, 0)
                }
            }, areas);
        }

        [Test]
        public void CreateCohesionAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateCohesionAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateCohesionAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Cohesion = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateCohesionAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateCohesionAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Cohesion = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateCohesionAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateCohesionAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Cohesion = 30.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateCohesionAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -3.75),
                    new Point2D(0, -3.75)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(4.75, 1),
                    new Point2D(4.75, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(4.65165, -1.65165),
                    new Point2D(3.65165, -2.65165)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateEffectiveStressAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateEffectiveStressAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                EffectiveStress = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateEffectiveStressAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                EffectiveStress = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateEffectiveStressAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                EffectiveStress = 40.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -5),
                    new Point2D(0, -5)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(6, 1),
                    new Point2D(6, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(5.535534, -2.535534),
                    new Point2D(4.535534, -3.535534)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateEffectiveStressDailyAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressDailyAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateEffectiveStressDailyAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                EffectiveStressDaily = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressDailyAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateEffectiveStressDailyAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                EffectiveStressDaily = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressDailyAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateEffectiveStressDailyAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                EffectiveStressDaily = 50.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressDailyAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -6.25),
                    new Point2D(0, -6.25)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(7.25, 1),
                    new Point2D(7.25, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(6.419417, -3.419417),
                    new Point2D(5.419417, -4.419417)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateTotalPorePressureAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateTotalPorePressureAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateTotalPorePressureAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                TotalPorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateTotalPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateTotalPorePressureAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                TotalPorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateTotalPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateTotalPorePressureAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                TotalPorePressure = 60.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateTotalPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -7.5),
                    new Point2D(0, -7.5)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(8.5, 1),
                    new Point2D(8.5, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(7.303301, -4.303301),
                    new Point2D(6.303301, -5.303301)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateWeightAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateWeightAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateWeightAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Weight = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateWeightAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateWeightAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Weight = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateWeightAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateWeightAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Weight = 70.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateWeightAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -8.75),
                    new Point2D(0, -8.75)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(9.75, 1),
                    new Point2D(9.75, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(8.187184, -5.187184),
                    new Point2D(7.187184, -6.187184)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreatePiezometricPorePressureAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePiezometricPorePressureAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreatePiezometricPorePressureAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                PiezometricPorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePiezometricPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreatePiezometricPorePressureAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                PiezometricPorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePiezometricPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreatePiezometricPorePressureAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                PiezometricPorePressure = 80.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePiezometricPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -10),
                    new Point2D(0, -10)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(11, 1),
                    new Point2D(11, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(9.071068, -6.071068),
                    new Point2D(8.071068, -7.071068)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreatePorePressureAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePorePressureAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreatePorePressureAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                PorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreatePorePressureAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                PorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreatePorePressureAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                PorePressure = 90.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -11.25),
                    new Point2D(0, -11.25)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(12.25, 1),
                    new Point2D(12.25, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(9.954951, -6.954951),
                    new Point2D(8.954951, -7.954951)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateVerticalPorePressureAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateVerticalPorePressureAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateVerticalPorePressureAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                VerticalPorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateVerticalPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateVerticalPorePressureAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                VerticalPorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateVerticalPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateVerticalPorePressureAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                VerticalPorePressure = 100.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateVerticalPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -12.5),
                    new Point2D(0, -12.5)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(13.5, 1),
                    new Point2D(13.5, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(10.838835, -7.838835),
                    new Point2D(9.838835, -8.838835)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateHorizontalPorePressureAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateHorizontalPorePressureAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateHorizontalPorePressureAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                HorizontalPorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateHorizontalPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateHorizontalPorePressureAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                HorizontalPorePressure = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateHorizontalPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateHorizontalPorePressureAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                HorizontalPorePressure = 110.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateHorizontalPorePressureAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -13.75),
                    new Point2D(0, -13.75)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(14.75, 1),
                    new Point2D(14.75, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(11.722718, -8.722718),
                    new Point2D(10.722718, -9.722718)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateOverConsolidationRatioAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateOverConsolidationRatioAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateOverConsolidationRatioAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                OverConsolidationRatio = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateOverConsolidationRatioAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -100),
                    new Point2D(0, -100)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(101, 1),
                    new Point2D(101, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(72.710678, -69.710678),
                    new Point2D(71.710678, -70.710678)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateOverConsolidationRatioAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                OverConsolidationRatio = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateOverConsolidationRatioAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 100),
                    new Point2D(0, 100)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-99, 1),
                    new Point2D(-99, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-68.710678, 71.710678),
                    new Point2D(-69.710678, 70.710678)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateOverConsolidationRatioAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                OverConsolidationRatio = 40.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateOverConsolidationRatioAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -2),
                    new Point2D(0, -2)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(3, 1),
                    new Point2D(3, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(3.414213, -0.414213),
                    new Point2D(2.414213, -1.414213)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreatePopAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreatePopAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Pop = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreatePopAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Pop = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreatePopAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Pop = 120.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -15),
                    new Point2D(0, -15)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(16, 1),
                    new Point2D(16, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(12.606602, -9.606602),
                    new Point2D(11.606602, -10.606602)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateNormalStressAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateNormalStressAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateNormalStressAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                NormalStress = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateNormalStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateNormalStressAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                NormalStress = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateNormalStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateNormalStressAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                NormalStress = 130.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateNormalStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -16.25),
                    new Point2D(0, -16.25)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(17.25, 1),
                    new Point2D(17.25, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(13.490485, -10.490485),
                    new Point2D(12.490485, -11.490485)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateShearStressAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateShearStressAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateShearStressAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                ShearStress = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateShearStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateShearStressAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                ShearStress = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateShearStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateShearStressAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                ShearStress = 150.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateShearStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -18.75),
                    new Point2D(0, -18.75)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(19.75, 1),
                    new Point2D(19.75, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(15.258252, -12.258252),
                    new Point2D(14.258252, -13.258252)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateLoadStressAreas_SlidingCurveNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateLoadStressAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        [TestCaseSource(nameof(GetValuesAboveUpperLimit))]
        public void CreateLoadStressAreas_ValueAboveUpperLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                LoadStress = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateLoadStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -250),
                    new Point2D(0, -250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(251, 1),
                    new Point2D(251, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(178.776695, -175.776695),
                    new Point2D(177.776695, -176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        [TestCaseSource(nameof(GetValuesBelowLowerLimit))]
        public void CreateLoadStressAreas_ValueBelowLowerLimit_ReturnsExpectedAreas(double value)
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                LoadStress = value
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateLoadStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, 250),
                    new Point2D(0, 250)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(-249, 1),
                    new Point2D(-249, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(-174.776695, 177.776695),
                    new Point2D(-175.776695, 176.776695)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        [Test]
        public void CreateLoadStressAreas_ValidParameters_ReturnsExpectedAreas()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                LoadStress = 160.0
            };

            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(constructionProperties);

            // Call
            IEnumerable<IEnumerable<Point2D>> areas = MacroStabilityInwardsSliceChartDataPointsFactory.CreateLoadStressAreas(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    new Point2D(1, -20),
                    new Point2D(0, -20)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(1, 1),
                    new Point2D(21, 1),
                    new Point2D(21, 0)
                },
                new[]
                {
                    new Point2D(1, 0),
                    new Point2D(2, 1),
                    new Point2D(16.142136, -13.142136),
                    new Point2D(15.142136, -14.142136)
                }
            }, areas, new Point2DComparerWithTolerance(1e-6));
        }

        private static IEnumerable<TestCaseData> GetCreateParameterMethods()
        {
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreateCohesionAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateCohesionAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressDailyAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateEffectiveStressDailyAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreateTotalPorePressureAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateTotalPorePressureAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreateWeightAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateWeightAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreatePiezometricPorePressureAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreatePiezometricPorePressureAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreatePorePressureAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreatePorePressureAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreateVerticalPorePressureAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateVerticalPorePressureAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreateHorizontalPorePressureAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateHorizontalPorePressureAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreateOverConsolidationRatioAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateOverConsolidationRatioAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateNormalStressAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateShearStressAreas));
            yield return new TestCaseData(new Func<MacroStabilityInwardsSlidingCurve, IEnumerable<IEnumerable<Point2D>>>(
                                              MacroStabilityInwardsSliceChartDataPointsFactory.CreatePopAreas))
                .SetName(nameof(MacroStabilityInwardsSliceChartDataPointsFactory.CreateLoadStressAreas));
        }

        private static IEnumerable<TestCaseData> GetValuesAboveUpperLimit()
        {
            yield return new TestCaseData(2000.0);
            yield return new TestCaseData(3500.0);
            yield return new TestCaseData(double.MaxValue);
        }

        private static IEnumerable<TestCaseData> GetValuesBelowLowerLimit()
        {
            yield return new TestCaseData(-2000.0);
            yield return new TestCaseData(-3500.0);
            yield return new TestCaseData(double.MinValue);
        }

        private static MacroStabilityInwardsSlidingCurve CreateSlidingCurve(MacroStabilityInwardsSlice.ConstructionProperties constructionProperties)
        {
            return new MacroStabilityInwardsSlidingCurve(
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                MacroStabilityInwardsSlidingCircleTestFactory.Create(), new[]
                {
                    new MacroStabilityInwardsSlice(new Point2D(0, 1),
                                                   new Point2D(1, 1),
                                                   new Point2D(0, 0),
                                                   new Point2D(1, 0),
                                                   constructionProperties),
                    new MacroStabilityInwardsSlice(new Point2D(0, 0),
                                                   new Point2D(0, 1),
                                                   new Point2D(1, 0),
                                                   new Point2D(1, 1),
                                                   constructionProperties),
                    new MacroStabilityInwardsSlice(new Point2D(0, 1),
                                                   new Point2D(1, 2),
                                                   new Point2D(1, 0),
                                                   new Point2D(2, 1),
                                                   constructionProperties)
                },
                0.0,
                0.0);
        }
    }
}