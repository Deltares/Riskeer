// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Common.Util.Settings;
using Core.Common.Util.TestUtil.Settings;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Util.TestUtil.Test.Settings
{
    [TestFixture]
    public class UseCustomSettingsHelperTest
    {
        [Test]
        public void GivenSettingsHelperInstance_WhenNewSettingsHelper_ThenSingletonInstanceTemporarilyChanged()
        {
            // Given
            ISettingsHelper originalHelper = SettingsHelper.Instance;

            var mocks = new MockRepository();
            var helper = mocks.Stub<ISettingsHelper>();
            mocks.ReplayAll();

            // When
            using (new UseCustomSettingsHelper(helper))
            {
                // Then
                Assert.AreSame(helper, SettingsHelper.Instance);
            }

            Assert.AreSame(originalHelper, SettingsHelper.Instance);
            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_AlreadyDisposed_DoesNotThrowException()
        {
            // Setup
            var mocks = new MockRepository();
            var helper = mocks.Stub<ISettingsHelper>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () =>
            {
                using (var control = new UseCustomSettingsHelper(helper))
                {
                    control.Dispose();
                }
            };

            // Assert
            Assert.DoesNotThrow(call);
            mocks.VerifyAll();
        }
    }
}