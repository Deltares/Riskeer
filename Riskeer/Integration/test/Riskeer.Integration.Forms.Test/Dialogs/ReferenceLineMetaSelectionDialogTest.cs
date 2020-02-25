// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Controls.Dialogs;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Integration.Forms.Dialogs;

namespace Riskeer.Integration.Forms.Test.Dialogs
{
    [TestFixture]
    public class ReferenceLineMetaSelectionDialogTest : NUnitFormTestWithHiddenDesktop
    {
        private const int assessmentSectionIdColumn = 0;
        private const int signalingValueColumn = 1;
        private const int lowerLimitValueColumn = 2;

        [Test]
        public void Constructor_WithoutParent_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ReferenceLineMetaSelectionDialog(null, Enumerable.Empty<ReferenceLineMeta>());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("dialogParent", parameter);
        }

        [Test]
        public void Constructor_WithoutReferenceLineMetas_ThrowsArgumentNullException()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                TestDelegate test = () => new ReferenceLineMetaSelectionDialog(viewParent, null);

                // Assert
                string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
                Assert.AreEqual("referenceLineMetas", parameter);
            }
        }

        [Test]
        public void Constructor_WithParentAndEmptyReferenceLineMeta_DefaultProperties()
        {
            // Setup
            using (var viewParent = new Form())
            {
                // Call
                using (var dialog = new ReferenceLineMetaSelectionDialog(viewParent, Enumerable.Empty<ReferenceLineMeta>()))
                {
                    // Assert
                    Assert.IsInstanceOf<DialogBase>(dialog);
                    Assert.IsNull(dialog.SelectedReferenceLineMeta);
                    Assert.AreEqual(@"Stel een traject samen", dialog.Text);

                    GroupBox groupBox = ControlTestHelper.GetControls<GroupBox>(dialog, "groupBox1").Single();
                    Assert.AreEqual("Kies de norm van het dijktraject:", groupBox.Text);

                    AssertReferenceLineMetaDataGridViewControl(dialog);
                }
            }
        }

        [Test]
        public void Constructor_WithParentAndUnorderedReferenceLineMetas_ShowsOrderedGrid()
        {
            // Setup
            var referenceLineMetas = new[]
            {
                new ReferenceLineMeta(),
                new ReferenceLineMeta
                {
                    AssessmentSectionId = "101-10"
                },
                new ReferenceLineMeta
                {
                    AssessmentSectionId = "101b-1"
                },
                new ReferenceLineMeta
                {
                    AssessmentSectionId = "101-2"
                },
                new ReferenceLineMeta
                {
                    AssessmentSectionId = "101-1"
                },
                new ReferenceLineMeta
                {
                    AssessmentSectionId = "101a-1"
                },
                new ReferenceLineMeta
                {
                    AssessmentSectionId = "10"
                },
                new ReferenceLineMeta
                {
                    AssessmentSectionId = "102-1"
                }
            };

            using (var viewParent = new Form())
            {
                // Call
                using (var dialog = new ReferenceLineMetaSelectionDialog(viewParent, referenceLineMetas))
                {
                    // Assert
                    var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", dialog).TheObject;
                    DataGridView dataGridView = grid.Controls.OfType<DataGridView>().First();

                    var assessmentIdValuesInGrid = new List<string>();
                    for (var i = 0; i < dataGridView.Rows.Count; i++)
                    {
                        object currentIdValue = dataGridView[assessmentSectionIdColumn, i].FormattedValue;
                        if (currentIdValue != null)
                        {
                            assessmentIdValuesInGrid.Add(currentIdValue.ToString());
                        }
                    }

                    CollectionAssert.AreEqual(new[]
                    {
                        "",
                        "10",
                        "101-1",
                        "101-2",
                        "101-10",
                        "101a-1",
                        "101b-1",
                        "102-1"
                    }, assessmentIdValuesInGrid);
                }
            }
        }

        [Test]
        [Combinatorial]
        public void Constructor_WithParentAndReferenceLineMetas_ShowsExpectedGrid(
            [Values("", "10")] string assessmentSectionId,
            [Values(null, int.MinValue, -1, 0, 1, int.MaxValue)]
            int? signalingValue,
            [Values(int.MinValue, -1, 0, 1, int.MaxValue)]
            int lowerLimitValue)
        {
            // Setup
            var referenceLineMetas = new[]
            {
                new ReferenceLineMeta
                {
                    AssessmentSectionId = assessmentSectionId,
                    SignalingValue = signalingValue,
                    LowerLimitValue = lowerLimitValue
                }
            };

            using (var viewParent = new Form())
            {
                // Call
                using (var dialog = new ReferenceLineMetaSelectionDialog(viewParent, referenceLineMetas))
                {
                    // Assert
                    var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", dialog).TheObject;
                    DataGridView dataGridView = grid.Controls.OfType<DataGridView>().First();

                    Assert.AreEqual(1, dataGridView.Rows.Count);
                    object currentIdValue = dataGridView[assessmentSectionIdColumn, 0].FormattedValue;
                    Assert.IsNotNull(currentIdValue);
                    Assert.AreEqual(assessmentSectionId, currentIdValue.ToString());

                    object currentSignalingValue = dataGridView[signalingValueColumn, 0].FormattedValue;
                    Assert.IsNotNull(currentSignalingValue);
                    string expectedSignalingValue = signalingValue.HasValue && signalingValue.Value > 0
                                                        ? ProbabilityFormattingHelper.FormatFromReturnPeriod(signalingValue.Value)
                                                        : string.Empty;
                    Assert.AreEqual(expectedSignalingValue, currentSignalingValue.ToString());

                    object currentLowerLimitValue = dataGridView[lowerLimitValueColumn, 0].FormattedValue;
                    Assert.IsNotNull(currentLowerLimitValue);
                    string expectedLowerLimitValue = lowerLimitValue > 0
                                                         ? ProbabilityFormattingHelper.FormatFromReturnPeriod(lowerLimitValue)
                                                         : string.Empty;
                    Assert.AreEqual(expectedLowerLimitValue, currentLowerLimitValue.ToString());
                }
            }
        }

        [Test]
        public void OnLoad_Always_SetMinimumSize()
        {
            // Setup
            using (var viewParent = new Form())
            using (var dialog = new ReferenceLineMetaSelectionDialog(viewParent, Enumerable.Empty<ReferenceLineMeta>()))
            {
                // Call
                dialog.Show();

                // Assert
                Assert.AreEqual(372, dialog.MinimumSize.Width);
                Assert.AreEqual(350, dialog.MinimumSize.Height);
            }
        }

        private static void AssertReferenceLineMetaDataGridViewControl(ReferenceLineMetaSelectionDialog dialog)
        {
            var grid = (DataGridViewControl) new ControlTester("ReferenceLineMetaDataGridViewControl", dialog).TheObject;
            Assert.IsFalse(grid.MultiSelect);
            Assert.AreEqual(DataGridViewSelectionMode.FullRowSelect, grid.SelectionMode);
        }
    }
}