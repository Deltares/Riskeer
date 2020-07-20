// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class StructureCalculationConfigurationHelperTest
    {
        [Test]
        public void GenerateCalculations_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StructureCalculationConfigurationHelper.GenerateCalculations<TestStructure, TestStructureInput>(null, new List<TestStructure>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        public void GenerateCalculations_StructuresNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StructureCalculationConfigurationHelper.GenerateCalculations<TestStructure, TestStructureInput>(new CalculationGroup(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("structures", exception.ParamName);
        }

        [Test]
        public void GenerateCalculations_Always_SetsCorrectCalculations()
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            var structure1 = new TestStructure(new StructureBase.ConstructionProperties
            {
                Id = "testStructure1",
                Location = new Point2D(0.0, 0.0),
                Name = "structure1",
                StructureNormalOrientation = (RoundedDouble) 4.0
            });
            var structure2 = new TestStructure(new StructureBase.ConstructionProperties
            {
                Id = "testStructure2",
                Location = new Point2D(5.0, 0.0),
                Name = "structure2",
                StructureNormalOrientation = (RoundedDouble) 5.0
            });

            // Call
            StructureCalculationConfigurationHelper.GenerateCalculations<TestStructure, TestStructureInput>(calculationGroup, new[]
            {
                structure1,
                structure2
            });

            // Assert
            var calculation1 = (StructuresCalculationScenario<TestStructureInput>) calculationGroup.Children.First();
            Assert.AreEqual("structure1", calculation1.Name);
            Assert.AreEqual(structure1, calculation1.InputParameters.Structure);

            var calculation2 = (StructuresCalculationScenario<TestStructureInput>) calculationGroup.Children.Last();
            Assert.AreEqual("structure2", calculation2.Name);
            Assert.AreEqual(structure2, calculation2.InputParameters.Structure);
        }

        private class TestStructure : StructureBase
        {
            public TestStructure(ConstructionProperties constructionProperties) : base(constructionProperties) {}
        }

        private class TestStructureInput : StructuresInputBase<TestStructure>
        {
            public override bool IsStructureInputSynchronized => true;

            public override void SynchronizeStructureInput() {}
        }
    }
}