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
using NUnit.Framework;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil.Probability;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.MacroStabilityInwards;

namespace Riskeer.Storage.Core.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void ReadProbabilityAssessmentInput_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            ProbabilityAssessmentInput input = ProbabilityAssessmentInputTestFactory.Create();

            // Call
            void Call() => ((MacroStabilityInwardsFailureMechanismMetaEntity) null).ReadProbabilityAssessmentInput(input);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ReadProbabilityAssessmentInput_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new MacroStabilityInwardsFailureMechanismMetaEntity();

            // Call
            void Call() => entity.ReadProbabilityAssessmentInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("probabilityAssessmentInput", exception.ParamName);
        }

        [Test]
        public void ReadProbabilityAssessmentInput_ValidParameters_SetMacroStabilityInwardsProbabilityAssessmentInputProperties()
        {
            // Setup
            var random = new Random(31);
            ProbabilityAssessmentInput inputToUpdate = ProbabilityAssessmentInputTestFactory.Create();
            var entity = new MacroStabilityInwardsFailureMechanismMetaEntity
            {
                A = random.NextDouble()
            };

            // Call
            entity.ReadProbabilityAssessmentInput(inputToUpdate);

            // Assert
            Assert.AreEqual(entity.A, inputToUpdate.A);
        }
    }
}