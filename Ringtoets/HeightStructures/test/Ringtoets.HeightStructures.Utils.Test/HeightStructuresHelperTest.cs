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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;

namespace Ringtoets.HeightStructures.Utils.Test
{
    [TestFixture]
    public class HeightStructuresHelperTest
    {
        private const string firstSectionName = "firstSection";
        private const string secondSectionName = "secondSection";

        [Test]
        public void CollectCalculationsPerSection_SectionsAreNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.CollectCalculationsPerSection(
                null,
                new HeightStructuresCalculation[]
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
            // Call
            TestDelegate test = () => HeightStructuresHelper.CollectCalculationsPerSection(
                new FailureMechanismSection[]
                {
                    null,
                    null
                },
                new[]
                {
                    calculationInSectionA
                });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void CollectCalculationsPerSection_CalculationsAreNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.CollectCalculationsPerSection(
                twoSections,
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void CollectCalculationsPerSection_CalculationElementsAreNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.CollectCalculationsPerSection(
                twoSections,
                new HeightStructuresCalculation[]
                {
                    null
                });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SectionsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresHelper.FailureMechanismSectionForCalculation(null, calculationInSectionA);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sections", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SectionElementsNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.FailureMechanismSectionForCalculation(
                new FailureMechanismSection[]
                {
                    null,
                    null
                },
                calculationInSectionA);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresHelper.FailureMechanismSectionForCalculation(oneSection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_ValidSectionWithoutCalculationStructureSet_ReturnsNull()
        {
            // Setup
            var calculation = new HeightStructuresCalculation();

            // Call
            FailureMechanismSection failureMechanismSection =
                HeightStructuresHelper.FailureMechanismSectionForCalculation(oneSection, calculation);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_EmptySectionWithoutCalculationStructureSet_ReturnsNull()
        {
            // Setup
            var emptySections = new FailureMechanismSection[0];
            var calculation = new HeightStructuresCalculation();

            // Call
            FailureMechanismSection failureMechanismSection =
                HeightStructuresHelper.FailureMechanismSectionForCalculation(emptySections, calculation);

            // Assert
            Assert.IsNull(failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_FirstSectionContainsCalculation_FailureMechanismSectionOfFirstSection()
        {
            // Call
            FailureMechanismSection failureMechanismSection =
                HeightStructuresHelper.FailureMechanismSectionForCalculation(twoSections, calculationInSectionA);

            // Assert
            Assert.AreSame(twoSections[0], failureMechanismSection);
        }

        [Test]
        public void FailureMechanismSectionForCalculation_SecondSectionContainsCalculation_FailureMechanismSectionOfSecondSection()
        {
            // Call
            FailureMechanismSection failureMechanismSection =
                HeightStructuresHelper.FailureMechanismSectionForCalculation(twoSections, calculationInSectionB);

            // Assert
            Assert.AreSame(twoSections[1], failureMechanismSection);
        }

        [Test]
        public void Update_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.Update(
                null,
                calculationInSectionA);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void Update_SectionResultElementsNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.Update(
                new HeightStructuresFailureMechanismSectionResult[]
                {
                    null,
                    null
                },
                calculationInSectionA);

            // Assert
            ArgumentException exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                test, "SectionResults contains an entry without value.");
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void Update_SectionResultWithoutCalculationAndValidCalculation_UpdatesSectionResult()
        {
            // Setup
            var failureMechanismSectionResult = new HeightStructuresFailureMechanismSectionResult(
                failureMechanismSectionA);

            // Call
            HeightStructuresHelper.Update(new[]
            {
                failureMechanismSectionResult
            }, calculationInSectionA);

            // Assert
            Assert.AreSame(calculationInSectionA, failureMechanismSectionResult.Calculation);
        }

        [Test]
        public void Delete_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.Delete(
                null,
                calculationInSectionA,
                new[]
                {
                    calculationInSectionB
                });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void Delete_SectionResultElementsNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.Delete(
                new HeightStructuresFailureMechanismSectionResult[]
                {
                    null,
                    null
                },
                calculationInSectionA,
                new[]
                {
                    calculationInSectionB
                });

            // Assert
            ArgumentException exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                test, "SectionResults contains an entry without value.");
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void Delete_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.Delete(
                new[]
                {
                    sectionResult
                },
                null,
                new[]
                {
                    calculationInSectionA
                });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Delete_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.Delete(
                new[]
                {
                    sectionResult
                },
                calculationInSectionA,
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Delete_CalculationsElementNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresHelper.Delete(
                new[]
                {
                    sectionResult
                },
                calculationInSectionA,
                new HeightStructuresCalculation[]
                {
                    null
                });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Delete_SectionResultWithCalculationNoRemainingCalculations_SectionResultCalculationIsNull()
        {
            // Setup
            var failureMechanismSectionResult = new HeightStructuresFailureMechanismSectionResult(
                failureMechanismSectionA)
            {
                Calculation = calculationInSectionA
            };

            // Call
            HeightStructuresHelper.Delete(
                new[]
                {
                    failureMechanismSectionResult
                },
                calculationInSectionA,
                Enumerable.Empty<HeightStructuresCalculation>());

            // Assert
            Assert.IsNull(failureMechanismSectionResult.Calculation);
        }

        [Test]
        public void Delete_SectionResultWithCalculationWithRemainingCalculations_SectionResultCalculationSetToRemainingCalculation()
        {
            // Setup
            var failureMechanismSectionResult = new HeightStructuresFailureMechanismSectionResult(
                failureMechanismSectionA)
            {
                Calculation = calculationInSectionA
            };

            // Call
            HeightStructuresHelper.Delete(
                new[]
                {
                    failureMechanismSectionResult
                },
                calculationInSectionA,
                new[]
                {
                    calculationInSectionB
                });

            // Assert
            Assert.AreSame(calculationInSectionB, failureMechanismSectionResult.Calculation);
        }

        #region Prepared data

        private static readonly FailureMechanismSection failureMechanismSectionA = new FailureMechanismSection(firstSectionName, new List<Point2D>
        {
            new Point2D(0.0, 0.0),
            new Point2D(10.0, 10.0),
        });

        private static readonly FailureMechanismSection failureMechanismSectionB = new FailureMechanismSection(secondSectionName, new List<Point2D>
        {
            new Point2D(11.0, 11.0),
            new Point2D(100.0, 100.0),
        });

        private static readonly HeightStructuresFailureMechanismSectionResult sectionResult = new HeightStructuresFailureMechanismSectionResult(
            failureMechanismSectionA);

        private readonly FailureMechanismSection[] oneSection =
        {
            failureMechanismSectionA
        };

        private readonly FailureMechanismSection[] twoSections =
        {
            failureMechanismSectionA,
            failureMechanismSectionB
        };

        private readonly HeightStructuresCalculation calculationInSectionA = new HeightStructuresCalculation
        {
            InputParameters =
            {
                Structure = new TestHeightStructure(new Point2D(1.1, 2.2))
            }
        };

        private readonly HeightStructuresCalculation calculationInSectionB = new HeightStructuresCalculation
        {
            InputParameters =
            {
                Structure = new TestHeightStructure(new Point2D(50.0, 66.0))
            }
        };

        #endregion
    }
}