// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui.Plugin;
using Core.Gui.PropertyBag;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class AssemblyGroupsContextPropertyInfoTest
    {
        private RiskeerPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RiskeerPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.DataType == typeof(AssemblyGroupsContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(AssemblyGroupsContext), info.DataType);
            Assert.AreEqual(typeof(AssemblyGroupsProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_ValidArguments_ReturnProperties()
        {
            // Setup
            var random = new Random();
            var context = new AssemblyGroupsContext(new AssessmentSection(AssessmentSectionComposition.Dike));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;
                var output = new List<FailureMechanismSectionAssemblyGroupBoundaries>();

                int newGroupBoundaries = random.Next(1, 10);
                for (var i = 1; i <= newGroupBoundaries; i++)
                {
                    output.Add(CreateRandomDisplayFailureMechanismSectionAssemblyGroupBoundaries(random));
                }

                calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput = output;

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<AssemblyGroupsProperties>(objectProperties);

                var properties = (AssemblyGroupsProperties) objectProperties;

                Assert.AreEqual(calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput.Count(), properties.FailureMechanismAssemblyGroups.Length);
                for (var i = 0; i < calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput.Count(); i++)
                {
                    FailureMechanismSectionAssemblyGroupBoundaries category = calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput.ElementAt(i);
                    AssemblyGroupProperties property = properties.FailureMechanismAssemblyGroups[i];
                    Assert.AreEqual(category.Group, property.Group);
                    Assert.AreEqual(category.UpperBoundary, property.UpperBoundary);
                    Assert.AreEqual(category.LowerBoundary, property.LowerBoundary);
                }
            }
        }

        private static FailureMechanismSectionAssemblyGroupBoundaries CreateRandomDisplayFailureMechanismSectionAssemblyGroupBoundaries(Random random)
        {
            return new FailureMechanismSectionAssemblyGroupBoundaries(random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                                                                      random.NextDouble(),
                                                                      random.NextDouble());
        }
    }
}