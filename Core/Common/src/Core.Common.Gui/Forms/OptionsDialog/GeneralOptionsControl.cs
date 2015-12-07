using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Security.Permissions;
using System.Windows.Forms;
using Core.Common.Utils.Reflection;

namespace Core.Common.Gui.Forms.OptionsDialog
{
    public partial class GeneralOptionsControl : Form
    {
        private ApplicationSettingsBase userSettings;

        public GeneralOptionsControl()
        {
            InitializeComponent();
            SetColorThemeOptions();
        }

        public ApplicationSettingsBase UserSettings
        {
            get
            {
                return userSettings;
            }
            set
            {
                UpdateUserSettings(value);
            }
        }

        public ColorTheme ColorTheme
        {
            get
            {
                return (ColorTheme) comboBoxTheme.SelectedValue;
            }
            set
            {
                comboBoxTheme.SelectedValue = value;
            }
        }

        // TODO: Call this method
        public Action<GeneralOptionsControl> OnAcceptChanges { get; set; }

        private void AcceptChanges()
        {
            SetValuesToSettings();
            if (OnAcceptChanges != null)
            {
                OnAcceptChanges(this);
            }
        }

        private void DeclineChanges()
        {
            SetSettingsValuesToControls();
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

        /// <summary>
        /// Safe call because of linkdemand
        /// </summary>
        /// <param name="settings"></param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private void UpdateUserSettings(ApplicationSettingsBase settings)
        {
            userSettings = settings;
            SetSettingsValuesToControls();
        }

        /// <summary>
        /// Dialog created.
        /// </summary>
        private void SetSettingsValuesToControls()
        {
            checkBoxStartPage.Checked = (bool) userSettings["showStartPage"];
        }

        /// <summary>
        /// Ok clicked.
        /// </summary>
        private void SetValuesToSettings()
        {
            if ((bool) userSettings["showStartPage"] != checkBoxStartPage.Checked)
            {
                userSettings["showStartPage"] = checkBoxStartPage.Checked;
            }
        }

        private void comboBoxTheme_SelectedIndexChanged(object sender, EventArgs e) {}

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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                DeclineChanges();
            }

            base.OnFormClosed(e);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            AcceptChanges();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DeclineChanges();
        }
    }
}