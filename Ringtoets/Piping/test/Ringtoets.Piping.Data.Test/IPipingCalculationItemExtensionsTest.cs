// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class IPipingCalculationItemExtensionsTest
    {
        [Test]
        public void GetPipingCalculations_FromPipingCalculation_ReturnThatCalculationInstance()
        {
            // Setup
            ICalculation calculation = new PipingCalculationScenario(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            // Call
            IEnumerable<PipingCalculationScenario> result = calculation.GetPipingCalculations();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, result);
        }

        [Test]
        public void GetPipingCalculations_FromPipingCalculationGroupWithoutChildren_ReturnEmpty()
        {
            // Setup
            ICalculation groupWithoutChildren = new PipingCalculationGroup();

            // Call
            IEnumerable<PipingCalculation> result = groupWithoutChildren.GetPipingCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromPipingCalculationGroupWithEmptyGroups_ReturnEmpty()
        {
            // Setup
            var rootGroup = new PipingCalculationGroup();
            rootGroup.Children.Add(new PipingCalculationGroup());
            rootGroup.Children.Add(new PipingCalculationGroup());
            rootGroup.Children.Add(new PipingCalculationGroup());

            ICalculation groupsWithoutChildren = rootGroup;

            // Call
            IEnumerable<PipingCalculationScenario> result = groupsWithoutChildren.GetPipingCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromPipingCalculationGroupWithGroupsAndCalculations_ReturnAllCalculationsRecursiveslyInAnyOrder()
        {
            // Setup
            var generalPipingInput = new GeneralPipingInput();
            var semiProbabilisticInput = new SemiProbabilisticPipingInput();
            var calculation1 = new PipingCalculationScenario(generalPipingInput, semiProbabilisticInput);
            var calculation2 = new PipingCalculationScenario(generalPipingInput, semiProbabilisticInput);
            var calculation3 = new PipingCalculationScenario(generalPipingInput, semiProbabilisticInput);
            var calculation4 = new PipingCalculationScenario(generalPipingInput, semiProbabilisticInput);

            var subsubGroup = new PipingCalculationGroup();
            subsubGroup.Children.Add(calculation4);

            var subgroup1 = new PipingCalculationGroup();
            subgroup1.Children.Add(calculation2);
            subgroup1.Children.Add(subsubGroup);

            var subgroup2 = new PipingCalculationGroup();
            subgroup2.Children.Add(calculation3);

            var rootGroup = new PipingCalculationGroup();
            rootGroup.Children.Add(subgroup1);
            rootGroup.Children.Add(calculation1);
            rootGroup.Children.Add(subgroup2);

            ICalculation groupWithoutChildren = rootGroup;

            // Call
            IEnumerable<PipingCalculationScenario> result = groupWithoutChildren.GetPipingCalculations();

            // Assert
            var itemsThatShouldBeFound = new[]
            {
                calculation1,
                calculation2,
                calculation3,
                calculation4
            };
            CollectionAssert.AreEquivalent(itemsThatShouldBeFound, result);
        }

        [Test]
        public void GetPipingCalculations_FromEmptyEnummerable_ReturnEmpty()
        {
            // Setup
            IEnumerable<ICalculation> emptyEnumerable = Enumerable.Empty<ICalculation>();

            // Call
            IEnumerable<PipingCalculationScenario> result = emptyEnumerable.GetPipingCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromArrayWithCalculations_ReturnAllThoseCalculationsInAnyOrder()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInput = new SemiProbabilisticPipingInput();
            var calculation1 = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInput);
            var calculation2 = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInput);
            IEnumerable<ICalculation> calculationArray = new[]
            {
                calculation1,
                calculation2
            };

            // Call
            IEnumerable<PipingCalculationScenario> result = calculationArray.GetPipingCalculations();

            // Assert
            CollectionAssert.AreEquivalent(calculationArray, result);
        }

        [Test]
        public void GetPipingCalculations_FromArrayWithEmptyGroups_ReturnEmpty()
        {
            // Setup
            var emptyGroup1 = new PipingCalculationGroup();
            var emptyGroup2 = new PipingCalculationGroup();
            IEnumerable<ICalculation> emptyEnumerable = new[]
            {
                emptyGroup1,
                emptyGroup2
            };

            // Call
            IEnumerable<PipingCalculationScenario> result = emptyEnumerable.GetPipingCalculations();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetPipingCalculations_FromArrayWithMixedGroupsAndCalculations_ReturnAllCalculationsInAnyOrder()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInput = new SemiProbabilisticPipingInput();
            var rootcalculation = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInput);
            var calculation1 = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInput);
            var calculation2 = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInput);
            var calculation3 = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInput);
            var calculation4 = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInput);

            var subsubGroup = new PipingCalculationGroup();
            subsubGroup.Children.Add(calculation4);

            var subgroup1 = new PipingCalculationGroup();
            subgroup1.Children.Add(calculation2);
            subgroup1.Children.Add(subsubGroup);

            var subgroup2 = new PipingCalculationGroup();
            subgroup2.Children.Add(calculation3);

            var rootGroup = new PipingCalculationGroup();
            rootGroup.Children.Add(subgroup1);
            rootGroup.Children.Add(calculation1);
            rootGroup.Children.Add(subgroup2);

            var emptyRootGroup = new PipingCalculationGroup();

            IEnumerable<ICalculation> mixedArray = new ICalculation[]
            {
                emptyRootGroup,
                rootGroup,
                rootcalculation
            };

            // Call
            IEnumerable<PipingCalculationScenario> result = mixedArray.GetPipingCalculations();

            // Assert
            var expectedCalculationItems = new[]
            {
                rootcalculation,
                calculation1,
                calculation2,
                calculation3,
                calculation4
            };
            CollectionAssert.AreEquivalent(expectedCalculationItems, result);
        }

        #region IsSurfaceLineIntersectionWithReferenceLineInSection

        [Test]
        public void IsSurfaceLineIntersectionWithReferenceLineInSection_IPipingCalculationItemNotPipingCalculationScenario_ReturnsFalse()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            // Call
            var intersects = calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Enumerable.Empty<Segment2D>());

            // Assert
            Assert.IsFalse(intersects);
        }

        [Test]
        public void IsSurfaceLineIntersectionWithReferenceLineInSection_SurfaceLineNull_ReturnsFalse()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            // Call
            var intersects = calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Enumerable.Empty<Segment2D>());

            // Assert
            Assert.IsFalse(intersects);
        }

        [Test]
        public void IsSurfaceLineIntersectionWithReferenceLineInSection_EmptySegmentCollection_ThrowsInvalidOperationException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 0.0)
            });

            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            // Call
            TestDelegate call = () => calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Enumerable.Empty<Segment2D>());

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void IsSurfaceLineIntersectionWithReferenceLineInSection_SurfaceLineIntersectsReferenceline_ReturnsTrue()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 0.0)
            });

            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            var lineSegments = Math2D.ConvertLinePointsToLineSegments(referenceLine.Points);

            // Call
            var intersects = calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments);

            // Assert
            Assert.IsTrue(intersects);
        }

        [Test]
        public void IsSurfaceLineIntersectionWithReferenceLineInSection_SurfaceLineDoesNotIntersectsReferenceline_ReturnsFalse()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(20.0, 0.0)
            });

            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            var lineSegments = Math2D.ConvertLinePointsToLineSegments(referenceLine.Points);

            // Call
            var intersects = calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments);

            // Assert
            Assert.IsFalse(intersects);
        }

        #endregion
    }
}