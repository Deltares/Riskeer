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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Test.Assembly
{
    [TestFixture]
    public class ExportableFailureMechanismAssemblyResultWithProbabilityTest
    {
        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assemblyMethod = random.NextEnumValue<ExportableAssemblyMethod>();
            var assemblyCategory = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();
            double probability = random.NextDouble();

            // Call
            var assemblyResult = new ExportableFailureMechanismAssemblyResultWithProbability(assemblyMethod, assemblyCategory, probability);

            // Assert
            Assert.IsInstanceOf<ExportableFailureMechanismAssemblyResult>(assemblyResult);

            Assert.AreEqual(assemblyMethod, assemblyResult.AssemblyMethod);
            Assert.AreEqual(assemblyCategory, assemblyResult.AssemblyCategory);
            Assert.AreEqual(probability, assemblyResult.Probability);
        }
    }
}