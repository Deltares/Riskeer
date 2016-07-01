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
        public void Constructor_DefaultValues()
        {
            // Call
            using(var view = new GrassCoverErosionInwardsScenariosView())
            {
                // Assert
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (ShowScenariosView())
            {
                // Assert
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(2, dataGridView.ColumnCount);
                Assert.IsTrue(dataGridView.Columns[assessmentSectionNameColumnIndex].ReadOnly);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[calculationColumnIndex]);

                DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn) dataGridView.Columns[calculationColumnIndex];
                Assert.AreEqual("WrappedObject", column.ValueMember);
                Assert.AreEqual("DisplayName", column.DisplayMember);
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