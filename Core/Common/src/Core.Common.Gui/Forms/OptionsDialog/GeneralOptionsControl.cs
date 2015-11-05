using System;
using System.Configuration;
using System.Security.Permissions;
using System.Windows.Forms;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Globalization;

namespace Core.Common.Gui.Forms.OptionsDialog
{
    public partial class GeneralOptionsControl : UserControl, IOptionsControl
    {
        private ApplicationSettingsBase userSettings;

        private bool initializingControls;

        public GeneralOptionsControl()
        {
            InitializeComponent();
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

        public string ColorTheme
        {
            get
            {
                return (string) comboBoxTheme.SelectedItem;
            }
            set
            {
                comboBoxTheme.SelectedItem = value;
            }
        }

        // TODO: Call this method
        public Action<GeneralOptionsControl> OnAcceptChanges { get; set; }

        public string Title
        {
            get
            {
                return Resources.GeneralOptionsControl_Title_General;
            }
        }

        public string Category
        {
            get
            {
                return Resources.GeneralOptionsControl_Title_General;
            }
        }

        public void AcceptChanges()
        {
            SetValuesToSettings();
            if (OnAcceptChanges != null)
            {
                OnAcceptChanges(this);
            }
        }

        public void DeclineChanges()
        {
            SetSettingsValuesToControls();
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

            SetSettingsToNumberFormatControls((string) userSettings["realNumberFormat"]);
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

            string newRealNumberFormat = GetRealNumberFormatFromControls();

            if ((string) userSettings["realNumberFormat"] != newRealNumberFormat)
            {
                userSettings["realNumberFormat"] = newRealNumberFormat;
                RegionalSettingsManager.RealNumberFormat = newRealNumberFormat;
            }
        }

        private string GetRealNumberFormatFromControls()
        {
            string format;
            var numDecimals = (int) upDownNumberOfDecimals.Value;

            if (radioButtonCompactNotation.Checked)
            {
                format = "G";
            }
            else if (radioButtonNumberNotation.Checked)
            {
                format = "N";
            }
            else if (radioButtonScientificNotation.Checked)
            {
                format = "E";
            }
            else
            {
                throw new Exception(Resources.GeneralOptionsControl_GetRealNumberFormatFromControls_None_of_the_radiobuttons_is_selected__impossible);
            }

            return format + numDecimals;
        }

        private void UpdateRealNumberFormattingSample()
        {
            if (initializingControls)
            {
                return; // controls are being initializing, inconsistent state
            }

            var format = "{0:" + GetRealNumberFormatFromControls() + "}";

            textBoxRealFormatSample.Text =
                string.Format(format, 0.00123456) + Environment.NewLine +
                string.Format(format, 1.23456789) + Environment.NewLine +
                string.Format(format, 123.456789) + Environment.NewLine +
                string.Format(format, 12345.6789) + Environment.NewLine +
                string.Format(format, 12345678.9) + Environment.NewLine;
        }

        private void SetSettingsToNumberFormatControls(string numberFormat)
        {
            if (numberFormat == null || string.IsNullOrEmpty(numberFormat))
            {
                numberFormat = "G5"; //set default
            }

            initializingControls = true;

            int decimals = 5; //default
            Int32.TryParse(numberFormat.Substring(1), out decimals);
            upDownNumberOfDecimals.Value = decimals;

            switch (numberFormat[0])
            {
                case 'G':
                    radioButtonCompactNotation.Checked = true;
                    break;
                case 'E':
                    radioButtonScientificNotation.Checked = true;
                    break;
                case 'N':
                    radioButtonNumberNotation.Checked = true;
                    break;
                default:
                    throw new ArgumentException(Resources.GeneralOptionsControl_SetSettingsToNumberFormatControls_Unknown_numberFormat_for_user_settings);
            }

            initializingControls = false;
            UpdateRealNumberFormattingSample();
        }

        private void RadioButtonCompactNotationCheckedChanged(object sender, EventArgs e)
        {
            lblDecimalsOrSignificants.Text = radioButtonCompactNotation.Checked ? Resources.GeneralOptionsControl_RadioButtonCompactNotationCheckedChanged_Significant_digits_ : Resources.GeneralOptionsControl_RadioButtonCompactNotationCheckedChanged_____Decimal_places_;
            UpdateRealNumberFormattingSample();
        }

        private void RadioButtonNumberNotationCheckedChanged(object sender, EventArgs e)
        {
            UpdateRealNumberFormattingSample();
        }

        private void RadioButtonScientificNotationCheckedChanged(object sender, EventArgs e)
        {
            UpdateRealNumberFormattingSample();
        }

        private void UpDownNumberOfDecimalsValueChanged(object sender, EventArgs e)
        {
            UpdateRealNumberFormattingSample();
        }

        private void comboBoxTheme_SelectedIndexChanged(object sender, EventArgs e) {}
    }
}