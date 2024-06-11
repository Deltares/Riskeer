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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismSectionConfigurationRowTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            PipingFailureMechanismSectionConfiguration sectionConfiguration = GetFailureMechanismSectionConfiguration();
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            double b = random.NextDouble();

            // Call
            var sectionRow = new PipingFailureMechanismSectionConfigurationRow(sectionConfiguration, sectionStart, sectionEnd, b);

            // Assert
            Assert.IsInstanceOf<PipingFailureMechanismSectionConfigurationRow>(sectionRow);

            FailureMechanismSection section = sectionConfiguration.Section;
            Assert.AreEqual(section.Name, sectionRow.Name);

            Assert.AreEqual(sectionStart, sectionRow.SectionStart, sectionRow.SectionStart.GetAccuracy());
            Assert.AreEqual(sectionEnd, sectionRow.SectionEnd, sectionRow.SectionEnd.GetAccuracy());

            Assert.AreEqual(section.Length, sectionRow.Length, sectionRow.Length.GetAccuracy());

            Assert.AreEqual(sectionConfiguration.A.NumberOfDecimalPlaces, sectionRow.A.NumberOfDecimalPlaces);
            Assert.AreEqual(sectionConfiguration.A, sectionRow.A);

            Assert.AreEqual(2, sectionRow.N.NumberOfDecimalPlaces);
            AssertLengthEffectN(sectionConfiguration, b, sectionRow);

            Assert.AreEqual(2, sectionRow.FailureMechanismSensitiveSectionLength.NumberOfDecimalPlaces);
            AssertFailureMechanismSensitiveSectionLength(sectionConfiguration, sectionRow);
        }

        private static PipingFailureMechanismSectionConfiguration GetFailureMechanismSectionConfiguration()
        {
            var random = new Random();
            var section = new FailureMechanismSection("test", new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble()),
                new Point2D(random.NextDouble(), random.NextDouble())
            });
            return new PipingFailureMechanismSectionConfiguration(section);
        }

        private static void AssertLengthEffectN(FailureMechanismSectionConfiguration sectionConfiguration,
                                                double b,
                                                FailureMechanismSectionConfigurationRow sectionRow)
        {
            Assert.AreEqual(sectionConfiguration.GetN(b), sectionRow.N, sectionRow.N.GetAccuracy());
        }

        private static void AssertFailureMechanismSensitiveSectionLength(FailureMechanismSectionConfiguration sectionConfiguration,
                                                                         PipingFailureMechanismSectionConfigurationRow sectionRow)
        {
            Assert.AreEqual(sectionConfiguration.GetFailureMechanismSensitiveSectionLength(), 
                            sectionRow.FailureMechanismSensitiveSectionLength, 
                            sectionRow.FailureMechanismSensitiveSectionLength.GetAccuracy());
        }
    }
}