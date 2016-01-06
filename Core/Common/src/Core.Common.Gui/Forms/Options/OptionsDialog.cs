using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Forms;
using Core.Common.Forms.Dialogs;
using Core.Common.Gui.Properties;
using Core.Common.Gui.Theme;
using Core.Common.Utils.Reflection;

namespace Core.Common.Gui.Forms.Options
{
    public partial class OptionsDialog : DialogBase
    {
        private readonly ApplicationSettingsBase userSettings;

        public OptionsDialog(IWin32Window owner, ApplicationSettingsBase userSettings) : base(owner, Resources.OptionsHS1, 430, 170)
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