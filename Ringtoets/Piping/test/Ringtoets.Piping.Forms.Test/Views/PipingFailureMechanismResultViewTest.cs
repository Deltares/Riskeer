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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismResultViewTest
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
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var view = new PipingFailureMechanismResultView();

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
            Assert.IsNull(view.Data);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var failureMechanismResultView = new PipingFailureMechanismResultView();

            // Call
            ShowPipingCalculationsView(failureMechanismResultView);

            // Assert
            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

            Assert.AreEqual(5, dataGridView.ColumnCount);

            foreach (var column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
            {
                Assert.AreEqual("This", column.ValueMember);
                Assert.AreEqual("DisplayName", column.DisplayMember);
            }

            foreach (var column in dataGridView.Columns.OfType<DataGridViewColumn>())
            {
                Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, column.AutoSizeMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, column.HeaderCell.Style.Alignment);
            }
        }

        [Test]
        public void Data_SetPipingFailureMechanismResultData_DataSet()
        {
            // Setup
            var testData = new PipingFailureMechanismResult();
            var view = new PipingFailureMechanismResultView();

            // Call
            view.Data = testData;

            // Assert
            Assert.AreSame(testData, view.Data);
        }

        [Test]
        public void Data_SetOtherThanPipingFailureMechanismResultData_DataNull()
        {
            // Setup
            var testData = new object();
            var view = new PipingFailureMechanismResultView();

            // Call
            view.Data = testData;

            // Assert
            Assert.IsNull(view.Data);
        }

        private void ShowPipingCalculationsView(PipingFailureMechanismResultView pipingCalculationsView)
        {
            testForm.Controls.Add(pipingCalculationsView);
            testForm.Show();
        }
    }
}