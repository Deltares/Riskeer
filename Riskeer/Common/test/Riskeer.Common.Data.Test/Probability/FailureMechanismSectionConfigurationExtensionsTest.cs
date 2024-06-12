// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Probability
{
    [TestFixture]
    public class FailureMechanismSectionConfigurationExtensionsTest
    {
        [Test]
        public void GetN_ConfigurationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((FailureMechanismSectionConfiguration) null).GetN(new Random(39).NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("configuration", exception.ParamName);
        }

        [Test]
        [TestCase(0.2, 100, 100, 1.2)]
        [TestCase(0.5, 750, 300, 1.2)]
        [TestCase(0.9, -200, 750, -2.375)]
        [TestCase(0.8, 0, 100, double.PositiveInfinity)]
        public void GetN_WithValues_ReturnsExpectedResult(double a, double b, double length, double expectedN)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(length, 0)
            });

            var configuration = new FailureMechanismSectionConfiguration(section)
            {
                A = (RoundedDouble) a
            };

            // Call
            double actualN = configuration.GetN(b);

            // Assert
            Assert.AreEqual(expectedN, actualN);
        }
        
        [Test]
        public void GetFailureMechanismSensitiveSectionLength_ConfigurationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((FailureMechanismSectionConfiguration) null).GetFailureMechanismSensitiveSectionLength();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("configuration", exception.ParamName);
        }

        [Test]
        [TestCase(0.2, 100, 20)]
        [TestCase(0.5, 300, 150)]
        [TestCase(0.9, 750, 675)]
        [TestCase(0.8, -100, 80)]
        public void GetFailureMechanismSensitiveSectionLength_WithValues_ReturnsExpectedResult(double a, double length, double expectedFailureMechanismSensitiveSectionLength)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(0, 0),
                new Point2D(length, 0)
            });

            var configuration = new FailureMechanismSectionConfiguration(section)
            {
                A = (RoundedDouble) a
            };

            // Call
            double actualFailureMechanismSensitiveSectionLength = configuration.GetFailureMechanismSensitiveSectionLength();

            // Assert
            Assert.AreEqual(expectedFailureMechanismSensitiveSectionLength, 
                            actualFailureMechanismSensitiveSectionLength);
        }
    }
}