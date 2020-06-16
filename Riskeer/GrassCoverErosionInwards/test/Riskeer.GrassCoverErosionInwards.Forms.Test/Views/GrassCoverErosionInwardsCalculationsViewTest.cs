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
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
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

        [Test]
        public void Constructor_ListBoxCorrectlyInitialized()
        {
            // Call
            using (ShowGrassCoverErosionInwardsCalculationsView())
            {
                var listBox = (ListBox) new ControlTester("listBox").TheObject;

                // Assert
                Assert.AreEqual(0, listBox.Items.Count);
            }
        }

        [Test]
        public void Data_SetToNull_DoesNotThrow()
        {
            // Setup
            using (GrassCoverErosionInwardsCalculationsView grassCoverErosionInwardsCalculationsView = ShowGrassCoverErosionInwardsCalculationsView())
            {
                // Call
                var testDelegate = new TestDelegate(() => grassCoverErosionInwardsCalculationsView.Data = null);

                // Assert
                Assert.DoesNotThrow(testDelegate);
            }
        }

        [Test]
        public void GrassCoverErosionInwardsFailureMechanism_FailureMechanismWithSections_SectionsListBoxCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var failureMechanismSection1 = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            });
            var failureMechanismSection2 = new FailureMechanismSection("Section 2", new[]
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            });
            var failureMechanismSection3 = new FailureMechanismSection("Section 3", new[]
            {
                new Point2D(10.0, 0.0),
                new Point2D(15.0, 0.0)
            });

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                failureMechanismSection1,
                failureMechanismSection2,
                failureMechanismSection3
            });

            using (GrassCoverErosionInwardsCalculationsView pipingCalculationsView = ShowGrassCoverErosionInwardsCalculationsView())
            {
                // Call
                pipingCalculationsView.GrassCoverErosionInwardsFailureMechanism = failureMechanism;

                // Assert
                var listBox = (ListBox) new ControlTester("listBox").TheObject;
                Assert.AreEqual(3, listBox.Items.Count);
                Assert.AreSame(failureMechanismSection1, listBox.Items[0]);
                Assert.AreSame(failureMechanismSection2, listBox.Items[1]);
                Assert.AreSame(failureMechanismSection3, listBox.Items[2]);
            }
        }

        [Test]
        public void ButtonGenerateCalculations_WithoutGrassCoverErosionInwardsFailureMechanism_ButtonDisabled()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = null;

            using (GrassCoverErosionInwardsCalculationsView grassCoverErosionInwardsCalculationsView = ShowGrassCoverErosionInwardsCalculationsView())
            {
                grassCoverErosionInwardsCalculationsView.GrassCoverErosionInwardsFailureMechanism = grassCoverErosionInwardsFailureMechanism;
                var button = (Button) grassCoverErosionInwardsCalculationsView.Controls.Find("buttonGenerateCalculations", true)[0];

                // Call
                bool state = button.Enabled;

                // Assert
                Assert.IsFalse(state);
            }
        }

        [Test]
        public void ButtonGenerateCalculations_WithGrassCoverErosionInwardsFailureMechanism_ButtonDisabled()
        {
            // Setup
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            using (GrassCoverErosionInwardsCalculationsView grassCoverErosionInwardsCalculationsView = ShowGrassCoverErosionInwardsCalculationsView())
            {
                grassCoverErosionInwardsCalculationsView.GrassCoverErosionInwardsFailureMechanism = grassCoverErosionInwardsFailureMechanism;
                var button = (Button) grassCoverErosionInwardsCalculationsView.Controls.Find("buttonGenerateCalculations", true)[0];

                // Call
                bool state = button.Enabled;

                // Assert
                Assert.IsTrue(state);
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