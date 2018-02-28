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
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Test.Builders
{
    [TestFixture]
    public class FailureMechanismSectionResultColumnBuilderTest
    {
        private const string dataPropertyName = "test property";

        [Test]
        public void AddSectionNameColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSectionNameColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSectionNameColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSectionNameColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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
                FailureMechanismSectionResultColumnBuilder.AddSectionNameColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Vak", columnData.HeaderText);
                Assert.IsTrue(columnData.ReadOnly);
            }
        }

        private static IEnumerable<EnumDisplayWrapper<T>> CreateExpectedEnumDisplayWrappers<T>()
        {
            return Enum.GetValues(typeof(T))
                       .OfType<T>()
                       .Select(e => new EnumDisplayWrapper<T>(e))
                       .ToArray();
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

        #region Simple Assessment

        [Test]
        public void AddSimpleAssessmentResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssessmentResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentResultColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssessmentResultColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentResultColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Eenvoudige toets", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<SimpleAssessmentResultType>> expectedDataSource = CreateExpectedEnumDisplayWrappers<SimpleAssessmentResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<SimpleAssessmentResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddSimpleAssessmentResultValidityOnlyColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentResultValidityOnlyColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssessmentResultValidityOnlyColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentResultValidityOnlyColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssessmentResultValidityOnlyColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentResultValidityOnlyColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Eenvoudige toets", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<SimpleAssessmentResultValidityOnlyType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<SimpleAssessmentResultValidityOnlyType>[]) columnData.DataSource);
            }
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void AddDetailedAssessmentResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentResultColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentResultColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets per vak", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<DetailedAssessmentResultType>> expectedDataSource = CreateExpectedEnumDisplayWrappers<DetailedAssessmentResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<DetailedAssessmentResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddDetailedAssessmentProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets per vak\r\nfaalkans", columnData.HeaderText);
            }
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        public void AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () =>
                FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toets op maat", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource,
                                                  (EnumDisplayWrapper<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityCalculationResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityCalculationResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityCalculationResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () =>
                FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityCalculationResultColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityCalculationResultColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityCalculationResultColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toets op maat", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<TailorMadeAssessmentProbabilityCalculationResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<TailorMadeAssessmentProbabilityCalculationResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource,
                                                  (EnumDisplayWrapper<TailorMadeAssessmentProbabilityCalculationResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toets op maat\r\nfaalkans", columnData.HeaderText);
            }
        }

        #endregion

        #region Assessment Assembly

        [Test]
        public void AddSimpleAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssemblyCategoryGroupColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\neenvoudige toets", columnData.HeaderText);
            }
        }

        [Test]
        public void AddDetailedAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssemblyCategoryGroupColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\ngedetailleerde toets per vak", columnData.HeaderText);
            }
        }

        [Test]
        public void AddTailorMadeAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssemblyCategoryGroupColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\ntoets op maat", columnData.HeaderText);
            }
        }

        [Test]
        public void AddCombinedAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddCombinedAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddCombinedAssemblyCategoryGroupColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\ngecombineerd", columnData.HeaderText);
            }
        }

        [Test]
        public void AddCombinedAssemblyProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddCombinedAssemblyProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddCombinedAssemblyProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddCombinedAssemblyProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddCombinedAssemblyProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddCombinedAssemblyProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\ngecombineerde\r\nfaalkansschatting", columnData.HeaderText);
            }
        }

        #endregion

        #region Manual Assembly

        [Test]
        public void AddUseManualAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddUseManualAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddUseManualAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddUseManualAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddUseManualAssemblyCategoryGroupColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddUseManualAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewCheckBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Overschrijf\r\nassemblageresultaat", columnData.HeaderText);
            }
        }

        [Test]
        public void AddManualAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddManualAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddManualAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddManualAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddManualAssemblyCategoryGroupColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddManualAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\nhandmatig", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<FailureMechanismSectionAssemblyCategoryGroup>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<FailureMechanismSectionAssemblyCategoryGroup>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<FailureMechanismSectionAssemblyCategoryGroup>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddManualAssemblyProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddManualAssemblyProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddManualAssemblyProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddManualAssemblyProbabilityColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddManualAssemblyProbabilityColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultColumnBuilder.AddManualAssemblyProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\nhandmatig", columnData.HeaderText);
            }
        }

        #endregion
    }
}