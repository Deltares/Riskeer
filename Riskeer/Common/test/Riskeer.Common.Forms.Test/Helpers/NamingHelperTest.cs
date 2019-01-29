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
using Ringtoets.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class NamingHelperTest
    {
        [Test]
        public void GetUniqueName_EmptyCollection_ReturnNameBase()
        {
            // Setup
            const string nameBase = "The basic name";

            IEnumerable<ObjectWithName> existingObjects = Enumerable.Empty<ObjectWithName>();

            // Call
            string name = NamingHelper.GetUniqueName(existingObjects, nameBase, namedObject => namedObject.Name);

            // Assert
            Assert.AreEqual(nameBase, name);
        }

        [Test]
        public void GetUniqueName_CollectionWithNamedObjectMatchingNameBase_ReturnNameBaseAppendedWithPostfixIncrement()
        {
            // Setup
            const string nameBase = "The basic name";

            var existingObjects = new[]
            {
                new ObjectWithName(nameBase)
            };

            // Call
            string name = NamingHelper.GetUniqueName(existingObjects, nameBase, namedObject => namedObject.Name);

            // Assert
            Assert.AreEqual(nameBase + " (1)", name);
        }

        [Test]
        public void GetUniqueName_CollectionWithNamedObjectMatchingNameBaseAndPostFix_ReturnNameBaseAppendedWithNextPostfixIncrement()
        {
            // Setup
            const string nameBase = "The basic name";

            var existingObjects = new[]
            {
                new ObjectWithName(nameBase),
                new ObjectWithName(nameBase + " (3)"),
                new ObjectWithName(nameBase + " (1)"),
                new ObjectWithName(nameBase + " (2)")
            };

            // Call
            string name = NamingHelper.GetUniqueName(existingObjects, nameBase, namedObject => namedObject.Name);

            // Assert
            Assert.AreEqual(nameBase + " (4)", name);
        }

        [Test]
        public void GetUniqueName_CollectionWithNamedObjectNotMatchingNameBase_ReturnNameBase()
        {
            // Setup
            const string nameBase = "The basic name";

            var existingObjects = new[]
            {
                new ObjectWithName("Something original!")
            };

            // Call
            string name = NamingHelper.GetUniqueName(existingObjects, nameBase, namedObject => namedObject.Name);

            // Assert
            Assert.AreEqual(nameBase, name);
        }

        private class ObjectWithName
        {
            public ObjectWithName(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}