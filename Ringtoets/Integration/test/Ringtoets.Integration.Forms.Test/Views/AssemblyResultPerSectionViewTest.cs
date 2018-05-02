﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssemblyResultPerSectionViewTest
    {
        private const int sectionStartColumnIndex = 0;
        private const int sectionEndColumnIndex = 1;
        private const int sectionTotalAssemblyResultColumnIndex = 2;
        private const int pipingColumnIndex = 3;
        private const int grassCoverErosionInwardsColumnIndex = 4;
        private const int macroStabilityInwardsColumnIndex = 5;
        private const int macroStabilityOutwardsColumnIndex = 6;
        private const int microStabilityColumnIndex = 7;
        private const int stabilityStoneCoverColumnIndex = 8;
        private const int waveImpactAsphaltCoverColumnIndex = 9;
        private const int waterPressureAsphaltCoverColumnIndex = 10;
        private const int grassCoverErosionOutwardsColumnIndex = 11;
        private const int grassCoverSlipOffOutwardsColumnIndex = 12;
        private const int grassCoverSlipOffInwardsColumnIndex = 13;
        private const int heightStructuresColumnIndex = 14;
        private const int closingStructures = 15;
        private const int pipingStructures = 16;
        private const int stabilityPointStructuresColumnIndex = 17;
        private const int strengthStabilityLengthwiseColumnIndex = 18;
        private const int duneErosionColumnIndex = 19;
        private const int technicalInnovationColumnIndex = 20;
        private const int expectedColumnCount = 21;

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
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyResultPerSectionView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithAssessmentSection_ExpectedValues()
        {
            // Setup
            var random = new Random(39);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            using (var view = new AssemblyResultPerSectionView(assessmentSection))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                // Assert
                Assert.AreEqual(2, view.Controls.Count);

                var button = (Button) new ControlTester("RefreshAssemblyResultsButton").TheObject;
                Assert.AreEqual("Assemblageresultaat verversen", button.Text);
                Assert.IsTrue(button.Enabled);

                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }

        [Test]
        public void GivenWithAssemblyResultTotalView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (ShowAssemblyResultPerSectionView())
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreEqual(expectedColumnCount, dataGridView.ColumnCount);

                DataGridViewColumnCollection dataGridViewColumns = dataGridView.Columns;

                AssertColumn(dataGridViewColumns[sectionStartColumnIndex], "Kilometrering van [km]");
                AssertColumn(dataGridViewColumns[sectionEndColumnIndex], "Kilometrering tot [km]");
                AssertColumn(dataGridViewColumns[sectionTotalAssemblyResultColumnIndex], "Totaal vakoordeel");
                AssertColumn(dataGridViewColumns[pipingColumnIndex], "STPH");
                AssertColumn(dataGridViewColumns[grassCoverErosionInwardsColumnIndex], "GEKB");
                AssertColumn(dataGridViewColumns[macroStabilityInwardsColumnIndex], "STBI");
                AssertColumn(dataGridViewColumns[macroStabilityOutwardsColumnIndex], "STBU");
                AssertColumn(dataGridViewColumns[microStabilityColumnIndex], "STMI");
                AssertColumn(dataGridViewColumns[stabilityStoneCoverColumnIndex], "ZST");
                AssertColumn(dataGridViewColumns[waveImpactAsphaltCoverColumnIndex], "AGK");
                AssertColumn(dataGridViewColumns[waterPressureAsphaltCoverColumnIndex], "AWO");
                AssertColumn(dataGridViewColumns[grassCoverErosionOutwardsColumnIndex], "GEBU");
                AssertColumn(dataGridViewColumns[grassCoverSlipOffOutwardsColumnIndex], "GABU");
                AssertColumn(dataGridViewColumns[grassCoverSlipOffInwardsColumnIndex], "GABI");
                AssertColumn(dataGridViewColumns[heightStructuresColumnIndex], "HTKW");
                AssertColumn(dataGridViewColumns[closingStructures], "BSKW");
                AssertColumn(dataGridViewColumns[pipingStructures], "PKW");
                AssertColumn(dataGridViewColumns[stabilityPointStructuresColumnIndex], "STKWp");
                AssertColumn(dataGridViewColumns[strengthStabilityLengthwiseColumnIndex], "STKWl");
                AssertColumn(dataGridViewColumns[duneErosionColumnIndex], "DA");
                AssertColumn(dataGridViewColumns[technicalInnovationColumnIndex], "INN");
            }
        }

        private static void AssertColumn(DataGridViewColumn column, string headerText)
        {
            Assert.IsInstanceOf<DataGridViewTextBoxColumn>(column);
            Assert.AreEqual(headerText, column.HeaderText);
            Assert.IsTrue(column.ReadOnly);
        }

        private AssemblyResultPerSectionView ShowAssemblyResultPerSectionView()
        {
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            var view = new AssemblyResultPerSectionView(assessmentSection);
            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}