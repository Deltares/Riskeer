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
using System.Drawing;
using System.Windows.Forms;

namespace Core.Common.Controls.Dialogs
{
    /// <summary>
    /// Base class for dialogs which should be derived in order to get a consistent look and feel.
    /// The base class ensures:
    /// <list type="bullet">
    /// <item>
    /// <description>the dialog is shown in the center of the parent form (<see cref="Form.StartPosition"/> is set to <see cref="FormStartPosition.CenterParent"/>);</description>
    /// </item>
    /// <item>
    /// <description>no task bar icon is shown (<see cref="Form.ShowInTaskbar"/> is set to <c>false</c>);</description>
    /// </item>
    /// <item>
    /// <description>no minimize control box item is shown (<see cref="Form.MinimizeBox"/> is set to <c>false</c>);</description>
    /// </item>
    /// <item>
    /// <description>no maximize control box item is shown (<see cref="Form.MaximizeBox"/> is set to <c>false</c>);</description>
    /// </item>
    /// <item>
    /// <description>the minimum width and minimum height are always set during creation time;</description>
    /// </item>
    /// <item>
    /// <description>the owning form is always provided when showing the dialog (see <see cref="ShowDialog"/>);</description>
    /// </item>
    /// <item>
    /// <description>the dialog can be closed using the <c>ESC</c> key on the keyboard (see <see cref="GetCancelButton"/>).</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// The "new" implementation for <see cref="ShowDialog"/> only hides the base method. Therefore, don't cast to <see cref="Form"/>.
    /// Otherwise the original <see cref="Form.ShowDialog()"/> implementation will be used.
    /// </remarks>
    public abstract partial class DialogBase : Form
    {
        private readonly IWin32Window dialogParent;
        private readonly int minHeight;
        private readonly int minWidth;

        /// <summary>
        /// Constructs a new <see cref="DialogBase"/>.
        /// </summary>
        /// <param name="dialogParent">The owner of the dialog.</param>
        /// <param name="icon">The icon to show in the control box.</param>
        /// <param name="minWidth">The minimum width of the dialog.</param>
        /// <param name="minHeight">The minimum height of the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> or <paramref name="icon"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="minWidth"/> or <paramref name="minHeight"/> is not greater than <c>0</c>.</exception>
        protected DialogBase(IWin32Window dialogParent, Bitmap icon, int minWidth, int minHeight) :
            this(dialogParent, BitmapToIcon(icon), minWidth, minHeight) {}

        /// <summary>
        /// Constructs a new <see cref="DialogBase"/>.
        /// </summary>
        /// <param name="dialogParent">The owner of the dialog.</param>
        /// <param name="icon">The icon to show in the control box.</param>
        /// <param name="minWidth">The minimum width of the dialog.</param>
        /// <param name="minHeight">The minimum height of the dialog.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dialogParent"/> or <paramref name="icon"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="minWidth"/> or <paramref name="minHeight"/> is not greater than <c>0</c>.</exception>
        protected DialogBase(IWin32Window dialogParent, Icon icon, int minWidth, int minHeight)
        {
            InitializeComponent();

            if (dialogParent == null)
            {
                throw new ArgumentNullException(nameof(dialogParent));
            }

            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon));
            }

            if (minWidth <= 0)
            {
                throw new ArgumentException("The minimum width of the dialog should be greater than 0");
            }

            if (minHeight <= 0)
            {
                throw new ArgumentException("The minimum height of the dialog should be greater than 0");
            }

            this.dialogParent = dialogParent;
            this.minWidth = minWidth;
            this.minHeight = minHeight;

            Icon = icon;
        }

        /// <summary>
        /// This method provides a new implementation of <see cref="Form.ShowDialog()"/>.
        /// In this new implementation the dialog is shown by passing the owner provided during creation time (see <see cref="DialogBase(IWin32Window, Icon,int,int)"/>).
        /// </summary>
        /// <returns>A <see cref="DialogResult"/>.</returns>
        public new DialogResult ShowDialog()
        {
            return base.ShowDialog(dialogParent);
        }

        protected override void OnLoad(EventArgs e)
        {
            // Set the minimum size here in order to prevent virtual member calls
            MinimumSize = new Size(minWidth, minHeight);

            // Initialize the cancel button (as this cannot be done during creation time)
            CancelButton = GetCancelButton();

            base.OnLoad(e);
        }

        /// <summary>
        /// Gets the cancel button of the <see cref="DialogBase"/>.
        /// </summary>
        /// <returns>The cancel button.</returns>
        /// <remarks>By forcing derivatives to provide a cancel button, dialogs can be closed by hitting the <c>ESC</c> key on the keyboard.</remarks>
        protected abstract Button GetCancelButton();

        private static Icon BitmapToIcon(Bitmap icon)
        {
            return icon == null ? null : Icon.FromHandle(icon.GetHicon());
        }
    }
}