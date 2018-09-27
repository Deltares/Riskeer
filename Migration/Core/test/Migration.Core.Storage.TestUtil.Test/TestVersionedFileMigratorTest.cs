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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Migration.Scripts.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Migration.Core.Storage.TestUtil.Test
{
    [TestFixture]
    public class TestVersionedFileMigratorTest
    {
        [Test]
        public void Constructor_ComparerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestVersionedFileMigrator(null, Enumerable.Empty<UpgradeScript>(),
                                                                    Enumerable.Empty<CreateScript>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("comparer", paramName);
        }

        [Test]
        public void Constructor_UpgradeScriptsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new TestVersionedFileMigrator(comparer, null,
                                                                    Enumerable.Empty<CreateScript>());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("upgradeScripts", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_CreateScriptsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new TestVersionedFileMigrator(comparer, Enumerable.Empty<UpgradeScript>(),
                                                                    null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("createScripts", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ValidComparer_ExpectedProperties()
        {
            // Setup
            var mockRepository = new MockRepository();
            var comparer = mockRepository.Stub<IComparer>();
            mockRepository.ReplayAll();

            IEnumerable<UpgradeScript> upgradeScripts = Enumerable.Empty<UpgradeScript>();
            IEnumerable<CreateScript> createScripts = Enumerable.Empty<CreateScript>();

            // Call
            var migrator = new SimpleTestVersionedFileMigrator(comparer, upgradeScripts,
                                                               createScripts);

            // Assert
            Assert.IsInstanceOf<VersionedFileMigrator>(migrator);
            Assert.AreSame(upgradeScripts, migrator.AvailableUpgradeScripts());
            Assert.AreSame(createScripts, migrator.AvailableCreateScripts());
            mockRepository.VerifyAll();
        }

        private class SimpleTestVersionedFileMigrator : TestVersionedFileMigrator
        {
            public SimpleTestVersionedFileMigrator(IComparer comparer, IEnumerable<UpgradeScript> upgradeScripts,
                                                   IEnumerable<CreateScript> createScripts)
                : base(comparer, upgradeScripts, createScripts) {}

            public IEnumerable<UpgradeScript> AvailableUpgradeScripts()
            {
                return GetAvailableUpgradeScripts();
            }

            public IEnumerable<CreateScript> AvailableCreateScripts()
            {
                return GetAvailableCreateScripts();
            }
        }
    }
}