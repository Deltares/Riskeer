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
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Application.Ringtoets.Storage.Read.MacroStabilityInwards;

namespace Application.Ringtoets.Storage.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void ReadMacroStabilityInwardsProbabilityAssessmentInput_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var input = new MacroStabilityInwardsProbabilityAssessmentInput();

            // Call
            TestDelegate test = () => ((MacroStabilityInwardsFailureMechanismMetaEntity)null).ReadProbabilityAssessmentInput(input);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ReadMacroStabilityInwardsProbabilityAssessmentInput_MacroStabilityInwardsProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new MacroStabilityInwardsFailureMechanismMetaEntity();

            // Call
            TestDelegate test = () => entity.ReadProbabilityAssessmentInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void ReadMacroStabilityInwardsProbabilityAssessmentInput_ValidParameters_SetMacroStabilityInwardsProbabilityAssessmentInputProperties()
        {
            // Setup
            var random = new Random(31);
            var entity = new MacroStabilityInwardsFailureMechanismMetaEntity
            {
                A = random.NextDouble(),
                SectionLength = random.NextDouble()
            };
            var input = new MacroStabilityInwardsProbabilityAssessmentInput();

            // Call
            entity.ReadProbabilityAssessmentInput(input);

            // Assert
            Assert.AreEqual(entity.A, input.A);
            Assert.AreEqual(entity.SectionLength, input.SectionLength);
        }

        [Test]
        public void ReadMacroStabilityInwardsProbabilityAssessmentInput_NullProperty_SetMacroStabilityInwardsProbabilityAssessmentInputPropertiesAsNaN()
        {
            // Setup
            var entity = new MacroStabilityInwardsFailureMechanismMetaEntity();
            var input = new MacroStabilityInwardsProbabilityAssessmentInput();

            // Call
            entity.ReadProbabilityAssessmentInput(input);

            // Assert
            Assert.AreEqual(entity.A, input.A);
            Assert.IsNaN(input.SectionLength);
        }
    }
}