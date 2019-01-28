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

namespace Ringtoets.GrassCoverErosionInwards.Service.TestUtil
{
    public static class GrassCoverErosionInwardsCalculationServiceTestHelper
    {
        public const string OvertoppingCalculationDescription = "overloop en overslag";
        public const string OvertoppingRateCalculationDescription = "overslagdebiet";
        public const string HbnCalculationDescription = "HBN";

        public static void AssertCalculationFinishedMessage(string calculationDescription, string calculatorOutputDirectory, string actual)
        {
            Assert.AreEqual($"De {calculationDescription} berekening is uitgevoerd op de tijdelijke locatie '{calculatorOutputDirectory}'. " +
                            "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.",
                            actual);
        }

        public static void AssertCalculationFailedMessage(string calculationDescription, string calculationName, string detailedReport, string actual)
        {
            Assert.AreEqual($"De {calculationDescription} berekening voor grasbekleding erosie kruin en binnentalud '{calculationName}' is mislukt. " +
                            (detailedReport != null
                                 ? $"Bekijk het foutrapport door op details te klikken.\r\n{detailedReport}"
                                 : "Er is geen foutrapport beschikbaar."),
                            actual);
        }

        public static void AssertCalculationNotConvergedMessage(string calculationDescription, string calculationName, string actual)
        {
            Assert.AreEqual($"De {calculationDescription} berekening voor grasbekleding erosie kruin en binnentalud '{calculationName}' is niet geconvergeerd.", actual);
        }

        public static void AssertGeneralResultNotSetMessage(string actual)
        {
            Assert.AreEqual("Het uitlezen van illustratiepunten is mislukt.", actual);
        }

        public static void AssertGeneralResultWithDuplicateStochasts(string calculationDescription, string calculationName, string actual)
        {
            Assert.AreEqual($"Fout bij het uitlezen van de illustratiepunten voor berekening {calculationName} ({calculationDescription}): " +
                            "Een of meerdere stochasten hebben dezelfde naam. Het uitlezen van illustratiepunten wordt overgeslagen.", actual);
        }

        public static void AssertGeneralResultWithDuplicateIllustrationPoints(string calculationDescription, string calculationName, string actual)
        {
            Assert.AreEqual($"Fout bij het uitlezen van de illustratiepunten voor berekening {calculationName} ({calculationDescription}): " +
                            "Een of meerdere illustratiepunten hebben dezelfde combinatie van keringsituatie en windrichting. Het uitlezen van illustratiepunten wordt overgeslagen.", actual);
        }

        public static void AssertGeneralResultWithDuplicateIllustrationPointResults(string calculationDescription, string calculationName, string actual)
        {
            Assert.AreEqual($"Fout bij het uitlezen van de illustratiepunten voor berekening {calculationName} ({calculationDescription}): " +
                            "Een of meerdere uitvoer variabelen hebben dezelfde naam. Het uitlezen van illustratiepunten wordt overgeslagen.", actual);
        }

        public static void AssertGeneralResultIncorrectTopLevelStochasts(string calculationDescription, string calculationName, string actual)
        {
            Assert.AreEqual($"Fout bij het uitlezen van de illustratiepunten voor berekening {calculationName} ({calculationDescription}): " +
                            "De stochasten van een illustratiepunt bevatten niet dezelfde stochasten als in de onderliggende illustratiepunten. " +
                            "Het uitlezen van illustratiepunten wordt overgeslagen.", actual);
        }

        public static void AssertGeneralResultWithDuplicateChildNames(string calculationDescription, string calculationName, string actual)
        {
            Assert.AreEqual($"Fout bij het uitlezen van de illustratiepunten voor berekening {calculationName} ({calculationDescription}): " +
                            "Een of meerdere illustratiepunten bevatten illustratiepunten met dezelfde naam. " +
                            "Het uitlezen van illustratiepunten wordt overgeslagen.", actual);
        }
    }
}