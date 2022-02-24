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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Forms;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupsViewTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismSectionAssemblyGroupsView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateAssemblyGroupsView_CalculatorThrowsException_ReturnsEmptyCollection()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                using (var view = new FailureMechanismSectionAssemblyGroupsView(assessmentSection))
                {
                    AssemblyGroupsTable<DisplayFailureMechanismSectionAssemblyGroup> failureMechanismSectionGroupsTable = GetAssemblyGroupsTable(view);

                    // Assert
                    Assert.IsEmpty(failureMechanismSectionGroupsTable.Rows);
                }
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            using (var view = new FailureMechanismSectionAssemblyGroupsView(assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(assessmentSection, view.AssessmentSection);

                Assert.AreEqual(1, view.Controls.Count);

                Panel groupBoxPanel = ControlTestHelper.GetControls<Panel>(view, "groupBoxPanel").Single();
                Assert.AreEqual(1, groupBoxPanel.Controls.Count);
                Assert.AreEqual(DockStyle.Fill, groupBoxPanel.Dock);

                GroupBox groupBox = ControlTestHelper.GetControls<GroupBox>(groupBoxPanel, "groupBox").Single();
                Assert.AreEqual(1, groupBox.Controls.Count);
                Assert.AreEqual(DockStyle.Fill, groupBox.Dock);
                Assert.AreEqual("Duidingsklassen", groupBox.Text);

                AssemblyGroupsTable<DisplayFailureMechanismSectionAssemblyGroup> assemblyGroupsTable = GetAssemblyGroupsTable(view);
                Assert.AreEqual(DockStyle.Fill, assemblyGroupsTable.Dock);
            }
        }

        [Test]
        public void Constructor_WithValidParameters_InputCorrectlySet()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;
                using (new FailureMechanismSectionAssemblyGroupsView(assessmentSection))
                {
                    // Assert
                    Assert.AreEqual(assessmentSection.FailureMechanismContribution.LowerLimitNorm, calculator.LowerLimitNorm);
                    Assert.AreEqual(assessmentSection.FailureMechanismContribution.SignalingNorm, calculator.SignalingNorm);
                }
            }
        }

        [Test]
        public void Constructor_WithValidParameters_FillTableWithData()
        {
            // Setup
            var random = new Random();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;
                var failureMechanismSectionAssemblyGroup = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
                double lowerBoundary = random.NextDouble();
                double upperBoundary = random.NextDouble();
                calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput = new List<FailureMechanismSectionAssemblyGroupBoundaries>
                {
                    new FailureMechanismSectionAssemblyGroupBoundaries(failureMechanismSectionAssemblyGroup, lowerBoundary, upperBoundary)
                };

                // Call
                using (var view = new FailureMechanismSectionAssemblyGroupsView(assessmentSection))
                {
                    AssemblyGroupsTable<DisplayFailureMechanismSectionAssemblyGroup> assemblyGroupsTable = GetAssemblyGroupsTable(view);

                    // Assert
                    Assert.AreEqual(calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput.Count(), assemblyGroupsTable.Rows.Count);

                    for (int i = 0; i < calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput.Count(); i++)
                    {
                        FailureMechanismSectionAssemblyGroupBoundaries expectedBoundary = calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput.ElementAt(i);
                        var actualBoundary = (AssemblyGroupRow<DisplayFailureMechanismSectionAssemblyGroup>) assemblyGroupsTable.Rows[i].DataBoundItem;

                        Assert.AreEqual(DisplayFailureMechanismSectionAssemblyGroupConverter.Convert(expectedBoundary.Group), actualBoundary.Group);
                        Assert.AreEqual(expectedBoundary.LowerBoundary, actualBoundary.LowerBoundary);
                        Assert.AreEqual(expectedBoundary.UpperBoundary, actualBoundary.UpperBoundary);
                    }
                }
            }
        }

        [Test]
        public void GivenViewWithValidData_WhenFailureMechanismContributionUpdated_ThenDataTableUpdated()
        {
            // Given
            var random = new Random();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;
                var output = new List<FailureMechanismSectionAssemblyGroupBoundaries>
                {
                    CreateFailureMechanismSectionAssemblyGroupBoundaries(random)
                };

                calculator.FailureMechanismSectionAssemblyGroupBoundariesOutput = output;

                using (var view = new FailureMechanismSectionAssemblyGroupsView(assessmentSection))
                {
                    AssemblyGroupsTable<DisplayFailureMechanismSectionAssemblyGroup> failureMechanismSectionGroupsTable = GetAssemblyGroupsTable(view);

                    // Precondition
                    int groupBoundaries = output.Count;
                    Assert.AreEqual(groupBoundaries, failureMechanismSectionGroupsTable.Rows.Count);

                    int newGroupBoundaries = random.Next(1, 10);
                    for (var i = 1; i <= newGroupBoundaries; i++)
                    {
                        output.Add(CreateFailureMechanismSectionAssemblyGroupBoundaries(random));
                    }

                    // When
                    assessmentSection.FailureMechanismContribution.NotifyObservers();

                    // Then
                    Assert.AreEqual(groupBoundaries + newGroupBoundaries, failureMechanismSectionGroupsTable.Rows.Count);
                }
            }
        }

        private static AssemblyGroupsTable<DisplayFailureMechanismSectionAssemblyGroup> GetAssemblyGroupsTable(
            FailureMechanismSectionAssemblyGroupsView view)
        {
            return ControlTestHelper.GetControls<AssemblyGroupsTable<DisplayFailureMechanismSectionAssemblyGroup>>(
                view, "assemblyGroupsTable").Single();
        }

        private static FailureMechanismSectionAssemblyGroupBoundaries CreateFailureMechanismSectionAssemblyGroupBoundaries(Random random)
        {
            return new FailureMechanismSectionAssemblyGroupBoundaries(random.NextEnumValue<FailureMechanismSectionAssemblyGroup>(),
                                                                      random.NextDouble(),
                                                                      random.NextDouble());
        }
    }
}