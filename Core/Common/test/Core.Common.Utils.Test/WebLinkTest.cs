// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using NUnit.Framework;

namespace Core.Common.Utils.Test
{
    [TestFixture]
    public class WebLinkTest
    {
        [Test]
        public void Initialization()
        {
            // Setup
            const string name = "Deltares";
            var path = new Uri("http://www.deltares.com");

            // Call
            var url = new WebLink(name, path);

            // Assert
            Assert.AreEqual(name, url.Name);
            Assert.AreEqual(path, url.Path);
        }

        [Test]
        public void SimpleProperties_SetAndGetValue_ReturnNewlySetValue()
        {
            // Setup
            var url = new WebLink("Deltares", new Uri("http://www.deltares.com"));

            const string newName = "Google";
            var newPath = new Uri("http://www.google.com");

            // Call
            url.Name = newName;
            url.Path = newPath;

            // Assert
            Assert.AreEqual(newName, url.Name);
            Assert.AreEqual(newPath, url.Path);
        }
    }
}