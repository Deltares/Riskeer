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
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data;

namespace Riskeer.Integration.Util.Test
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupsHelperTest
    {
        [Test]
        public void GetFailureMechanismSectionAssemblyGroupBoundaries_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionAssemblyGroupsHelper.GetFailureMechanismSectionAssemblyGroupBoundaries(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetFailureMechanismSectionAssemblyGroupBoundaries_CalculationSuccessful_ReturnsCorrectAssemblyGroupBoundaries()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyGroupBoundariesCalculator;

                // Call
                IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> assemblyGroupBoundaries =
                    FailureMechanismSectionAssemblyGroupsHelper.GetFailureMechanismSectionAssemblyGroupBoundaries(assessmentSection);

                // Assert
                Assert.AreEqual(calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput.Count() + 2, assemblyGroupBoundaries.Count());

                for (int i = 0; i < calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput.Count(); i++)
                {
                    FailureMechanismSectionAssemblyGroupBoundaries expectedBoundary = calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput.ElementAt(i);
                    FailureMechanismSectionAssemblyGroupBoundaries actualBoundary = assemblyGroupBoundaries.ElementAt(i);
                    
                    Assert.AreEqual(expectedBoundary.FailureMechanismSectionAssemblyGroup, actualBoundary.FailureMechanismSectionAssemblyGroup);
                    Assert.AreEqual(expectedBoundary.LowerBoundary, actualBoundary.LowerBoundary);
                    Assert.AreEqual(expectedBoundary.UpperBoundary, actualBoundary.UpperBoundary);
                }

                FailureMechanismSectionAssemblyGroupBoundaries lastAssemblyGroupBoundaries = assemblyGroupBoundaries.Last();
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.NotDominant, lastAssemblyGroupBoundaries.FailureMechanismSectionAssemblyGroup);
                Assert.AreEqual(double.NaN, lastAssemblyGroupBoundaries.UpperBoundary);
                Assert.AreEqual(double.NaN, lastAssemblyGroupBoundaries.LowerBoundary);
                
                Assert.AreEqual(FailureMechanismSectionAssemblyGroup.NotDominant, assemblyGroupBoundaries.ElementAt(assemblyGroupBoundaries.Count() - 2).FailureMechanismSectionAssemblyGroup);
                Assert.AreEqual(double.NaN, assemblyGroupBoundaries.ElementAt(assemblyGroupBoundaries.Count() - 2).UpperBoundary);
                Assert.AreEqual(double.NaN, assemblyGroupBoundaries.ElementAt(assemblyGroupBoundaries.Count() - 2).LowerBoundary);
            }
        }
        
        [Test]
        public void CreateAssemblyGroupsView_CalculatorThrowsException_ReturnsEmptyCollectionOfAssemblyGroupBoundaries()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyGroupBoundariesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> assemblyGroupBoundaries =
                    FailureMechanismSectionAssemblyGroupsHelper.GetFailureMechanismSectionAssemblyGroupBoundaries(assessmentSection);

                // Assert
                Assert.IsEmpty(assemblyGroupBoundaries);
            }
        }
    }
}