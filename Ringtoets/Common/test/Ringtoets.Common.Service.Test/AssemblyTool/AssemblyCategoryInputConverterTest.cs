// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.Data.Input;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Service.AssemblyTool;

namespace Ringtoets.Common.Service.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyCategoryInputConverterTest
    {
        [Test]
        public void Convert_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyCategoryInputConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Convert_WithInput_ReturnsCalculatorInput()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerBoundaryNorm = random.NextDouble();

            var input = new AssemblyCategoryInput(signalingNorm, lowerBoundaryNorm);

            // Call
            AssemblyCategoriesCalculatorInput calculatorInput = AssemblyCategoryInputConverter.Convert(input);

            // Assert
            Assert.AreEqual(signalingNorm, calculatorInput.SignalingNorm);
            Assert.AreEqual(lowerBoundaryNorm, calculatorInput.LowerBoundaryNorm);
        }
    }
}