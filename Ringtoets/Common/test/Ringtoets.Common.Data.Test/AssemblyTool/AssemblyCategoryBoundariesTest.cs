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

using System;
using NUnit.Framework;
using Ringtoets.Common.Data.AssemblyTool;

namespace Ringtoets.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyCategoryBoundariesTest
    {
        [Test]
        public void Constructor_CategoriesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyCategoryBoundaries<AssemblyCategory>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("categories", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var categories = new[]
            {
                new SimpleAssemblyCategory(0, 0)
            };

            // Call
            var boundaries = new AssemblyCategoryBoundaries<SimpleAssemblyCategory>(categories);

            // Assert
            Assert.AreSame(categories, boundaries.Categories);
        }

        private class SimpleAssemblyCategory : AssemblyCategory
        {
            public SimpleAssemblyCategory(double lowerBoundary, double upperBoundary) 
                : base(lowerBoundary, upperBoundary) {}
        }
    }
}