﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Windows;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Forms;
using Fluent;

namespace Ringtoets.Integration.Plugin
{
    /// <summary>
    /// Interaction logic for RingtoetsRibbon.xaml
    /// </summary>
    public partial class RingtoetsRibbon : IRibbonCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RingtoetsRibbon"/> class.
        /// </summary>
        public RingtoetsRibbon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the command used to control the add assessment section button.
        /// </summary>
        public ICommand AddAssessmentSectionButtonCommand { set; private get; }

        public Ribbon GetRibbonControl()
        {
            return RingtoetsRibbonControl;
        }

        public void ValidateItems() {}

        public bool IsContextualTabVisible(string tabGroupName)
        {
            return false;
        }

        private void ButtonAddAssessmentSectionToolWindowClick(object sender, RoutedEventArgs e)
        {
            AddAssessmentSectionButtonCommand.Execute();

            ValidateItems();
        }
    }
}