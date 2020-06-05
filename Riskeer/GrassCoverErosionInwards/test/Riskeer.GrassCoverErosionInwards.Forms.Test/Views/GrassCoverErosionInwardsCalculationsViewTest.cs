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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationsViewTest : NUnitFormTest
    {
        private const int nameColumnIndex = 0;
        private const int selectableHydraulicBoundaryLocationsColumnIndex = 1;
        private const int dikeProfileColumnIndex = 2;
        private const int useDamColumnIndex = 3;
        private const int damTypeColumnIndex = 4;
        private const int damHeightColumnIndex = 5;
        private const int useForeShoreGeometryColumnIndex = 6;
        private const int dikeHeightColumnIndex = 7;
        private const int expectedCriticalOvertoppingRateColumnIndex = 8;
        private const int standardDeviationCriticalOvertoppingRateColumnIndex = 9;
        private Form testForm;

        [Test]
        public void Constructor_DefaultValues()
        {
            // Call
            using (var grassCoverErosionInwardsCalculationsView = new GrassCoverErosionInwardsCalculationsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(grassCoverErosionInwardsCalculationsView);
                Assert.IsInstanceOf<IView>(grassCoverErosionInwardsCalculationsView);
                Assert.IsInstanceOf<ISelectionProvider>(grassCoverErosionInwardsCalculationsView);
                Assert.IsNull(grassCoverErosionInwardsCalculationsView.Data);
                Assert.IsNull(grassCoverErosionInwardsCalculationsView.AssessmentSection);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (ShowGrassCoverErosionInwardsCalculationsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                Assert.IsFalse(dataGridView.AutoGenerateColumns);
                Assert.AreEqual(10, dataGridView.ColumnCount);

                foreach (DataGridViewComboBoxColumn column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
                {
                    Assert.AreEqual("This", column.ValueMember);
                    Assert.AreEqual("DisplayName", column.DisplayMember);
                }
            }
        }

        [Test]
        public void Constructor_DataGridViewControlColumnHeadersCorrectlyInitialized_()
        {
            // Call
            using (ShowGrassCoverErosionInwardsCalculationsView())
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                Assert.AreEqual(10, dataGridView.ColumnCount);
                Assert.AreEqual("Naam", dataGridView.Columns[nameColumnIndex].HeaderText);
                Assert.AreEqual("Hydraulische belastingenlocatie", dataGridView.Columns[selectableHydraulicBoundaryLocationsColumnIndex].HeaderText);
                Assert.AreEqual("Dijkprofiel", dataGridView.Columns[dikeProfileColumnIndex].HeaderText);
                Assert.AreEqual("Gebruik dam", dataGridView.Columns[useDamColumnIndex].HeaderText);
                Assert.AreEqual("Damtype", dataGridView.Columns[damTypeColumnIndex].HeaderText);
                Assert.AreEqual("Damhoogte [m+NAP]", dataGridView.Columns[damHeightColumnIndex].HeaderText);
                Assert.AreEqual("Gebruik voorlandgeometrie", dataGridView.Columns[useForeShoreGeometryColumnIndex].HeaderText);
                Assert.AreEqual("Dijkhoogte [m+NAP]", dataGridView.Columns[dikeHeightColumnIndex].HeaderText);
                Assert.AreEqual("Verwachtingswaarde kritiek overslagdebiet [m3/m/s]", dataGridView.Columns[expectedCriticalOvertoppingRateColumnIndex].HeaderText);
                Assert.AreEqual("Standaardafwijking kritiek overslagdebiet [m3/m/s]", dataGridView.Columns[standardDeviationCriticalOvertoppingRateColumnIndex].HeaderText);
            }
        }

        public override void Setup()
        {
            base.Setup();

            testForm = new Form();
        }

        public override void TearDown()
        {
            base.TearDown();

            testForm.Dispose();
        }

        private GrassCoverErosionInwardsCalculationsView ShowGrassCoverErosionInwardsCalculationsView()
        {
            var grassCoverErosionInwardsCalculationsView = new GrassCoverErosionInwardsCalculationsView();

            testForm.Controls.Add(grassCoverErosionInwardsCalculationsView);
            testForm.Show();

            return grassCoverErosionInwardsCalculationsView;
        }
    }
}