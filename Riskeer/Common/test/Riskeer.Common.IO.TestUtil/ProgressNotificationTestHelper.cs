// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Riskeer.Common.IO.TestUtil
{
    /// <summary>
    /// A helper for asserting progress notifications.
    /// </summary>
    public static class ProgressNotificationTestHelper
    {
        /// <summary>
        /// Asserts that <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The value that is expected.</param>
        /// <param name="actual">The actual value.</param>
        /// <exception cref="AssertionException">Thrown when the <paramref name="actual"/> is 
        /// not equal to <paramref name="expected"/>.</exception>
        public static void AssertProgressNotificationsAreEqual(IEnumerable<ProgressNotification> expected,
                                                               IEnumerable<ProgressNotification> actual)
        {
            if (expected == null)
            {
                Assert.IsNull(expected);
                return;
            }

            ProgressNotification[] expectedArray = expected.ToArray();
            ProgressNotification[] actualArray = actual.ToArray();

            Assert.AreEqual(expectedArray.Length, actualArray.Length);
            for (var i = 0; i < expectedArray.Length; i++)
            {
                AssertProgressNotificationsAreEqual(expectedArray[i], actualArray[i]);
            }
        }

        /// <summary>
        /// Asserts that <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The value that is expected.</param>
        /// <param name="actual">The actual value.</param>
        /// <exception cref="AssertionException">Thrown when the <paramref name="actual"/> is 
        /// not equal to <paramref name="expected"/>.</exception>
        private static void AssertProgressNotificationsAreEqual(ProgressNotification expected,
                                                                ProgressNotification actual)
        {
            if (expected == null)
            {
                Assert.IsNull(expected);
                return;
            }

            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.CurrentStep, actual.CurrentStep);
            Assert.AreEqual(expected.TotalSteps, actual.TotalSteps);
        }
    }
}