// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using Core.Common.Util.Enums;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Builders;
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Forms.Test.Builders
{
    [TestFixture]
    public class FailureMechanismSectionResultViewColumnBuilderTest
    {
        private const string dataPropertyName = "test property";

        [Test]
        public void AddSectionNameColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSectionNameColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddSectionNameColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Vaknaam", columnData.HeaderText);
                Assert.IsTrue(columnData.ReadOnly);
            }
        }

        [Test]
        public void AddIsRelevantColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddIsRelevantColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddIsRelevantColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddIsRelevantColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddIsRelevantColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddIsRelevantColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewCheckBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Is relevant", columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
            }
        }

        /// <summary>
        /// Method that asserts whether <paramref name="expected"/> and <paramref name="actual"/>
        /// are equal.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="expected"/> and
        /// <paramref name="actual"/> are not equal.</exception>
        private static void AssertEnumDisplayWrappersAreEqual<T>(IEnumerable<EnumDisplayWrapper<T>> expected,
                                                                 IEnumerable<EnumDisplayWrapper<T>> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());
            for (var i = 0; i < expected.Count(); i++)
            {
                EnumDisplayWrapper<T> expectedWrapper = expected.ElementAt(i);
                EnumDisplayWrapper<T> actualWrapper = actual.ElementAt(i);
                Assert.AreEqual(expectedWrapper.Value, actualWrapper.Value);
                Assert.AreEqual(expectedWrapper.DisplayName, actualWrapper.DisplayName);
            }
        }

        #region Initial Failure Mechanism Result

        [Test]
        public void AddInitialFailureMechanismResultTypeColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultTypeColumn<TestEnum>(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddInitialFailureMechanismResultTypeColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultTypeColumn<TestEnum>(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddInitialFailureMechanismResultTypeColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultTypeColumn<TestEnum>(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Resultaat initieel mechanisme", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<TestEnum>> expectedDataSource = EnumDisplayWrapperHelper.GetEnumTypes<TestEnum>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<TestEnum>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddInitialFailureMechanismResultProfileProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultProfileProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddInitialFailureMechanismResultProfileProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultProfileProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddInitialFailureMechanismResultProfileProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultProfileProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Faalkans initieel\r\nmechanisme per doorsnede\r\n[1/jaar]", columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
            }
        }

        [Test]
        public void AddInitialFailureMechanismResultSectionProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultSectionProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddInitialFailureMechanismResultSectionProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultSectionProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddInitialFailureMechanismResultSectionProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddInitialFailureMechanismResultSectionProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Faalkans initieel\r\nmechanisme per vak\r\n[1/jaar]", columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
            }
        }

        #endregion

        #region Further Analysis

        [Test]
        public void AddFurtherAnalysisTypeColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisTypeColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddFurtherAnalysisTypeColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisTypeColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddFurtherAnalysisTypeColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddFurtherAnalysisTypeColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Vervolganalyse", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<FailureMechanismSectionResultFurtherAnalysisType>> expectedDataSource = EnumDisplayWrapperHelper.GetEnumTypes<FailureMechanismSectionResultFurtherAnalysisType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<FailureMechanismSectionResultFurtherAnalysisType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddProbabilityRefinementTypeColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddProbabilityRefinementTypeColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddProbabilityRefinementTypeColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddProbabilityRefinementTypeColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddProbabilityRefinementTypeColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddProbabilityRefinementTypeColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Aanscherpen faalkans", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<ProbabilityRefinementType>> expectedDataSource = EnumDisplayWrapperHelper.GetEnumTypes<ProbabilityRefinementType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<ProbabilityRefinementType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddRefinedProfileProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddRefinedProfileProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddRefinedProfileProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddRefinedProfileProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddRefinedProfileProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddRefinedProfileProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Aangescherpte\r\nfaalkans per doorsnede\r\n[1/jaar]", columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
            }
        }

        [Test]
        public void AddRefinedSectionProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddRefinedSectionProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddRefinedSectionProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddRefinedSectionProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddRefinedSectionProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddRefinedSectionProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Aangescherpte\r\nfaalkans per vak\r\n[1/jaar]", columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
            }
        }

        #endregion

        #region Assembly

        [Test]
        public void AddAssemblyProfileProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddAssemblyProfileProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddAssemblyProfileProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddAssemblyProfileProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddAssemblyProfileProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddAssemblyProfileProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Rekenwaarde\r\nfaalkans per doorsnede\r\n[1/jaar]", columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
            }
        }

        [Test]
        public void AddAssemblySectionProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddAssemblySectionProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddAssemblySectionProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Rekenwaarde\r\nfaalkans per vak\r\n[1/jaar]", columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
            }
        }

        [Test]
        public void AddAssemblySectionNColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionNColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddAssemblySectionNColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionNColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddAssemblySectionNColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddAssemblySectionNColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Rekenwaarde Nvak*\r\n[-]", columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
            }
        }

        [Test]
        public void AddAssemblyGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddAssemblyGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddAssemblyGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultViewColumnBuilder.AddAssemblyGroupColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddAssemblyGroupColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultViewColumnBuilder.AddAssemblyGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Duidingsklasse", columnData.HeaderText);
                Assert.IsFalse(columnData.ReadOnly);
            }
        }

        #endregion
    }
}