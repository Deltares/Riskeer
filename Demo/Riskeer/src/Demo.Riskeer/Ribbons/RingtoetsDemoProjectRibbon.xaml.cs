// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms;
using Demo.Riskeer.Commands;
using Fluent;

namespace Demo.Riskeer.Ribbons
{
    /// <summary>
    /// Interaction logic for RingtoetsDemoProjectRibbon.xaml
    /// </summary>
    public partial class RingtoetsDemoProjectRibbon : IRibbonCommandHandler
    {
        private readonly ICommand addNewAssessmentSection,
                                  openMapViewCommand,
                                  openThematicMapViewCommand,
                                  openChartViewCommand,
                                  openStackChartViewCommand,
                                  openPointedTreeGraphViewCommand;

        public RingtoetsDemoProjectRibbon(IProjectOwner projectOwner, IViewCommands viewCommands)
        {
            InitializeComponent();

            addNewAssessmentSection = new AddNewDemoAssessmentSectionCommand(projectOwner, viewCommands);
            openChartViewCommand = new OpenChartViewCommand(viewCommands);
            openMapViewCommand = new OpenMapViewCommand(viewCommands);
            openThematicMapViewCommand = new OpenThematicalMapViewCommand(viewCommands);
            openStackChartViewCommand = new OpenStackChartViewCommand(viewCommands);
            openPointedTreeGraphViewCommand = new OpenPointedTreeGraphViewCommand(viewCommands);
        }

        public Ribbon GetRibbonControl()
        {
            return RingtoetsDemoProjectRibbonControl;
        }

        public void ValidateItems() {}

        private void AddNewDemoAssessmentSectionButton_Click(object sender, RoutedEventArgs e)
        {
            addNewAssessmentSection.Execute();
        }

        private void ButtonOpenChartView_Click(object sender, RoutedEventArgs e)
        {
            openChartViewCommand.Execute();
        }

        private void ButtonOpenMapView_Click(object sender, RoutedEventArgs e)
        {
            openMapViewCommand.Execute();
        }

        private void ButtonOpenThematicMapView_Click(object sender, RoutedEventArgs e)
        {
            openThematicMapViewCommand.Execute();
        }

        private void ButtonOpenStackChartView_Click(object sender, RoutedEventArgs e)
        {
            openStackChartViewCommand.Execute();
        }

        private void OpenPointedTreeGraphViewButton_OnClick(object sender, RoutedEventArgs e)
        {
            openPointedTreeGraphViewCommand.Execute();
        }
    }
}