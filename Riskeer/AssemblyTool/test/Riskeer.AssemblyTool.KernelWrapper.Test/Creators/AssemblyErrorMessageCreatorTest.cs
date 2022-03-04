// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.Creators;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class AssemblyErrorMessageCreatorTest
    {
        [Test]
        public void CreateErrorMessage_ErrorMessagesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssemblyErrorMessageCreator.CreateErrorMessage(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("errorMessages", exception.ParamName);
        }

        [Test]
        public void CreateErrorMessage_InvalidAssemblyError_ThrowsInvalidEnumArgumentException()
        {
            // Call
            void Call() => AssemblyErrorMessageCreator.CreateErrorMessage(new[]
            {
                new AssemblyErrorMessage(string.Empty, (EAssemblyErrors) 9999)
            });

            // Assert
            var expectedMessage = $"The value of argument 'assemblyError' (9999) is invalid for Enum type '{nameof(EAssemblyErrors)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(EAssemblyErrors.LengthEffectFactorOutOfRange, "Lengte-effect factor moet minimaal 1 zijn.")]
        [TestCase(EAssemblyErrors.SectionLengthOutOfRange, "De trajectlengte moet groter zijn dan 0 [m].")]
        [TestCase(EAssemblyErrors.SignalingLimitAboveLowerLimit, "De signaleringskans moet kleiner zijn dan de ondergrens.")]
        [TestCase(EAssemblyErrors.LowerLimitIsAboveUpperLimit, "De categoriebovengrens moet boven de categorieondergrens liggen.")]
        [TestCase(EAssemblyErrors.ValueMayNotBeNull, "Er is ongeldige invoer gedefinieerd voor de gebruikte methode.")]
        [TestCase(EAssemblyErrors.FailureMechanismSectionLengthInvalid, "Gezamenlijke lengte van alle deelvakken moet gelijk zijn aan de trajectlengte.")]
        [TestCase(EAssemblyErrors.FailureMechanismSectionSectionStartEndInvalid, "De lengte van een berekende deelvak kon niet goed worden bepaald.")]
        [TestCase(EAssemblyErrors.FailureProbabilityOutOfRange, "De gespecificeerde kans moet in het bereik [0,1] liggen.")]
        [TestCase(EAssemblyErrors.InputNotTheSameType, "De resultaten voor alle vakken moeten allen wel of geen kansspecificatie bevatten.")]
        [TestCase(EAssemblyErrors.CommonFailureMechanismSectionsInvalid, "Ieder toetsspoor in de assemblage moet een vakindeling geïmporteerd hebben.")]
        [TestCase(EAssemblyErrors.CommonFailureMechanismSectionsNotConsecutive, "Alle (deel)vakken moeten minimaal een lengte hebben van 0.01 [m].")]
        [TestCase(EAssemblyErrors.RequestedPointOutOfRange, "De gespecificeerde resultaten voor een of meerdere toetssporen dekken niet de volledige lengte van het traject.")]
        [TestCase(EAssemblyErrors.SectionsWithoutCategory, "Er zijn een of meerdere vakindelingen gevonden die geen categorie hebben.")]
        [TestCase(EAssemblyErrors.InvalidCategoryLimits, "De categoriegrenzen zijn niet aaneengesloten en spannen niet de volledige faalkansruimte.")]
        [TestCase(EAssemblyErrors.EmptyResultsList, "Er ontbreekt invoer voor de assemblage rekenmodule waardoor de assemblage niet uitgevoerd kan worden.")]
        [TestCase(EAssemblyErrors.ProfileProbabilityGreaterThanSectionProbability, "De faalkans per vak moet groter zijn dan of gelijk zijn aan de faalkans per doorsnede.")]
        [TestCase(EAssemblyErrors.ValueMayNotBeNaN, "Er is ongeldige invoer gedefinieerd voor de gebruikte methode.")]
        [TestCase(EAssemblyErrors.ErrorConstructingErrorMessage, "Er is een onverwachte fout opgetreden.")]
        [TestCase(EAssemblyErrors.DominantSectionCannotBeAssembled, "Er zijn een of meerdere vakken met duidingsklasse 'Dominant'. Dit kan niet verder geassembleerd worden.")]
        [TestCase(EAssemblyErrors.EncounteredOneOrMoreSectionsWithoutResult, "Alle vakken moeten een resultaat hebben.")]
        [TestCase(EAssemblyErrors.NonMatchingProbabilityValues, "Er is ongeldige invoer gedefinieerd voor de gebruikte methode.")]
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
                new AssemblyErrorMessage(string.Empty, EAssemblyErrors.LengthEffectFactorOutOfRange),
                new AssemblyErrorMessage(string.Empty, EAssemblyErrors.FailureProbabilityOutOfRange)
            });

            // Assert
            Assert.AreEqual("- Lengte-effect factor moet minimaal 1 zijn.\n" +
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