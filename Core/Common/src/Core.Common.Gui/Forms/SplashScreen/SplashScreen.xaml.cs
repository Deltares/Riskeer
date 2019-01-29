// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Windows;
using System.Windows.Media;

namespace Core.Common.Gui.Forms.SplashScreen
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        private string versionText;
        private string supportPhoneNumber;
        private string supportEmailAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreen"/> class.
        /// </summary>
        public SplashScreen()
        {
            InitializeComponent();

            VersionText = "";
        }

        /// <summary>
        /// Gets or sets the version to be shown.
        /// </summary>
        public string VersionText
        {
            private get
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

            if (LabelVersion.Content.ToString() != VersionText)
            {
                LabelVersion.Content = VersionText;
            }

            SetSupportValues();
        }

        private void SetSupportValues()
        {
            Visibility supportVisibility = string.IsNullOrWhiteSpace(SupportPhoneNumber) || string.IsNullOrWhiteSpace(SupportEmail) ? Visibility.Collapsed : Visibility.Visible;

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