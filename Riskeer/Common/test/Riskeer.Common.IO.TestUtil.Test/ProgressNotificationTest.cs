// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using NUnit.Framework;

namespace Riskeer.Common.IO.TestUtil.Test
{
    [TestFixture]
    public class ProgressNotificationTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedProperties()
        {
            // Setup
            const string description = "text";
            const int currentStep = 1;
            const int totalSteps = 10;

            // Call
            var notification = new ProgressNotification(description, currentStep, totalSteps);

            // Assert
            Assert.AreEqual(description, notification.Description);
            Assert.AreEqual(currentStep, notification.CurrentStep);
            Assert.AreEqual(totalSteps, notification.TotalSteps);
        }
    }
}