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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Builders;
using Ringtoets.Common.Forms.Test.Views;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Test.Builders
{
    [TestFixture]
    public class FailureMechanismSectionResultColumnBuilderTest
    {
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

                control.SetDataSource(new[]
                {
                    new TestFailureMechanismSectionResultRow(FailureMechanismSectionResultTestFactory.CreateFailureMechanismSectionResult())
                });

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

                EnumDisplayWrapper<SimpleAssessmentResultType>[] expectedDataSource = Enum.GetValues(typeof(SimpleAssessmentResultType))
                                                                                          .OfType<SimpleAssessmentResultType>()
                                                                                          .Select(sa => new EnumDisplayWrapper<SimpleAssessmentResultType>(sa))
                                                                                          .ToArray();
                var actualDataSource = (EnumDisplayWrapper<SimpleAssessmentResultType>[]) columnData.DataSource;

                Assert.AreEqual(expectedDataSource.Length, actualDataSource.Length);
                for (var i = 0; i < actualDataSource.Length; i++)
                {
                    EnumDisplayWrapper<SimpleAssessmentResultType> expectedWrapper = expectedDataSource[i];
                    EnumDisplayWrapper<SimpleAssessmentResultType> actualWrapper = actualDataSource[i];
                    Assert.AreEqual(expectedWrapper.Value, actualWrapper.Value);
                    Assert.AreEqual(expectedWrapper.DisplayName, actualWrapper.DisplayName);
                }
            }
        }
    }
}