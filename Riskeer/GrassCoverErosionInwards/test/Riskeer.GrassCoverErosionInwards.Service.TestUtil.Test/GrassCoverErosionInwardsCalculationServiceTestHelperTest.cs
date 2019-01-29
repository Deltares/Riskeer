// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.GrassCoverErosionInwards.Service.TestUtil.Test
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

        [Test]
        [TestCase("description", "output")]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void AssertCalculationNotConvergedMessage_ForDifferentParametersAndEqualMessage_DoesNotThrow(
            string description, string name)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                    description,
                    name,
                    $"De {description} berekening voor grasbekleding erosie kruin en binnentalud '{name}' is niet geconvergeerd.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("De  berekening voor grasbekleding erosie kruin en binnentalud ' ' is niet geconvergeer")]
        [TestCase("e  berekening voor grasbekleding erosie kruin en binnentalud ' ' is niet geconvergeerd.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertCalculationNotConvergedMessage_NotEqualMessage_ThrowsAssertionException(
            string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertCalculationNotConvergedMessage(
                    string.Empty,
                    string.Empty,
                    message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCase("description", "output")]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void AssertGeneralResultWithDuplicateStochasts_ForDifferentParametersAndEqualMessage_DoesNotThrow(
            string description, string name)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateStochasts(
                    description,
                    name,
                    $"Fout bij het uitlezen van de illustratiepunten voor berekening {name} ({description}): " +
                    "Een of meerdere stochasten hebben dezelfde naam. Het uitlezen van illustratiepunten wordt overgeslagen.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "Een of meerdere stochasten hebben dezelfde naam. Het uitlezen van illustratiepunten wordt overgeslagen.")]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "Een of meerdere stochasten hebben dezelfde naam. Het uitlezen van illustratiepunten wordt overgeslage")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertGeneralResultWithDuplicateStochasts_NotEqualMessage_ThrowsAssertionException(
            string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateStochasts(
                    string.Empty,
                    string.Empty,
                    message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCase("description", "output")]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void AssertGeneralResultWithDuplicateIllustrationPoints_ForDifferentParametersAndEqualMessage_DoesNotThrow(
            string description, string name)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateIllustrationPoints(
                    description,
                    name,
                    $"Fout bij het uitlezen van de illustratiepunten voor berekening {name} ({description}): " +
                    "Een of meerdere illustratiepunten hebben dezelfde combinatie van keringsituatie en windrichting. " +
                    "Het uitlezen van illustratiepunten wordt overgeslagen.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "Een of meerdere illustratiepunten hebben dezelfde combinatie van keringsituatie en windrichting. " +
                  "Het uitlezen van illustratiepunten wordt overgeslagen.")]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "Een of meerdere illustratiepunten hebben dezelfde combinatie van keringsituatie en windrichting. " +
                  "Het uitlezen van illustratiepunten wordt overgeslage")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertGeneralResultWithDuplicateIllustrationPoints_NotEqualMessage_ThrowsAssertionException(
            string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateIllustrationPoints(
                    string.Empty,
                    string.Empty,
                    message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCase("description", "output")]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void AssertGeneralResultWithDuplicateIllustrationPointResults_ForDifferentParametersAndEqualMessage_DoesNotThrow(
            string description, string name)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateIllustrationPointResults(
                    description,
                    name,
                    $"Fout bij het uitlezen van de illustratiepunten voor berekening {name} ({description}): " +
                    "Een of meerdere uitvoer variabelen hebben dezelfde naam. " +
                    "Het uitlezen van illustratiepunten wordt overgeslagen.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "Een of meerdere uitvoer variabelen hebben dezelfde naam. " +
                  "Het uitlezen van illustratiepunten wordt overgeslagen.")]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "Een of meerdere uitvoer variabelen hebben dezelfde naam. " +
                  "Het uitlezen van illustratiepunten wordt overgeslage")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertGeneralResultWithDuplicateIllustrationPointResults_NotEqualMessage_ThrowsAssertionException(
            string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateIllustrationPointResults(
                    string.Empty,
                    string.Empty,
                    message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCase("description", "output")]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void AssertGeneralResultIncorrectTopLevelStochasts_ForDifferentParametersAndEqualMessage_DoesNotThrow(
            string description, string name)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultIncorrectTopLevelStochasts(
                    description,
                    name,
                    $"Fout bij het uitlezen van de illustratiepunten voor berekening {name} ({description}): " +
                    "De stochasten van een illustratiepunt bevatten niet dezelfde stochasten als in de onderliggende illustratiepunten. " +
                    "Het uitlezen van illustratiepunten wordt overgeslagen.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "De stochasten van een illustratiepunt bevatten niet dezelfde stochasten als in de onderliggende illustratiepunten. " +
                  "Het uitlezen van illustratiepunten wordt overgeslagen.")]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "De stochasten van een illustratiepunt bevatten niet dezelfde stochasten als in de onderliggende illustratiepunten. " +
                  "Het uitlezen van illustratiepunten wordt overgeslage")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertGeneralResultIncorrectTopLevelStochasts_NotEqualMessage_ThrowsAssertionException(
            string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultIncorrectTopLevelStochasts(
                    string.Empty,
                    string.Empty,
                    message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        [TestCase("description", "output")]
        [TestCase("", null)]
        [TestCase(null, "")]
        public void AssertGeneralResultWithDuplicateChildNames_ForDifferentParametersAndEqualMessage_DoesNotThrow(
            string description, string name)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateChildNames(
                    description,
                    name,
                    $"Fout bij het uitlezen van de illustratiepunten voor berekening {name} ({description}): " +
                    "Een of meerdere illustratiepunten bevatten illustratiepunten met dezelfde naam. " +
                    "Het uitlezen van illustratiepunten wordt overgeslagen.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "Een of meerdere illustratiepunten bevatten illustratiepunten met dezelfde naam. " +
                  "Het uitlezen van illustratiepunten wordt overgeslagen.")]
        [TestCase("out bij het uitlezen van de illustratiepunten voor berekening  ( ): " +
                  "Een of meerdere illustratiepunten bevatten illustratiepunten met dezelfde naam. " +
                  "Het uitlezen van illustratiepunten wordt overgeslage")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertGeneralResultWithDuplicateChildNames_NotEqualMessage_ThrowsAssertionException(
            string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultWithDuplicateChildNames(
                    string.Empty,
                    string.Empty,
                    message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }

        [Test]
        public void AssertGeneralResultNotSetMessage_EqualMessage_DoesNotThrow()
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultNotSetMessage(
                    "Het uitlezen van illustratiepunten is mislukt.");

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase("Het uitlezen van illustratiepunten is misluk")]
        [TestCase("et uitlezen van illustratiepunten is mislukt.")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertGeneralResultNotSetMessage_EqualMessage_ThrowsAssertionException(string message)
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionInwardsCalculationServiceTestHelper.AssertGeneralResultNotSetMessage(message);

            // Assert
            Assert.Throws<AssertionException>(test);
        }
    }
}