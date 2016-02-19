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
using System.Windows;
using System.Windows.Media;

namespace Core.Common.Gui.Forms.SplashScreen
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        private string progressText;
        private int progressValuePercent;
        private string licenseText;
        private string copyrightText;
        private string versionText;
        private bool hasProgress;

        private string supportPhoneNumber;
        private string supportEmailAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreen"/> class with a progress
        /// bar from 0 to 100%.
        /// </summary>
        public SplashScreen()
        {
            InitializeComponent();

            ProgressBar.Maximum = 100; // classic percentage approach, there is no need for the splash screen to be more precise

            HasProgress = true;
            ProgressValuePercent = 0;
            ProgressText = "";
            CopyrightText = "";
            LicenseText = "";
            VersionText = "";
        }

        /// <summary>
        /// Indicates where or not the progress bar and progress label are visible in the window.
        /// </summary>
        public bool HasProgress
        {
            get
            {
                return hasProgress;
            }
            set
            {
                hasProgress = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Gets or sets the version to be shown.
        /// </summary>
        public string VersionText
        {
            get
            {
                return versionText;
            }
            set
            {
                versionText = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Gets or sets the copyright owner to be shown.
        /// </summary>
        public string CopyrightText
        {
            get
            {
                return copyrightText;
            }
            set
            {
                copyrightText = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Gets or sets the type of the license, plain text.
        /// </summary>
        public string LicenseText
        {
            get
            {
                return licenseText;
            }
            set
            {
                licenseText = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Gets or sets the percentage value (in the range [0, 100]) to be set as progress indication. 
        /// </summary>
        public int ProgressValuePercent
        {
            get
            {
                return progressValuePercent;
            }
            set
            {
                progressValuePercent = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Gets or sets the descriptive text for current status of the progress.
        /// </summary>
        public string ProgressText
        {
            get
            {
                return progressText;
            }
            set
            {
                progressText = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Gets or sets the support e-mail.
        /// </summary>
        public string SupportEmail
        {
            private get
            {
                return supportEmailAddress;
            }
            set
            {
                supportEmailAddress = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Gets or sets the support phone number.
        /// </summary>
        public string SupportPhoneNumber
        {
            private get
            {
                return supportPhoneNumber;
            }
            set
            {
                supportPhoneNumber = value;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// Shuts this instance down.
        /// </summary>
        public void Shutdown()
        {
            Focusable = false;
            Close();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (LabelLicense.Content.ToString() != LicenseText)
            {
                LabelLicense.Content = LicenseText;
            }

            if (LabelCopyright.Content.ToString() != CopyrightText)
            {
                LabelCopyright.Content = CopyrightText;
            }

            if (LabelVersion.Content.ToString() != VersionText)
            {
                LabelVersion.Content = VersionText;
            }

            SetSupportValues();

            var progressVisibility = HasProgress ? Visibility.Visible : Visibility.Hidden;

            ProgressBar.Visibility = progressVisibility;
            LabelProgressBar.Visibility = progressVisibility;
            LabelProgressMessage.Visibility = progressVisibility;

            if (!HasProgress)
            {
                return; // no need to update progress related labels below
            }

            if (ProgressBar.Value != ProgressValuePercent)
            {
                ProgressBar.Value = ProgressValuePercent;
                LabelProgressBar.Content = string.Format("{0} %", ProgressValuePercent);
            }

            if (LabelProgressMessage.Content.ToString() != ProgressText)
            {
                LabelProgressMessage.Content = ProgressText;
            }
        }

        private void SetSupportValues()
        {
            var supportVisibility = (String.IsNullOrWhiteSpace(SupportPhoneNumber) || String.IsNullOrWhiteSpace(SupportEmail)) ? Visibility.Collapsed : Visibility.Visible;

            LabelSupportTitle.Visibility = supportVisibility;
            LabelSupportPhoneNumberTitle.Visibility = supportVisibility;
            LabelSupportPhoneNumber.Visibility = supportVisibility;
            LabelSupportEmailAddressTitle.Visibility = supportVisibility;
            LabelSupportEmailAddress.Visibility = supportVisibility;

            if (supportVisibility != Visibility.Visible)
            {
                return;
            }

            if (LabelSupportPhoneNumber.Content.ToString() != SupportPhoneNumber)
            {
                LabelSupportPhoneNumber.Content = SupportPhoneNumber;
            }
            if (LabelSupportEmailAddress.Content.ToString() != SupportEmail)
            {
                LabelSupportEmailAddress.Content = SupportEmail;
            }
        }
    }
}