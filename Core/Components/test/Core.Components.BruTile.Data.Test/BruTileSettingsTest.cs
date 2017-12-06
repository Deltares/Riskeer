﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Util.Settings;
using NUnit.Framework;

namespace Core.Components.BruTile.Data.Test
{
    [TestFixture]
    public class BruTileSettingsTest
    {
        [Test]
        public void BruTileSettings_Always_ReturnExpectedSettings()
        {
            Assert.AreEqual(4, BruTileSettings.MaximumNumberOfThreads);
            Assert.AreEqual("png", BruTileSettings.PersistentCacheFormat);
            Assert.AreEqual(14, BruTileSettings.PersistentCacheExpireInDays);
            Assert.AreEqual(100, BruTileSettings.MemoryCacheMinimum);
            Assert.AreEqual(200, BruTileSettings.MemoryCacheMaximum);
            Assert.AreEqual(SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory("tilecaches"), BruTileSettings.PersistentCacheDirectoryRoot);
        }
    }
}