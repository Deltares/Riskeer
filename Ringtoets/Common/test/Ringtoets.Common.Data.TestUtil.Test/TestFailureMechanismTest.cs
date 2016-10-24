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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_InitializeProperties()
        {
            // Call
            var failureMechanism = new TestFailureMechanism();

            // Assert
            Assert.IsEmpty(failureMechanism.Calculations);
            Assert.IsEmpty(failureMechanism.SectionResults);
            Assert.AreEqual("Test failure mechanism", failureMechanism.Name);
            Assert.AreEqual("TFM", failureMechanism.Code);
        }

        [Test]
        public void Constructor_WithNameAndCode_SetNameAndCodeProperties()
        {
            // Setup
            var testName = "Other name";
            var testCode = "ON";

            // Call
            var failureMechanism = new TestFailureMechanism(testName, testCode);

            // Assert
            Assert.IsEmpty(failureMechanism.Calculations);
            Assert.IsEmpty(failureMechanism.SectionResults);
            Assert.AreEqual(testName, failureMechanism.Name);
            Assert.AreEqual(testCode, failureMechanism.Code);
        }

        [Test]
        public void Constructor_WithCalculations_InitializeCalculationsProperties()
        {
            // Setup
            List<ICalculation> testCalculations = new List<ICalculation>();

            // Call
            var failureMechanism = new TestFailureMechanism(testCalculations);

            // Assert
            Assert.AreSame(testCalculations, failureMechanism.Calculations);
            Assert.IsEmpty(failureMechanism.SectionResults);
            Assert.AreEqual("Test failure mechanism", failureMechanism.Name);
            Assert.AreEqual("TFM", failureMechanism.Code);
        }

        [Test]
        public void AddSection_WithNullSection_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.AddSection(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void AddSection_WithSection_AddsSectionAsResult()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            var section = new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });

            // Call
            failureMechanism.AddSection(section);

            // Assert
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.AreSame(section, failureMechanism.SectionResults.ElementAt(0).Section);
        }
    }
}