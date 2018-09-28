// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using NUnit.Extensions.Forms;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Class providing high level actions on winforms controls.
    /// </summary>
    public static class ControlsTestHelper
    {
        /// <summary>
        /// Emulates the user selecting new value for a <see cref="ComboBox"/>.
        /// </summary>
        /// <param name="comboBox">The combo box to be edited.</param>
        /// <param name="newValue">The new value to be selected.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="comboBox"/>
        /// isn't configured to use <see cref="ComboBox.DataSource"/>.</exception>
        /// <remarks>
        /// <para>
        /// Do not use this method for <see cref="ComboBox"/> instances configured
        /// to use <see cref="ComboBox.Items"/> with <see cref="ComboBox.SelectedItem"/>.
        /// </para>
        /// <para>
        /// The <see cref="Control.Validating"/> and <see cref="Control.Validated"/> events
        /// will not be fired when calling this method, as this method does not deal with
        /// focus changes.
        /// </para>
        /// </remarks>
        public static void FakeUserSelectingNewValue(ComboBox comboBox, object newValue)
        {
            if (comboBox.DataSource == null || comboBox.DisplayMember == null || comboBox.ValueMember == null)
            {
                throw new InvalidOperationException("Call FakeUserSelectingNewValue only for ComboBox instances that are configured to use a DataSource.");
            }

            comboBox.SelectedValue = newValue;
            EventHelper.RaiseEvent(comboBox, "SelectionChangeCommitted");
        }
    }
}