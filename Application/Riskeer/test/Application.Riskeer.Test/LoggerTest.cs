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

using Core.Common.Gui.Forms.MessageWindow;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using NUnit.Framework;

namespace Application.Riskeer.Test
{
    [TestFixture]
    public class LoggerTest
    {
        [Test]
        public void Setup_Always_SetsCorrectValues()
        {
            // When
            Logger.Setup();

            // Then
            var hierarchy = (Hierarchy) LogManager.GetRepository();
            Assert.AreEqual(Level.Debug, hierarchy.Root.Level);
            Assert.IsTrue(hierarchy.Configured);
        }

        [Test]
        public void LogAppenders_Always_OfCorrectType()
        {
            // When
            Logger.Setup();

            // Then
            var hierarchy = (Hierarchy) LogManager.GetRepository();
            Assert.AreEqual(2, hierarchy.Root.Appenders.Count);
            Assert.IsInstanceOf<FileAppender>(hierarchy.Root.Appenders[0]);
            Assert.IsInstanceOf<MessageWindowLogAppender>(hierarchy.Root.Appenders[1]);
        }
    }
}