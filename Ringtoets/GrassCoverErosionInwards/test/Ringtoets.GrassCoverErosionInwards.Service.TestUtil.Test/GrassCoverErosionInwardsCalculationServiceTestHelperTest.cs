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

using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionInwards.Service.TestUtil.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationServiceTestHelperTest
    {
        [Test]
        public void Constants_Always_ReturnExpectedValues()
        {
            // Call
            const string overtoppingDescription = GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingCalculationDescription;
            const string overtoppingRateDescription = GrassCoverErosionInwardsCalculationServiceTestHelper.OvertoppingRateCalculationDescription;
            const string hbnDescription = GrassCoverErosionInwardsCalculationServiceTestHelper.HbnCalculationDescription;

            // Assert
            Assert.AreEqual("overloop en overslag", overtoppingDescription);
            Assert.AreEqual("overslagdebiet", overtoppingRateDescription);
            Assert.AreEqual("HBN", hbnDescription);
        }

        [Test]
        [TestCase("description", "output")]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void AssertCalculationFinishedMessage_ForDifferentParametersWithEqualMessage_DoesNotThrow(
            string description, string output)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(
                    description,
                    output,
                    $"De {description} berekening is uitgevoerd op de tijdelijke locatie '{output}'. " +
                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("De  is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.")]
        [TestCase("De  berekening is uitgevoerd op de tijdelijke locatie ''. edetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationFinishedMessage_WithNotEqualMessage_ThrowsAssertionException(string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFinishedMessage(string.Empty, string.Empty, message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCase("description", "name", "error")]
        [TestCase("", null, "")]
        [TestCase(null, "", "  ")]
        public void AssertCalculationFinishedMessage_ForDifferentParametersWithErrorWithEqualMessage_DoesNotThrow(
            string description, string name, string error)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                    description,
                    name,
                    error,
                    $"De {description} berekening voor grasbekleding erosie kruin en binnentalud '{name}' is mislukt. " +
                    $"Bekijk het foutrapport door op details te klikken.\r\n{error}");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("description", "output")]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void AssertCalculationFinishedMessage_ForDifferentParametersWithoutErrorWithEqualMessage_DoesNotThrow(
            string description, string name)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(
                    description,
                    name,
                    null,
                    $"De {description} berekening voor grasbekleding erosie kruin en binnentalud '{name}' is mislukt. " +
                    "Er is geen foutrapport beschikbaar.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("De  berekening voor grasbekleding erosie kruin en binnentalud '' is mislukt. Bekijk het foutrapport door op details te klikken.\n")]
        [TestCase("De  berekening voor grasbekleding erosie kruin en binnentalud '' is mislukt. ekijk het foutrapport door op details te klikken.\r\n")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationFailedMessage_WithoutErrorWithNotEqualMessage_ThrowsAssertionException(string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(string.Empty, string.Empty, null, message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCase("e  berekening is uitgevoerd op de tijdelijke locatie ''. Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.")]
        [TestCase("De  berekening is uitgevoerd op de tijdelijke locatie ''. edetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationFailedMessage_WithErrorWithNotEqualMessage_ThrowsAssertionException(string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationFailedMessage(string.Empty, string.Empty, string.Empty, message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }
    }
}