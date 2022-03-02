﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.Test.FailureMechanism
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

            // Call
            var failureMechanism = new SimpleFailureMechanismBase(name, code);

            // Assert
            Assert.IsInstanceOf<Observable>(failureMechanism);
            Assert.IsInstanceOf<IFailureMechanism>(failureMechanism);
            Assert.AreEqual(0, failureMechanism.Contribution);
            Assert.AreEqual(name, failureMechanism.Name);
            Assert.AreEqual(code, failureMechanism.Code);
            Assert.IsNotNull(failureMechanism.InAssemblyInputComments);
            Assert.IsNotNull(failureMechanism.InAssemblyOutputComments);
            Assert.IsNotNull(failureMechanism.NotInAssemblyComments);
            Assert.IsNotNull(failureMechanism.CalculationsInputComments);
            Assert.IsNotNull(failureMechanism.AssemblyResult);
            Assert.IsTrue(failureMechanism.InAssembly);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
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
            TestDelegate call = () => failureMechanism.SetSections(null, string.Empty);

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
            TestDelegate call = () => failureMechanism.SetSections(Enumerable.Empty<FailureMechanismSection>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourcePath", exception.ParamName);
        }

        [Test]
        public void SetSections_ValidSections_SectionsAndSourcePathSet()
        {
            // Setup
            string sourcePath = TestHelper.GetScratchPadPath();
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
            failureMechanism.SetSections(new[]
            {
                section1,
                section2
            }, sourcePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                section1,
                section2
            }, failureMechanism.Sections);
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
            Assert.IsTrue(failureMechanism.SectionDependentDataAdded);
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
            TestDelegate call = () => failureMechanism.SetSections(new[]
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
            TestDelegate call = () => failureMechanism.SetSections(new[]
            {
                section1,
                section2
            }, string.Empty);

            // Assert
            const string expectedMessage = "Vak 'B' sluit niet aan op de al gedefinieerde vakken van het toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ClearAllSections_HasSections_ClearSectionsAndSectionDependentDataAndSourcePath()
        {
            // Setup
            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });

            var failureMechanism = new SimpleFailureMechanismBase();
            string sourcePath = TestHelper.GetScratchPadPath();
            failureMechanism.SetSections(new[]
            {
                section
            }, sourcePath);

            // Precondition
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
            CollectionAssert.IsNotEmpty(failureMechanism.Sections);

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            Assert.IsNull(failureMechanism.FailureMechanismSectionSourcePath);
            Assert.IsTrue(failureMechanism.SectionDependentDataCleared);
        }

        private class SimpleFailureMechanismBase : FailureMechanismBase<FailureMechanismSectionResult>
        {
            public SimpleFailureMechanismBase(string name = "SomeName",
                                              string failureMechanismCode = "SomeCode")
                : base(name, failureMechanismCode) {}

            public override IEnumerable<ICalculation> Calculations => throw new NotImplementedException();

            public bool SectionDependentDataCleared { get; private set; }

            public bool SectionDependentDataAdded { get; private set; }

            public override IObservableEnumerable<FailureMechanismSectionResult> SectionResults { get; }

            protected override void AddSectionDependentData(FailureMechanismSection section)
            {
                base.AddSectionDependentData(section);
                SectionDependentDataAdded = true;
            }

            protected override void ClearSectionDependentData()
            {
                base.ClearSectionDependentData();
                SectionDependentDataCleared = true;
            }
        }
    }
}