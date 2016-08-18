﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Read.Piping;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Read.Piping
{
    [TestFixture]
    public class PipingFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void ReadAsPipingProbabilityAssessmentInput_Always_ReturnsNewPipingProbabilityAssessmentInputWithPropertiesSet()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = entityId,
                A = 0.95,
                UpliftCriticalSafetyFactor = 2.6
            };

            // Call
            PipingProbabilityAssessmentInput pipingProbabilityAssessmentInput = entity.Read();

            // Assert
            Assert.IsNotNull(pipingProbabilityAssessmentInput);
            Assert.AreEqual(entityId, pipingProbabilityAssessmentInput.StorageId);
            Assert.AreEqual(entity.A, pipingProbabilityAssessmentInput.A);
            Assert.AreEqual(entity.UpliftCriticalSafetyFactor, pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor, pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor.GetAccuracy());
        }
    }
}