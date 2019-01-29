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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Primitives;
using Riskeer.AssemblyTool.Forms;

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
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSectionNameColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddSectionNameColumn(control, dataPropertyName);

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
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssessmentResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentResultColumn(control, dataPropertyName);

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
        public void AddSimpleAssessmentValidityOnlyResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentValidityOnlyResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssessmentValidityOnlyResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentValidityOnlyResultColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssessmentValidityOnlyResultColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssessmentValidityOnlyResultColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Eenvoudige toets", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<SimpleAssessmentValidityOnlyResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<SimpleAssessmentValidityOnlyResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<SimpleAssessmentValidityOnlyResultType>[]) columnData.DataSource);
            }
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void AddDetailedAssessmentProbabilityOnlyResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityOnlyResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentProbabilityOnlyResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityOnlyResultColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentProbabilityOnlyResultColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityOnlyResultColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets per vak", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<DetailedAssessmentProbabilityOnlyResultType>> expectedDataSource = CreateExpectedEnumDisplayWrappers<DetailedAssessmentProbabilityOnlyResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<DetailedAssessmentProbabilityOnlyResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddDetailedAssessmentProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets per vak\r\nfaalkans", columnData.HeaderText);
            }
        }

        [Test]
        public void AddDetailedAssessmentResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets per vak", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<DetailedAssessmentResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<DetailedAssessmentResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource,
                                                  (EnumDisplayWrapper<DetailedAssessmentResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddDetailedAssessmentResultForFactorizedSignalingNormColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedSignalingNormColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForFactorizedSignalingNormColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedSignalingNormColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForFactorizedSignalingNormColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedSignalingNormColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens Iv", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<DetailedAssessmentResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<DetailedAssessmentResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource,
                                                  (EnumDisplayWrapper<DetailedAssessmentResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddDetailedAssessmentResultForSignalingNormColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForSignalingNormColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForSignalingNormColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForSignalingNormColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForSignalingNormColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForSignalingNormColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens IIv", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<DetailedAssessmentResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<DetailedAssessmentResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource,
                                                  (EnumDisplayWrapper<DetailedAssessmentResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddDetailedAssessmentResultForMechanismSpecificLowerLimitNormColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForMechanismSpecificLowerLimitNormColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForMechanismSpecificLowerLimitNormColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForMechanismSpecificLowerLimitNormColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForMechanismSpecificLowerLimitNormColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForMechanismSpecificLowerLimitNormColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens IIIv", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<DetailedAssessmentResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<DetailedAssessmentResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource,
                                                  (EnumDisplayWrapper<DetailedAssessmentResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddDetailedAssessmentResultForLowerLimitNormColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForLowerLimitNormColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForLowerLimitNormColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForLowerLimitNormColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForLowerLimitNormColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForLowerLimitNormColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens IVv", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<DetailedAssessmentResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<DetailedAssessmentResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource,
                                                  (EnumDisplayWrapper<DetailedAssessmentResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddDetailedAssessmentResultForFactorizedLowerLimitNormColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedLowerLimitNormColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForFactorizedLowerLimitNormColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedLowerLimitNormColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssessmentResultForFactorizedLowerLimitNormColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssessmentResultForFactorizedLowerLimitNormColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\ncategoriegrens Vv", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<DetailedAssessmentResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<DetailedAssessmentResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource,
                                                  (EnumDisplayWrapper<DetailedAssessmentResultType>[]) columnData.DataSource);
            }
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        public void AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () =>
                FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityAndDetailedCalculationResultColumn(control, dataPropertyName);

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
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityCalculationResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityCalculationResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () =>
                FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityCalculationResultColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityCalculationResultColumn(control, dataPropertyName);

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
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toets op maat\r\nfaalkans", columnData.HeaderText);
            }
        }

        [Test]
        public void AddTailorMadeAssessmentCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentCategoryGroupColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentCategoryGroupColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toets op maat", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<TailorMadeAssessmentCategoryGroupResultType>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<TailorMadeAssessmentCategoryGroupResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<TailorMadeAssessmentCategoryGroupResultType>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddTailorMadeAssessmentResultColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentResultColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentResultColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentResultColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssessmentResultColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssessmentResultColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toets op maat", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<TailorMadeAssessmentResultType>> expectedDataSource = CreateExpectedEnumDisplayWrappers<TailorMadeAssessmentResultType>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<TailorMadeAssessmentResultType>[]) columnData.DataSource);
            }
        }

        #endregion

        #region Assessment Assembly

        [Test]
        public void AddSimpleAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSimpleAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddSimpleAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toetsoordeel\r\neenvoudige toets", columnData.HeaderText);
            }
        }

        [Test]
        public void AddDetailedAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddDetailedAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddDetailedAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toetsoordeel\r\ngedetailleerde toets per vak", columnData.HeaderText);
            }
        }

        [Test]
        public void AddTailorMadeAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddTailorMadeAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddTailorMadeAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toetsoordeel\r\ntoets op maat", columnData.HeaderText);
            }
        }

        [Test]
        public void AddCombinedAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddCombinedAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toetsoordeel\r\ngecombineerd", columnData.HeaderText);
            }
        }

        [Test]
        public void AddCombinedAssemblyProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddCombinedAssemblyProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyProbabilityColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddCombinedAssemblyProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toetsoordeel\r\ngecombineerde\r\nfaalkansschatting", columnData.HeaderText);
            }
        }

        #endregion

        #region Manual Assembly

        [Test]
        public void AddUseManualAssemblyColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddUseManualAssemblyColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddUseManualAssemblyColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddUseManualAssemblyColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewCheckBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Overschrijf\r\ntoetsoordeel", columnData.HeaderText);
            }
        }

        [Test]
        public void AddSelectableAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSelectableAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddSelectableAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddSelectableAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataPropertyName", exception.ParamName);
        }

        [Test]
        public void AddSelectableAssemblyCategoryGroupColumn_WithParameters_AddsColumnToDataGridViewControl()
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
                FailureMechanismSectionResultViewColumnBuilder.AddSelectableAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toetsoordeel\r\nhandmatig", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<SelectableFailureMechanismSectionAssemblyCategoryGroup>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<SelectableFailureMechanismSectionAssemblyCategoryGroup>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<SelectableFailureMechanismSectionAssemblyCategoryGroup>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddManualAssemblyCategoryGroupColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyCategoryGroupColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddManualAssemblyCategoryGroupColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyCategoryGroupColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyCategoryGroupColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewComboBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toetsoordeel\r\nhandmatig", columnData.HeaderText);
                Assert.AreEqual("Value", columnData.ValueMember);
                Assert.AreEqual("DisplayName", columnData.DisplayMember);

                IEnumerable<EnumDisplayWrapper<ManualFailureMechanismSectionAssemblyCategoryGroup>> expectedDataSource =
                    CreateExpectedEnumDisplayWrappers<ManualFailureMechanismSectionAssemblyCategoryGroup>();
                AssertEnumDisplayWrappersAreEqual(expectedDataSource, (EnumDisplayWrapper<ManualFailureMechanismSectionAssemblyCategoryGroup>[]) columnData.DataSource);
            }
        }

        [Test]
        public void AddManualAssemblyProbabilityColumn_DataGridViewControlNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyProbabilityColumn(null, "property");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("dataGridViewControl", exception.ParamName);
        }

        [Test]
        public void AddManualAssemblyProbabilityColumn_DataPropertyNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyProbabilityColumn(new DataGridViewControl(), null);

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
                FailureMechanismSectionResultViewColumnBuilder.AddManualAssemblyProbabilityColumn(control, dataPropertyName);

                // Assert
                Assert.AreEqual(1, dataGridView.ColumnCount);

                var columnData = (DataGridViewTextBoxColumn) dataGridView.Columns[0];
                Assert.AreEqual(dataPropertyName, columnData.DataPropertyName);
                Assert.AreEqual("Toetsoordeel\r\nhandmatig", columnData.HeaderText);
            }
        }

        #endregion
    }
}