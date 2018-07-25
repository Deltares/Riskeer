﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismBaseTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_NullOrEmptyName_ThrowsArgumentException(string name)
        {
            // Call
            TestDelegate test = () => new SimpleFailureMechanismBase(name, "testCode");

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("failureMechanismName", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor_NullOrEmptyCode_ThrowsArgumentException(string code)
        {
            // Call
            TestDelegate test = () => new SimpleFailureMechanismBase("testName", code);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("failureMechanismCode", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "<cool name>";
            const string code = "<cool code>";
            int group = new Random(39).Next();

            // Call
            var failureMechanism = new SimpleFailureMechanismBase(name, code, group);

            // Assert
            Assert.IsInstanceOf<Observable>(failureMechanism);
            Assert.IsInstanceOf<IFailureMechanism>(failureMechanism);
            Assert.AreEqual(0, failureMechanism.Contribution);
            Assert.AreEqual(name, failureMechanism.Name);
            Assert.AreEqual(code, failureMechanism.Code);
            Assert.AreEqual(group, failureMechanism.Group);
            Assert.IsNotNull(failureMechanism.InputComments);
            Assert.IsNotNull(failureMechanism.OutputComments);
            Assert.IsNotNull(failureMechanism.NotRelevantComments);
            Assert.IsTrue(failureMechanism.IsRelevant);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsRelevant_SetNewValue_GetNewlySetValue(bool relevant)
        {
            // Setup
            var failureMechanism = new SimpleFailureMechanismBase();

            // Call
            failureMechanism.IsRelevant = relevant;

            // Assert
            Assert.AreEqual(relevant, failureMechanism.IsRelevant);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.NaN)]
        [TestCase(101)]
        [TestCase(-1e-6)]
        [TestCase(-1)]
        [TestCase(100 + 1e-6)]
        public void Contribution_ValueOutsideValidRegionOrNaN_ThrowsArgumentException(double value)
        {
            // Setup
            var failureMechanism = new SimpleFailureMechanismBase();

            // Call
            TestDelegate test = () => failureMechanism.Contribution = value;

            // Assert
            const string expectedMessage = "De waarde voor de toegestane bijdrage aan de faalkans moet in het bereik [0,0, 100,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCase(100)]
        [TestCase(50)]
        [TestCase(0)]
        public void Contribution_ValueInsideValidRegion_DoesNotThrow(double value)
        {
            // Setup
            var failureMechanism = new SimpleFailureMechanismBase();

            // Call
            TestDelegate test = () => failureMechanism.Contribution = value;

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void SetSections_SectionsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new SimpleFailureMechanismBase();

            // Call 
            TestDelegate call = () => FailureMechanismTestHelper.SetSections(failureMechanism, null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sections", exception.ParamName);
        }

        [Test]
        public void SetSections_SourcePathNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new SimpleFailureMechanismBase();

            // Call 
            TestDelegate call = () => FailureMechanismTestHelper.SetSections(failureMechanism, Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourcePath", exception.ParamName);
        }

        [Test]
        public void SetSections_FirstSectionAdded_SectionAddedToSectionsAndSourcePathSet()
        {
            // Setup
            var failureMechanism = new SimpleFailureMechanismBase();
            string sourcePath = TestHelper.GetScratchPadPath();

            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });

            // Call
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            }, sourcePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                section
            }, failureMechanism.Sections);

            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
        }

        [Test]
        public void SetSections_SecondSectionStartConnectingToEndOfFirst_Section2AddedAfterSection1()
        {
            // Setup
            var failureMechanism = new SimpleFailureMechanismBase();

            const int matchingX = 1;
            const int matchingY = 2;

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(3, 4),
                new Point2D(matchingX, matchingY)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(matchingX, matchingY),
                new Point2D(-2, -1)
            });

            // Call
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section1,
                section2
            }, string.Empty);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                section1,
                section2
            }, failureMechanism.Sections);
        }

        [Test]
        public void SetSections_SecondSectionEndConnectingToStartOfFirst_ThrowArgumentException()
        {
            // Setup
            var failureMechanism = new SimpleFailureMechanismBase();

            const int matchingX = 1;
            const int matchingY = 2;

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(matchingX, matchingY),
                new Point2D(3, 4)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(-2, -1),
                new Point2D(matchingX, matchingY)
            });

            // Call
            TestDelegate call = () => FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section1,
                section2
            }, string.Empty);

            // Assert
            const string expectedMessage = "Vak 'B' sluit niet aan op de al gedefinieerde vakken van het toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void SetSections_SecondSectionDoesNotConnectToFirst_ThrowArgumentException()
        {
            // Setup
            var failureMechanism = new SimpleFailureMechanismBase();

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(5, 6),
                new Point2D(7, 8)
            });

            // Call
            TestDelegate call = () => FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section1,
                section2
            }, string.Empty);

            // Assert
            const string expectedMessage = "Vak 'B' sluit niet aan op de al gedefinieerde vakken van het toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ClearAllSections_HasSections_ClearSectionsAndSourcePath()
        {
            // Setup
            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });

            var failureMechanism = new SimpleFailureMechanismBase();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            }, string.Empty);

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            Assert.IsNull(failureMechanism.FailureMechanismSectionSourcePath);
        }

        private class SimpleFailureMechanismBase : FailureMechanismBase
        {
            public SimpleFailureMechanismBase(string name = "SomeName",
                                              string failureMechanismCode = "SomeCode",
                                              int group = 1) : base(name, failureMechanismCode, group) {}

            public override IEnumerable<ICalculation> Calculations
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}