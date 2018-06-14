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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.TestUtil;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class FailureMechanismAssemblyCategoriesContextBasePropertyInfoTest
    {
        private RingtoetsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(FailureMechanismAssemblyCategoriesProperties));
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
            Assert.AreEqual(typeof(FailureMechanismAssemblyCategoriesContextBase), info.DataType);
        }

        [Test]
        public void CreateInstance_ValidArguments_ReturnProperties()
        {
            // Setup
            var random = new Random(21);
            double n = random.NextDouble(1, 10);

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            IEnumerable<FailureMechanismSectionAssemblyCategory> failureMechanismSectionAssemblyCategories = GetFailureMechanismSectionAssemblyCategories();
            var context = new TestFailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection,
                                                                            () => 0,
                                                                            () => failureMechanismSectionAssemblyCategories);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                IObjectProperties objectProperties = info.CreateInstance(context);

                // Assert
                Assert.IsInstanceOf<FailureMechanismAssemblyCategoriesProperties>(objectProperties);
                Assert.AreSame(calculator.FailureMechanismCategoriesOutput, objectProperties.Data);

                var properties = (FailureMechanismAssemblyCategoriesProperties) objectProperties;
                AssemblyCategoryPropertiesTestHelper.AssertFailureMechanismAssemblyCategoryProperties(
                    calculator.FailureMechanismCategoriesOutput,
                    failureMechanismSectionAssemblyCategories,
                    properties);
            }

            mocks.VerifyAll();
        }

        private class TestFailureMechanismAssemblyCategoriesContext : FailureMechanismAssemblyCategoriesContextBase
        {
            public TestFailureMechanismAssemblyCategoriesContext(IFailureMechanism wrappedData,
                                                                 IAssessmentSection assessmentSection,
                                                                 Func<double> getNFunc,
                                                                 Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> getFailureMechanismSectionAssemblyCategoriesFunc)
                : base(wrappedData, assessmentSection, getNFunc)
            {
                GetFailureMechanismSectionAssemblyCategoriesFunc = getFailureMechanismSectionAssemblyCategoriesFunc;
            }

            public override Func<IEnumerable<FailureMechanismSectionAssemblyCategory>> GetFailureMechanismSectionAssemblyCategoriesFunc { get; }
        }

        private static IEnumerable<FailureMechanismSectionAssemblyCategory> GetFailureMechanismSectionAssemblyCategories()
        {
            var random = new Random(21);

            return new[]
            {
                new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                            random.NextDouble(),
                                                            random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };
        }
    }
}