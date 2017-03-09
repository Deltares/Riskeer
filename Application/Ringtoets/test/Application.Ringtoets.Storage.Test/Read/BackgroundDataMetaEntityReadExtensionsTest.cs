﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class BackgroundDataMetaEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowArgumentNullException()
        {
            // Setup
            BackgroundDataMetaEntity entity = null;

            // Call
            TestDelegate test = () => entity.Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_WithEntity_ReturnKeyValuePairWithData()
        {
            // Setup
            BackgroundDataMetaEntity entity = new BackgroundDataMetaEntity
            {
                Key = "Test key",
                Value = "Test value"
            };

            // Call
            KeyValuePair<string, string> data = entity.Read();

            // Assert
            Assert.AreEqual(entity.Key, data.Key);
            Assert.AreEqual(entity.Value, data.Value);
        }
    }
}