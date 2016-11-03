// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Create.Piping
{
    [TestFixture]
    public class PipingProbabilityAssessmentInputCreateExtensionsTest
    {
        [Test]
        public void Create_WithCollector_ReturnsPipingFailureMechanismMetaEntityWithPropertiesSet()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            PipingFailureMechanismMetaEntity entity = failureMechanism.PipingProbabilityAssessmentInput.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.A, entity.A);
        }
    }
}