// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System;
using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Components.Gis.Test
{
    [TestFixture]
    public class WmtsConnectionInfoTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsConnectionInfo(null, "url");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("name", paramName);
        }

        [Test]
        public void Constructor_UrlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsConnectionInfo("name", null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "url must have a value.");
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedProperties()
        {
            // Setup
            const string name = "name";
            const string url = "url";

            // Call
            var connectionInfo = new WmtsConnectionInfo(name, url);

            // Assert
            Assert.AreEqual(name, connectionInfo.Name);
            Assert.AreEqual(url, connectionInfo.Url);
        }

        [TestFixture]
        private class WmtsConnectionInfoEqualsTest : EqualsTestFixture<WmtsConnectionInfo, DerivedWmtsConnectionInfo>
        {
            protected override WmtsConnectionInfo CreateObject()
            {
                return CreateWmtsConnectionInfo();
            }

            protected override DerivedWmtsConnectionInfo CreateDerivedObject()
            {
                return new DerivedWmtsConnectionInfo(CreateWmtsConnectionInfo());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                WmtsConnectionInfo baseConnectionInfo = CreateWmtsConnectionInfo();
                yield return new TestCaseData(new WmtsConnectionInfo("Different name", baseConnectionInfo.Url))
                    .SetName("Name");
                yield return new TestCaseData(new WmtsConnectionInfo(baseConnectionInfo.Name, "Different url"))
                    .SetName("URL");
            }

            private static WmtsConnectionInfo CreateWmtsConnectionInfo()
            {
                return new WmtsConnectionInfo("name", "url");
            }
        }

        private class DerivedWmtsConnectionInfo : WmtsConnectionInfo
        {
            public DerivedWmtsConnectionInfo(WmtsConnectionInfo connectionInfo)
                : base(connectionInfo.Name, connectionInfo.Url) {}
        }
    }
}