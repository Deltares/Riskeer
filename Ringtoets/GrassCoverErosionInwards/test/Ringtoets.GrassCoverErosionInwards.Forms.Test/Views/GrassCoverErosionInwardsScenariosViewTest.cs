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

using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;

using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsScenariosViewTest
    {
        private Form testForm;
        private const int assessmentSectionNameColumnIndex = 0;
        private const int calculationColumnIndex = 1;

        [SetUpAttribute]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDownAttribute]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (var view = ShowScenariosView())
            {
                var dataGridViewControl = (DataGridViewControl)new ControlTester("dataGridViewControl").TheObject;
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                // Assert
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.IsNull(view.FailureMechanism);
                Assert.IsNotNull(dataGridViewControl);

                Assert.AreEqual(0, dataGridView.RowCount);
                Assert.AreEqual(2, dataGridView.ColumnCount);

                Assert.AreEqual("Vak", dataGridViewControl.GetColumnFromIndex(0).HeaderText);
                Assert.AreEqual("Berekening", dataGridViewControl.GetColumnFromIndex(1).HeaderText);

                // TODO How to test that rows are of type GrassCoverErosionInwardsSectionResultRow?
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridViewControl.GetColumnFromIndex(0));
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridViewControl.GetColumnFromIndex(1));

                Assert.IsTrue(dataGridViewControl.GetColumnFromIndex(0).ReadOnly);

                DataGridViewComboBoxColumn comboBoxColumn = (DataGridViewComboBoxColumn) dataGridViewControl.GetColumnFromIndex(1);
                Assert.AreEqual("WrappedObject", comboBoxColumn.ValueMember);
                Assert.AreEqual("DisplayName", comboBoxColumn.DisplayMember);
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