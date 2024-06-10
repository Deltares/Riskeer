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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionConfigurationRowTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            FailureMechanismSectionConfiguration sectionConfiguration = GetTestFailureMechanismSectionConfiguration();
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            double b = random.NextDouble();

            // Call
            var sectionRow = new FailureMechanismSectionConfigurationRow(sectionConfiguration, sectionStart, sectionEnd, b);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionRow>(sectionRow);

            FailureMechanismSection section = sectionConfiguration.Section;
            Assert.AreEqual(section.Name, sectionRow.Name);

            Assert.AreEqual(sectionStart, sectionRow.SectionStart, sectionRow.SectionStart.GetAccuracy());
            Assert.AreEqual(sectionEnd, sectionRow.SectionEnd, sectionRow.SectionEnd.GetAccuracy());

            Assert.AreEqual(section.Length, sectionRow.Length, sectionRow.Length.GetAccuracy());

            Assert.AreEqual(sectionConfiguration.A.NumberOfDecimalPlaces, sectionRow.A.NumberOfDecimalPlaces);
            Assert.AreEqual(sectionConfiguration.A, sectionRow.A);
            
            Assert.AreEqual(2, sectionRow.N.NumberOfDecimalPlaces);
            AssertLengthEffectN(sectionConfiguration, b, sectionRow);
        }

        [Test]
        public void GivenRow_WhenParameterAChanged_ThenObserversNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            FailureMechanismSectionConfiguration sectionConfiguration = GetTestFailureMechanismSectionConfiguration();
            sectionConfiguration.Attach(observer);

            var sectionRow = new FailureMechanismSectionConfigurationRow(sectionConfiguration, double.NaN, double.NaN, double.NaN);

            // When
            sectionRow.A = random.NextRoundedDouble();

            // Then
            mocks.VerifyAll();
        }

        private static FailureMechanismSectionConfiguration GetTestFailureMechanismSectionConfiguration()
        {
            var random = new Random();
            var section = new FailureMechanismSection("test", new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            });
            return new TestFailureMechanismSectionConfiguration(section, random.NextRoundedDouble());
        }

        private static void AssertLengthEffectN(FailureMechanismSectionConfiguration sectionConfiguration,
                                                double b,
                                                FailureMechanismSectionConfigurationRow sectionRow)
        {
            Assert.AreEqual(sectionConfiguration.GetN(b), sectionRow.N, sectionRow.N.GetAccuracy());
        }
    }
}