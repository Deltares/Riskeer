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

using System;
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssemblyErrorMessageCreatorTest
    {
        [Test]
        public void CreateErrorMessage_ErrorMessagesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AssemblyErrorMessageCreator.CreateErrorMessage(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("errorMessages", exception.ParamName);
        }

        [Test]
        public void CreateErrorMessage_InvalidAssemblyError_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => AssemblyErrorMessageCreator.CreateErrorMessage(new[]
            {
                new AssemblyErrorMessage(string.Empty, (EAssemblyErrors) 9999)
            });

            // Assert
            string expectedMessage = $"The value of argument 'assemblyError' (9999) is invalid for Enum type '{nameof(EAssemblyErrors)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(EAssemblyErrors.SignallingLimitOutOfRange, "Signaleringskans moet in het bereik [0,1] liggen.")]
        [TestCase(EAssemblyErrors.LowerLimitOutOfRange, "Ondergrens moet in het bereik [0,1] liggen.")]
        [TestCase(EAssemblyErrors.FailurePropbabilityMarginOutOfRange, "Faalkansruimte moet in het bereik [0,1] liggen.")]
        [TestCase(EAssemblyErrors.LengthEffectFactorOutOfRange, "Lengte-effect factor moet minimaal 1 zijn.")]
        [TestCase(EAssemblyErrors.SectionLengthOutOfRange, "De trajectlengte moet groter zijn dan 0 [m].")]
        [TestCase(EAssemblyErrors.SignallingLimitAboveLowerLimit, "De signaleringskans moet kleiner zijn dan de ondergrens.")]
        [TestCase(EAssemblyErrors.PsigDsnAbovePsig, "Berekende signaleringskans per doorsnede is groter dan de signaleringskans van het traject.")]
        [TestCase(EAssemblyErrors.PlowDsnAbovePlow, "Berekende ondergrens per doorsnede is groter dan de ondergrens van het traject.")]
        [TestCase(EAssemblyErrors.LowerLimitIsAboveUpperLimit, "De categoriebovengrens moet boven de categorieondergrens liggen.")]
        [TestCase(EAssemblyErrors.CategoryLowerLimitOutOfRange, "Categoriebovengrens moet in het bereik [0,1] liggen.")]
        [TestCase(EAssemblyErrors.CategoryUpperLimitOutOfRange, "Categorieondergrens moet in het bereik [0,1] liggen.")]
        [TestCase(EAssemblyErrors.TranslateAssessmentInvalidInput, "Er is een ongeldig resultaat gespecificeerd voor de gebruikte methode.")]
        [TestCase(EAssemblyErrors.ValueMayNotBeNull, "Er is ongeldige invoer gedefinieerd voor de gebruikte methode.")]
        [TestCase(EAssemblyErrors.CategoryNotAllowed, "Het specificeren van een toetsoordeel voor deze categorie is niet mogelijk.")]
        [TestCase(EAssemblyErrors.DoesNotComplyAfterComply, "Een lagere categorie moet als voldoende worden aangemerkt indien het vak voor een hogere categorie voldoet.")]
        [TestCase(EAssemblyErrors.FmSectionLengthInvalid, "Gezamenlijke lengte van alle deelvakken moet gelijk zijn aan de trajectlengte.")]
        [TestCase(EAssemblyErrors.FmSectionSectionStartEndInvalid, "De lengte van een berekende deelvak kon niet goed worden bepaald.")]
        [TestCase(EAssemblyErrors.FailureProbabilityOutOfRange, "De gespecificeerde kans moet in het bereik [0,1] liggen.")]
        [TestCase(EAssemblyErrors.InputNotTheSameType, "De resultaten voor alle vakken moeten allen wel of geen kansspecificatie bevatten.")]
        [TestCase(EAssemblyErrors.FailureMechanismAssemblerInputInvalid, "Er moet een vakindeling zijn geïmporteerd.")]
        [TestCase(EAssemblyErrors.CommonFailureMechanismSectionsInvalid, "Ieder relevant toetsspoor moet een vakindeling geïmporteerd hebben.")]
        [TestCase(EAssemblyErrors.CommonFailureMechanismSectionsNotConsecutive, "Alle (deel)vakken moeten minimaal een lengte hebben van 0.01 [m].")]
        [TestCase(EAssemblyErrors.RequestedPointOutOfRange, "De gespecificeerde resultaten voor een of meerdere toetssporen dekken niet de volledige lengte van het traject.")]
        [TestCase(EAssemblyErrors.SectionsWithoutCategory, "Er zijn een of meerdere vakindelingen gevonden die geen categorie hebben.")]
        [TestCase(EAssemblyErrors.InvalidCategoryLimits, "De categoriegrenzen zijn niet aaneengesloten en spannen niet de volldige faalskansruimte.")]
        public void CreateErrorMessage_SingleAssemblyError_ReturnsExpectedErrorMessage(EAssemblyErrors assemblyError, string expectedErrorMessage)
        {
            // Call
            string errorMessage = AssemblyErrorMessageCreator.CreateErrorMessage(new[]
            {
                new AssemblyErrorMessage(string.Empty, assemblyError)
            });

            // Assert
            Assert.AreEqual(expectedErrorMessage, errorMessage);
        }

        [Test]
        public void CreateErrorMessage_MultipleAssemblyErrors_ReturnsExpectedErrorMessage()
        {
            // Call
            string errorMessage = AssemblyErrorMessageCreator.CreateErrorMessage(new[]
            {
                new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryNotAllowed),
                new AssemblyErrorMessage(string.Empty, EAssemblyErrors.FailureProbabilityOutOfRange)
            });

            // Assert
            Assert.AreEqual("- Het specificeren van een toetsoordeel voor deze categorie is niet mogelijk.\n" +
                            "- De gespecificeerde kans moet in het bereik [0,1] liggen.\n", errorMessage);
        }

        [Test]
        public void CreateErrorMessage_NoAssemblyErrors_ReturnsEmptyErrorMessage()
        {
            // Call
            string errorMessage = AssemblyErrorMessageCreator.CreateErrorMessage(Enumerable.Empty<AssemblyErrorMessage>());

            // Assert
            Assert.IsEmpty(errorMessage);
        }

        [Test]
        public void CreateGenericErrorMessage_Always_ReturnsExpectedErrorMessage()
        {
            // Call
            string errorMessage = AssemblyErrorMessageCreator.CreateGenericErrorMessage();

            // Assert
            Assert.AreEqual("Er is een onverwachte fout opgetreden tijdens het assembleren.", errorMessage);
        }
    }
}