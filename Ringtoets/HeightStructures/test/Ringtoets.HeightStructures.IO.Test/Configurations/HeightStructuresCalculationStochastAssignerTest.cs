// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.IO.Configurations;

namespace Ringtoets.HeightStructures.IO.Test.Configurations
{
    [TestFixture]
    public class HeightStructuresCalculationStochastAssignerTest
    {
        [Test]
        public void Constructor_WithParameters_ReturnsNewInstance()
        {
            // Setup
            var configuration = new HeightStructuresCalculationConfiguration("name");

            var calculation = new StructuresCalculation<HeightStructuresInput>();

            // Call
            var assigner = new HeightStructuresCalculationStochastAssigner(
                configuration,
                calculation,
                StandardDeviationStochastSetter,
                VariationCoefficientStochastSetter);

            // Assert
            Assert.NotNull(assigner);
        }

        private static bool StandardDeviationStochastSetter(
            HeightStructuresCalculationStochastAssigner.StandardDeviationDefinition definition)
        {
            return true;
        }

        private static bool VariationCoefficientStochastSetter(
            HeightStructuresCalculationStochastAssigner.VariationCoefficientDefinition definition)
        {
            return true;
        }
    }
}