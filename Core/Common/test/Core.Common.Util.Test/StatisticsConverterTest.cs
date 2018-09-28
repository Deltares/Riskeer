// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using NUnit.Framework;

namespace Core.Common.Util.Test
{
    [TestFixture]
    public class StatisticsConverterTest
    {
        [Test]
        [TestCase(-1.335177736118940, 9.090909E-01)]
        [TestCase(3.402932835385330, 3.333333E-04)]
        [TestCase(3.890591886413120, 5.000000E-05)]
        [TestCase(3.987878936606940, 3.333333E-05)]
        [TestCase(4.649132934007460, 1.666667E-06)]
        [TestCase(4.753424308817090, 1.000000E-06)]
        [TestCase(5.103554002888150, 1.666667E-07)]
        public void ReliabilityToProbability_ConvertReliability_CorrectProbability(double reliability, double expectedProbability)
        {
            // Call
            double probability = StatisticsConverter.ReliabilityToProbability(reliability);

            // Assert 
            Assert.AreEqual(expectedProbability, probability, 1.0e-6);
        }

        [Test]
        [TestCase(9.090909E-01, -1.335177736118940)]
        [TestCase(3.333333E-04, 3.402932835385330)]
        [TestCase(5.000000E-05, 3.890591886413120)]
        [TestCase(3.333333E-05, 3.987878936606940)]
        [TestCase(1.666667E-06, 4.649132934007460)]
        [TestCase(1.000000E-06, 4.753424308817090)]
        [TestCase(1.666667E-07, 5.103554002888150)]
        public void ProbabilityToReliability_ConvertProbability_CorrectReliability(double probability, double expectedReliability)
        {
            // Call
            double reliability = StatisticsConverter.ProbabilityToReliability(probability);

            // Assert
            Assert.AreEqual(expectedReliability, reliability, 1.0e-6);
        }
    }
}