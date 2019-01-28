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
using Ringtoets.Common.IO.Configurations;
using Ringtoets.HeightStructures.IO.Configurations;

namespace Ringtoets.HeightStructures.IO.Test.Configurations
{
    [TestFixture]
    public class HeightStructuresCalculationConfigurationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HeightStructuresCalculationConfiguration(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Call
            var configuration = new HeightStructuresCalculationConfiguration("some name");

            // Assert
            Assert.IsInstanceOf<StructuresCalculationConfiguration>(configuration);
            Assert.AreEqual("some name", configuration.Name);
            Assert.IsNull(configuration.ModelFactorSuperCriticalFlow);
            Assert.IsNull(configuration.LevelCrestStructure);
        }

        [Test]
        public void SimpleProperties_SetNewValues_NewValuesSet()
        {
            // Setup
            var modelFactorSuperCriticalFlow = new StochastConfiguration();
            var levelCrestStructure = new StochastConfiguration();
            var configuration = new HeightStructuresCalculationConfiguration("some name");

            // Call
            configuration.ModelFactorSuperCriticalFlow = modelFactorSuperCriticalFlow;
            configuration.LevelCrestStructure = levelCrestStructure;

            // Assert
            Assert.AreSame(modelFactorSuperCriticalFlow, configuration.ModelFactorSuperCriticalFlow);
            Assert.AreSame(levelCrestStructure, configuration.LevelCrestStructure);
        }

        [Test]
        public void Name_Null_ThrowsArgumentNullException()
        {
            // Setup
            var calculationConfiguration = new HeightStructuresCalculationConfiguration("valid name");

            // Call
            TestDelegate test = () => calculationConfiguration.Name = null;

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}