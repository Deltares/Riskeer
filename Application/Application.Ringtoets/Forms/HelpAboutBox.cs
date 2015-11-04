using System;
using System.Diagnostics;
using System.Windows.Forms;
using Application.Ringtoets.Properties;

namespace Application.Ringtoets.Forms
{
    public partial class HelpAboutBox : Form
    {
        public HelpAboutBox()
        {
            InitializeComponent();
        }

        public void UpdateAboutBox(HelpAboutBoxData helpAboutBoxData)
        {
            Text = String.Format(Resources.HelpAboutBox_UpdateAboutBox_About__0_, helpAboutBoxData.ProductName);
            labelProductName.Text = helpAboutBoxData.ProductName;
            labelVersion.Text = helpAboutBoxData.Version;
            labelCopyright.Text = helpAboutBoxData.Copyright;
            labelSupportEmail.Text = helpAboutBoxData.SupportEmail;
            labelSupportPhone.Text = helpAboutBoxData.SupportPhone;
        }

        private void textBoxDescription_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}