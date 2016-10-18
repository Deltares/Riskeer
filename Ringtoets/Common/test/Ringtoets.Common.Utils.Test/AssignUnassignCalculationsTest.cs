﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Utils.Test
{
    [TestFixture]
    public class AssignUnassignCalculationsTest
    {
        private const string firstSectionName = "firstSection";
        private const string secondSectionName = "secondSection";

        private readonly FailureMechanismSection[] oneSection =
        {
            new FailureMechanismSection("testFailureMechanismSection", new List<Point2D>
            {
                new Point2D(0.0, 0.0)
            })
        };

        private readonly FailureMechanismSection[] twoSections =
        {
            new FailureMechanismSection(firstSectionName, new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(10.0, 10.0),
            }),
            new FailureMechanismSection(secondSectionName, new List<Point2D>
            {
                new Point2D(11.0, 11.0),
                new Point2D(100.0, 100.0),
            })
        };

        [Test]
        public void Update_NullSectionResultsElement_ThrowsArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            var calculationWithLocation = new CalculationWithLocation(calculation, new Point2D(0, 0));

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(
                new SectionResultWithCalculationAssignment[]
                {
                    null
                },
                calculationWithLocation);

            // Assert
            ArgumentException exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "SectionResults contains an entry without value.");
            Assert.AreEqual("sectionResults", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Update_NullCalculation_ThrowsArgumentNullException()
        {
            // Setup
            var sectionResults = Enumerable.Empty<SectionResultWithCalculationAssignment>();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(sectionResults, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void Update_NullSectionResults_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            var calculationWithLocation = new CalculationWithLocation(calculation, new Point2D(0, 0));

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Update(null, calculationWithLocation);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Update_CalculationLocationChangedToMatchOtherSection_FirstSectionResultCalculationNullSecondSectionResultCalculationSet()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            var calculationLocation = new Point2D(1.51, 1.51);

            var sectionA = new FailureMechanismSection("firstSection", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            });
            var sectionB = new FailureMechanismSection("secondSection", new List<Point2D>
            {
                new Point2D(1.1, 1.1), new Point2D(2.2, 2.2)
            });

            var sectionResultA = new FailureMechanismSectionResultWithCalculation(sectionA);
            var sectionResultB = new FailureMechanismSectionResultWithCalculation(sectionB);

            var sectionResults = new List<SectionResultWithCalculationAssignment>
            {
                new SectionResultWithCalculationAssignment(sectionResultA,
                                                           r => ((FailureMechanismSectionResultWithCalculation) r).Calculation,
                                                           (r, c) => ((FailureMechanismSectionResultWithCalculation) r).Calculation = c),
                new SectionResultWithCalculationAssignment(sectionResultB,
                                                           r => ((FailureMechanismSectionResultWithCalculation) r).Calculation,
                                                           (r, c) => ((FailureMechanismSectionResultWithCalculation) r).Calculation = c)
            };
            sectionResultA.Calculation = calculation;

            // Call
            AssignUnassignCalculations.Update(sectionResults, new CalculationWithLocation(calculation, calculationLocation));

            // Assert
            Assert.IsNull(sectionResultA.Calculation);
            Assert.AreSame(calculation, sectionResultB.Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Update_CalculationCalculationChangedToMatchOtherSection_FirstSectionResultCalculationNullSecondSectionResultCalculationUnchanged()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationA = mockRepository.Stub<ICalculation>();
            var calculationB = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            var locationB = new Point2D(1.51, 1.51);

            var sectionA = new FailureMechanismSection("firstSection", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            });
            var sectionB = new FailureMechanismSection("secondSection", new List<Point2D>
            {
                new Point2D(1.1, 1.1), new Point2D(2.2, 2.2)
            });

            var sectionResultA = new FailureMechanismSectionResultWithCalculation(sectionA);
            var sectionResultB = new FailureMechanismSectionResultWithCalculation(sectionB);

            var sectionResults = new List<SectionResultWithCalculationAssignment>
            {
                new SectionResultWithCalculationAssignment(sectionResultA,
                                                           r => ((FailureMechanismSectionResultWithCalculation) r).Calculation,
                                                           (r, c) => ((FailureMechanismSectionResultWithCalculation) r).Calculation = c),
                new SectionResultWithCalculationAssignment(sectionResultB,
                                                           r => ((FailureMechanismSectionResultWithCalculation) r).Calculation,
                                                           (r, c) => ((FailureMechanismSectionResultWithCalculation) r).Calculation = c)
            };
            sectionResultA.Calculation = calculationA;
            sectionResultB.Calculation = calculationB;

            // Call
            AssignUnassignCalculations.Update(sectionResults, new CalculationWithLocation(calculationA, locationB));

            // Assert
            Assert.IsNull(sectionResultA.Calculation);
            Assert.AreSame(calculationB, sectionResultB.Calculation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Delete_NullSectionResults_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Delete(
                null,
                calculation,
                Enumerable.Empty<CalculationWithLocation>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sectionResults", paramName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Delete_NullSectionResultsElement_ThrowsArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Delete(
                new SectionResultWithCalculationAssignment[]
                {
                    null
                },
                calculation,
                Enumerable.Empty<CalculationWithLocation>());

            // Assert
            ArgumentException exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "SectionResults contains an entry without value.");
            Assert.AreEqual("sectionResults", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Delete_NullCalculation_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssignUnassignCalculations.Delete(
                Enumerable.Empty<SectionResultWithCalculationAssignment>(),
                null,
                Enumerable.Empty<CalculationWithLocation>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void Delete_NullCalculations_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Delete(
                Enumerable.Empty<SectionResultWithCalculationAssignment>(),
                calculation,
                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculations", paramName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Delete_NullCalculationsElement_ThrowsArgumentException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => AssignUnassignCalculations.Delete(
                Enumerable.Empty<SectionResultWithCalculationAssignment>(),
                calculation,
                new CalculationWithLocation[]
                {
                    null
                });

            // Assert
            ArgumentException exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Calculations contains an entry without value.");
            Assert.AreEqual("calculations", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Delete_RemoveCalculationAssignedToSectionResult_SectionResultCalculationNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            var section = new FailureMechanismSection("firstSection", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            });

            var sectionResult = new FailureMechanismSectionResultWithCalculation(section);

            var sectionResults = new List<SectionResultWithCalculationAssignment>
            {
                new SectionResultWithCalculationAssignment(sectionResult,
                                                           r => ((FailureMechanismSectionResultWithCalculation) r).Calculation,
                                                           (r, c) => ((FailureMechanismSectionResultWithCalculation) r).Calculation = c)
            };
            sectionResult.Calculation = calculation;

            // Call
            AssignUnassignCalculations.Delete(sectionResults, calculation, Enumerable.Empty<CalculationWithLocation>());

            // Assert
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void Delete_RemoveCalculationAssignedToSectionResult_SingleRemainingCalculationAssignedToSectionResult()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationA = mockRepository.Stub<ICalculation>();
            var calculationB = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            var location = new Point2D(0.51, 0.51);

            var sectionA = new FailureMechanismSection("firstSection", new List<Point2D>
            {
                new Point2D(0.0, 0.0), new Point2D(1.1, 1.1)
            });

            var sectionResult = new FailureMechanismSectionResultWithCalculation(sectionA);

            var sectionResults = new List<SectionResultWithCalculationAssignment>
            {
                new SectionResultWithCalculationAssignment(sectionResult,
                                                           r => ((FailureMechanismSectionResultWithCalculation) r).Calculation,
                                                           (r, c) => ((FailureMechanismSectionResultWithCalculation) r).Calculation = c)
            };
            sectionResult.Calculation = calculationA;

            // Call
            AssignUnassignCalculations.Delete(sectionResults, calculationA, new[]
            {
                new CalculationWithLocation(calculationB, location),
            });

            // Assert
            Assert.AreSame(calculationB, sectionResult.Calculation);
        }

        [Test]
        public void CollectCalculationsPerSegment_NullSections_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssignUnassignCalculations.CollectCalculationsPerSection(null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sections", paramName);
        }

        [Test]
        public void CollectCalculationsPerSegment_NullSectionsElement_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => AssignUnassignCalculations.CollectCalculationsPerSection(new FailureMechanismSection[]
            {
                null
            }, Enumerable.Empty<CalculationWithLocation>());

            // Assert
            ArgumentException exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Sections contains an entry without value.");
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void CollectCalculationsPerSegment_NullCalculations_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssignUnassignCalculations.CollectCalculationsPerSection(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculations", paramName);
        }

        [Test]
        public void CollectCalculationsPerSegment_NullCalculationsElement_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => AssignUnassignCalculations.CollectCalculationsPerSection(Enumerable.Empty<FailureMechanismSection>(), new CalculationWithLocation[]
            {
                null
            });

            // Assert
            ArgumentException exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Calculations contains an entry without value.");
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void CollectCalculationsPerSegment_ValidEmptySectionResults_EmptyDictionary()
        {
            // Setup
            var mockRepository = new MockRepository();
            IEnumerable<CalculationWithLocation> twoCalculationsInFirstSection = CreateTwoCalculationsInFirstSection(mockRepository);
            mockRepository.ReplayAll();

            // Call
            Dictionary<string, IList<ICalculation>> collectCalculationsPerSegment =
                AssignUnassignCalculations.CollectCalculationsPerSection(Enumerable.Empty<FailureMechanismSection>(), twoCalculationsInFirstSection);

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CollectCalculationsPerSegment_ValidEmptyCalculations_EmptyDictionary()
        {
            // Call
            Dictionary<string, IList<ICalculation>> collectCalculationsPerSegment =
                AssignUnassignCalculations.CollectCalculationsPerSection(oneSection, Enumerable.Empty<CalculationWithLocation>());

            // Assert
            Assert.AreEqual(0, collectCalculationsPerSegment.Count);
        }

        [Test]
        public void CollectCalculationsPerSegment_MultipleCalculationsInSegment_OneSegmentHasAllCalculations()
        {
            // Setup
            var mockRepository = new MockRepository();
            IEnumerable<CalculationWithLocation> twoCalculationsInFirstSection = CreateTwoCalculationsInFirstSection(mockRepository);
            mockRepository.ReplayAll();

            // Call
            Dictionary<string, IList<ICalculation>> collectCalculationsPerSegment =
                AssignUnassignCalculations.CollectCalculationsPerSection(twoSections, twoCalculationsInFirstSection);

            // Assert
            Assert.AreEqual(1, collectCalculationsPerSegment.Count);
            Assert.IsTrue(collectCalculationsPerSegment.ContainsKey(firstSectionName));
            CollectionAssert.AreEqual(twoCalculationsInFirstSection.Select(cwl => cwl.Calculation), collectCalculationsPerSegment[firstSectionName]);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CollectCalculationsPerSegment_SingleCalculationPerSegment_OneCalculationPerSegment()
        {
            var mockRepository = new MockRepository();
            CalculationWithLocation[] oneCalculationInEachSection = CreateOneCalculationInEachSection(mockRepository);
            mockRepository.ReplayAll();

            // Call
            Dictionary<string, IList<ICalculation>> collectCalculationsPerSegment =
                AssignUnassignCalculations.CollectCalculationsPerSection(twoSections, oneCalculationInEachSection);

            // Assert
            Assert.AreEqual(2, collectCalculationsPerSegment.Count);
            Assert.AreEqual(1, collectCalculationsPerSegment[firstSectionName].Count);
            Assert.AreSame(oneCalculationInEachSection[0].Calculation, collectCalculationsPerSegment[firstSectionName][0]);
            Assert.AreEqual(1, collectCalculationsPerSegment[secondSectionName].Count);
            Assert.AreSame(oneCalculationInEachSection[1].Calculation, collectCalculationsPerSegment[secondSectionName][0]);
            mockRepository.VerifyAll();
        }

        [Test]
        public void FailureMechanismSectionForCalculation_NullSections_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssignUnassignCalculations.FailureMechanismSectionForCalculation(null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sections", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_NullSectionsElement_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => AssignUnassignCalculations.FailureMechanismSectionForCalculation(
                new FailureMechanismSection[]
                {
                    null
                },
                null);

            // Assert
            ArgumentException exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                call,
                "Sections contains an entry without value.");
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssignUnassignCalculations.FailureMechanismSectionForCalculation(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_ValidEmptySectionResults_ReturnsNull()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            FailureMechanismSection failureMechanismSection =
                AssignUnassignCalculations.FailureMechanismSectionForCalculation(Enumerable.Empty<FailureMechanismSection>(), new CalculationWithLocation(calculation, new Point2D(0, 0)));

            // Assert
            Assert.IsNull(failureMechanismSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void FailureMechanismSectionForCalculation_FirstSectionResultContainsCalculation_FailureMechanismSectionOfFirstSectionResult()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            FailureMechanismSection failureMechanismSection =
                AssignUnassignCalculations.FailureMechanismSectionForCalculation(twoSections, new CalculationWithLocation(calculation, new Point2D(1.1, 2.2)));

            // Assert
            Assert.AreSame(twoSections[0], failureMechanismSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SecondSectionResultContainsCalculation_FailureMechanismSectionOfSecondSectionResult()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculation = mockRepository.Stub<ICalculation>();
            mockRepository.ReplayAll();

            // Call
            FailureMechanismSection failureMechanismSection =
                AssignUnassignCalculations.FailureMechanismSectionForCalculation(twoSections, new CalculationWithLocation(calculation, new Point2D(50.0, 66.0)));

            // Assert
            Assert.AreSame(twoSections[1], failureMechanismSection);
            mockRepository.VerifyAll();
        }

        private static CalculationWithLocation[] CreateTwoCalculationsInFirstSection(MockRepository mockRepository)
        {
            return new[]
            {
                new CalculationWithLocation(mockRepository.Stub<ICalculation>(), new Point2D(1.1, 2.2)),
                new CalculationWithLocation(mockRepository.Stub<ICalculation>(), new Point2D(3.3, 4.4))
            };
        }

        private static CalculationWithLocation[] CreateOneCalculationInEachSection(MockRepository mockRepository)
        {
            return new[]
            {
                new CalculationWithLocation(mockRepository.Stub<ICalculation>(), new Point2D(1.1, 2.2)),
                new CalculationWithLocation(mockRepository.Stub<ICalculation>(), new Point2D(50.0, 66.0))
            };
        }

        private class FailureMechanismSectionResultWithCalculation : FailureMechanismSectionResult
        {
            public FailureMechanismSectionResultWithCalculation(FailureMechanismSection section) : base(section) {}
            public ICalculation Calculation { get; set; }
        }
    }
}