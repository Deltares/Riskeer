// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util.Settings;
using NUnit.Framework;
using Riskeer.Integration.Forms;

namespace Application.Riskeer.Test
{
    [TestFixture]
    public class AppTest
    {
        private App app;
        private string[] startupLogMessages;

        [Test]
        public void Constructor_ExpectedValues()
        {
            Assert.IsInstanceOf<System.Windows.Application>(app);
            Assert.IsInstanceOf<RiskeerSettingsHelper>(SettingsHelper.Instance);
        }

        [Test]
        public void Constructor_LogsStartupMessage()
        {
            Assert.AreEqual(1, startupLogMessages.Length);
            Assert.AreEqual($"Riskeer versie {SettingsHelper.Instance.ApplicationVersion} wordt gestart door {UserDisplay()}...", startupLogMessages[0]);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestHelper.AssertLogMessages(
                () => app = new App(),
                messages => startupLogMessages = messages.ToArray());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            app.Shutdown();
        }

        private static string UserDisplay()
        {
            try
            {
                return $"{UserPrincipal.Current.DisplayName} ({UserPrincipal.Current.SamAccountName})";
            }
            catch (SystemException)
            {
                // Cannot only catch specified exceptions, as there are some hidden exception
                // that can be thrown when calling UserPrincipal.Current.
                return Environment.UserName;
            }
        }
    }
}