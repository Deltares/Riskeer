// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Properties;
using Core.Common.Gui.Theme;
using Core.Common.Utils.Reflection;

namespace Core.Common.Gui.Forms.Options
{
    /// <summary>
    /// Dialog for edition user settings.
    /// </summary>
    public partial class OptionsDialog : DialogBase
    {
        private readonly ApplicationSettingsBase userSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsDialog"/> class.
        /// </summary>
        /// <param name="dialogParent">The dialog parent for which this should be shown on top.</param>
        /// <param name="userSettings">The user settings.</param>
        public OptionsDialog(IWin32Window dialogParent, ApplicationSettingsBase userSettings) : base(dialogParent, Resources.OptionsHS1, 430, 170)
        {
            InitializeComponent();

            this.userSettings = userSettings;

            SetColorThemeOptions();
            SetSettingsValuesToControls();
        }

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }

        private void SetColorThemeOptions()
        {
            var colorThemeItems = new Collection<ColorThemeItem>();
            foreach (var theme in (ColorTheme[]) Enum.GetValues(typeof(ColorTheme)))
            {
                colorThemeItems.Add(new ColorThemeItem
                {
                    Theme = theme,
                    DisplayName = theme.Localized()
                });
            }
            comboBoxTheme.DataSource = colorThemeItems;
            comboBoxTheme.ValueMember = TypeUtils.GetMemberName<ColorThemeItem>(cti => cti.Theme);
            comboBoxTheme.DisplayMember = TypeUtils.GetMemberName<ColorThemeItem>(cti => cti.DisplayName);
        }

        private void SetSettingsValuesToControls()
        {
            if (userSettings == null)
            {
                return;
            }

            checkBoxStartPage.Checked = (bool) userSettings["showStartPage"];
            comboBoxTheme.SelectedValue = (ColorTheme) userSettings["colorTheme"];
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            if (userSettings == null)
            {
                return;
            }

            userSettings["showStartPage"] = checkBoxStartPage.Checked;
            userSettings["colorTheme"] = comboBoxTheme.SelectedValue;
        }

        /// <summary>
        /// Used for localizing the items in the theme selection combo box.
        /// </summary>
        private class ColorThemeItem
        {
            /// <summary>
            /// Gets or sets the <see cref="ColorTheme"/> for this item.
            /// </summary>
            public ColorTheme Theme { get; set; }

            /// <summary>
            /// Gets or sets the name to display for the <see cref="ColorTheme"/>.
            /// </summary>
            public string DisplayName { get; set; }
        }
    }
}