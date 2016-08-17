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
using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.IO;

namespace Ringtoets.HydraRing.Calculation.Test.IO
{
    [TestFixture]
    public class HydraRingSettingsCsvReaderTest
    {
        [Test]
        public void Constructor_FileContentsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestCsvReader(null, new object());

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "File contents must be set.");
        }

        [Test]
        public void Constructor_SettingsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestCsvReader("path.csv", null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "The settings object must be provided.");
        }

        [Test]
        public void Constructor_ExpectedValue()
        {
            // Setup
            object settings = new object();

            // Call
            TestCsvReader reader = new TestCsvReader("path.csv", settings);

            // Assert
            Assert.AreSame(settings, reader.TestSettings);
        }

        private class TestCsvReader : HydraRingSettingsCsvReader<object>
        {
            public TestCsvReader(string fileContents, object settings)
                : base(fileContents, settings) {}

            public object TestSettings
            {
                get
                {
                    return Settings;
                }
            }

            protected override void CreateSetting(IList<string> line)
            {
                throw new NotImplementedException();
            }
        }
    }
}