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
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Test.Builders
{
    [TestFixture]
    public class FailureMechanismSectionResultColumnBuilderTest
    {
        private static IEnumerable<EnumDisplayWrapper<T>> CreateExpectedEnumDisplayWrappers<T>()
        {
            return Enum.GetValues(typeof(T))
                       .OfType<T>()
                       .Select(e => new EnumDisplayWrapper<T>(e))
                       .ToArray();
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
            const string dataPropertyName = "test property";

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
                var actualDataSource = (EnumDisplayWrapper<SimpleAssessmentResultType>[]) columnData.DataSource;

                Assert.AreEqual(expectedDataSource.Count(), actualDataSource.Length);
                for (var i = 0; i < actualDataSource.Length; i++)
                {
                    EnumDisplayWrapper<SimpleAssessmentResultType> expectedWrapper = expectedDataSource.ElementAt(i);
                    EnumDisplayWrapper<SimpleAssessmentResultType> actualWrapper = actualDataSource[i];
                    Assert.AreEqual(expectedWrapper.Value, actualWrapper.Value);
                    Assert.AreEqual(expectedWrapper.DisplayName, actualWrapper.DisplayName);
                }
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
            const string dataPropertyName = "test property";

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
                var actualDataSource = (EnumDisplayWrapper<DetailedAssessmentResultType>[]) columnData.DataSource;

                Assert.AreEqual(expectedDataSource.Count(), actualDataSource.Length);
                for (var i = 0; i < actualDataSource.Length; i++)
                {
                    EnumDisplayWrapper<DetailedAssessmentResultType> expectedWrapper = expectedDataSource.ElementAt(i);
                    EnumDisplayWrapper<DetailedAssessmentResultType> actualWrapper = actualDataSource[i];
                    Assert.AreEqual(expectedWrapper.Value, actualWrapper.Value);
                    Assert.AreEqual(expectedWrapper.DisplayName, actualWrapper.DisplayName);
                }
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
            const string dataPropertyName = "test property";

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
                Assert.AreEqual("Gedetailleerde toets per vak\r\nFaalkans", columnData.HeaderText);
            }
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        public void AddTailorMadeAssessmentResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentResultColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentResultColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            const string dataPropertyName = "test property";

            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentResultColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toets op maat", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<TailorMadeAssessmentResultType>> expectedDataSource = CreateExpectedEnumDisplayWrappers<TailorMadeAssessmentResultType>();
                var actualDataSource = (EnumDisplayWrapper<TailorMadeAssessmentResultType>[]) columnData.DataSource;

                Assert.AreEqual(expectedDataSource.Count(), actualDataSource.Length);
                for (var i = 0; i < actualDataSource.Length; i++)
                {
                    EnumDisplayWrapper<TailorMadeAssessmentResultType> expectedWrapper = expectedDataSource.ElementAt(i);
                    EnumDisplayWrapper<TailorMadeAssessmentResultType> actualWrapper = actualDataSource[i];
                    Assert.AreEqual(expectedWrapper.Value, actualWrapper.Value);
                    Assert.AreEqual(expectedWrapper.DisplayName, actualWrapper.DisplayName);
                }
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
            const string dataPropertyName = "test property";

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
                Assert.AreEqual("Toets op maat\r\nFaalkans", columnData.HeaderText);
            }
        }

        #endregion

        #region Assessment Assembly

        [Test]
        public void AddSimpleAssessmentAssemblyColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentAssemblyColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssessmentAssemblyColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentAssemblyColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssessmentAssemblyColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            const string dataPropertyName = "test property";

            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultColumnBuilder.AddSimpleAssessmentAssemblyColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\neenvoudige toets", columnData.HeaderText);
            }
        }

        [Test]
        public void AddDetailedAssessmentAssemblyColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentAssemblyColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentAssemblyColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentAssemblyColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentAssemblyColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            const string dataPropertyName = "test property";

            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultColumnBuilder.AddDetailedAssessmentAssemblyColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\ngedetailleerde toets per vak", columnData.HeaderText);
            }
        }

        [Test]
        public void AddTailorMadeAssessmentAssemblyColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentAssemblyColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentAssemblyColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentAssemblyColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentAssemblyColumn_WithParameters_AddsColumnToDataGridViewControl()
        {
            // Setup
            const string dataPropertyName = "test property";

            using (var form = new Form())
            using (var control = new DataGridViewControl())
            {
                form.Controls.Add(control);
                form.Show();
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Precondition
                Assert.AreEqual(0, dataGridView.ColumnCount);

                // Call
                FailureMechanismSectionResultColumnBuilder.AddTailorMadeAssessmentAssemblyColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Assemblageresultaat\r\ntoets op maat", columnData.HeaderText);
            }
        }

        #endregion
    }
}