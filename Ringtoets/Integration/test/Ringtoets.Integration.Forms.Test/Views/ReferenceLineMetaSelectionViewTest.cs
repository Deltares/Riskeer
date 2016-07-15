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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    public class ReferenceLineMetaSelectionViewTest
    {
        private const int assessmentSectionIdColumnIndex = 0;
        private const int signalingValueColumnIndex = 1;
        private const int lowerLimitColumnIndex = 2;
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
        public void Constructor_ReferenceLineMetasNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ReferenceLineMetaSelectionView(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("referenceLineMetas", parameter);
        }

        [Test]
        public void Constructor_EmptyReferenceLineMetas_DefaultProperties()
        {
            // Call
            var view = new ReferenceLineMetaSelectionView(new List<ReferenceLineMeta>());

            // Assert
            ShowReferenceLineSelectionView(view);
            var referenceLineMetaDataGrid = (DataGridView) new ControlTester("ReferenceLineMetaDataGrid").TheObject;

            Assert.AreEqual(3, referenceLineMetaDataGrid.ColumnCount);
            Assert.IsFalse(referenceLineMetaDataGrid.RowHeadersVisible);
            Assert.IsFalse(referenceLineMetaDataGrid.MultiSelect);
            Assert.AreEqual(0, referenceLineMetaDataGrid.RowCount);

            var signalingLowerLimitComboBox = (ComboBox) new ControlTester("SignalingLowerLimitComboBox").TheObject;
            Assert.AreEqual(2, signalingLowerLimitComboBox.Items.Count);
            CollectionAssert.AreEqual(new[]
            {
                "Signaleringsnorm", "Ondergrens"
            }, signalingLowerLimitComboBox.Items);

            var assessmentSectionIdColumn = (DataGridViewTextBoxColumn) referenceLineMetaDataGrid.Columns[assessmentSectionIdColumnIndex];
            Assert.AreEqual("AssessmentSectionId", assessmentSectionIdColumn.DataPropertyName);
            Assert.AreEqual("Identificatiecode ", assessmentSectionIdColumn.HeaderText);
            Assert.AreEqual(DataGridViewAutoSizeColumnMode.Fill, assessmentSectionIdColumn.AutoSizeMode);
            Assert.IsTrue(assessmentSectionIdColumn.ReadOnly);

            var signalingValueColumn = (DataGridViewTextBoxColumn) referenceLineMetaDataGrid.Columns[signalingValueColumnIndex];
            Assert.AreEqual("SignalingValue", signalingValueColumn.DataPropertyName);
            Assert.AreEqual("Signaleringswaarde", signalingValueColumn.HeaderText);
            Assert.AreEqual(110, signalingValueColumn.Width);
            Assert.IsTrue(signalingValueColumn.ReadOnly);

            var lowerLimitColumn = (DataGridViewTextBoxColumn) referenceLineMetaDataGrid.Columns[lowerLimitColumnIndex];
            Assert.AreEqual("LowerLimitValue", lowerLimitColumn.DataPropertyName);
            Assert.AreEqual("Ondergrens", lowerLimitColumn.HeaderText);
            Assert.AreEqual(110, lowerLimitColumn.Width);
            Assert.IsTrue(lowerLimitColumn.ReadOnly);
        }

        [Test]
        public void Constructor_OneReferenceLineMeta_OneRowInGrid()
        {
            // Setup
            ReferenceLineMeta referenceLineMeta = TestReferenceLineMeta();
            const string testname = "testName";
            referenceLineMeta.AssessmentSectionId = testname;

            // Call
            var view = new ReferenceLineMetaSelectionView(new[]
            {
                referenceLineMeta
            });

            // Assert
            ShowReferenceLineSelectionView(view);
            var referenceLineMetaDataGrid = (DataGridView) new ControlTester("ReferenceLineMetaDataGrid").TheObject;
            Assert.AreEqual(1, referenceLineMetaDataGrid.RowCount);
            var firstRow = referenceLineMetaDataGrid.Rows[0];
            Assert.AreEqual(testname, (string) firstRow.Cells[assessmentSectionIdColumnIndex].Value);
            Assert.AreEqual(referenceLineMeta.SignalingValue, (int) firstRow.Cells[signalingValueColumnIndex].Value);
            Assert.AreEqual(referenceLineMeta.LowerLimitValue, (int) firstRow.Cells[lowerLimitColumnIndex].Value);
        }

        private static ReferenceLineMeta TestReferenceLineMeta()
        {
            return new ReferenceLineMeta
            {
                AssessmentSectionId = "ReferenceLineMeta", LowerLimitValue = 10000, SignalingValue = 30000
            };
        }

        private void ShowReferenceLineSelectionView(ReferenceLineMetaSelectionView referenceLineSelectionView)
        {
            testForm.Controls.Add(referenceLineSelectionView);
            testForm.Show();
        }
    }
}