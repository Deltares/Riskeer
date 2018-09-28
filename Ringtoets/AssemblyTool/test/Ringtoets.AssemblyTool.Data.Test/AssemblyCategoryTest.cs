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

using System;
using NUnit.Framework;

namespace Ringtoets.AssemblyTool.Data.Test
{
    [TestFixture]
    public class AssemblyCategoryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            var random = new Random(11);

            double lowerBoundary = random.NextDouble();
            double upperBoundary = random.NextDouble();

            // Call
            var category = new SimpleCategory(lowerBoundary, upperBoundary);

            // Assert
            Assert.AreEqual(lowerBoundary, category.LowerBoundary);
            Assert.AreEqual(upperBoundary, category.UpperBoundary);
        }

        private class SimpleCategory : AssemblyCategory
        {
            public SimpleCategory(double lowerBoundary, double upperBoundary)
                : base(lowerBoundary, upperBoundary) {}
        }
    }
}