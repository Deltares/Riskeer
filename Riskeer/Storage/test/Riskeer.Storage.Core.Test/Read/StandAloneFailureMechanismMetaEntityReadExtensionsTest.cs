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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class StandAloneFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((IStandAloneFailureMechanismMetaEntity) null).Read(new GeneralInput());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_GeneralInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var entity = mocks.Stub<IStandAloneFailureMechanismMetaEntity>();
            mocks.ReplayAll();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void Read_WithAllData_SetsGeneralInputProperties()
        {
            // Setup
            var random = new Random();
            var mocks = new MockRepository();
            var entity = mocks.Stub<IStandAloneFailureMechanismMetaEntity>();
            mocks.ReplayAll();

            entity.N = random.NextDouble(1, 20);
            entity.ApplyLengthEffectInSection = Convert.ToByte(random.NextBoolean());
            var generalInput = new GeneralInput();

            // Call
            entity.Read(generalInput);

            // Assert
            Assert.AreEqual(entity.N, generalInput.N, generalInput.N.GetAccuracy());
            Assert.AreEqual(Convert.ToBoolean(entity.ApplyLengthEffectInSection), generalInput.ApplyLengthEffectInSection);

            mocks.VerifyAll();
        }
    }
}