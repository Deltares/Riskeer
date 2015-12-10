﻿using System;
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
    /// <description>the owning form is always set as provided during creation time.</description>
    /// </item>
    /// </list>
    /// </summary>
    public abstract partial class DialogBase : Form
    {
        private readonly IWin32Window owner;

        /// <summary>
        /// Constructs a new <see cref="DialogBase"/>.
        /// </summary>
        /// <param name="owner">The owner of the dialog.</param>
        /// <param name="icon">The icon to show in the control box.</param>
        protected DialogBase(IWin32Window owner, Icon icon)
        {
            InitializeComponent();

            this.owner = owner;

            Icon = icon;
        }

        /// <summary>
        /// This method provides a new implementation of <see cref="Form.ShowDialog()"/>.
        /// In this new implementation the dialog is shown by passing the owner provided during creation time (<see cref="DialogBase(IWin32Window, Icon)"/>.
        /// </summary>
        /// <returns>A <see cref="DialogResult"/>.</returns>
        public new DialogResult ShowDialog()
        {
            return base.ShowDialog(owner);
        }

        protected override void OnShown(EventArgs e)
        {
            // Initialize the cancel button (as this cannot be done during creation time)
            CancelButton = GetCancelButton();

            base.OnShown(e);
        }

        /// <summary>
        /// Gets the cancel button of the <see cref="DialogBase"/>.
        /// </summary>
        /// <returns>The cancel button.</returns>
        /// <remarks>By forcing derivatives to provide a cancel button, dialogs can be closed by hitting the <c>ESC</c> key on the keyboard.</remarks>
        protected abstract Button GetCancelButton();
    }
}