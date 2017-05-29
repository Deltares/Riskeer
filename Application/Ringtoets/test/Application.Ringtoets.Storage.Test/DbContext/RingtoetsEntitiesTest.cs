﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Data.Entity.Infrastructure;
using System.Globalization;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.DbContext
{
    [TestFixture]
    public class RingtoetsEntitiesTest
    {
        private const string entityConnectionString = "metadata=res://*/DbContext.RingtoetsEntities.csdl|" +
                                                      "res://*/DbContext.RingtoetsEntities.ssdl|" +
                                                      "res://*/DbContext.RingtoetsEntities.msl;" +
                                                      "provider=System.Data.SQLite.EF6;" +
                                                      "provider connection string=\"{0}\"";

        private const string connectionString = "failifmissing=True;data source=C:\\file.sqlite;" +
                                                "read only=False;" +
                                                "foreign keys=True;" +
                                                "version=3;" +
                                                "pooling=False";

        [Test]
        public void Constructor_WithConnectionString_ExpectedValues()
        {
            // Setup
            string fullConnectionString = string.Format(CultureInfo.CurrentCulture, entityConnectionString,
                                                        connectionString);

            // Call
            using (var ringtoetsEntities = new RingtoetsEntities(fullConnectionString))
            {
                // Assert
                Assert.IsInstanceOf<System.Data.Entity.DbContext>(ringtoetsEntities);
                Assert.AreEqual(connectionString, ringtoetsEntities.Database.Connection.ConnectionString);
                Assert.IsFalse(ringtoetsEntities.Configuration.LazyLoadingEnabled);
            }
        }

        [Test]
        public void OnModelCreating_Always_ThrowsUnintentionalCodeFirstException()
        {
            // Setup
            string fullConnectionString = string.Format(CultureInfo.CurrentCulture, entityConnectionString,
                                                        connectionString);

            // Call
            using (var ringtoetsEntities = new TestRingtoetsEntities(fullConnectionString))
            {
                TestDelegate test = () => ringtoetsEntities.CallOnModelCreating();

                // Assert
                Assert.Throws<UnintentionalCodeFirstException>(test);
            }
        }

        private class TestRingtoetsEntities : RingtoetsEntities
        {
            public TestRingtoetsEntities(string connString) : base(connString) {}

            public void CallOnModelCreating()
            {
                OnModelCreating(null);
            }
        }
    }
}