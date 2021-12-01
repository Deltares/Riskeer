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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.Piping;

namespace Riskeer.Storage.Core.Test.Read.Piping
{
    [TestFixture]
    public class PipingFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void ReadProbabilityAssessmentInput_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var input = new PipingProbabilityAssessmentInput();

            // Call
            void Call() => ((PipingFailureMechanismMetaEntity) null).ReadProbabilityAssessmentInput(input);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ReadProbabilityAssessmentInput_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new PipingFailureMechanismMetaEntity();

            // Call
            void Call() => entity.ReadProbabilityAssessmentInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("probabilityAssessmentInput", exception.ParamName);
        }

        [Test]
        public void ReadProbabilityAssessmentInput_ValidParameters_SetPipingProbabilityAssessmentInputProperties()
        {
            // Setup
            var inputToUpdate = new PipingProbabilityAssessmentInput();
            var entity = new PipingFailureMechanismMetaEntity
            {
                A = new Random(31).NextDouble()
            };

            // Call
            entity.ReadProbabilityAssessmentInput(inputToUpdate);

            // Assert
            Assert.AreEqual(entity.A, inputToUpdate.A);
        }

        [Test]
        public void ReadGeneralPipingInput_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var input = new GeneralPipingInput();

            // Call
            void Call() => ((PipingFailureMechanismMetaEntity) null).ReadGeneralPipingInput(input);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ReadGeneralPipingInput_GeneralPipingInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new PipingFailureMechanismMetaEntity();

            // Call
            void Call() => entity.ReadGeneralPipingInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalPipingInput", exception.ParamName);
        }

        [Test]
        public void ReadGeneralPipingInput_ValidParameters_SetGeneralPipingInputWithProperties()
        {
            // Setup
            var inputToUpdate = new GeneralPipingInput();
            var entity = new PipingFailureMechanismMetaEntity
            {
                WaterVolumetricWeight = new Random(31).NextDouble()
            };

            // Call
            entity.ReadGeneralPipingInput(inputToUpdate);

            // Assert
            Assert.AreEqual(entity.WaterVolumetricWeight, inputToUpdate.WaterVolumetricWeight, inputToUpdate.WaterVolumetricWeight.GetAccuracy());
        }

        [Test]
        public void ReadFailureMechanismValues_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((PipingFailureMechanismMetaEntity) null).ReadFailureMechanismValues(new PipingFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ReadFailureMechanismValues_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new PipingFailureMechanismMetaEntity();

            // Call
            void Call() => entity.ReadFailureMechanismValues(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ReadFailureMechanismValues_ValidParameters_SetPipingProbabilityAssessmentInputProperties()
        {
            // Setup
            var random = new Random(31);
            var configurationType = random.NextEnumValue<PipingScenarioConfigurationType>();

            var failureMechanismToUpdate = new PipingFailureMechanism();
            var entity = new PipingFailureMechanismMetaEntity
            {
                PipingScenarioConfigurationType = Convert.ToByte(configurationType)
            };

            // Call
            entity.ReadFailureMechanismValues(failureMechanismToUpdate);

            // Assert
            Assert.AreEqual(configurationType, failureMechanismToUpdate.ScenarioConfigurationType);
        }
    }
}