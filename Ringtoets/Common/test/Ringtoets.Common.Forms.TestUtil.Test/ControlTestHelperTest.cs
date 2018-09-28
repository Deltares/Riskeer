// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using NUnit.Framework;

namespace Ringtoets.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class ControlTestHelperTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void GetDataGridViewControl_InvalidName_ThrowsArgumentNullException(string name)
        {
            // Call
            TestDelegate test = () => ControlTestHelper.GetDataGridViewControl(new Form(), name);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void GetDataGridViewControl_NoControlOnForm_ReturnNull()
        {
            // Call
            DataGridViewControl control = ControlTestHelper.GetDataGridViewControl(new Form(), "name");

            // Assert
            Assert.IsNull(control);
        }

        [Test]
        public void GetDataGridViewControl_ControlOnForm_ReturnControl()
        {
            // Setup
            using (var form = new Form())
            using (var dataGridViewControl = new DataGridViewControl())
            {
                form.Controls.Add(dataGridViewControl);

                // Call
                DataGridViewControl control = ControlTestHelper.GetDataGridViewControl(form, dataGridViewControl.Name);

                // Assert
                Assert.AreSame(dataGridViewControl, control);
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void GetDataGridView_InvalidName_ThrowsArgumentNullException(string name)
        {
            // Call
            TestDelegate test = () => ControlTestHelper.GetDataGridView(new Form(), name);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void GetDataGridView_NoControlOnForm_ReturnNull()
        {
            // Call
            DataGridView control = ControlTestHelper.GetDataGridView(new Form(), "name");

            // Assert
            Assert.IsNull(control);
        }

        [Test]
        public void GetDataGridView_ControlOnForm_ReturnControl()
        {
            // Setup
            using (var form = new Form())
            using (var dataGridView = new DataGridView
            {
                Name = "dataGridView"
            })
            {
                form.Controls.Add(dataGridView);

                // Call
                DataGridView control = ControlTestHelper.GetDataGridView(form, dataGridView.Name);

                // Assert
                Assert.AreSame(dataGridView, control);
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void GetControls_InvalidName_ThrowsArgumentNullException(string name)
        {
            // Call
            TestDelegate test = () => ControlTestHelper.GetControls<Control>(new Form(), name);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void GetControls_NoControlsOnForm_ReturnEmptyList()
        {
            // Call
            IEnumerable<Control> controls = ControlTestHelper.GetControls<Control>(new Form(), "name");

            // Assert
            CollectionAssert.IsEmpty(controls);
        }

        [Test]
        public void GetControls_ControlsOnFormWithSameName_ReturnControls()
        {
            // Setup
            const string controlName = "control";
            using (var form = new Form())
            using (var control1 = new Control
            {
                Name = controlName
            })
            using (var control2 = new Control
            {
                Name = controlName
            })
            {
                form.Controls.Add(control1);
                form.Controls.Add(control2);

                // Call
                IEnumerable<Control> controls = ControlTestHelper.GetControls<Control>(form, controlName);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    control1,
                    control2
                }, controls);
            }
        }

        [Test]
        public void GetControls_ControlOnForm_ReturnControl()
        {
            // Setup
            using (var form = new Form())
            using (var control1 = new Control
            {
                Name = "control1"
            })
            using (var control2 = new Control
            {
                Name = "control2"
            })
            {
                form.Controls.Add(control1);
                form.Controls.Add(control2);

                // Call
                IEnumerable<Control> controls = ControlTestHelper.GetControls<Control>(form, "control1");

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    control1
                }, controls);
            }
        }

        [Test]
        public void GetControls_DifferentControlsWithSameName_ReturnUserControl()
        {
            // Setup
            const string controlName = "control";
            using (var form = new Form())
            using (var userControl = new UserControl
            {
                Name = controlName
            })
            using (var control = new Control
            {
                Name = controlName
            })
            {
                form.Controls.Add(userControl);
                form.Controls.Add(control);

                // Call
                IEnumerable<UserControl> controls = ControlTestHelper.GetControls<UserControl>(form, controlName);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    userControl
                }, controls);
            }
        }
    }
}