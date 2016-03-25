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
using Core.Common.Gui.Selection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingCalculationsViewTest
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
        public void Constructor_DefaultValues()
        {
            // Call
            var pipingCalculationsView = new PipingCalculationsView();

            // Assert
            Assert.IsInstanceOf<UserControl>(pipingCalculationsView);
            Assert.IsInstanceOf<IView>(pipingCalculationsView);
            Assert.IsNull(pipingCalculationsView.Data);
            Assert.IsNull(pipingCalculationsView.PipingFailureMechanism);
            Assert.IsNull(pipingCalculationsView.AssessmentSection);
            Assert.IsNull(pipingCalculationsView.ApplicationSelection);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var pipingCalculationsView = new PipingCalculationsView();

            // Call
            ShowPipingCalculationsView(pipingCalculationsView);

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            Assert.AreEqual(7, dataGridView.ColumnCount);

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
        public void Dispose_ViewWithAdditionalPropertiesSet_AdditionalPropertiesSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();

            var pipingCalculationsView = new PipingCalculationsView
            {
                PipingFailureMechanism = pipingFailureMechanism,
                AssessmentSection = assessmentSection,
                ApplicationSelection = applicationSelection
            };

            // Precondition
            Assert.IsNotNull(pipingCalculationsView.PipingFailureMechanism);
            Assert.IsNotNull(pipingCalculationsView.AssessmentSection);
            Assert.IsNotNull(pipingCalculationsView.ApplicationSelection);

            // Call
            pipingCalculationsView.Dispose();

            // Assert
            Assert.IsNull(pipingCalculationsView.PipingFailureMechanism);
            Assert.IsNull(pipingCalculationsView.AssessmentSection);
            Assert.IsNull(pipingCalculationsView.ApplicationSelection);
        }

        private void ShowPipingCalculationsView(PipingCalculationsView pipingCalculationsView)
        {
            testForm.Controls.Add(pipingCalculationsView);
            testForm.Show();
        }
    }
}
