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

using System.Drawing;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Commands;
using Core.Components.Stack.Data;

namespace Demo.Riskeer.Commands
{
    /// <summary>
    /// This class describes the command for opening a view for <see cref="StackChartData"/> with some arbitrary data.
    /// </summary>
    public class OpenStackChartViewCommand : ICommand
    {
        private readonly IViewCommands viewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="OpenStackChartViewCommand"/>.
        /// </summary>
        /// <param name="viewCommands">The <see cref="IViewCommands"/> to use internally.</param>
        public OpenStackChartViewCommand(IViewCommands viewCommands)
        {
            this.viewCommands = viewCommands;
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public void Execute()
        {
            var data = new StackChartData();
            data.AddColumn("Column 1");
            data.AddColumn("Column 2");
            data.AddColumn("Column 3");

            data.AddRow("Row 1", new[]
            {
                0.25,
                0.60,
                0.15
            });
            data.AddRow("Row 2", new[]
            {
                0.25,
                0.20,
                0.81
            });
            data.AddRow("Row 3", new[]
            {
                0.25,
                0.10,
                0.01
            });
            data.AddRow("Row 4", new[]
            {
                0.25,
                0.10,
                0.03
            }, Color.Gray);

            viewCommands.OpenView(data);
        }
    }
}