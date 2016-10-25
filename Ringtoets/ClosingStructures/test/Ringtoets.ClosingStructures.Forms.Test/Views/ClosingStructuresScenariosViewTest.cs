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
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms;

namespace Ringtoets.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresScenariosViewTest
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
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsTrue(view.AutoScroll);
                Assert.IsNull(view.Data);
                Assert.IsNull(view.FailureMechanism);

                var scenarioSelectionControl = new ControlTester("scenarioSelectionControl").TheObject as ScenarioSelectionControl;

                Assert.NotNull(scenarioSelectionControl);
                Assert.AreEqual(new Size(0, 0), scenarioSelectionControl.MinimumSize);
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
        public void FailureMechanism_ValidFailureMechanismSet_ValidFailureMechanism()
        {
            // Setup
            using (var view = ShowScenariosView())
            {
                var failureMechanism = new ClosingStructuresFailureMechanism();

                // Call
                view.FailureMechanism = failureMechanism;

                // Assert
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        private ClosingStructuresScenariosView ShowScenariosView()
        {
            var scenariosView = new ClosingStructuresScenariosView();
            testForm.Controls.Add(scenariosView);
            testForm.Show();

            return scenariosView;
        }
    }
}