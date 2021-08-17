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
using System.Collections.Generic;
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.Integration.TestUtil;

namespace Riskeer.Integration.Plugin.Test.Handlers
{
    [TestFixture]
    public class AssessmentSectionCompositionChangeHandlerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var handler = new AssessmentSectionCompositionChangeHandler();

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionCompositionChangeHandler>(handler);
        }

        [Test]
        public void ChangeComposition_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            void Call() => handler.ChangeComposition(null, AssessmentSectionComposition.Dike);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void ChangeComposition_ChangeToSameValue_ReturnNoAffectedObjects()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllCalculationConfigurations();
            AssessmentSectionComposition originalComposition = assessmentSection.Composition;

            var handler = new AssessmentSectionCompositionChangeHandler();

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, originalComposition);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dike, DikeDune)")]
        [TestCase(AssessmentSectionComposition.Dike, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dike, Dune)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(DikeDune, Dike)")]
        [TestCase(AssessmentSectionComposition.DikeAndDune, AssessmentSectionComposition.Dune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(DikeDune, Dune)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.Dike, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dune, Dike)")]
        [TestCase(AssessmentSectionComposition.Dune, AssessmentSectionComposition.DikeAndDune, TestName = "ChangeComposition_SetNewValue_ChangeRelevancyAndReturnAffectedObjects(Dune, DikeDune)")]
        public void ChangeComposition_SetNewValue_ReturnAffectedObjects(AssessmentSectionComposition oldComposition,
                                                                        AssessmentSectionComposition newComposition)
        {
            // Setup
            var handler = new AssessmentSectionCompositionChangeHandler();
            var assessmentSection = new AssessmentSection(oldComposition);

            // Call
            IEnumerable<IObservable> affectedObjects = handler.ChangeComposition(assessmentSection, newComposition);

            // Assert
            IEnumerable<IObservable> expectedAffectedObjects = new List<IObservable>(assessmentSection.GetFailureMechanisms())
            {
                assessmentSection
            };

            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }
    }
}