using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using DeltaShell.Gui.Properties;

namespace DeltaShell.Gui.Forms
{
    public partial class HelpAboutBox : Form
    {
        public HelpAboutBox()
        {
            InitializeComponent();
        }
        
        private void textBoxDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText); 
        }

        public void UpdateAboutBox(HelpAboutBoxData helpAboutBoxData)
        {
            Text = String.Format(Resources.HelpAboutBox_UpdateAboutBox_About__0_, helpAboutBoxData.ProductName);
            labelProductName.Text = helpAboutBoxData.ProductName;
            labelVersion.Text = helpAboutBoxData.Version;
            labelCopyright.Text = helpAboutBoxData.Copyright;
            labelSupportEmail.Text = helpAboutBoxData.SupportEmail;
            labelSupportPhone.Text = helpAboutBoxData.SupportPhone;

            AddPluginsToTextBox(helpAboutBoxData);
        }

        private void AddPluginsToTextBox(HelpAboutBoxData helpAboutBoxData)
        {
            foreach (var plugin in helpAboutBoxData.Plugins)
            {
                var builder = new StringBuilder(textBoxDescription.Text);
                builder.AppendLine("");
                builder.AppendLine(String.Format("{0} {1}", plugin.DisplayName, plugin.Version));
                builder.AppendLine(plugin.Description);
                textBoxDescription.Text = builder.ToString();
            }
        }
    }
}