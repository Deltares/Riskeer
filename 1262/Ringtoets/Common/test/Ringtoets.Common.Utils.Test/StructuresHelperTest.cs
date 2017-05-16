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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.Common.Utils.Test
{
    [TestFixture]
    public class StructuresHelperTest
    {
        private const string firstSectionName = "firstSection";
        private const string secondSectionName = "secondSection";

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
            // Call
            TestDelegate test = () => StructuresHelper.CollectCalculationsPerSection(
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
            TestDelegate test = () => StructuresHelper.CollectCalculationsPerSection<TestStructuresInput>(
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
            TestDelegate test = () => StructuresHelper.CollectCalculationsPerSection(
                twoSections,
                new StructuresCalculation<TestStructuresInput>[]
                {
                    null
                });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void UpdateCalculationToSectionResultAssignments_SectionResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StructuresHelper.UpdateCalculationToSectionResultAssignments(
                null,
                new[]
                {
                    calculationInSectionB
                });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void UpdateCalculationToSectionResultAssignments_SectionResultElementsNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => StructuresHelper.UpdateCalculationToSectionResultAssignments(
                new TestStructuresFailureMechanismSectionResult[]
                {
                    null,
                    null
                },
                new[]
                {
                    calculationInSectionB
                });

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(
                test, "SectionResults contains an entry without value.");
            Assert.AreEqual("sectionResults", exception.ParamName);
        }

        [Test]
        public void UpdateCalculationToSectionResultAssignments_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StructuresHelper.UpdateCalculationToSectionResultAssignments(
                new[]
                {
                    sectionResult
                },
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void UpdateCalculationToSectionResultAssignments_CalculationsElementNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StructuresHelper.UpdateCalculationToSectionResultAssignments(
                new[]
                {
                    sectionResult
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
        public void UpdateCalculationToSectionResultAssignments_SectionResultWithCalculationNoRemainingCalculations_SectionResultCalculationIsNull()
        {
            // Setup
            var failureMechanismSectionResult = new TestStructuresFailureMechanismSectionResult(
                failureMechanismSectionA)
            {
                Calculation = calculationInSectionA
            };

            // Call
            StructuresHelper.UpdateCalculationToSectionResultAssignments(
                new[]
                {
                    failureMechanismSectionResult
                },
                Enumerable.Empty<StructuresCalculation<TestStructuresInput>>());

            // Assert
            Assert.IsNull(failureMechanismSectionResult.Calculation);
        }

        [Test]
        public void UpdateCalculationToSectionResultAssignments_SectionResultWithCalculationWithRemainingCalculations_SectionResultCalculationSetToRemainingCalculation()
        {
            // Setup
            var failureMechanismSectionResult = new TestStructuresFailureMechanismSectionResult(
                failureMechanismSectionA)
            {
                Calculation = calculationInSectionA
            };

            // Call
            StructuresHelper.UpdateCalculationToSectionResultAssignments(
                new[]
                {
                    failureMechanismSectionResult
                },
                new[]
                {
                    calculationInSectionB
                });

            // Assert
            Assert.AreSame(calculationInSectionB, failureMechanismSectionResult.Calculation);
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

        private class TestStructuresFailureMechanismSectionResult : StructuresFailureMechanismSectionResult<TestStructuresInput>
        {
            public TestStructuresFailureMechanismSectionResult(FailureMechanismSection section) : base(section) {}
        }

        #region Prepared data

        private static readonly FailureMechanismSection failureMechanismSectionA = new FailureMechanismSection(firstSectionName, new List<Point2D>
        {
            new Point2D(0.0, 0.0),
            new Point2D(10.0, 10.0)
        });

        private static readonly FailureMechanismSection failureMechanismSectionB = new FailureMechanismSection(secondSectionName, new List<Point2D>
        {
            new Point2D(11.0, 11.0),
            new Point2D(100.0, 100.0)
        });

        private static readonly TestStructuresFailureMechanismSectionResult sectionResult = new TestStructuresFailureMechanismSectionResult(
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

        private readonly StructuresCalculation<TestStructuresInput> calculationInSectionA = new StructuresCalculation<TestStructuresInput>
        {
            InputParameters =
            {
                Structure = new TestStructure("aName", "anId", new Point2D(1.1, 2.2), 0.0)
            }
        };

        private readonly StructuresCalculation<TestStructuresInput> calculationInSectionB = new StructuresCalculation<TestStructuresInput>
        {
            InputParameters =
            {
                Structure = new TestStructure("aName", "anId", new Point2D(50.0, 66.0), 0.0)
            }
        };

        private class TestStructure : StructureBase
        {
            public TestStructure(string name, string id, Point2D location, double normal) : base(new ConstructionProperties
            {
                Name = name,
                Id = id,
                Location = location,
                StructureNormalOrientation = (RoundedDouble) normal
            }) {}
        }

        #endregion
    }
}