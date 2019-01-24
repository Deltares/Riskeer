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
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.Data.Test
{
    [TestFixture]
    public class AssemblyCategoriesInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            var random = new Random(11);
            double n = random.NextDouble();
            double failureMechanismContribution = random.NextDouble();
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            // Call
            var input = new AssemblyCategoriesInput(n, failureMechanismContribution, signalingNorm, lowerLimitNorm);

            // Assert
            Assert.AreEqual(n, input.N);
            Assert.AreEqual(failureMechanismContribution, input.FailureMechanismContribution, 1e-6);
            Assert.AreEqual(signalingNorm, input.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, input.LowerLimitNorm);
        }
    }
}