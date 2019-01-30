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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class StructuresHelperTest
    {
        [Test]
        public void CollectCalculationsPerSection_SectionsAreNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StructuresHelper.CollectCalculationsPerSection(
                null,
                new StructuresCalculation<TestStructuresInput>[]
                {
                    null
                });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void CollectCalculationsPerSection_SectionElementsAreNull_ThrowsArgumentException()
        {
            // Setup
            var structuresCalculation = new StructuresCalculation<TestStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure()
                }
            };

            // Call
            TestDelegate test = () => StructuresHelper.CollectCalculationsPerSection(new FailureMechanismSection[]
                                                                                     {
                                                                                         null,
                                                                                         null
                                                                                     },
                                                                                     new[]
                                                                                     {
                                                                                         structuresCalculation
                                                                                     });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void CollectCalculationsPerSection_CalculationsAreNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StructuresHelper.CollectCalculationsPerSection<TestStructuresInput>(
                new[]
                {
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
                },
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void CollectCalculationsPerSection_CalculationElementsAreNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StructuresHelper.CollectCalculationsPerSection(
                new[]
                {
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
                },
                new StructuresCalculation<TestStructuresInput>[]
                {
                    null
                });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void CollectCalculationsPerSegment_ValidEmptySectionResults_EmptyDictionary()
        {
            // Setup
            var structuresCalculation = new StructuresCalculation<TestStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure(new Point2D(1.1, 2.2))
                }
            };

            // Call
            IDictionary<string, List<ICalculation>> collectCalculationsPerSegment =
                StructuresHelper.CollectCalculationsPerSection(Enumerable.Empty<FailureMechanismSection>(), new[]
                {
                    structuresCalculation
                });

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_ValidEmptyCalculations_EmptyDictionary()
        {
            // Call
            IDictionary<string, List<ICalculation>> collectCalculationsPerSegment =
                StructuresHelper.CollectCalculationsPerSection(new[]
                                                               {
                                                                   FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
                                                               },
                                                               Enumerable.Empty<StructuresCalculation<TestStructuresInput>>());

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_MultipleCalculationsInSegment_OneSegmentHasAllCalculations()
        {
            // Setup
            const string firstSectionName = "A";
            var location = new Point2D(1, 1);

            var calculationOne = new StructuresCalculation<TestStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure(location)
                }
            };
            var calculationTwo = new StructuresCalculation<TestStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure(location)
                }
            };
            StructuresCalculation<TestStructuresInput>[] calculations =
            {
                calculationOne,
                calculationTwo
            };

            // Call
            IDictionary<string, List<ICalculation>> collectCalculationsPerSegment =
                StructuresHelper.CollectCalculationsPerSection(new[]
                {
                    new FailureMechanismSection(firstSectionName, new[]
                    {
                        location,
                        new Point2D(2, 2)
                    }),
                    new FailureMechanismSection("B", new[]
                    {
                        new Point2D(2, 2),
                        new Point2D(3, 3)
                    })
                }, calculations);

            // Assert
            Assert.AreEqual(1, collectCalculationsPerSegment.Count);
            Assert.IsTrue(collectCalculationsPerSegment.ContainsKey(firstSectionName));
            CollectionAssert.AreEqual(calculations, collectCalculationsPerSegment[firstSectionName]);
        }

        [Test]
        public void CollectCalculationsPerSegment_SingleCalculationPerSegment_OneCalculationPerSegment()
        {
            // Setup
            const string firstSectionName = "A";
            const string secondSectionName = "B";

            var locationOne = new Point2D(1, 1);
            var locationTwo = new Point2D(3, 3);

            var calculationOne = new StructuresCalculation<TestStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure(locationOne)
                }
            };
            var calculationTwo = new StructuresCalculation<TestStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStructure(locationTwo)
                }
            };
            StructuresCalculation<TestStructuresInput>[] calculations =
            {
                calculationOne,
                calculationTwo
            };

            var failureMechanismSections = new[]
            {
                new FailureMechanismSection(firstSectionName, new[]
                {
                    locationOne,
                    new Point2D(2, 2)
                }),
                new FailureMechanismSection(secondSectionName, new[]
                {
                    new Point2D(2, 2),
                    locationTwo
                })
            };

            // Call
            IDictionary<string, List<ICalculation>> collectCalculationsPerSegment =
                StructuresHelper.CollectCalculationsPerSection(failureMechanismSections, calculations);

            // Assert
            Assert.AreEqual(2, collectCalculationsPerSegment.Count);
            Assert.AreEqual(1, collectCalculationsPerSegment[firstSectionName].Count);
            Assert.AreSame(calculationOne, collectCalculationsPerSegment[firstSectionName][0]);
            Assert.AreEqual(1, collectCalculationsPerSegment[secondSectionName].Count);
            Assert.AreSame(calculationTwo, collectCalculationsPerSegment[secondSectionName][0]);
        }

        private class TestStructuresInput : StructuresInputBase<StructureBase>
        {
            public override bool IsStructureInputSynchronized
            {
                get
                {
                    return false;
                }
            }

            public override void SynchronizeStructureInput() {}
        }
    }
}