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
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.Piping;

namespace Ringtoets.Storage.Core.Test.Read.Piping
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
            TestDelegate test = () => ((PipingFailureMechanismMetaEntity) null).ReadProbabilityAssessmentInput(input);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ReadProbabilityAssessmentInput_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new PipingFailureMechanismMetaEntity();

            // Call
            TestDelegate test = () => entity.ReadProbabilityAssessmentInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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
            TestDelegate test = () => ((PipingFailureMechanismMetaEntity) null).ReadGeneralPipingInput(input);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ReadGeneralPipingInput_GeneralPipingInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new PipingFailureMechanismMetaEntity();

            // Call
            TestDelegate test = () => entity.ReadGeneralPipingInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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
    }
}