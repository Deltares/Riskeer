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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Helper class to get controls.
    /// </summary>
    public static class ControlTestHelper
    {
        /// <summary>
        /// Gets the <see cref="DataGridViewControl"/> from the given form.
        /// </summary>
        /// <param name="form">The form to get the control from.</param>
        /// <param name="controlName">The name of the control.</param>
        /// <returns>The found control.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="controlName"/> is <c>null</c> or empty.</exception>
        public static DataGridViewControl GetDataGridViewControl(Form form, string controlName)
        {
            return GetControls<DataGridViewControl>(form, controlName).FirstOrDefault();
        }

        /// <summary>
        /// Gets the <see cref="DataGridView"/> from the given form.
        /// </summary>
        /// <param name="control">The control to get the nested control from.</param>
        /// <param name="controlName">The name of the control.</param>
        /// <returns>The found control.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="controlName"/> is <c>null</c> or empty.</exception>
        public static DataGridView GetDataGridView(Control control, string controlName)
        {
            return GetControls<DataGridView>(control, controlName).FirstOrDefault();
        }

        /// <summary>
        /// Gets the controls by name.
        /// </summary>
        /// <param name="control">The control to get the nested control from.</param>
        /// <param name="controlName">The name of the controls.</param>
        /// <returns>The found controls.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="controlName"/> is <c>null</c> or empty.</exception>
        public static IEnumerable<TView> GetControls<TView>(Control control, string controlName)
        {
            return control.Controls.Find(controlName, true).OfType<TView>();
        }
    }
}