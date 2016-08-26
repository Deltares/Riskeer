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

using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsScenariosViewTest
    {
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
        public void DefaultConstructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (var view = ShowScenariosView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var dataGridViewControl = (DataGridViewControl) new ControlTester("dataGridViewControl").TheObject;

                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);

                Assert.AreEqual(new Size(0, 0), dataGridViewControl.MinimumSize);
                Assert.IsTrue(view.AutoScroll);
                Assert.IsNull(view.Data);
                Assert.IsNull(view.FailureMechanism);

                Assert.AreEqual(0, dataGridView.RowCount);
                Assert.AreEqual(2, dataGridView.ColumnCount);

                var sectionColumn = dataGridView.Columns[0];
                var calculationColumn = dataGridView.Columns[1];

                Assert.AreEqual("Vak", sectionColumn.HeaderText);
                Assert.AreEqual("Berekening", calculationColumn.HeaderText);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(sectionColumn);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(calculationColumn);

                Assert.IsTrue(sectionColumn.ReadOnly);

                DataGridViewComboBoxColumn comboBoxColumn = (DataGridViewComboBoxColumn) calculationColumn;
                Assert.AreEqual("WrappedObject", comboBoxColumn.ValueMember);
                Assert.AreEqual("DisplayName", comboBoxColumn.DisplayMember);
            }
        }

        [Test]
        public void Data_ValidDataSet_ValidData()
        {
            // Setup
            using (var view = ShowScenariosView())
            {
                var calculationGroup = new CalculationGroup();

                // Call
                view.Data = calculationGroup;

                // Assert
                Assert.AreSame(calculationGroup, view.Data);
            }
        }

        [Test]
        public void Data_NullifyValidData_DataIsNull()
        {
            // Setup
            using (var view = ShowScenariosView())
            {
                var calculationGroup = new CalculationGroup();

                // Call
                view.Data = calculationGroup;

                // Assert
                Assert.AreSame(calculationGroup, view.Data);
            }
        }

        [Test]
        public void FailureMechanism_ValidFailureMechanismSet_ValidFailureMechanism()
        {
            // Setup
            using (var view = ShowScenariosView())
            {
                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

                // Call
                view.FailureMechanism = failureMechanism;

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void FailureMechanism_NullifyValidFailureMechanism_FailureMechanismIsNull()
        {
            // Setup
            using (var view = ShowScenariosView())
            {
                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

                // Call
                view.FailureMechanism = failureMechanism;

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        private GrassCoverErosionInwardsScenariosView ShowScenariosView()
        {
            var scenariosView = new GrassCoverErosionInwardsScenariosView();
            testForm.Controls.Add(scenariosView);
            testForm.Show();

            return scenariosView;
        }
    }
}