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
using System.Collections.Generic;
using Core.Common.TestUtil;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Forms.Factories;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.Integration.Forms.Test.Factories
{
    [TestFixture]
    public class StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateSimpleAssemblyFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void CreateSimpleAssemblyFeatures_WithValidData_ReturnsFeaturesCollection()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(random.NextDouble(),
                                                                                                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Call
                IEnumerable<MapFeature> features = StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateSimpleAssemblyFeatures(failureMechanism);

                // Assert
                MapFeaturesTestHelper.AssertAssemblyCategoryGroupMapFeatures(calculator.SimpleAssessmentAssemblyOutput.Group, failureMechanism, features);
            }
        }

        [Test]
        public void CreateTailorMadeAssemblyFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void CreateTailorMadeAssemblyFeatures_WithValidData_ReturnsFeaturesCollection()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.TailorMadeAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

                // Call
                IEnumerable<MapFeature> features = StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateTailorMadeAssemblyFeatures(failureMechanism);

                // Assert
                MapFeaturesTestHelper.AssertAssemblyCategoryGroupMapFeatures(calculator.TailorMadeAssemblyCategoryOutput.Value, failureMechanism, features);
            }
        }

        [Test]
        public void CreateCombinedAssemblyFeatures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void CreateCombinedAssemblyFeatures_WithValidData_ReturnsFeaturesCollection()
        {
            // Setup
            var random = new Random(39);
            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism();
            FailureMechanismTestHelper.AddSections(failureMechanism, random.Next(0, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                calculator.CombinedAssemblyCategoryOutput = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

                // Call
                IEnumerable<MapFeature> features = StrengthStabilityLengthwiseConstructionAssemblyMapDataFeaturesFactory.CreateCombinedAssemblyFeatures(failureMechanism);

                // Assert
                MapFeaturesTestHelper.AssertAssemblyCategoryGroupMapFeatures(calculator.CombinedAssemblyCategoryOutput.Value, failureMechanism, features);
            }
        }
    }
}