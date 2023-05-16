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
using Rhino.Mocks;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilProfileWrapperTest
    {
        [Test]
        public void Constructor_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();
            
            // Call
            void Call() => new SoilProfileWrapper<ISoilProfile>(null, failureMechanismType);
            
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismType = random.NextEnumValue<FailureMechanismType>();

            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();
            
            // Call
            var wrapper = new SoilProfileWrapper<ISoilProfile>(soilProfile, failureMechanismType);
            
            // Assert
            Assert.AreSame(soilProfile, wrapper.SoilProfile);
            Assert.AreEqual(failureMechanismType, wrapper.FailureMechanismType);
            mocks.VerifyAll();
        }
    }
}