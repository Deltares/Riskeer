// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class DataGridViewTestHelperTest
    {
        private const string textBoxColumnHeaderText = "Textbox";
        private const string checkBoxColumnHeaderText = "Checkbox";
        private Form form;
        private DataGridView dataGridView;

        [SetUp]
        public void SetUp()
        {
            dataGridView = CreateFullyConfiguredDataGridView();
            form = new Form();
            form.Controls.Add(dataGridView);
            form.Show();
        }

        [TearDown]
        public void TearDown()
        {
            form.Close();
            form.Dispose();
        }

        [Test]
        public void AssertExpectedHeaders_AllColumnHeadersMatch_DoNotThrow()
        {
            // Setup
            var expectedHeaderNames = new[]
            {
                checkBoxColumnHeaderText,
                textBoxColumnHeaderText
            };

            // Call
            TestDelegate call = () => DataGridViewTestHelper.AssertExpectedHeaders(expectedHeaderNames, dataGridView);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void AssertExpectedHeaders_IncorrectOrder_ThrowsAssertionException()
        {
            // Setup
            var expectedHeaderNames = new[]
            {
                textBoxColumnHeaderText,
                checkBoxColumnHeaderText
            };

            // Call
            TestDelegate call = () => DataGridViewTestHelper.AssertExpectedHeaders(expectedHeaderNames, dataGridView);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        public void AssertExpectedHeaders_MismatchingExpectancy_ThrowsAssertionException(int numberOfHeaders)
        {
            // Setup
            string[] expectedHeaderNames = Enumerable.Range(1, numberOfHeaders)
                                                     .Select(i => i.ToString())
                                                     .ToArray();

            // Call
            TestDelegate call = () => DataGridViewTestHelper.AssertExpectedHeaders(expectedHeaderNames, dataGridView);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertColumnTypes_AllColumnsMatch_DoesNotThrow()
        {
            // Setup
            var expectedTypes = new[]
            {
                typeof(DataGridViewCheckBoxColumn),
                typeof(DataGridViewTextBoxColumn)
            };

            // Call
            TestDelegate call = () => DataGridViewTestHelper.AssertColumnTypes(expectedTypes, dataGridView);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void AssertColumnTypes_IncorrectOrder_ThrowsAssertionException()
        {
            // Setup
            var expectedTypes = new[]
            {
                typeof(DataGridViewTextBoxColumn),
                typeof(DataGridViewCheckBoxColumn)
            };

            // Call
            TestDelegate call = () => DataGridViewTestHelper.AssertColumnTypes(expectedTypes, dataGridView);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        public void AssertColumnTypes_MismatchingExpectancy_ThrowAssertionException(int numberOfTypes)
        {
            Type[] expectedTypes = Enumerable.Repeat(typeof(DataGridViewColumn), numberOfTypes)
                                             .ToArray();

            // Call
            TestDelegate call = () => DataGridViewTestHelper.AssertColumnTypes(expectedTypes, dataGridView);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertExpectedRowFormattedValues_AllCellsMatch_DoesNotThrow()
        {
            var expectedRow0FormattedValues = new object[]
            {
                false,
                "A"
            };
            var expectedRow1FormattedValues = new object[]
            {
                true,
                "B"
            };

            // Call
            TestDelegate call1 = () => DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRow0FormattedValues, dataGridView.Rows[0]);
            TestDelegate call2 = () => DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRow1FormattedValues, dataGridView.Rows[1]);

            // Assert
            Assert.DoesNotThrow(call1);
            Assert.DoesNotThrow(call2);
        }

        [Test]
        public void AssertExpectedRowFormattedValues_IncorrectOrder_ThrowsAssertionException()
        {
            var expectedRow0FormattedValues = new object[]
            {
                "A",
                false
            };

            // Call
            TestDelegate call = () => DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRow0FormattedValues, dataGridView.Rows[0]);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertExpectedRowFormattedValues_MismatchingExpectancy_ThrowsAssertionException()
        {
            var expectedRow0FormattedValues = new object[]
            {
                1,
                2,
                3
            };

            // Call
            TestDelegate call = () => DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRow0FormattedValues, dataGridView.Rows[0]);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        public void AssertCellIsDisabled_DisabledCell_DoesNotThrowAssertionException()
        {
            // Setup
            var row = new DataGridViewRow
            {
                ReadOnly = true,
                Cells =
                {
                    new DataCell(new DataGridViewCellStyle
                    {
                        ForeColor = Color.FromKnownColor(KnownColor.GrayText),
                        BackColor = Color.FromKnownColor(KnownColor.DarkGray)
                    })
                }
            };

            // Call 
            TestDelegate call = () => DataGridViewTestHelper.AssertCellIsDisabled(row.Cells[0]);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssertCellIsEnabled_EnabledCell_DoesNotThrowAssertionException(bool readOnlyProperty)
        {
            // Setup
            var row = new DataGridViewRow
            {
                ReadOnly = readOnlyProperty,
                Cells =
                {
                    new DataCell(new DataGridViewCellStyle
                    {
                        ForeColor = Color.FromKnownColor(KnownColor.ControlText),
                        BackColor = Color.FromKnownColor(KnownColor.White)
                    })
                }
            };

            // Call 
            TestDelegate call = () => DataGridViewTestHelper.AssertCellIsEnabled(row.Cells[0], readOnlyProperty);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource(nameof(InvalidDisabledCells))]
        public void AssertCellIsDisabled_InvalidDisabledCells_ThrowAssertionException(DataGridViewRow row)
        {
            // Call 
            TestDelegate call = () => DataGridViewTestHelper.AssertCellIsDisabled(row.Cells[0]);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCaseSource(nameof(InvalidEnabledCells))]
        public void AssertCellIsEnabled_InvalidDisabledCells_ThrowAssertionException(DataGridViewRow row)
        {
            // Call 
            TestDelegate call = () => DataGridViewTestHelper.AssertCellIsEnabled(row.Cells[0]);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        private class DataCell : DataGridViewCell
        {
            public DataCell(DataGridViewCellStyle style)
            {
                Style = style;
            }
        }

        private DataGridView CreateFullyConfiguredDataGridView()
        {
            var gridView = new DataGridView
            {
                AutoGenerateColumns = false
            };
            gridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = checkBoxColumnHeaderText,
                DataPropertyName = nameof(TestRowData.Boolean)
            });
            gridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = textBoxColumnHeaderText,
                DataPropertyName = nameof(TestRowData.Text)
            });
            gridView.DataSource = new[]
            {
                new TestRowData
                {
                    Boolean = false, Text = "A"
                },
                new TestRowData
                {
                    Boolean = true, Text = "B"
                }
            };
            gridView.Refresh();
            return gridView;
        }

        private class TestRowData
        {
            public bool Boolean { get; set; }
            public string Text { get; set; }
        }

        #region Test data

        private static IEnumerable<TestCaseData> InvalidDisabledCells
        {
            get
            {
                yield return new TestCaseData(new DataGridViewRow
                {
                    ReadOnly = false,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.GrayText),
                            BackColor = Color.FromKnownColor(KnownColor.DarkGray)
                        })
                    }
                }).SetName("ReadOnlyPropertyFalse");
                yield return new TestCaseData(new DataGridViewRow
                {
                    ReadOnly = true,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.ControlText),
                            BackColor = Color.FromKnownColor(KnownColor.DarkGray)
                        })
                    }
                }).SetName("ForeColorPropertyEnabledColor");
                yield return new TestCaseData(new DataGridViewRow
                {
                    ReadOnly = true,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.GrayText),
                            BackColor = Color.FromKnownColor(KnownColor.White)
                        })
                    }
                }).SetName("BackColorPropertyEnabledColor");
            }
        }

        private static IEnumerable<TestCaseData> InvalidEnabledCells
        {
            get
            {
                yield return new TestCaseData(new DataGridViewRow
                {
                    ReadOnly = true,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.ControlText),
                            BackColor = Color.FromKnownColor(KnownColor.White)
                        })
                    }
                }).SetName("ReadOnlyPropertyTrue");
                yield return new TestCaseData(new DataGridViewRow
                {
                    ReadOnly = false,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.GrayText),
                            BackColor = Color.FromKnownColor(KnownColor.White)
                        })
                    }
                }).SetName("ForeColorPropertyDisabledColor");
                yield return new TestCaseData(new DataGridViewRow
                {
                    ReadOnly = false,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.ControlText),
                            BackColor = Color.FromKnownColor(KnownColor.DarkGray)
                        })
                    }
                }).SetName("BackColorPropertyDisabledColor");
            }
        }

        #endregion
    }
}