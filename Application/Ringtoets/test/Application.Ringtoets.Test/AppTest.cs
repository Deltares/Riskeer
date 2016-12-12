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

using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Application.Ringtoets.Test
{
    [TestFixture]
    public class AppTest
    {
        private AppDomain appDomain;

        [SetUp]
        public void SetUp()
        {
            appDomain = AppDomain.CreateDomain("AppTest", AppDomain.CurrentDomain.Evidence,
                                               AppDomain.CurrentDomain.SetupInformation);
        }

        [TearDown]
        public void TearDown()
        {
            AppDomain.Unload(appDomain);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            appDomain.DoCallBack(() =>
            {
                // Call
                var app = new App();

                // Assert
                Assert.IsInstanceOf<System.Windows.Application>(app);
                app.Shutdown();
            });
        }

        [Test]
        public void Constructor_LogsStartupMessage()
        {
            // Setup
            appDomain.DoCallBack(() =>
            {
                // Call
                Action call = () =>
                {
                    var app = new App();
                    app.Shutdown();
                };

                // Assert
                string userDisplayInfo = UserDisplay();

                TestHelper.AssertLogMessages(call, messages =>
                {
                    var msgs = messages.ToArray();
                    Assert.AreEqual(1, msgs.Length);
                    Assert.AreEqual(string.Format("Ringtoets versie {0} wordt gestart door {1}...",
                                                  SettingsHelper.ApplicationVersion, userDisplayInfo), msgs[0]);
                });
            });
        }

        private static string UserDisplay()
        {
            try
            {
                return string.Format("{0} ({1})", UserPrincipal.Current.DisplayName,
                                     UserPrincipal.Current.SamAccountName);
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