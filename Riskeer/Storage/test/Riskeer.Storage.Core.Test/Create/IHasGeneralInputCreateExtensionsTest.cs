// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class IHasGeneralInputCreateExtensionsTest
    {
        [Test]
        public void Create_WithAllData_SetsMetaEntityProperties()
        {
            // Setup
            var random = new Random();
            var failureMechanism = new TestFailureMechanism
            {
                GeneralInput =
                {
                    N = random.NextRoundedDouble(1, 20),
                    ApplyLengthEffectInSection = random.NextBoolean()
                }
            };

            // Call
            var metaEntity = failureMechanism.Create<TestFailureMechanismMetaEntity>();

            // Assert
            Assert.IsInstanceOf<IStandAloneFailureMechanismMetaEntity>(metaEntity);
            Assert.AreEqual(failureMechanism.GeneralInput.N, metaEntity.N);
            Assert.AreEqual(Convert.ToByte(failureMechanism.GeneralInput.ApplyLengthEffectInSection), metaEntity.ApplyLengthEffectInSection);
        }

        private class TestFailureMechanismMetaEntity : IStandAloneFailureMechanismMetaEntity
        {
            public double N { get; set; }
            public byte ApplyLengthEffectInSection { get; set; }
        }
    }
}