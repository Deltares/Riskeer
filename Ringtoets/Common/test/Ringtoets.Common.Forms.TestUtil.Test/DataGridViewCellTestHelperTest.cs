// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace Ringtoets.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class DataGridViewCellTestHelperTest
    {
        [Test]
        public void AssertCellIsDisabled_DisabledCell_DoesNotThrowAssertionException()
        {
            // Setup
            DataGridViewRow row = new DataGridViewRow()
            {
                ReadOnly = true,
                Cells =
                {
                    new DataCell(new DataGridViewCellStyle()
                    {
                        ForeColor = Color.FromKnownColor(KnownColor.GrayText),
                        BackColor = Color.FromKnownColor(KnownColor.DarkGray)
                    })
                }
            };

            // Call 
            TestDelegate call = () => DataGridViewCellTestHelper.AssertCellIsDisabled(row.Cells[0]);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssertCellIsEnabled_EnabledCell_DoesNotThrowAssertionException(bool readOnlyProperty)
        {
            // Setup
            DataGridViewRow row = new DataGridViewRow()
            {
                ReadOnly = readOnlyProperty,
                Cells =
                {
                    new DataCell(new DataGridViewCellStyle()
                    {
                        ForeColor = Color.FromKnownColor(KnownColor.ControlText),
                        BackColor = Color.FromKnownColor(KnownColor.White)
                    })
                }
            };

            // Call 
            TestDelegate call = () => DataGridViewCellTestHelper.AssertCellIsEnabled(row.Cells[0], readOnlyProperty);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        [TestCaseSource("InvalidDisabledCells")]
        public void AssertCellIsDisabled_InvalidDisabledCells_ThrowAssertionException(DataGridViewRow row)
        {
            // Call 
            TestDelegate call = () => DataGridViewCellTestHelper.AssertCellIsDisabled(row.Cells[0]);

            // Assert
            Assert.Throws<AssertionException>(call);
        }

        [Test]
        [TestCaseSource("InvalidEnabledCells")]
        public void AssertCellIsEnabled_InvalidDisabledCells_ThrowAssertionException(DataGridViewRow row)
        {
            // Call 
            TestDelegate call = () => DataGridViewCellTestHelper.AssertCellIsEnabled(row.Cells[0]);

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

        #region Test data

        private static IEnumerable<TestCaseData> InvalidDisabledCells
        {
            get
            {
                yield return new TestCaseData(new DataGridViewRow()
                {
                    ReadOnly = false,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle()
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.GrayText),
                            BackColor = Color.FromKnownColor(KnownColor.DarkGray)
                        })
                    }
                }).SetName("ReadOnlyPropertyFalse");
                yield return new TestCaseData(new DataGridViewRow()
                {
                    ReadOnly = true,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle()
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.ControlText),
                            BackColor = Color.FromKnownColor(KnownColor.DarkGray)
                        })
                    }
                }).SetName("ForeColorPropertyEnabledColor");
                yield return new TestCaseData(new DataGridViewRow()
                {
                    ReadOnly = true,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle()
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
                yield return new TestCaseData(new DataGridViewRow()
                {
                    ReadOnly = true,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle()
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.ControlText),
                            BackColor = Color.FromKnownColor(KnownColor.White)
                        })
                    }
                }).SetName("ReadOnlyPropertyTrue");
                yield return new TestCaseData(new DataGridViewRow()
                {
                    ReadOnly = false,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle()
                        {
                            ForeColor = Color.FromKnownColor(KnownColor.GrayText),
                            BackColor = Color.FromKnownColor(KnownColor.White)
                        })
                    }
                }).SetName("ForeColorPropertyDisabledColor");
                yield return new TestCaseData(new DataGridViewRow()
                {
                    ReadOnly = false,
                    Cells =
                    {
                        new DataCell(new DataGridViewCellStyle()
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