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
using Core.Common.Base;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Data.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class NonAdoptableWithProfileProbabilityFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int isRelevantIndex = 1;
        private const int initialFailureMechanismResultTypeIndex = 2;
        private const int initialFailureMechanismResultProfileProbabilityIndex = 3;
        private const int initialFailureMechanismResultSectionProbabilityIndex = 4;
        private const int furtherAnalysisTypeIndex = 5;
        private const int refinedProfileProbabilityIndex = 6;
        private const int refinedSectionProbabilityIndex = 7;
        private const int profileProbabilityIndex = 8;
        private const int sectionProbabilityIndex = 9;
        private const int sectionNIndex = 10;
        private const int assemblyGroupIndex = 11;
        private const int columnCount = 12;
        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void Constructor_GetNFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestNonAdoptableWithProfileProbabilityFailureMechanism();
            Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc =
                sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();

            // Call
            void Call() => new NonAdoptableWithProfileProbabilityFailureMechanismResultView<TestNonAdoptableWithProfileProbabilityFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism, null, fm => false, performAssemblyFunc);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getNFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_GetUseLengthEffectFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestNonAdoptableWithProfileProbabilityFailureMechanism();
            Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc =
                sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();

            // Call
            void Call() => new NonAdoptableWithProfileProbabilityFailureMechanismResultView<TestNonAdoptableWithProfileProbabilityFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism, fm => double.NaN, null, performAssemblyFunc);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getUseLengthEffectFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_PerformAssemblyFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestNonAdoptableWithProfileProbabilityFailureMechanism();

            // Call
            void Call() => new NonAdoptableWithProfileProbabilityFailureMechanismResultView<TestNonAdoptableWithProfileProbabilityFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism, fm => double.NaN, fm => false, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("performAssemblyFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new TestNonAdoptableWithProfileProbabilityFailureMechanism();

            Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc = sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();

            // Call
            using (var view = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<TestNonAdoptableWithProfileProbabilityFailureMechanism>(
                       failureMechanism.SectionResults, failureMechanism, fm => double.NaN, fm => false, performAssemblyFunc))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult,
                    NonAdoptableWithProfileProbabilityFailureMechanismSectionResultRow,
                    TestNonAdoptableWithProfileProbabilityFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenFormWithNonAdoptableWithProfileProbabilityFailureMechanismResultView_ThenExpectedColumnsAreVisible(bool useLengthEffect)
        {
            // Given
            var failureMechanism = new TestNonAdoptableWithProfileProbabilityFailureMechanism
            {
                UseLengthEffect = useLengthEffect
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                // Then
                DataGridView dataGridView = GetDataGridView();

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[isRelevantIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[initialFailureMechanismResultTypeIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[initialFailureMechanismResultSectionProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[furtherAnalysisTypeIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[refinedProfileProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[refinedSectionProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[profileProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[sectionProbabilityIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[sectionNIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assemblyGroupIndex]);

                Assert.AreEqual("Vaknaam", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Is relevant", dataGridView.Columns[isRelevantIndex].HeaderText);
                Assert.AreEqual("Resultaat initieel mechanisme", dataGridView.Columns[initialFailureMechanismResultTypeIndex].HeaderText);
                Assert.AreEqual("Faalkans initieel\r\nmechanisme per doorsnede\r\n[1/jaar]", dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].HeaderText);
                Assert.AreEqual("Faalkans initieel\r\nmechanisme per vak\r\n[1/jaar]", dataGridView.Columns[initialFailureMechanismResultSectionProbabilityIndex].HeaderText);
                Assert.AreEqual("Vervolganalyse", dataGridView.Columns[furtherAnalysisTypeIndex].HeaderText);
                Assert.AreEqual("Aangescherpte\r\nfaalkans per doorsnede\r\n[1/jaar]", dataGridView.Columns[refinedProfileProbabilityIndex].HeaderText);
                Assert.AreEqual("Aangescherpte\r\nfaalkans per vak\r\n[1/jaar]", dataGridView.Columns[refinedSectionProbabilityIndex].HeaderText);
                Assert.AreEqual("Rekenwaarde\r\nfaalkans per doorsnede\r\n[1/jaar]", dataGridView.Columns[profileProbabilityIndex].HeaderText);
                Assert.AreEqual("Rekenwaarde\r\nfaalkans per vak\r\n[1/jaar]", dataGridView.Columns[sectionProbabilityIndex].HeaderText);
                Assert.AreEqual("Rekenwaarde Nvak*\r\n[-]", dataGridView.Columns[sectionNIndex].HeaderText);
                Assert.AreEqual("Duidingsklasse", dataGridView.Columns[assemblyGroupIndex].HeaderText);

                Assert.IsTrue(dataGridView.Columns[nameColumnIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[isRelevantIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[initialFailureMechanismResultTypeIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[initialFailureMechanismResultSectionProbabilityIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[furtherAnalysisTypeIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[refinedProfileProbabilityIndex].ReadOnly);
                Assert.IsFalse(dataGridView.Columns[refinedSectionProbabilityIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[profileProbabilityIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[sectionProbabilityIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[sectionNIndex].ReadOnly);
                Assert.IsTrue(dataGridView.Columns[assemblyGroupIndex].ReadOnly);

                Assert.AreEqual(useLengthEffect, dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[refinedProfileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[profileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[sectionNIndex].Visible);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");

            var failureMechanism = new TestNonAdoptableWithProfileProbabilityFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            var assemblyResult = new FailureMechanismSectionAssemblyResult(0.01, 0.1, 10, FailureMechanismSectionAssemblyGroup.I);

            // Call
            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism, sr => assemblyResult))
            {
                DataGridView dataGridView = GetDataGridView();

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(columnCount, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(true, cells[isRelevantIndex].Value);
                Assert.AreEqual(NonAdoptableInitialFailureMechanismResultType.Manual, cells[initialFailureMechanismResultTypeIndex].Value);
                Assert.AreEqual("-", cells[initialFailureMechanismResultProfileProbabilityIndex].FormattedValue);
                Assert.AreEqual("-", cells[initialFailureMechanismResultSectionProbabilityIndex].FormattedValue);
                Assert.AreEqual(FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, cells[furtherAnalysisTypeIndex].Value);
                Assert.AreEqual("-", cells[refinedProfileProbabilityIndex].FormattedValue);
                Assert.AreEqual("-", cells[refinedSectionProbabilityIndex].FormattedValue);
                Assert.AreEqual("1/100", cells[profileProbabilityIndex].FormattedValue);
                Assert.AreEqual("1/10", cells[sectionProbabilityIndex].FormattedValue);
                Assert.AreEqual("10,00", cells[sectionNIndex].FormattedValue);
                Assert.AreEqual("+I", cells[assemblyGroupIndex].FormattedValue);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_PassesInputToPerformAssemblyFunc()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            var failureMechanism = new TestNonAdoptableWithProfileProbabilityFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            NonAdoptableWithProfileProbabilityFailureMechanismSectionResult functionInput = null;
            Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc = sr =>
            {
                functionInput = sr;
                return FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult();
            };

            // Call
            using (ShowFailureMechanismResultsView(failureMechanism, performAssemblyFunc))
            {
                // Assert
                Assert.AreSame(functionInput, failureMechanism.SectionResults.Single());
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenNonAdoptableWithProfileProbabilityFailureMechanismResultView_WhenApplyLengthEffectChangedAndFailureMechanismObserversNotified_ThenDataGridViewUpdated(
            bool useLengthEffect)
        {
            var failureMechanism = new TestNonAdoptableWithProfileProbabilityFailureMechanism
            {
                UseLengthEffect = !useLengthEffect
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1");
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            using (new AssemblyToolCalculatorFactoryConfig())
            using (ShowFailureMechanismResultsView(failureMechanism))
            {
                var rowsChanged = false;
                DataGridView dataGridView = GetDataGridView();
                dataGridView.Rows.CollectionChanged += (sender, args) => rowsChanged = true;

                // Precondition
                Assert.IsFalse(rowsChanged);
                Assert.AreEqual(!useLengthEffect, dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].Visible);
                Assert.AreEqual(!useLengthEffect, dataGridView.Columns[refinedProfileProbabilityIndex].Visible);
                Assert.AreEqual(!useLengthEffect, dataGridView.Columns[profileProbabilityIndex].Visible);
                Assert.AreEqual(!useLengthEffect, dataGridView.Columns[sectionNIndex].Visible);

                // When
                failureMechanism.UseLengthEffect = useLengthEffect;
                failureMechanism.NotifyObservers();

                // Then
                Assert.IsTrue(rowsChanged);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[initialFailureMechanismResultProfileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[refinedProfileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[profileProbabilityIndex].Visible);
                Assert.AreEqual(useLengthEffect, dataGridView.Columns[sectionNIndex].Visible);
            }
        }

        private static DataGridView GetDataGridView()
        {
            return (DataGridView) new ControlTester("dataGridView").TheObject;
        }

        private NonAdoptableWithProfileProbabilityFailureMechanismResultView<TestNonAdoptableWithProfileProbabilityFailureMechanism> ShowFailureMechanismResultsView(
            TestNonAdoptableWithProfileProbabilityFailureMechanism failureMechanism)
        {
            return ShowFailureMechanismResultsView(failureMechanism, sr => FailureMechanismSectionAssemblyResultTestFactory.CreateFailureMechanismSectionAssemblyResult());
        }

        private NonAdoptableWithProfileProbabilityFailureMechanismResultView<TestNonAdoptableWithProfileProbabilityFailureMechanism> ShowFailureMechanismResultsView(
            TestNonAdoptableWithProfileProbabilityFailureMechanism failureMechanism,
            Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, FailureMechanismSectionAssemblyResult> performAssemblyFunc)
        {
            var failureMechanismResultView = new NonAdoptableWithProfileProbabilityFailureMechanismResultView<TestNonAdoptableWithProfileProbabilityFailureMechanism>(
                failureMechanism.SectionResults, failureMechanism, fm => 1.0, fm => fm.UseLengthEffect, performAssemblyFunc);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }

        private class TestNonAdoptableWithProfileProbabilityFailureMechanism : FailureMechanismBase<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>
        {
            private readonly ObservableList<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> sectionResults;

            public TestNonAdoptableWithProfileProbabilityFailureMechanism() : base("Test", "T")
            {
                sectionResults = new ObservableList<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>();
            }

            public override IEnumerable<ICalculation> Calculations { get; }

            public bool UseLengthEffect { get; set; }
            public override IObservableEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> SectionResults => sectionResults;

            protected override void AddSectionDependentData(FailureMechanismSection section)
            {
                base.AddSectionDependentData(section);
                sectionResults.Add(new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section));
            }
        }
    }
}