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

using System.IO;
using Core.Common.Gui.Appenders;
using Core.Common.Util.Settings;
using log4net.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Appenders
{
    [TestFixture]
    public class RingtoetsUserDataFolderConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new RingtoetsUserDataFolderConverter();

            // Assert
            Assert.IsInstanceOf<FormattingInfo>(converter.FormattingInfo);
            Assert.IsNull(converter.Next);
            Assert.IsNull(converter.Option);
            Assert.IsNull(converter.Properties);
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("some string")]
        public void Convert_Always_WriteLocalUserDataDirectory(string infix)
        {
            // Setup
            string settingsDirectory = SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(infix);

            var mocks = new MockRepository();
            var textWriter = mocks.StrictMock<TextWriter>();
            textWriter.Expect(w => w.Write(settingsDirectory));
            mocks.ReplayAll();

            var converter = new RingtoetsUserDataFolderConverter
            {
                Option = infix
            };

            // Call
            converter.Format(textWriter, null);

            // Assert
            mocks.VerifyAll();
        }
    }
}